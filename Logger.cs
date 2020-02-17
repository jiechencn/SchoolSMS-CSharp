using System;

namespace SchoolSMS
{
    public class Logger
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("LogInfo");
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("LogError");
        public static void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void WriteLog(string info, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, ex);
            }
        }
    }
}
