using System;
using log4net;

namespace SchoolSMS
{
    public class Logger
    {
        public static readonly ILog loginfo = log4net.LogManager.GetLogger("LogInfo");
        public static readonly ILog logerror = log4net.LogManager.GetLogger("LogError");
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
