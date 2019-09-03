using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.AsteriskOption
{
    public class AsteriskOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Secret { get; set; }
        public string OriginateContext { get; set; }
        public int OriginateTimeout { get; set; }
        public string RecordsBaseAddr { get; set; }
    }
}
