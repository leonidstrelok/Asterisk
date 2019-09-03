using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Models
{
    public class CrmSearchResponseModel
    {
        public string CRM_ENTITY_TYPE { get; set; }
        public int? CRM_ENTITY_ID { get; set; }
        public int ASSIGNED_BY_ID { get; set; }
    }
}
