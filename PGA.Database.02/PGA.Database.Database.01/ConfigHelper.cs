#region

using System.Configuration;

#endregion

namespace PGA.Database
{
    public static class ConfigHelper
    {
        public static void WriteToConfigFile(string filename)
        {
            //string ConStrng = ConfigurationSettings.AppSettings["ConnectionString"];
            var ConStrng = ConfigurationManager.AppSettings["ConnectionString"];
            //var sss = "Data Source=";
            //var xxx = ";Initial Catalog=AlfalahScholarship;Integrated Security=True";
            //ConfigurationSettings.AppSetting;
            var config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Get the appSettings section.
            var appSettings = (AppSettingsSection) config.GetSection("appSettings");
            appSettings.Settings.Remove("ConnectionString");
            // appSettings.Settings.Add("ConnectionString", sss + txtServerName.Text + xxx);
            appSettings.Settings.Add("ConnectionString", filename);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }


        public static ConnectionStringSettings save_new_connection(string filename)
        {
            var ConStrng = ConfigurationManager.ConnectionStrings.ToString();
            var conSetting = new ConnectionStringSettings();

            conSetting.ConnectionString = filename;
            conSetting.Name = "PGAEntities";
            conSetting.ProviderName = "System.Data.EntityClient";

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var conSettings = (ConnectionStringsSection) config.GetSection("connectionStrings");
            conSettings.ConnectionStrings.Remove(conSetting);
            conSettings.ConnectionStrings.Add(conSetting);
            conSettings.ConnectionStrings.IndexOf(conSetting);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

            return conSetting;
        }
    }
}