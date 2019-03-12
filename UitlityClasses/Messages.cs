using AmtestCommunicator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmtestCommunicator.UitlityClasses
{

    public static class Messages
    {
        public static int iMsgCounter = MainForm.Counter;

        public static string GetMsgPing(string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            return "\u0002" + "PING," + StationName + "," + iMsgCounter + "," + GetDateTime() + "\u0003";
        }

        public static string GetMsgIdentification(string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            return "\u0002" + "IDENTIFICATION," + StationName + "," + iMsgCounter + "," + GetDateTime() + "\u0003";
        }

        public static string GetMsgReqTimeDate(string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            return "\u0002" + "REQ_TIME_DATE," + StationName + "," + iMsgCounter + "," + GetDateTime() + "\u0003";
        }

        public static string GetMsgReqCarrierInfo(string CarrierId, string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            string message = ",<ReqCarrierUnits tokens=" + @"""2""" + " cid=" + @"""" + CarrierId + @"""" + @"" + "/>";
            return "\u0002" + "REQ_CARRIER_UNITS," + StationName + "," + iMsgCounter + "," + GetDateTime() + message + "\u0003";
        }

        public static string GetMsgReqUnitInfo(string uid, string uid_type, string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            string message = ",<ReqUnitInfo tokens=" + @"""3""" + " uid=" + @"""" + uid + @"""" + " uid_type=" + @"""" + uid_type + @"""" + @"" + "/>";
            return "\u0002" + "REQ_UNIT_INFO," + StationName + "," + iMsgCounter + "," + GetDateTime() + message + "\u0003";
        }

        public static string GetMsgSetupChange(string Material, string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            string message = ",<SetupChange tokens=" + @"""2""" + " mat_in=" + @"""" + Material + @"""" + @"" + "/>";
            return "\u0002" + "SETUP_CHANGE," + StationName + "," + iMsgCounter + "," + GetDateTime() + message + "\u0003";
        }

        public static string GetMsgUnitProgress(string UnitId, string StationName)
        {
            MainForm.Counter = iMsgCounter++;
            string message = ",<UnitProgress tokens=" + @"""2""" + " uid_in=" + @"""" + UnitId + @"""" + @"" + "/>";
            return "\u0002" + "UNIT_PROGRESS," + StationName + "," + iMsgCounter + "," + GetDateTime() + message + "\u0003";
        }

        public static string GetMsgCarrierAssign(string StationName, string carrierId, string carrierIdType, List<CarrierModel> Units)
        {
            MainForm.Counter = iMsgCounter++;
            StringBuilder variableMessageBody = new StringBuilder();

            foreach (var board in Units)
            {
                variableMessageBody.Append(" uid_" + board.BoardPosition + "=" + @"""" + board.BoardSerialNo + '"' + " status_" + board.BoardPosition + "=" + @"""" + board.BoardResult + @"""" + " cid_pos_" + board.BoardPosition + "=" + @"""" + board.BoardPosition + @"""");
            }

            string mainMessageBody = ",<CarrierAssign tokens=" + @"""" + (Units.Count() * 3 + 3).ToString() + @"""" + " cid=" + @"""" + carrierId + @"""" + " cid_type=" + @"""" + carrierIdType + '"' + variableMessageBody + @"" + "/>";
            return "\u0002" + "CARRIER_ASSIGN," + StationName + "," + iMsgCounter + "," + GetDateTime() + mainMessageBody + "\u0003";
        }

        public static string GetDateTime()
        {
            DateTime dt = DateTime.Now;
            //sDateTime = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
            return dt.ToString("yyyyMMddHHmmss");
        }
    }
}
