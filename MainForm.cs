using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using Keyence.AutoID.SDK;
using System.Net;
using System.Windows.Forms;
using Automation.BDaq;
using AmtestCommunicator.Libs;
using AmtestCommunicator.Entities;
using NLog.Windows.Forms;
using NLog.Config;
using NLog.Targets.Wrappers;
using NLog;
using System.Text;
using System.Text.RegularExpressions;
using AmtestCommunicator.UitlityClasses;

namespace AmtestCommunicator
{
    public partial class MainForm : Form
    {

        //General Variables
        public List<CarrierModel> currentCarrier;
        public bool IsFirstRun = true;
        public string CarrierName = string.Empty;
        public bool ReadIsExecuting = false;
        public int _pcbtotalNumber = 0;
        public List<Projects> configuredProjects = new List<Projects>();

        public bool MESErrorReceived = false;

        public string scannerName = "";
        public bool ErrorReceived = false;

        public int noOfScanners = Properties.Settings.Default.NoOfScanners;
        public string unitIdType = Properties.Settings.Default.UnitIdType;

        public string MaterialWatch = "";


        //TCP Message Counter
        public string ipModel = "";
        public static int Counter = 0;
        /*Init TCP communication*/
        class HelperComEventArgs
        {
            public bool _result = false;
            public string _info = "";
            public int _failPosition = 0;
        }
       
        public static IPAddress IPAddressMes { get; set; }
        public static int PortMes { get; set; }
        private TCPCommunication Client;
        public string StationName = Properties.Settings.Default.StationName;

        //Init Scanner TCP connection variables
        private ReaderAccessor m_reader = new ReaderAccessor();
        private ReaderSearcher m_searcher = new ReaderSearcher();
        List<NicSearchResult> m_nicList = new List<NicSearchResult>();
        List<string> ipListofScanners = new List<string>();

        //Init USB device
        public string _usbDeviceType = Properties.Settings.Default.USBDeviceType;
        public int _usbDevicePortIn = Properties.Settings.Default.USBDevicePortIn;
        public int _usbDevicePortOut = Properties.Settings.Default.USBDevicePortOut;
        public int _usbDevicePortOutPin = Properties.Settings.Default.USBDevicePortOutPin;

        /*Logger Init*/
        public static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        NLog.Windows.Forms.RichTextBoxTarget target;

        public MainForm()
        {
            InitializeComponent();
          
            #region Initialize working Threads
            backgroundWorkerMain = new BackgroundWorker();
            backgroundWorkerMain.WorkerReportsProgress = true;
            backgroundWorkerMain.DoWork += new DoWorkEventHandler(backgroundWorkerMain_DoWork);
            backgroundWorkerMain.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerMain_ProgressChanged);
            backgroundWorkerMain.WorkerSupportsCancellation = true;

            backgroundWorker_Scan1 = new BackgroundWorker();
            backgroundWorker_Scan1.WorkerReportsProgress = true;
            backgroundWorker_Scan1.DoWork += new DoWorkEventHandler(backgroundWorker_Scan1_DoWork);
            backgroundWorker_Scan1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_Scan1_ProgressChanged);
            backgroundWorker_Scan1.WorkerSupportsCancellation = true;

