using Automation.BDaq;

namespace AmtestCommunicator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnOutput = new System.Windows.Forms.Button();
            this.buttonScanners = new System.Windows.Forms.Button();
            this.buttonMES = new System.Windows.Forms.Button();
            this.groupBoxWitnesses = new System.Windows.Forms.GroupBox();
            this.btnInput = new System.Windows.Forms.Button();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.richTextBoxLogger = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanelFrame = new System.Windows.Forms.TableLayoutPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.instantDiCtrl1 = new Automation.BDaq.InstantDiCtrl(this.components);
            this.instantDoCtrl = new Automation.BDaq.InstantDoCtrl(this.components);
            this.backgroundWorkerMain = new System.ComponentModel.BackgroundWorker();
            this.receive = new System.Windows.Forms.Button();
            this.backgroundWorker_Scan1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_Scan2 = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCarrierLabel = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxWitnesses.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(145, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnOutput
            // 
            this.btnOutput.BackColor = System.Drawing.Color.Red;
            this.btnOutput.Location = new System.Drawing.Point(6, 48);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(75, 23);
            this.btnOutput.TabIndex = 3;
            this.btnOutput.Text = "Output";
            this.btnOutput.UseVisualStyleBackColor = false;
            // 
            // buttonScanners
            // 
            this.buttonScanners.BackColor = System.Drawing.Color.Red;
            this.buttonScanners.Location = new System.Drawing.Point(6, 77);
            this.buttonScanners.Name = "buttonScanners";
            this.buttonScanners.Size = new System.Drawing.Size(75, 23);
            this.buttonScanners.TabIndex = 4;
            this.buttonScanners.Text = "Scanners";
            this.buttonScanners.UseVisualStyleBackColor = false;
            // 
            // buttonMES
            // 
            this.buttonMES.BackColor = System.Drawing.Color.Red;
            this.buttonMES.Location = new System.Drawing.Point(6, 105);
            this.buttonMES.Name = "buttonMES";
            this.buttonMES.Size = new System.Drawing.Size(75, 23);
            this.buttonMES.TabIndex = 5;
            this.buttonMES.Text = "MES";
            this.buttonMES.UseVisualStyleBackColor = false;
            // 
            // groupBoxWitnesses
            // 
            this.groupBoxWitnesses.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxWitnesses.Controls.Add(this.btnInput);
            this.groupBoxWitnesses.Controls.Add(this.btnOutput);
            this.groupBoxWitnesses.Controls.Add(this.buttonMES);
            this.groupBoxWitnesses.Controls.Add(this.buttonScanners);
            this.groupBoxWitnesses.Location = new System.Drawing.Point(12, 87);
            this.groupBoxWitnesses.Name = "groupBoxWitnesses";
            this.groupBoxWitnesses.Size = new System.Drawing.Size(89, 134);
            this.groupBoxWitnesses.TabIndex = 6;
            this.groupBoxWitnesses.TabStop = false;
            this.groupBoxWitnesses.Text = "Witnesses";
            // 
            // btnInput
            // 
            this.btnInput.BackColor = System.Drawing.Color.Red;
            this.btnInput.Location = new System.Drawing.Point(6, 19);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(75, 23);
            this.btnInput.TabIndex = 6;
            this.btnInput.Text = "Input";
            this.btnInput.UseVisualStyleBackColor = false;
            // 
            // groupBoxLog
            // 
            this.groupBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLog.Controls.Add(this.richTextBoxLogger);
            this.groupBoxLog.Location = new System.Drawing.Point(12, 249);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(877, 267);
            this.groupBoxLog.TabIndex = 7;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "Logger";
            // 
            // richTextBoxLogger
            // 
            this.richTextBoxLogger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLogger.Location = new System.Drawing.Point(6, 19);
            this.richTextBoxLogger.Name = "richTextBoxLogger";
            this.richTextBoxLogger.Size = new System.Drawing.Size(865, 242);
            this.richTextBoxLogger.TabIndex = 0;
            this.richTextBoxLogger.Text = "";
            // 
            // tableLayoutPanelFrame
            // 
            this.tableLayoutPanelFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelFrame.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetPartial;
            this.tableLayoutPanelFrame.ColumnCount = 1;
            this.tableLayoutPanelFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelFrame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelFrame.Location = new System.Drawing.Point(163, 31);
            this.tableLayoutPanelFrame.Name = "tableLayoutPanelFrame";
            this.tableLayoutPanelFrame.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanelFrame.RowCount = 1;
            this.tableLayoutPanelFrame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelFrame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanelFrame.Size = new System.Drawing.Size(726, 221);
            this.tableLayoutPanelFrame.TabIndex = 8;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // instantDiCtrl1
            // 
            this.instantDiCtrl1._StateStream = ((Automation.BDaq.DeviceStateStreamer)(resources.GetObject("instantDiCtrl1._StateStream")));
            // 
            // instantDoCtrl
            // 
            this.instantDoCtrl._StateStream = ((Automation.BDaq.DeviceStateStreamer)(resources.GetObject("instantDoCtrl._StateStream")));
            // 
            // backgroundWorkerMain
            // 
            this.backgroundWorkerMain.WorkerReportsProgress = true;
            this.backgroundWorkerMain.WorkerSupportsCancellation = true;
            this.backgroundWorkerMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerMain_DoWork);
            this.backgroundWorkerMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerMain_ProgressChanged);
            this.backgroundWorkerMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerMain_RunWorkerCompleted);
            // 
            // receive
            // 
            this.receive.Location = new System.Drawing.Point(304, 523);
            this.receive.Name = "receive";
            this.receive.Size = new System.Drawing.Size(75, 23);
            this.receive.TabIndex = 10;
            this.receive.Text = "Receive";
            this.receive.UseVisualStyleBackColor = true;
            this.receive.Visible = false;
            // 
            // backgroundWorker_Scan1
            // 
            this.backgroundWorker_Scan1.WorkerReportsProgress = true;
            this.backgroundWorker_Scan1.WorkerSupportsCancellation = true;
            this.backgroundWorker_Scan1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_Scan1_DoWork);
            this.backgroundWorker_Scan1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_Scan1_ProgressChanged);
            // 
            // backgroundWorker_Scan2
            // 
            this.backgroundWorker_Scan2.WorkerReportsProgress = true;
            this.backgroundWorker_Scan2.WorkerSupportsCancellation = true;
            this.backgroundWorker_Scan2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_Scan2_DoWork);
            this.backgroundWorker_Scan2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_Scan2_ProgressChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(174, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "Carrier Label:";
            // 
            // tbCarrierLabel
            // 
            this.tbCarrierLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCarrierLabel.Location = new System.Drawing.Point(302, 2);
            this.tbCarrierLabel.Name = "tbCarrierLabel";
            this.tbCarrierLabel.ReadOnly = true;
            this.tbCarrierLabel.Size = new System.Drawing.Size(299, 35);
            this.tbCarrierLabel.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 550);
            this.Controls.Add(this.tbCarrierLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.receive);
            this.Controls.Add(this.tableLayoutPanelFrame);
            this.Controls.Add(this.groupBoxLog);
            this.Controls.Add(this.groupBoxWitnesses);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "AmtestCommunicator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxWitnesses.ResumeLayout(false);
            this.groupBoxLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button buttonScanners;
        private System.Windows.Forms.Button buttonMES;
        private System.Windows.Forms.GroupBox groupBoxWitnesses;
        private System.Windows.Forms.GroupBox groupBoxLog;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanelFrame;
        private System.Windows.Forms.Timer timer1;
        private  Automation.BDaq.InstantDiCtrl instantDiCtrl1;
        private Automation.BDaq.InstantDoCtrl instantDoCtrl;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.RichTextBox richTextBoxLogger;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMain;
        private System.Windows.Forms.Button receive;
        private System.ComponentModel.BackgroundWorker backgroundWorker_Scan1;
        private System.ComponentModel.BackgroundWorker backgroundWorker_Scan2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCarrierLabel;
    }
}


