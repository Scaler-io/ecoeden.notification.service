﻿namespace Ecoeden.Notification.Service.Models.Notifications
{
    public class TemplateFields
    {
        public TemplateFields(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
