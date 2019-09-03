using AsterNET.Manager.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Paginations
{
    public class IndexViewModel
    {
        public IEnumerable<GetConfigResponse> GetConfigResponses { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Lines { get; set; }
    }
}
