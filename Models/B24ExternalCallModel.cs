using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Models
{
    public class B24ExternalCallModel
    {
        public string @event { get; set; }

        public ExternalCallDataModel data { get; set; }
        public ExternalCallAuthModel auth { get; set; }
        public class ExternalCallDataModel
        {
            public string USER_PHONE_INNER { get; set; }
            public string PHONE_NUMBER { get; set; }
            public string PHONE_NUMBER_INTERNATIONAL { get; set; }
            public string EXTENSION { get; set; }
            public int CRM_CREATE { get; set; }
            public int CALL_LIST_ID { get; set; }
            public int USER_ID { get; set; }
            public string LINE_NUMBER { get; set; }
            public string CALL_ID { get; set; }
            public string CRM_ENTITY_TYPE { get; set; }
            public string CRM_ENTITY_ID { get; set; }
            public int SHOW { get; set; }
            public int TYPE { get; set; }
            public DateTime CALL_START_DATE { get; set; }
        }

        public class ExternalCallAuthModel
        {
            public string domain { get; set; }
            public string client_endpoint { get; set; }
            public string server_endpoint { get; set; }
            public string member_id { get; set; }
            public string application_token { get; set; }
        }
    }
}