            backgroundWorker_Scan2 = new BackgroundWorker();
            backgroundWorker_Scan2.WorkerReportsProgress = true;
            backgroundWorker_Scan2.DoWork += new DoWorkEventHandler(backgroundWorker_Scan2_DoWork);
            backgroundWorker_Scan2.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_Scan2_ProgressChanged);
            backgroundWorker_Scan2.WorkerSupportsCancellation = true;
            backgroundWorkerMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(backgroundWorkerMain_RunWorkerCompleted);
            #endregion
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Init Logger
            try
            {
                //Init logger
                target = new NLog.Windows.Forms.RichTextBoxTarget();
                target.Name = "RichTextBox";
                target.Layout = "${longdate} ${level:uppercase=true} ${message}";
                target.ControlName = "richTextBoxLogger";
                target.FormName = "MainForm";
                target.AutoScroll = true;
                target.MaxLines = 30;
                target.UseDefaultRowColoringRules = false;
                target.RowColoringRules.Add(
                    new RichTextBoxRowColoringRule(
                        "level == LogLevel.Trace", // condition
                        "DarkGray", // font color
                        "Control", // background color
                        FontStyle.Regular
                    )
                );
                target.RowColoringRules.Add(new RichTextBoxRowColoringRule("level == LogLevel.Debug", "Gray", "Control"));
                target.RowColoringRules.Add(new RichTextBoxRowColoringRule("level == LogLevel.Info", "ControlText", "Control"));
                target.RowColoringRules.Add(new RichTextBoxRowColoringRule("level == LogLevel.Warn", "DarkRed", "Control"));
                target.RowColoringRules.Add(new RichTextBoxRowColoringRule("level == LogLevel.Error", "White", "Red", FontStyle.Bold));
                target.RowColoringRules.Add(new RichTextBoxRowColoringRule("level == LogLevel.Fatal", "Yellow", "DarkRed", FontStyle.Bold));
                
                AsyncTargetWrapper asyncWrapper = new AsyncTargetWrapper();
                asyncWrapper.Name = "AsyncRichTextBox";
                asyncWrapper.WrappedTarget = target;
                asyncWrapper.WriteAsyncLogEvents();
                SimpleConfigurator.ConfigureForTargetLogging(asyncWrapper, LogLevel.Trace);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                this.Close();
                return;
            }
            #endregion

            #region Init TCP/IP communication

            // INIT IP Model for Scanners
            byte[] bytes = Encoding.ASCII.GetBytes("LON");

            ipModel = Properties.Settings.Default.Scan_IP_Model;

            m_nicList = m_searcher.ListUpNic();

            //Init MES TCP
            try
            {
                Client = new TCPCommunication();
                IPAddressMes = IPAddress.Parse(Properties.Settings.Default.TCP_IP);
                PortMes = Properties.Settings.Default.TCP_Port;
                Client.Connect(IPAddressMes, PortMes);
                buttonMES.BackColor = Color.Green;
                logger.Info("MES connected");
                log.Info("MES connected");
            }
            catch (System.Exception ex)
            {
                log.Error("Failed to connect to MES: " + ex.Message.ToString());
                logger.Error("Failed to connect to MES: "+ ex.Message.ToString());
                buttonMES.BackColor = Color.Red;
            }
            #endregion

            #region Init IO/Device
            try
            {
              
                var device = DeviceCtrl.InstalledDevices.Where(o => o.Description.Contains(_usbDeviceType)).ToList();
                //Select Input Output Devices
                //IN
                instantDiCtrl1.SelectedDevice = new DeviceInformation(device[0].DeviceNumber);
                //OUT
                instantDoCtrl.SelectedDevice = new DeviceInformation(device[0].DeviceNumber);

                if (!instantDiCtrl1.Initialized)
                {
                    log.Error("No device be selected or device open failed!" + "StaticDI");
                    logger.Error("No device be selected or device open failed!", "StaticDI");
                    this.Close();
                    return;
                }
                else
                {
                    log.Info("Input communication device connected " + instantDiCtrl1.SelectedDevice.Description);
                    logger.Info("Input communication device connected {0}", instantDiCtrl1.SelectedDevice.Description);
                    btnInput.BackColor = Color.Green;
                }

                if (!instantDoCtrl.Initialized)
                {
                    log.Error("No device be selected or device open failed!" + "StaticDO");
                    logger.Error("No device be selected or device open failed!", "StaticDO");
                    this.Close();
                    return;
                }
                else
                {
                    log.Info("Output communication device connected {0}" + instantDoCtrl.SelectedDevice.Description);
                    logger.Info("Output communication device connected {0}", instantDoCtrl.SelectedDevice.Description);
                    btnOutput.BackColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed to connect digital IO device: " + ex.Message);
                logger.Error("Failed to connect digital IO device: "+ex.Message);
            }
            #endregion

            #region Enable Timer
            timer1.Enabled = true;
            #endregion

            #region Connect scanners via TCP
            SearchScannersAndConnect();
            #endregion

            #region ReadAppConfig
            ReadProjectConfig();
            #endregion
        }

        #region IO Timer Thread
        private void timer1_Tick(object sender, EventArgs e)
        {
            Automation.BDaq.ErrorCode err = Automation.BDaq.ErrorCode.Success;
            // read Di port state
            byte portData = 0;
            int intValue = 1;
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            byte[] result = intBytes;

            //Read input
            err = instantDiCtrl1.Read(_usbDevicePortIn,out portData);
            
            //Handle input errors
            if (err != Automation.BDaq.ErrorCode.Success)
            {
                timer1.Enabled = false;
                HandleError(err);
                return;
            }
            else
            {
                //Scan and send TCP messages if input received
                if (portData.ToString("X2") != "00" && backgroundWorker_Scan1.IsBusy == false)
                {
                    log.Info("Scanning process started");
                    logger.Info("Scanning process started");
                    currentCarrier = new List<CarrierModel>();
                    //Start first scan
                    backgroundWorker_Scan1.RunWorkerAsync();
                } 
            }
        }

        private void HandleError(Automation.BDaq.ErrorCode err)
        {
            if ((err >= Automation.BDaq.ErrorCode.ErrorHandleNotValid) && (err != Automation.BDaq.ErrorCode.Success))
            {
                log.Error("Sorry ! Some errors happened, the error code is: " + err.ToString() + " Static DI");
                logger.Error("Sorry ! Some errors happened, the error code is: " + err.ToString(), "Static DI");
            }
        }
        #endregion

        #region Scanner Threads
        private void backgroundWorker_Scan1_DoWork(object sender, DoWorkEventArgs e)
        {
            scannerName = "Scanner_1";
            HelperComEventArgs ProcessResult = new HelperComEventArgs();

            while (ReadIsExecuting == false)
            {
                if (ipListofScanners.Count > 0 && ReadIsExecuting == false)
                {
                    m_reader.IpAddress = ipListofScanners[0];
                    m_reader.Connect((data) =>
                    {
                        //Define received data actions here.Defined actions work asynchronously.
                        //"ReceivedDataWrite" works when reading data was received.
                        BeginInvoke(new delegateUserControl(ReceivedDataWrite), Encoding.ASCII.GetString(data));
                    });

                    ReceivedDataWrite(m_reader.ExecCommand("LON"));
                }
            }

            if (currentCarrier.Count != 0 && ErrorReceived == false)
            {
                ProcessResult._result = true;
                ProcessResult._info = "SUCCESS";
                backgroundWorker_Scan1.ReportProgress(100, ProcessResult);
            }
            else
            {
                ProcessResult._result = false;
                ProcessResult._info = "Error while reading!";
                backgroundWorker_Scan1.ReportProgress(100, ProcessResult);
            }
        }

        private void backgroundWorker_Scan1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var arg = e.UserState as HelperComEventArgs;
            if (arg._result == true)
            {
                if (!String.IsNullOrEmpty(CarrierName) && tableLayoutPanelFrame.Controls.Count == 0)
                {
                    var match = configuredProjects.Where(p => CarrierName.Contains(p.ProjectName)).FirstOrDefault();
                    if (match != null)
                    {
                        _pcbtotalNumber = match.Columns * match.Rows;
                        GenerateInterface(match.Columns, match.Rows);
                    }
                }

                if (noOfScanners > 1)
                {
                    logger.Info("{0} completed work " + scannerName);
                    logger.Info("{0} completed work", scannerName);
                    backgroundWorker_Scan2.RunWorkerAsync();
                    ErrorReceived = false;
                    ReadIsExecuting = false;
                }
                else
                {
                    log.Info(scannerName + " completed work");
                    logger.Info(scannerName + " completed work");
                    backgroundWorkerMain.RunWorkerAsync();//switch to message send via tcp ip
                    ReadIsExecuting = false;
                }
            }
            else
            {
                logger.Error(arg._info);
            }

            #region Commented
            //if(ErrorReceived == true)
            //{
            //    DialogResult result = MessageBox.Show("EMPTY SLOTS? ==> PRESS YES\r\nSCANNER ERROR ==> PRESS NO", "OPERATOR CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if(result == DialogResult.Yes)
            //    {
            //        if(noOfScanners > 1)
            //        {
            //            logger.Info("{0} completed work", scannerName);
            //            backgroundWorker_Scan2.RunWorkerAsync();
            //            ErrorReceived = false;
            //            ReadIsExecuting = false;
            //        }
            //        else
            //        {
            //            logger.Info(scannerName + " completed work");
            //            backgroundWorkerMain.RunWorkerAsync();//switch to message send via tcp ip
            //            ReadIsExecuting = false;
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("PRESS BUTTON AGAIN");
            //        ErrorReceived = false;
            //        ReadIsExecuting = false;
            //    }
            //} 
            //else
            //{
            //    if(noOfScanners > 1)
            //    {
            //        logger.Info(scannerName + " completed work");
            //        backgroundWorker_Scan2.RunWorkerAsync();
            //        ReadIsExecuting = false;
            //    }
            //    else
            //    {
            //        logger.Info(scannerName + " completed work");
            //        backgroundWorkerMain.RunWorkerAsync();//switch to message send via tcp ip
            //        ReadIsExecuting = false;
            //    }
            //}
            #endregion
        }

        private void backgroundWorker_Scan2_DoWork(object sender, DoWorkEventArgs e)
        {
            HelperComEventArgs ProcessResult = new HelperComEventArgs();
            scannerName = "Scanner_2";
            while (ReadIsExecuting == false)
            {
                if (ipListofScanners.Count > 0 && ReadIsExecuting == false)
                {
                    m_reader.IpAddress = ipListofScanners[1];
                    m_reader.Connect((data) =>
                    {
                        //Define received data actions here.Defined actions work asynchronously.
                        //"ReceivedDataWrite" works when reading data was received.
                        BeginInvoke(new delegateUserControl(ReceivedDataWrite), Encoding.ASCII.GetString(data));
                    });

                    ReceivedDataWrite(m_reader.ExecCommand("LON"));
                }
            }

            if (currentCarrier.Count != 0 && ErrorReceived == false)
            {
                ProcessResult._result = true;
                ProcessResult._info = "SUCCESS";
                backgroundWorker_Scan2.ReportProgress(100, ProcessResult);
            }
            else
            {
                ProcessResult._result = false;
                ProcessResult._info = "Error while reading!";
                backgroundWorker_Scan2.ReportProgress(100, ProcessResult);
            }
        }

        private void backgroundWorker_Scan2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            var arg = e.UserState as HelperComEventArgs;
            if (arg._result == true)
            {

                if (!String.IsNullOrEmpty(CarrierName) && tableLayoutPanelFrame.Controls.Count == 0)
                {
                    var match = configuredProjects.Where(p => CarrierName.Contains(p.ProjectName)).FirstOrDefault();
                    if (match != null)
                    {
                        _pcbtotalNumber = match.Columns * match.Rows;
                        GenerateInterface(match.Columns, match.Rows);
                    }
                }

                if (noOfScanners > 2)
                {
                    log.Info("Application does not support more than 2 scanners");
                    logger.Info("Application does not support more than 2 scanners");
                    backgroundWorker_Scan2.CancelAsync();
                }
                else
                {
                    log.Info(scannerName + " completed work");
                    logger.Info(scannerName + " completed work");
                    backgroundWorkerMain.RunWorkerAsync();//switch to message send via tcp ip
                    ReadIsExecuting = false;
                }
            }
            else
            {
                logger.Error(arg._info);
            }

            #region Commented area
            //if (ErrorReceived == true)
            //{
            //    DialogResult result = MessageBox.Show("EMPTY SLOTS ==> PRESS YES\r\nSCANNER ERROR ==> PRESS NO", "OPERATOR CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if (result == DialogResult.Yes)
            //    {
            //        logger.Info("{0} completed work", scannerName);
            //        backgroundWorkerMain.RunWorkerAsync();//switch to message send
            //        ErrorReceived = false;
            //        ReadIsExecuting = false;
            //    }
            //    else
            //    {
            //        MessageBox.Show("PRESS BUTTON AGAIN");
            //        ErrorReceived = false;
            //        ReadIsExecuting = false;

            //    }
            //}
            //else
            //{
            //    logger.Info(scannerName + " completed work");
            //    backgroundWorkerMain.RunWorkerAsync();//switch to message send via tcp ip
            //    ReadIsExecuting = false;
            //}
            #endregion
        }
        #endregion

        #region MES Thread
        private void backgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
        {
            //Init transport helper class
            HelperComEventArgs ProcessResult = new HelperComEventArgs();
            string ReceivedInfo = string.Empty;

            ClearInterface();

            while (backgroundWorkerMain.CancellationPending == false)
            {

                if (String.IsNullOrEmpty(CarrierName) && String.IsNullOrWhiteSpace(CarrierName))
                {
                    
                    ProcessResult._info = "MISSING CARRIER";
                    ProcessResult._result = false;
                    backgroundWorkerMain.ReportProgress(100, ProcessResult);
                    break;
                }

                if (currentCarrier.Count < _pcbtotalNumber)
                {
                    DialogResult result = MessageBox.Show("EMPTY SLOTS? ==> PRESS YES\r\nSCANNER ERROR ==> PRESS NO", "OPERATOR CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        logger.Info("Work will be resumed...");
                    }
                    else
                    {
                        ProcessResult._info = "SCANNER ERROR";
                        ProcessResult._result = false;
                        backgroundWorkerMain.ReportProgress(100, ProcessResult);
                        break;
                    }
                }

                if (backgroundWorkerMain.CancellationPending == false)
                {
                    if (IsFirstRun == true)
                    {
                        //send all necessary MES messages
                        Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgPing(StationName));
                        ReceivedInfo = Client.Receive();
                        if (!ReceivedInfo.Contains("ERROR"))
                        {
                            Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgIdentification(StationName));
                            ReceivedInfo = Client.Receive();
                            if (!ReceivedInfo.Contains("ERROR"))
                            {
                                Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgReqTimeDate(StationName));
                                ReceivedInfo = Client.Receive();

                                if (ReceivedInfo.Contains("ERROR"))
                                {
                                    ProcessResult._info = "STATION IDENTIFICATION ERROR";
                                    ProcessResult._result = false;
                                    backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                    break;
                                }
                            }
                            else
                            {
                                ProcessResult._info = "STATION IDENTIFICATION ERROR";
                                ProcessResult._result = false;
                                backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                break;
                            }
                        }
                        else
                        {
                            ProcessResult._info = "STATION IDENTIFICATION ERROR";
                            ProcessResult._result = false;
                            backgroundWorkerMain.ReportProgress(100, ProcessResult);
                            break;
                        }

                        IsFirstRun = false;
                    }

                    if (!ReceivedInfo.Contains("ERROR"))
                    {
                        foreach(var board in currentCarrier)
                        {
                            Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgReqUnitInfo(board.BoardSerialNo, unitIdType, StationName));
                            ReceivedInfo = Client.Receive();
                            if (!ReceivedInfo.Contains("ERROR"))
                            {
                                string material = ReturnMaterial(ReceivedInfo);
                                board.ProductA2C = material;
                            }
                            else
                            {
                                ProcessResult._result = false;
                                ProcessResult._info = "UNIT INFO ERROR";
                                ProcessResult._failPosition = board.BoardPosition;
                                board.BoardResult = "F";
                                backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                break;
                            }
                        }

                        if (ValidateBoardsA2CvsConfigA2C(currentCarrier, configuredProjects) == true)
                        {
                            foreach (var board in currentCarrier)
                            {
                                Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgUnitProgress(board.BoardSerialNo, StationName));
                                ReceivedInfo = Client.Receive();
                                if (!ReceivedInfo.Contains("ERROR"))
                                {
                                    board.BoardResult = "P";
                                }
                                else
                                {
                                    board.BoardResult = "F";
                                }
                            }

                            if (CheckCarrierResults(currentCarrier) == true)
                            {
                                Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgReqCarrierInfo(CarrierName, StationName));
                                ReceivedInfo = Client.Receive();
                                int noOfBoardsOnCarrierNow = Convert.ToInt32(ReceivedInfo);

                                if (noOfBoardsOnCarrierNow > 0)
                                {
                                    Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgCarrierAssign(StationName, currentCarrier[0].CarrierId, "FRAME", currentCarrier));
                                    ReceivedInfo = Client.Receive();
                                    if (!ReceivedInfo.Contains("ERROR"))
                                    {
                                        ProcessResult._result = true;
                                        ProcessResult._info = "SUCCES";
                                        backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                    }
                                    else
                                    {
                                        ProcessResult._result = false;
                                        ProcessResult._info = "FAILED ASSIGN CARRIER";
                                        backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                        break;
                                    }
                                }
                                else
                                {
                                    ProcessResult._result = false;
                                    ProcessResult._info = "CARRIER NOT REALEASED";
                                    backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                    break;

                                }
                            }
                            else
                            {
                                ProcessResult._result = false;
                                ProcessResult._info = "FAIL BOARDS";
                                backgroundWorkerMain.ReportProgress(100, ProcessResult);
                                break;
                            }
                        }
                        else
                        {
                            ProcessResult._result = false;
                            ProcessResult._info = "FAIL A2C VALIDATION";
                            backgroundWorkerMain.ReportProgress(100, ProcessResult);
                            break;
                        }

                    }
                    else
                    {
                        ProcessResult._result = false;
                        ProcessResult._info = "FAIL MES COM";
                        backgroundWorkerMain.ReportProgress(100, ProcessResult);
                        break;
                    }
                }

                #region Commented
                //if (!ReceivedInfo.Contains("ERROR"))
                //{
                //    string material = ReturnMaterial(ReceivedInfo);

                //    if (MaterialWatch != material)
                //    {
                //        MaterialWatch = material;
                //        Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgSetupChange(material, StationName));
                //        ReceivedInfo = Client.Receive();
                //        if (!ReceivedInfo.Contains("ERROR"))
                //        {
                //            foreach (var board in currentCarrier)
                //            {
                //                Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgUnitProgress(board.BoardSerialNo, StationName));
                //                ReceivedInfo = Client.Receive();
                //                if (!ReceivedInfo.Contains("ERROR"))
                //                {
                //                    board.BoardResult = "P";
                //                }
                //                else
                //                {
                //                    board.BoardResult = "F";
                //                }
                //            }

                //            if (CheckCarrierResults(currentCarrier) == true)
                //            {
                //                Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgReqCarrierInfo(CarrierName, StationName));
                //                ReceivedInfo = Client.Receive();
                //                int noOfBoardsOnCarrierNow = Convert.ToInt32(ReceivedInfo);

                //                if (noOfBoardsOnCarrierNow > 0)
                //                {
                //                    Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgCarrierAssign(StationName, currentCarrier[0].CarrierId, "FRAME", currentCarrier));
                //                    ReceivedInfo = Client.Receive();
                //                    if (!ReceivedInfo.Contains("ERROR"))
                //                    {
                //                        backgroundWorkerMain.ReportProgress(100);
                //                    }
                //                    else
                //                    {
                //                        backgroundWorkerMain.ReportProgress(100);
                //                    }
                //                }
                //                else
                //                {
                //                    Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgCarrierAssign(StationName, currentCarrier[0].CarrierId, "FRAME", currentCarrier));
                //                    ReceivedInfo = Client.Receive();
                //                    if (!ReceivedInfo.Contains("ERROR"))
                //                    {
                //                        backgroundWorkerMain.ReportProgress(100);
                //                    }
                //                    else
                //                    {
                //                        backgroundWorkerMain.ReportProgress(100);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        foreach (var board in currentCarrier)
                //        {
                //            Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgUnitProgress(board.BoardSerialNo, StationName));
                //            ReceivedInfo = Client.Receive();
                //            if (!ReceivedInfo.Contains("ERROR"))
                //            {
                //                board.BoardResult = "P";
                //            }
                //            else
                //            {
                //                board.BoardResult = "F";
                //            }
                //        }

                //        if (CheckCarrierResults(currentCarrier) == true)
                //        {
                //            Client.Send(AmtestCommunicator.UitlityClasses.Messages.GetMsgCarrierAssign(StationName, currentCarrier[0].CarrierId, "FRAME", currentCarrier));
                //            ReceivedInfo = Client.Receive();
                //            if (!ReceivedInfo.Contains("ERROR"))
                //            {
                //                backgroundWorkerMain.ReportProgress(100);
                //            }
                //            else
                //            {
                //                backgroundWorkerMain.ReportProgress(100);
                //            }
                //        }
                //        else
                //        {
                //            backgroundWorkerMain.ReportProgress(100);
                //        }
                //    }
                //}
                #endregion
            }
        }

        private void backgroundWorkerMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var arg = e.UserState as HelperComEventArgs;
            if(arg._result == true)
            {
                UpdateInterface(currentCarrier);
                SendOutSignal();
                backgroundWorkerMain.CancelAsync();
                log.Info(arg._info);
                logger.Info(arg._info);
            }
            else
            {
                if(arg._info == "MISSING CARRIER")
                {
                    log.Error(arg._info);
                    logger.Error(arg._info);
                    backgroundWorkerMain.CancelAsync();
                }

                if(arg._info == "SCANNER ERROR")
                {
                    log.Debug("PRESS BUTTON AGAIN");
                    logger.Debug("PRESS BUTTON AGAIN");
                    backgroundWorkerMain.CancelAsync();
                }

                if(arg._info == "FAIL BOARDS" )
                {
                    log.Warn(arg._info + " replace boards and restart process");
                    logger.Warn(arg._info + " replace boards and restart process");
                    backgroundWorkerMain.CancelAsync();
                }

                if(arg._info == "FAIL A2C VALIDATION")
                {
                    log.Warn(arg._info + " Carrier contains boards that don't belong to current project. Replace boards and restart process");
                    logger.Warn(arg._info+" Carrier contains boards that don't belong to current project. Replace boards and restart process");
                    backgroundWorkerMain.CancelAsync();
                }

                if(arg._info == "UNIT INFO ERROR")
                {
                    UpdateInterface(currentCarrier);
                    log.Warn("Board info error on position " + arg._failPosition);
                    logger.Warn("Board info error on position {0} ", arg._failPosition);
                    backgroundWorkerMain.CancelAsync();
                }

                if(arg._info == "STATION IDENTIFICATION ERROR")
                {
                    log.Warn("Failed to identify station");
                    logger.Warn("Failed to identify station");
                    backgroundWorkerMain.CancelAsync();
                }

                if (arg._info == "FAILED ASSIGN CARRIER")
                {
                    log.Warn("Failed to assign all boards to carrier");
                    logger.Warn("Failed to assign all boards to carrier");
                    backgroundWorkerMain.CancelAsync();
                }

                if (arg._info == "CARRIER NOT REALEASED")
                {
                    log.Warn("Failed to assign all boards to carrier");
                    logger.Warn("Failed to assign all boards to carrier");
                    backgroundWorkerMain.CancelAsync();
                }

                if(arg._info == "FAIL MES COM")
                {
                    log.Warn("Communication with MES failes");
                    logger.Warn("Communication with MES failes");
                    backgroundWorkerMain.CancelAsync();
                }
            }
        }

        private void backgroundWorkerMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorkerMain.CancelAsync();
        }
        #endregion

        #region Utility functions
        private void EditReceivedData(string data)
        {
            try
            {
                data = data.Trim(new char[] { '\u0002', '\u0003' });
                string[] _data = data.Split(',');
                List<CarrierModel> _2ndScanCarrier = new List<CarrierModel>();

                foreach (var board in _data)
                {
                    string _board = board.ToUpper();
                    if (!_board.Contains("TOP") && !_board.Contains("BOT"))
                    {
                        string serial;
                        CarrierModel _currentCarrierData = new CarrierModel();

                        serial = _board.Split(':').First();
                        _currentCarrierData.BoardPosition = Convert.ToInt32(board.Split(':').Last());
                        _currentCarrierData.BoardSerialNo = serial;
                        
                            currentCarrier.Add(_currentCarrierData);
                      
                    }
                    else
                    {
                        CarrierName = board.Split(':').First();
                        continue;
                    }
                }

                currentCarrier.Sort(delegate (CarrierModel x, CarrierModel y)
                {
                    return x.BoardPosition.CompareTo(y.BoardPosition);
                });
            }
            catch(Exception ex)
            {
                log.Error(ex.Message.ToString());
                logger.Error(ex.Message.ToString());
            }           
        }

        public void GenerateInterface(int columnCount,int rowCount)
        {
            //Clear out the existing controls and generate new
            tableLayoutPanelFrame.Controls.Clear();
            tableLayoutPanelFrame.RowStyles.Clear();
            //Generate table based on row column input
            tableLayoutPanelFrame.ColumnCount = columnCount;
            tableLayoutPanelFrame.RowCount = rowCount;
            int cellcount = 1;

            for (int x = 0; x < rowCount; x++)
            {
                //Add columns
                tableLayoutPanelFrame.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                for (int y = 0; y < columnCount; y++)
                {
                    //Add a row
                    if (x == 1)
                    {
                        tableLayoutPanelFrame.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                    }

                    //Create new position in grid
                    Button cmd = new Button();
                    cmd.Dock = DockStyle.Fill;
                    cmd.Enabled = false;
                    cmd.Font = new Font(cmd.Font.FontFamily, 30);
                    cmd.Text = string.Format("{0}{1}", "", cellcount);

                    tableLayoutPanelFrame.Controls.Add(cmd);
                    tableLayoutPanelFrame.PerformLayout();

                    cellcount++;
                }
            }
        }

        public void UpdateInterface(List<CarrierModel> Units)
        {
            tbCarrierLabel.ReadOnly = false;
            tbCarrierLabel.Text = currentCarrier[0].CarrierId;
            tbCarrierLabel.ReadOnly = true;
            foreach(Button _btnPosition in tableLayoutPanelFrame.Controls)
            {
                var unit = currentCarrier.Where(x => x.BoardPosition.ToString() == _btnPosition.Text).FirstOrDefault();
                if(unit != null)
                {
                    if(unit.BoardResult == "P")
                    {
                        _btnPosition.BeginInvoke(new MethodInvoker(() => { _btnPosition.Text = "POS_" + unit.BoardPosition + ": " + unit.BoardSerialNo; _btnPosition.BackColor = Color.Green; }));
                    }
                    else
                    {
                        if (unit.BoardResult == "F")
                        {
                            _btnPosition.BeginInvoke(new MethodInvoker(() => { _btnPosition.Text = "POS_" + unit.BoardPosition + ": " + unit.BoardSerialNo; _btnPosition.BackColor = Color.Red; }));
                        }
                        else
                        {
                            continue;
                        }
                    }  
                }
                else
                {
                    continue;
                }                
            }
        }

        public void ReadProjectConfig()
        {
            try
            {
                string Projects = Properties.Settings.Default.Projects;
                Projects = Projects.Replace("\r\n", string.Empty);
                string[] _projects = Projects.Split('*');

                for (int i = 0; i < _projects.Count(); i++)
                {
                    _projects[i] = _projects[i].Trim();
                    if(!String.IsNullOrEmpty(_projects[i]) && !String.IsNullOrWhiteSpace(_projects[i]))
                    {
                        string _tempProducts = "";
                        _tempProducts = _projects[i].GetStringInBetween("Products={", "}]", false, false).FirstOrDefault().ToString();

                        string[] _tempA2Cs = _tempProducts.Split(',').ToArray();
                        List<string> _tempProductList = new List<string>();

                        _tempProductList = _tempA2Cs.ToList();

                        configuredProjects.Add(new Projects
                        {
                            ProjectName = _projects[i].GetStringInBetween("[ProjectName=", "Rows=", false, false).First().ToString(),
                            Rows = Convert.ToInt32(_projects[i].GetStringInBetween("Rows=", "Cols=", false, false).First().ToString()),
                            Columns = Convert.ToInt32(_projects[i].GetStringInBetween("Cols=", "Products=", false, false).First().ToString()),
                            ProductA2Cs = _tempProductList,
                        });
                    }                         
                }

                log.Info("Reading configuration file completed.");
                logger.Info("Reading configuration file completed.");
            }
            catch(Exception ex)
            {
                log.Error("Error while reading configuration file: " + ex.Message);
                logger.Error("Error while reading configuration file: {0}", ex.Message);
            }  
        }

        public void SearchScannersAndConnect()
        {
            try
            {
                if (!m_searcher.IsSearching)
                {
                    m_searcher.SelectedNicSearchResult = m_nicList.Where(x => x.NicIpAddr.Contains(ipModel)).First();

                    log.Info("Searching for scanners!");
                    logger.Info("Searching for scanners!");
                    //Start searching readers.
                    m_searcher.Start((res) =>
                    {
                        //Define searched actions here.Defined actions work asynchronously.
                        //"SearchListUp" works when a reader was searched.^
                        BeginInvoke(new delegateUserControl(SearchListUp), res.IpAddress);
                    });
                } 
            }
            catch(System.Exception ex)
            {
                log.Error("Could not connect to scanners! " + ex.Message);
                logger.Error("Could not connect to scanners! " + ex.Message);
                buttonScanners.BackColor = Color.Red;
            }
        }

        private void SendOutSignal()
        {
            // read Di port state
            Automation.BDaq.ErrorCode err = Automation.BDaq.ErrorCode.Success;
            byte portData = 1;
            int intValue = 1;
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            byte[] result = intBytes;

            instantDoCtrl.Write(_usbDevicePortOut, portData);
            intValue = 0;
            intBytes = BitConverter.GetBytes(intValue);
            Array.Reverse(intBytes);
            result = intBytes;
            Thread.Sleep(500);
            err = instantDoCtrl.WriteBit(_usbDevicePortOut, _usbDevicePortOutPin, result[0]);
        }

        //delegateUserControl is for controlling UserControl from other threads.
        private delegate void delegateUserControl(string str);
        private void SearchListUp(string ip)
        {
            try
            {
                if (ip != "")
                {
                    ipListofScanners.Add(ip);
                    log.Info("Scanner found at address: "+ip);
                    logger.Info("Scanner found at address: {0}", ip);

                    if (ipListofScanners.Count > 0)
                    {
                        foreach (var IP in ipListofScanners)
                        {
                            //Set ip address of ReaderAccessor.
                            m_reader.IpAddress = IP;
                            //Connect TCP/IP.
                            m_reader.Connect();
                        }
                        log.Info("Scanner connected: "+ip);
                        logger.Info("Scanner connected: {0}", ip);
                        buttonScanners.BackColor = Color.Green;
                        return;
                    }
                }
            }
            catch(System.Exception ex)
            {
                log.Error("Failed to connect scanners! " + ex.ToString());
                logger.Error("Failed to connect scanners! " + ex.ToString());
            }
        }
 
        private void ReceivedDataWrite(string receivedData)
        {
            try
            {
                var fulldatareceived = ("[" + m_reader.IpAddress + "][" + DateTime.Now + "]" + receivedData);

                if (receivedData != "")
                {
                    if (!receivedData.Contains("ERROR"))
                    {
                        EditReceivedData(receivedData);
                        foreach (var board in currentCarrier)
                        {
                            board.CarrierId = CarrierName;
                            log.Info("Serial: "+ board.BoardSerialNo+"=> Position: "+ board.BoardPosition);
                            logger.Info("Serial: {0} => Position:{1}", board.BoardSerialNo, board.BoardPosition);
                        }
                        ReadIsExecuting = true;
                    }
                    else
                    {
                        ErrorReceived = true;
                        ReadIsExecuting = true;
                        log.Error(scannerName + " [" + m_reader.IpAddress + "][" + DateTime.Now + "] Error reading data");
                        logger.Error(scannerName + " [" + m_reader.IpAddress + "][" + DateTime.Now + "] Error reading data");
                    }
                }
            }
            catch(System.Exception ex)
            {
                log.Error("Connection error while receiving data from scanner " + ex.Message.ToString());
                logger.Error("Connection error while receiving data from scanner " + ex.Message.ToString());
            }          
        }

        private string ReturnMaterial(string reqUnitInfoResp)
        { 
            string[] SplitReceivedData = null;
            string material = "";
            SplitReceivedData = reqUnitInfoResp.Split(',');
            if(!String.IsNullOrEmpty(SplitReceivedData[5]))
            {
                SplitReceivedData[5] = SplitReceivedData[5].Replace("\"", "");
                material = SplitReceivedData[5].GetStringInBetween("material=", " location=", false, false).First().ToString();
             }
            return material;
        }

        private bool CheckCarrierResults(List<CarrierModel> Units)
        {
            bool AllBoardsPass = false;

            for (int i = 0; i <= Units.Count; i++)
            {
                if (Units[i].BoardResult == "P")
                {
                    AllBoardsPass = true;
                }
                else
                {
                    AllBoardsPass = false;
                    break;
                }
            }

            if (AllBoardsPass == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int ReturnBoardsNo(string reqUnitInfoResp)
        {
            string[] SplitReceivedData = null;
            int boardsNo = 0;
            SplitReceivedData = reqUnitInfoResp.Split(',');
            if (!String.IsNullOrEmpty(SplitReceivedData[5]))
            {
                SplitReceivedData[5] = SplitReceivedData[5].Replace("\"", "");
                boardsNo = Convert.ToInt32(SplitReceivedData[5].GetStringInBetween("use_counter=", " last_mant=", false, false).First().ToString());
            }
            return boardsNo;
        }

        private void ClearInterface()
        {
            try
            {
                tbCarrierLabel.ReadOnly = false;
                tbCarrierLabel.Clear();
                tbCarrierLabel.ReadOnly = true;
                while (tableLayoutPanelFrame.Controls.Count > 0)
                {
                    tableLayoutPanelFrame.Controls[0].Dispose();
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex.ToString());
                logger.Error(ex.ToString());
            }
            return;
        }

        private bool ValidateBoardsA2CvsConfigA2C(List<CarrierModel> Units, List<Projects>_projects)
        {
            if(!String.IsNullOrEmpty(CarrierName))
            {
                bool _a2ccheckok = false;
                var match = configuredProjects.Where(p => CarrierName.Contains(p.ProjectName)).FirstOrDefault();

                foreach(var board in Units)
                {
                    if(match.ProductA2Cs.Contains(board.ProductA2C))
                    {
                        _a2ccheckok = true;
                        continue;
                    }
                    else
                    {
                        _a2ccheckok = false;
                        break;
                    }
                }
                return _a2ccheckok;                
            }
            else
            {
                return false;
            }   
        }
        #endregion
    }
}
