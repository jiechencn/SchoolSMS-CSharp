using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SchoolSMS
{
    class EmailSubscriber
    {
        static ConfigResourcer cfg = ConfigResourcer.GetInstance();
        private static EmailSubscriber instance = new EmailSubscriber();
        private EmailService emailSrv;
        
        public static EmailSubscriber GetInstance()
        {
            if (instance == null)
                instance = new EmailSubscriber();
            return instance;
        }
        private EmailSubscriber()
        {
        }

        internal void OnSMSArrival(ReadSMSEventArgs e)
        {
            int totalCount = 0, newCount = 0, sentCount = 0, savedCount = 0;
            long maxID = -1;
            IList<SMSEntity> newSMSes = e.NewSMSes;
            totalCount = e.TotalSMS;
            maxID = e.MaxID;
            newCount = newSMSes.Count;

            if (newCount > 0)
            {
                Dictionary<string, string> smtp = cfg.GetSMTP();
                try
                {
                    emailSrv = new EmailService(smtp[Constants.SMTP_HOST], int.Parse(smtp[Constants.SMTP_PORT]), "1".Equals(smtp[Constants.SMTP_SSL]) ? true : false);
                    emailSrv.Autenticate(smtp[Constants.SMTP_USER], smtp[Constants.SMTP_PWD]);
                }
                catch (Exception exc)
                {
                    Logger.WriteLog($"[EmailSubscriber.EmailSubscriber] Fail to connect to: {smtp[Constants.SMTP_HOST]}", exc);
                }

                IList<Dictionary<string, object>> smses2db = new List<Dictionary<string, object>>();

                foreach (SMSEntity sms in newSMSes)
                {
                    string subject = sms.sendname + " sent SMS @ " + sms.submittime;
                    try
                    {
                        emailSrv.Send(cfg.GetRecipients(), subject, sms.content, false);
                        sentCount++;
                        Dictionary<string, object> s2db = new Dictionary<string, object>();
                        s2db.Add(Constants.POST_WRITE_SMSID, sms.messageid);
                        s2db.Add(Constants.POST_WRITE_SMSTIME, sms.submittime);
                        s2db.Add(Constants.POST_WRITE_SMSTEACHER, sms.sendname);
                        s2db.Add(Constants.POST_WRITE_SMSBODY, sms.content);
                        smses2db.Add(s2db);
                    }
                    catch (Exception exc)
                    {
                        Logger.WriteLog($"[EmailSubscriber.OnSMSArrival] Fail to send email: {subject}", exc);
                    }
                }
                emailSrv.Close();

                try
                {
                    HttpProcessor dbWriter = new HttpProcessor();
                    string sms2dbString = JsonConvert.SerializeObject(smses2db);

                    var smsPostdata = new Dictionary<string, string> {
                        { Constants.POST_WRITE_SMSES, sms2dbString}
                    };
                    var result = dbWriter.SendAsync(Constants.XWZSRV_SAVESMS_URL, cfg.GetHeader(), smsPostdata);
                    savedCount = smses2db.Count;
                }
                catch (Exception exc)
                {
                    Logger.WriteLog($"[EmailSubscriber.OnSMSArrival] Fail to update database: {Constants.XWZSRV_SAVESMS_URL}", exc);
                }
            }

            
            string info = $"Total {totalCount}, New {newCount}, Sent {sentCount}, Saved {savedCount}";
            Logger.WriteLog(info);
        }
    }
}
