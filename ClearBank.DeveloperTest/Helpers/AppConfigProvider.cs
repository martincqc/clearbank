namespace ClearBank.DeveloperTest.Helpers
{
    public class AppConfigProvider: IAppConfigProvider
    {
        public AppConfigProvider()
        {
            AppSettings = new AppSettingsProvider();
        }

        public AppSettingsProvider AppSettings { get; set; }
    }
}
