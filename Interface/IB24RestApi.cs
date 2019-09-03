using AsteriskWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Interface
{
    public interface IB24RestApi
    {
        Task<string> ExternalCallRegister(B24ExternalCallModel.ExternalCallDataModel model);
        Task<int> GetUserIdByInternalPhone(string phoneNumber);
        Task<CrmSearchResponseObject> SearchCRM(string callPhone);
        Task ShowCall(string registrId, int userId);
        Task FinishCall(string callId, int duration, int statusCode, int userId);
        Task HideCall(string callId, int userId);
        Task AttachRecord(string callId, string recordUrl, string callName);
    }
}
