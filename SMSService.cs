using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolSMS
{
    
    class SMSService
    {
        static ConfigResourcer cfg = ConfigResourcer.GetInstance();
        private ReadSMSEventHandler readHandler;

        public SMSService()
        {

        }
        internal event ReadSMSEventHandler OnRead
        {
            add
            {
                this.readHandler += value;
            }
            remove
            {
                this.readHandler -= value;
            }
        }

        internal void Read()
        {
            Dictionary<string, string> header = cfg.GetHeader();
            HttpProcessor smsReader = new HttpProcessor();
            long maxID = -1;
            try
            {
                var maxIDData = smsReader.SendAsync(Constants.XWZSRV_MAXID_URL, header, null);
                maxID = long.Parse(maxIDData.Result);
            }
            catch (Exception exc)
            {
                Logger.WriteLog($"[SMSService.Read] Fail to get maxID: {Constants.XWZSRV_MAXID_URL}", exc);
            }

            try
            {
                IList<SMSEntity> newSMSes = new List<SMSEntity>();
                var smsData = smsReader.SendAsync(Constants.SMSSRV_URL, header, cfg.GetSMSPost());
                Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(smsData.Result);
                string firstKey = json.ElementAt(0).Key;
                string secondKey = json.ElementAt(1).Key;

                if (json.Count == 3)
                {
                    JArray smses = (JArray)json.ElementAt(2).Value;

                    foreach (var smsItem in smses)
                    {
                        SMSEntity oneSMS = JsonConvert.DeserializeObject<SMSEntity>(JsonConvert.SerializeObject(smsItem));
                        if (oneSMS.messageid > maxID)
                            newSMSes.Add(oneSMS);
                    }

                    if (this.readHandler != null)
                    {
                        ReadSMSEventArgs e = new ReadSMSEventArgs();
                        e.NewSMSes = newSMSes;
                        e.MaxID = maxID;
                        e.TotalSMS = smses.Count;
                        this.readHandler.Invoke(e);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.WriteLog($"[SMSService.Read] Fail to read SMS from : {Constants.SMSSRV_URL}", exc);
            }

        }
    }
}
