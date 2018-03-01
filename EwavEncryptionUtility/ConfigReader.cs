/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       ConfigReader.cs
 *  Namespace:  EwavEncryptionUtility    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    24/07/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
namespace EpiInfoWebSecurity
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ConfigReader
    {
        private readonly string filename;

        public ConfigReader(string Filename)
        {
            this.filename = Filename;
        }

        public string ReadKey(string Key)
        {
            // the key of the setting
            string key = Key;

            // the new value you want to change the setting to

            // the path to the web.config
            string path = filename;

            // open your web.config, so far this is the ONLY way i've found to do this without it wanting a virtual directory or some nonsense
            // even "OpenExeConfiguration" will not work
            var config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = path }, ConfigurationUserLevel.None);

            // now that we have our config, grab the element out of the settings
            var element = config.AppSettings.Settings[key];

            if (element == null)
            {
                return string.Empty;
            }
            else
            {
                return element.Value;
            }

           
        }
        public void UpdateValue(string NodeName, string Value, string ConfigFilePath)
        {
            string appPath = System.IO.Path.GetDirectoryName(ConfigFilePath);
            string configFile = System.IO.Path.Combine(appPath, "Web.config");
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            config.AppSettings.Settings[NodeName].Value = Value;
            config.Save(); 

            
        }
    }
}