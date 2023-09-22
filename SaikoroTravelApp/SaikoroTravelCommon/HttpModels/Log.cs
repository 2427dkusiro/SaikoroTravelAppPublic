using SaikoroTravelCommon.Models;

using System;

namespace SaikoroTravelCommon.HttpModels
{
    public class Log
    {
        public int Id { get; set; }

        public LogLevel LogLevel { get; set; }

        public string? Message { get; set; }

        public Users User { get; set; }

        public DateTime TimeStamp { get; set; }

        public string? FilePath { get; set; }

        public string? Writer { get; set; }

        public int WriterLine { get; set; }

        public string? AdditionalObject { get; set; }

        public bool IsPosted { get; set; }
    }

    public enum LogLevel
    {
        Error = 0,
        Warning = 1,
        Info = 2,
        Debug = 4,
        Trace = 8,
        // 
        Report_Instruction_Timer = 256,
        Report_Lottery_Timer = 512,
        Report_Location = 1024,

        Event_Instruction_Opened = 65536,
        Event_Lottery_Opened = 131072,
    }

    public class LotteryReport
    {
        public string Id { get; set; }

        public int Number { get; set; }
    }

    public class LocationReport
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double? Altitude { get; set; }

        public string PostCode { get; set; }

        public string Name { get; set; }
    }
}
