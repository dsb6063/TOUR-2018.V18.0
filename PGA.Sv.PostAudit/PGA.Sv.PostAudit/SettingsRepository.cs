using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGA.Database;
using PGA.DataContext;

namespace PGA.Sv.PostAudit
{
    class SettingsRepository:ISettings
    {
        private Settings _settings = null;

        public SettingsRepository()
        {
            
        }
        public Settings GetSettings()
        {
            throw new NotImplementedException();
        }

        public Settings GetSettings(DateTime dt)
        {
            using (DatabaseCommands commands = new DatabaseCommands() )
            {
                _settings = commands.GetSettingsByDate(dt);
            }

            return _settings;
        }

 
    }
}
