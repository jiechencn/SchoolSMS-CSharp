using System;
using System.Drawing;
using System.Reflection;
using System.Timers;
using Out = Colorful.Console;

namespace SchoolSMS
{
   class Entrance
    {
        static SMSService smsSrv = new SMSService();
        static EmailSubscriber emailSrv = EmailSubscriber.GetInstance();
        static ConfigResourcer cfg = ConfigResourcer.GetInstance();
        static void Main(string[] args)
        {
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

            Out.WriteAscii($"SchoolSMS", Color.Red);
            Out.WriteLine($"\t\t[C# {version}]", Color.Pink);
            string __dash = "---------------------------------------------------";
            Console.WriteLine(__dash);
            Console.WriteLine($"\tTimer runs at interval of {cfg.getInterval()} minutes.");
            Console.WriteLine($"\tPress key x or X to exit.");
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
