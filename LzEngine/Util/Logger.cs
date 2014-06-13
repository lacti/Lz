using System;

namespace LzEngine.Util
{
    public static class Logger
    {
        public static void Write(string format, params object[] args)
        {
            var message = args == null || args.Length == 0
                              ? format
                              : string.Format(format, args);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lock (Console.Out)
            {
                Console.Out.WriteLine("[{0}] {1}", timestamp, message);
            }
        }

        public static void Write(Exception e)
        {
            Write(e.Message);
            Write(e.StackTrace);
        }
    }
}