using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Interface
{
    public interface IAsterisk
    {
        void Login();
        void CallStart(string originate_channel, string originate_callerid, string originate_exten);
    }
}
