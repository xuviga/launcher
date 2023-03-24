using System;
using System.Management;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace iMine.Launcher.Utils
{
    public static class HardwareInfo
    {
        private static readonly string[] OsFields = {"Caption", "CSDVersion", "OSArchitecture"};
        private static readonly string[] CpuFields = {"Name", "ProcessorId", "NumberOfCores", "NumberOfLogicalProcessors"};
        private static readonly string[] GpuFields = {"Name", "VideoModeDescription", "PNPDeviceID"};
        private static readonly string[] RamFields = {"Capacity", "Speed", "PartNumber", "SerialNumber"};

        private static readonly JObject Data;

        static HardwareInfo()
        {
            var osInfo = ReadValues("Win32_OperatingSystem", OsFields);
            Data = new JObject
            {
                {"os", osInfo},
                {"ver", App.GetVersion()},
                {"cpu", ReadValues("Win32_Processor", CpuFields)},
                {"gpu", ReadValues("Win32_VideoController", GpuFields)},
                {"ram", ReadValues("Win32_PhysicalMemory", RamFields)},
                {"free_ram", GetRamFreeMb()},
                {"username", Settings.Username},
                {"uuid", GetUuid().ToString()}
            };
        }


        public static int[] GetCoresAndSpeed()
        {
            var mc = new ManagementClass("Win32_Processor");
            var moc = mc.GetInstances();
            var cores = 0;
            var frequency = 0;
            foreach (var item in moc)
            {
                cores = Convert.ToInt32(item.Properties["NumberOfLogicalProcessors"]);
                frequency = Convert.ToInt32(item.Properties["CurrentClockSpeed"]);
            }

            return new[] {cores, frequency};
        }

        public static int GetTotalRam()
        {
            var mc = new ManagementClass("Win32_ComputerSystem");
            var moc = mc.GetInstances();
            long ram = 0;
            foreach (var item in moc)
                ram += Convert.ToInt64(item.Properties["TotalPhysicalMemory"].Value);
            return (int) (ram >> 20);
        }

        public static JObject GetRawData()
        {
            return Data;
        }

        private static JArray ReadValues(string bank, string[] fields)
        {
            var searcher = new ManagementObjectSearcher("select * from " + bank);
            var array = new JArray();
            ManagementObjectCollection entries;
            try
            {
                entries = searcher.Get();
            }
            catch
            {
                array.Add(new JObject {{"error", "<error>"}});
                return array;
            }

            foreach (var entry in entries)
            {
                var result = new JObject();
                foreach (var field in fields)
                {
                    var value = "<error>";
                    try
                    {
                        value = entry.GetPropertyValue(field).ToString().Trim();
                    }
                    catch
                    {
                    }

                    result.Add(field, value);
                }

                array.Add(result);
            }

            return array;
        }

        public static string GetOsVersion()
        {
            var mc = new ManagementClass("Win32_OperatingSystem");
            var moc = mc.GetInstances();
            foreach (var item in moc)
                return item.Properties["Caption"].Value.ToString();
            return "";
        }

        public static int GetRamFreeMb()
        {
            return (int) (new ComputerInfo().AvailablePhysicalMemory / 1024 / 1024);
        }

        public static int GetPageFileMb()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_PageFile");
                var entries = searcher.Get();
                long size = 0;
                foreach (var entry in entries)
                {
                    size += long.Parse(entry.GetPropertyValue("MaximumSize").ToString());
                }

                return (int) size;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetRamSizeMb()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
                var entries = searcher.Get();
                long size = 0;
                foreach (var entry in entries)
                {
                    size += long.Parse(entry.GetPropertyValue("Capacity").ToString()) / 1024 / 1024;
                }

                return (int) size;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetBits()
        {
            return Environment.Is64BitOperatingSystem ? 64 : 32;
        }

        public static Guid GetUuid()
        {
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\IM85", true)
                      ?? Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\IM85");

            if (key==null)
                return Guid.Empty;

            long? upper=null;
            long? lower=null;
            try
            {
                upper = (long?) key.GetValue("Upper");
            }
            catch { }
            try
            {
                lower = (long?) key.GetValue("Lower");
            }
            catch { }

            if (upper == null)
            {
                upper = DateTime.Now.Ticks;
                key.SetValue("Upper", upper, RegistryValueKind.QWord);
            }
            if (lower == null)
            {
                var rand = new Random();
                lower = rand.Next();
                lower = lower << 32;
                lower = lower | rand.Next();
                key.SetValue("Lower", lower, RegistryValueKind.QWord);
            }
            key.Close();

            var guidData = new byte[16];
            Array.Copy(BitConverter.GetBytes((long) upper), guidData, 8);
            Array.Copy(BitConverter.GetBytes((long) lower), 0, guidData, 8, 8);
            return new Guid(guidData);
        }
    }
}