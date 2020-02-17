using System;

namespace SchoolSMS
{
    public struct SMSEntity
    {
        public long messageid;
        public long sendid;
        public string sendname;
        public string content;
        public DateTime submittime;
        public long receid;
        public string recename;
        public int isread;
        public int isdel;
        public int msgtype;
        public int SerialNumber;
        public int colstatus;
    }
    
}
