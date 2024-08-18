using System;
using System.IO;
using System.Xml.Serialization;
using VRage.FileSystem;

namespace ParticleLimitRemover
{
    public class ConfigObject
    {
        public int Limit = 1024;
    }

    internal static class Config
    {
        public static ConfigObject CurrentConfig { get; private set; } = new ConfigObject();
        private static readonly string configPath = Path.Combine(MyFileSystem.UserDataPath, "Storage", "ParticleLimitRemover.cfg");
        private static readonly XmlSerializer configSerializer = new XmlSerializer(typeof(ConfigObject));

        public static bool SaveConfig()
        {
            ConfigObject config = CurrentConfig;
            try
            {
                using (TextWriter writer = File.CreateText(configPath))
                {
                    configSerializer.Serialize(writer, config);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static void ResetConfig()
        {
            CurrentConfig = new ConfigObject();
            SaveConfig();
        }

        public static bool LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                SaveConfig();
            }
            try
            {
                using (StreamReader reader = new StreamReader(configPath))
                {
                    CurrentConfig = (ConfigObject)configSerializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
