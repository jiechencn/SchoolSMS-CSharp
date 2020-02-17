using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SchoolSMS
{

    class ConfigResourcer
    {
        private static string cfgFile = "SchoolSMS.exe.config";
        private static ConfigResourcer instance = new ConfigResourcer();
        
        private static IEnumerable<XElement> elements;
        private static Dictionary<string, string> smtpDict = new Dictionary<string, string>();
        private static Dictionary<string, string> headerDict = new Dictionary<string, string>();
        private static Dictionary<string, string> smspostDict = new Dictionary<string, string>();
        private static IList<string> emailList = new List<string>();
        private static double interval = 0.0;

        private ConfigResourcer()
        {
            
            XElement xe = XElement.Load(ConfigResourcer.cfgFile);
            elements = from ele in xe.Elements("startup").Elements("app")
                       select ele;
        }

        public static ConfigResourcer GetInstance(string cfgFile = "SchoolSMS.exe.config")
        {
            ConfigResourcer.cfgFile = cfgFile;
            if (instance == null)
                instance = new ConfigResourcer();
            return instance;
        }

        public Dictionary<string, string> GetSMTP(){
            if (smtpDict.Count > 0)
                return smtpDict;
            IEnumerable<XElement> smtpEle =  elements.Elements("smtp");
            return smtpDict = ConvertEles2Dict(smtpEle);
        }

        public IList<string> GetRecipients()
        {
            if (emailList.Count > 0)
                return emailList;
            IEnumerable<XElement> emaile = elements.Elements("email");
            var emails = emaile.Elements().ToList();
            


            foreach (var ele in emails)
            {
                emailList.Add(ele.Value);
            }
            return emailList;
        }

        public Dictionary<string, string> GetHeader()
        {
            if (headerDict.Count > 0)
                return headerDict;
            IEnumerable<XElement> headerEle = elements.Elements("header");
            return headerDict = ConvertEles2Dict(headerEle);

        }
        public Dictionary<string, string> GetSMSPost()
        {
            if (smspostDict.Count > 0)
                return smspostDict;
            IEnumerable<XElement> smspostEle = elements.Elements("smspost");
            return smspostDict = ConvertEles2Dict(smspostEle);

        }

        public Dictionary<string, string> ConvertEles2Dict(IEnumerable<XElement> parent)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var ele in parent.Elements())
            {
                dict.Add(ele.Name.ToString(), ele.Value);
            }
            return dict;
        }

        public double getInterval()
        {
            if (interval > 0)
                return interval;
            XElement timer = elements.Elements("timer").First();
            return interval = double.Parse(timer.Attribute("interval").Value);
        }




    }
}
