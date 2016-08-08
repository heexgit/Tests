using System;
using ExpertSender.Common.Helpers;

namespace CommonRuns
{
    internal class Program
    {
        private static ModelsRegister _modelsRegister;

        private static void Main()
        {
            _modelsRegister = new ModelsRegister();

            var link = GetClickLink();
        }

        private static string GetClickLink()
        {
            var link = LinkHelper.GetTrackedLink(_modelsRegister.Domain, _modelsRegister.ServiceId, _modelsRegister.TrackedLinkId, _modelsRegister.MessageGuid, _modelsRegister.ListId, _modelsRegister.SubscriberId);
            return link;
        }

        private class ModelsRegister
        {
            public string Domain { get; }
            public int ServiceId { get; }
            public int TrackedLinkId { get; }
            public Guid MessageGuid { get; }
            public int ListId { get; }
            public int SubscriberId { get; }

            public ModelsRegister()
            {
                Domain = "subscriber.dev.expertsender";
                ServiceId = 7;
                TrackedLinkId = 5;
                MessageGuid = new Guid("12345678-1234-1234-1234-123456789012");
                ListId = 6452; // External recipients
                SubscriberId = 365567881; //grzegorz.tylak@expertsender.com
            }
        }
    }
}