using System;
using System.IO;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Client
{
    public static class CheckList
    {
        public static CheckResult CheckFreeSpace(ServerInfo serverInfo)
        {
            var name = Config.WorkingDir.Root.Name;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (string.Compare(drive.Name,name,StringComparison.InvariantCultureIgnoreCase)==0)
                {
                    var firstLaunch = Settings.GetFirstLaunch(serverInfo.ClientProfile.GetTitle());
                    if (firstLaunch == DateTime.MinValue)
                    {
                        if (drive.AvailableFreeSpace < 512*1024*1024)
                            return new CheckResult(1, "На диске свободно менее 512МБ.\nЭтого может быть недостаточно для установки или дальнейшей работы игры.");
                    }
                    else
                    {
                        if (drive.AvailableFreeSpace < 200*1024*1024)
                            return new CheckResult(1, "На диске свободно менее 200МБ.\nДальнейшее уменьшие свободного места может помешать работе игры.");
                    }
                    return CheckResult.Ok;
                }
            }
            return CheckResult.Ok;
        }

        public static CheckResult CheckMaxRam(ServerInfo serverInfo)
        {
            var ram = HardwareInfo.GetRamSizeMb();
            var recMin = (int) serverInfo.ClientProfile.systemRamMin.GetValue();
            if (ram < recMin)
                return new CheckResult(2, $"Для стабильной игры на {serverInfo.ClientProfile.GetTitle()} рекомендуется иметь {recMin}МБ RAM\nИгра может заработать, если закрыть прочие программы");
            var recRam = (int) serverInfo.ClientProfile.systemRamRec.GetValue();
            if (ram < recMin)
                return new CheckResult(1, $"Для лучшей производительности на {serverInfo.ClientProfile.GetTitle()} рекомендуется иметь {recRam}МБ RAM\nИгра может не заработать, если открыто много программ");
            return CheckResult.Ok;
        }

        public static CheckResult CheckCurrentRam(ServerInfo serverInfo)
        {
            var ram = HardwareInfo.GetRamFreeMb();
            var ramMin = (int) serverInfo.ClientProfile.ramMin.GetValue();
            if (ram < ramMin)
                return new CheckResult(2, $"Свободной памяти недостаточно для игры на {serverInfo.ClientProfile.GetTitle()}\nСледует освободить хотя бы освободить еще {ramMin-ram}МБ RAM\nРекомендуется закрыть все прочие приложения.\nБраузер и антивирус особенно требовательны к памяти.");
            if (ram < ramMin + 800)
                return new CheckResult(1, $"Свободной памяти может быть недостаточно для игры на {serverInfo.ClientProfile.GetTitle()},\nВ случае проблем, следует освободить хотя бы освободить еще {(ramMin+800)-ram}МБ RAM\nРекомендуется закрыть все прочие приложения.\nБраузер и антивирус особенно требовательны к памяти.");
            var recRam = (int) serverInfo.ClientProfile.ramRec.GetValue();
            if (ram < recRam - 100)
                return new CheckResult(1, $"Свободной памяти достаточно для игры на {serverInfo.ClientProfile.GetTitle()},\nно для лучшей производительности рекомендуется освободить еще {recRam-ram}МБ RAM.\nНапример, закрыть некоторые приложения.\nБраузер и антивирус особенно требовательны к памяти.");
            return CheckResult.Ok;
        }

        public static CheckResult CheckPageFile(ServerInfo serverInfo)
        {
            var pageFile = HardwareInfo.GetPageFileMb();
            if (pageFile > 4096)
                return CheckResult.Ok;
            var freeRam = HardwareInfo.GetRamFreeMb();
            var recRam = (int) serverInfo.ClientProfile.ramRec.GetValue();
            return pageFile < 100
                ? new CheckResult(2, "Рекомендуется задать файл подкачки. Система использует его в качестве дополнения для оперативной памяти")
                : new CheckResult(freeRam > recRam ? 1 : 2, "Рекомендуется увеличить размер файла подкачки. Система использует его в качестве дополнения для оперативной памяти.");
        }

        public static CheckResult CheckCPU(ServerInfo serverInfo)
        {
            return CheckResult.Ok;
        }

        public static CheckResult CheckBits(ServerInfo serverInfo)
        {
            if (HardwareInfo.GetBits()==64)
                return CheckResult.Ok;
            var recRam = (int) serverInfo.ClientProfile.ramRec.GetValue();
            return new CheckResult(recRam>1100 ? 2 : 1, "32-битная версия Windows не позволяет использовать все 4ГБ RAM.\nРекомендуется установка 64-битной версии Windows");
        }

        public class CheckResult
        {
            public static readonly CheckResult Ok = new CheckResult(0,"");
            public readonly int WarnLevel;
            public readonly string Message;

            public CheckResult(int warnLevel, string message)
            {
                WarnLevel = warnLevel;
                Message = message;
            }

        }
    }
}