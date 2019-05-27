using System;
using System.IO;
using System.Runtime.Serialization;
using NLog;

namespace RedistServ
{
    [DataContract]
    class Configuration
    {
        [DataMember] public UInt64 UnicalId;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string ConfigurationPath()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var vendor = Path.Combine(appdata, "STSTC");
            var programfolder = Path.Combine(vendor, "LanRedistClient");
            Directory.CreateDirectory(programfolder);
            return Path.Combine(programfolder, "config.xml");
        }
        public static Configuration Load()
        {
            try
            {
                using (var stream = File.OpenRead(ConfigurationPath()))
                {
                    var serializer = new DataContractSerializer(typeof(Configuration));
                    return (Configuration) serializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return CreateDefault();
            }
        }

        private static Configuration CreateDefault()
        {
            return new Configuration()
            {
                UnicalId=InstanceIdGenerate()
            };
        }
        private static ulong InstanceIdGenerate()
        {
            var rnd1 = new Random(DateTime.UtcNow.Millisecond);
            var result = ((long) rnd1.Next() << 32) | (long)rnd1.Next();
            return (ulong) result;
        }
    }
}