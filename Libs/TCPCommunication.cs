using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using AmtestCommunicator;
using NLog;


namespace AmtestCommunicator.Libs
{
    public class TCPCommunication
    {
        private Socket SocketMain { get; set; }
     
        public TCPCommunication()
        {
          
            SocketMain = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public string Connect(IPAddress ipAddress, int port)
        {
            string result = string.Empty;
            try
            {
                SocketMain.Connect(new IPEndPoint(ipAddress, port)); //connect to the GHP
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result;
        }

        public string Send(string message)
        {
            string result = string.Empty;
            MainForm.logger.Info(message);
            try
            {

                SocketMain.Send(ConvertStringToByteArrayToSend(message));
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }

            return result;
        }

        public string Receive()
        {
            string result = string.Empty;
            string[] SplitReceivedData = null;
            string response = string.Empty;
            byte[] buffer = new byte[1024];

            try
            {
                var count = SocketMain.Receive(buffer);

                if (count < 4)
                {
                    count = SocketMain.Receive(buffer);
                    var temp = new byte[count];
                    Array.Copy(buffer, 0, temp, 0, count - 0);
                    result = Encoding.ASCII.GetString(temp);
                    SplitReceivedData = result.Split(',');

                    if ((SplitReceivedData[4].Contains("ACK")) && (!result.Contains("ERROR")))
                    {
                        
                        response = result;
                        MainForm.log.Info(response);
                        MainForm.logger.Info(response);
                    }
                    else
                    {
                        response = result;
                        MainForm.log.Info(response);
                        MainForm.logger.Warn(response);
                    }
                }
                else
                {
                    var temp = new byte[count];
                    Array.Copy(buffer, 0, temp, 0, count - 0);
                    result = Encoding.ASCII.GetString(temp);
                    SplitReceivedData = result.Split(',');
                    result = CleanInvalidXmlChars(result);
                    if ((SplitReceivedData[4].Contains("ACK")) && (!result.Contains("ERROR")))
                    {
                        response = result;
                        MainForm.log.Info(response);
                        MainForm.logger.Info(response);
                    }
                    else
                    {
                        response = result;
                        MainForm.log.Info(response);
                        MainForm.logger.Warn(response);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MainForm.log.Info(ex.ToString());
                MainForm.logger.Error(ex.ToString);
            }

            return response;
        }

        public static string CleanInvalidXmlChars(string text)
        {
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        public static byte[] ConvertStringToByteArrayToSend(string data)
        {
            byte[] dataToArray = Encoding.ASCII.GetBytes(data);
            int dataByteCount = Encoding.ASCII.GetByteCount(data);
            byte[] dataByteCountToArray = (BitConverter.GetBytes(dataByteCount)).Reverse().ToArray();
            var result = dataByteCountToArray.Concat(dataToArray).ToArray();
            return dataToArray;
        }
    }
}
