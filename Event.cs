using System;
using System.Collections.Generic;

namespace SchoolSMS
{
    public delegate void ReadSMSEventHandler(ReadSMSEventArgs e);

    public class ReadSMSEventArgs : EventArgs
    {
        public IList<SMSEntity> NewSMSes { get; set; }
        public long MaxID { get; set; }
        public int TotalSMS { get; set; }
    }
}