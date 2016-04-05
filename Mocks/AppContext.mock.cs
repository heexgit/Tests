using System;
using ExpertSender.Common;

namespace WebDaoTests.Mocks
{
    public class AppContext : IAppContext
    {
        public int? CurrentAccountId { get; set; }
        public int? CurrentServiceId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public TimeZoneInfo CurrentTimeZone { get; set; }
        public TimeZoneInfo CurrentDataTablesTimeZone { get; set; }
        public bool? DisableGoogleAnalytics { get; set; }

        public bool IsServiceSet()
        {
            return CurrentServiceId.HasValue;
        }
    }
}
