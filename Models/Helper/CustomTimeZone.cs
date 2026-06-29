using System;

namespace ExcelTranscript.Models.Helper
{
    public static class CustomTimeZone
    {
        private static DateTime _US_UTC_Time { get; set; }
        
        public static DateTime Get_US_UTC_Time()
        {
            _US_UTC_Time = DateTime.UtcNow.AddHours(-5);
            return _US_UTC_Time;
        }        
    }

}