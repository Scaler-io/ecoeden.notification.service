﻿namespace Ecoeden.Notification.Service.Configurations
{
    public sealed class AppOptions
    {
        public const string OptionName = "AppConfigurations";
        public string ApplicationIdentifier { get; set; }
        public string ApplicationEnvironment { get; set; }
        public string NotificationProcessInterval { get; set; }
        public string IntervalUnit { get; set; }
    }
}
