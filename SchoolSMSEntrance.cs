using System;
using System.Reflection;
using System.Timers;

namespace SchoolSMS
{
   class Entrance
    {
        static SMSService smsSrv = new SMSService();
        static EmailSubscriber emailSrv = EmailSubscriber.GetInstance();
        static ConfigResourcer cfg = ConfigResourcer.GetInstance();
        static void Main(string[] args)
        {
            string test_git = "hello";
            cfg.GetRecipients();
            string version = "";
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            if (attributes.Length == 0)
            {
                version = "1.0";
            }
            else
            {
                version = ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }

            Console.WriteLine($"SchoolSMS for the poor [C# {version}]");
            string __dash = "---------------------------------------------";
            Console.WriteLine(__dash);
            Console.WriteLine($"Timer runs at interval of {cfg.getInterval()} minutes");
            Console.WriteLine($"Press key x or X to exit");
            Console.WriteLine(__dash);

            smsSrv.OnRead += emailSrv.OnSMSArrival;


            Timer timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 1000 * 60 * cfg.getInterval();
            timer.Start();

            timer.Elapsed += new System.Timers.ElapsedEventHandler(ProcessSMS);

            do
            {
                string key = Console.ReadLine();
                if (key.Equals("x", StringComparison.OrdinalIgnoreCase))
                {
                    timer.Stop();
                    timer.Dispose();
                    timer.Close();
                    return;
                }

            } while (true);
        }

        private static void ProcessSMS(object sender, ElapsedEventArgs e)
        {
            smsSrv.Read();
        }
    }
}
