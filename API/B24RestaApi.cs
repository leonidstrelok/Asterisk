using AsteriskWeb.Interface;
using AsteriskWeb.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AsteriskWeb.API
{
    public class B24RestaApi : IB24RestApi
    {
        private readonly IConfiguration _configuration;

        public B24RestaApi(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Регистрация звонка в битриксе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> ExternalCallRegister(B24ExternalCallModel.ExternalCallDataModel model)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var externalCaallRegisterAddr = _configuration["B24:WebHookInAddr"] + "telephony.externalcall.register.json";
                    var data = new
                    {
                        USER_PHONE_INNER = model.USER_PHONE_INNER,
                        CRM_CREATE = model.CRM_CREATE,
                        USER_ID = model.USER_ID,
                        CRM_ENTITY_TYPE = model.CRM_ENTITY_TYPE,
                        CRM_ENTITY_ID = model.CRM_ENTITY_ID,
                        PHONE_NUMBER = model.PHONE_NUMBER,
                        PHONE_NUMBER_INNER = model.PHONE_NUMBER_INTERNATIONAL,
                        TYPE = model.TYPE,
                        SHOW = model.SHOW,
                        CALL_START_DATE = model.CALL_START_DATE,

                    };
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(externalCaallRegisterAddr, content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();
                        JObject obj = JObject.Parse(resultString);
                        return (string)obj["result"]["CALL_ID"];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// Получение телефона
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<int> GetUserIdByInternalPhone(string phoneNumber)
        {
            using (HttpClient client = new HttpClient())
            {
                var getUserapi = _configuration["B24:WebHookInAddr"] + "user.get.json";
                var FILTER = new
                {
                    UF_PHONE_INNER = phoneNumber,

                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(FILTER), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(getUserapi, content);
                if (result.IsSuccessStatusCode)
                {
                    var resultString = await result.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(resultString);
                    return (int)obj["result"][0]["ID"];
                }
                return 0;


            }


        }


        /// <summary>
        /// Показывает карточку звонка
        /// </summary>
        /// <param name="registrId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task ShowCall(string registrId, int userId)
        {
            using (HttpClient client = new HttpClient())
            {
                var externalCaallRegisterAddr = _configuration["B24:WebHookInAddr"] + "telephony.externalcall.show.json";
                var data = new
                {
                    CALL_ID = registrId,
                    USER_ID = userId
                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(externalCaallRegisterAddr, content);
                var resultString = await result.Content.ReadAsStringAsync();
            }

        }

        /// <summary>
        /// Поиск контакта в CRM
        /// </summary>
        /// <param name="callPhone"></param>
        /// <returns></returns>
        public async Task<CrmSearchResponseObject> SearchCRM(string callPhone)
        {
            using (HttpClient client = new HttpClient())
            {
                var externalCaallRegisterAddr = _configuration["B24:WebHookInAddr"] + "telephony.externalCall.searchCrmEntities.json";
                var data = new
                {
                    PHONE_NUMBER = callPhone
                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(externalCaallRegisterAddr, content);
                var resultString = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<CrmSearchResponseObject>(resultString);
                return obj;
            }
        }

        /// <summary>
        /// Завершает звонок
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="duration"></param>
        /// <param name="statusCode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task FinishCall(string callId, int duration, int statusCode, int userId)
        {
            using (HttpClient client = new HttpClient())
            {
                var externalCaallRegisterAddr = _configuration["B24:WebHookInAddr"] + "telephony.externalcall.finish.json";
                var data = new
                {
                    CALL_ID = callId,
                    USER_ID = userId,
                    DURATION = duration,
                    STATUS_CODE = statusCode
                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(externalCaallRegisterAddr, content);
            }
        }

        /// <summary>
        /// Скрывает карту
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task HideCall(string callId, int userId)
        {
            using (HttpClient client = new HttpClient())
            {
                var externalCaallRegisterAddr = _configuration["B24:WebHookInAddr"] + "telephony.externalcall.hide.json";
                var data = new
                {
                    CALL_ID = callId,
                    USER_ID = userId
                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                _ = await client.PostAsync(externalCaallRegisterAddr, content);
            }
        }


        /// <summary>
        /// Записывает разговор
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="recordUrl"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task AttachRecord(string callId, string recordUrl, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                var externalCaallRegisterAddr = _configuration["B24:WebHookInAddr"] + "telephony.externalCall.attachRecord.json";
                var data = new
                {
                    CALL_ID = callId,
                    FILENAME = fileName,
                    //FILE_CONTENT = filecontent,
                    RECORD_URL = recordUrl

                };
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(externalCaallRegisterAddr, content);
            }
        }
    }
}
