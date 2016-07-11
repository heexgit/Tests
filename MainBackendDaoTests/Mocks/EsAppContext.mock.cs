using System;
using ExpertSender.Common;

namespace MainBackendDaoTests.Mocks
{
    public class EsAppContext : IAppContext
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
