using System.IO;

namespace ExtraIsland.Shared;

public static class EiUtils {
    public static bool IsOdd(int n) {
        return n % 2 == 1;
    }
    
    public static TimeSpan GetDateTimeSpan(DateTime startTime, DateTime endTime) {
        TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
        return daysSpan;
    }

    public static bool IsLyricsIslandInstalled() {
        return File.Exists("./Plugins/jiangyin14.lyrics/manifest.yml");
    }
}