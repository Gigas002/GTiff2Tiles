namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Some methods for initializing NetVips.
    /// </summary>
    public static class NetVipsHelper
    {
        //todo initialize netvips paths and disable log.

        //public static void DisableLog()
        //{
        //    //doesn't work as expected
        //    uint handlerId = Log.SetLogHandler("VIPS", NetVips.Enums.LogLevelFlags.Critical, (domain, level, message) =>
        //    {
        //        Console.WriteLine("Domain: '{0}' Level: {1}", domain, level);
        //        Console.WriteLine("Message: {0}", message);
        //    });
        //    Log.RemoveLogHandler("VIPS", handlerId);
        //}
    }
}
