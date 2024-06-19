using System.Security.Cryptography;

namespace MVCTest1.Models
{

    public class LogSuccessData
    {
        public string Id { get; set; }
        public string Time { get; set; } // 시간과 날짜가 포함된 문자열
        public string Commute { get; set; }
        public byte[] Face2 { get; set; }

        // 날짜만 추출하는 속성 추가
        public DateTime Date
        {
            get
            {
                DateTime dateTime;
                if (DateTime.TryParse(Time, out dateTime))
                {
                    Console.WriteLine($"test  {dateTime.Date}");
                    return dateTime.Date;
                }
                return DateTime.MinValue;
            }
        }
    }


}


