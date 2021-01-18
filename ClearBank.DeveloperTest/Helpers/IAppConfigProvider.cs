using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Helpers
{
    public interface IAppConfigProvider
    {
        AppSettingsProvider AppSettings { get; set; }
    }
}
