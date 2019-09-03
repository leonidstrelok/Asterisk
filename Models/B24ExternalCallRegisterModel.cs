using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Models
{
    public class B24ExternalCallRegisterModel
    {
        public string PHONE_NUMBER { get; set; }
        public string USER_ID { get; set; }
        public string CRM_ENTITY_TYPE { get; set; }
        public string CRM_ENTITY_ID { get; set; }
        public string PHONE_NUMBER_INNER { get; set; }
        public string CALL_ID { get; set; }
    }
}
