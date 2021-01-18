using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ClearBank.DeveloperTest.Helpers
{
    public class AppSettingsProvider
    {
        public string this[string key]
        {
            get
            {
                string customValue;
                return CustomValues.TryGetValue(key, out customValue) ? customValue : ConfigurationManager.AppSettings[key];
            }
        }

        public Dictionary<string, string> CustomValues { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
