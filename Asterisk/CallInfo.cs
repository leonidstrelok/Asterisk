using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Asterisk
{
    public class CallInfo
    {
        public int CallStatusCode { get; set; }
        public string CallerNumber { get; set; }
        public int BitrixUserId { get; set; }
        public string BitrixCallId { get; set; }
        public string DestinationNumber { get; set; }
        public DateTime CallStartDate { get; set; } = DateTime.Now;
        public DateTime? CallAnsweredDate { get; set; }
    }

    public enum CallType
    {
        Incoming = 2, Outgoing = 1, Transfer = 3, Callback = 4
    }

    public enum CallStatusCode
    {
        ANSWER = 200,
        BUSY = 486,
        NOANSWER = 304,
        CANCEL = 603
    }

}
