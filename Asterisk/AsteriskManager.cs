using AsteriskWeb.AsteriskOption;
using AsteriskWeb.Exceptions;
using AsteriskWeb.Interface;
using AsteriskWeb.Models;
using AsterNET.Manager;
using AsterNET.Manager.Action;
using AsterNET.Manager.Event;
using AsterNET.Manager.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsteriskWeb.Asterisk
{
    public class AsteriskManager : IAsterisk
    {
        private readonly ManagerConnection manager;
        private readonly AsteriskOptions options;
        IConfiguration _configuration;
        private readonly IB24RestApi restApi;
        private readonly Dictionary<string, CallInfo> keys;
        private object obj = new object();
        public AsteriskManager(IConfiguration configuration, IB24RestApi restApi, IOptions<AsteriskOptions> options)
        {
            this.options = options.Value;
            keys = new Dictionary<string, CallInfo>();
            _configuration = configuration;
            this.restApi = restApi;
            manager = new ManagerConnection(this.options.Host, this.options.Port, this.options.UserName, this.options.Secret);
        }

        public void Login()
        {
            if (manager.IsConnected())
            {
                return;
            }
            try
            {
                manager.Login();
                BindEvents();
            }
            catch (Exception e)
            {
                StartReconnectProcess();
            }
        }

        private void StartReconnectProcess()
        {
            Task.Delay(60000);
            Login();
        }

        private void BindEvents()
        {
            manager.Hangup += Manager_Hangup;
            manager.DialBegin += Manager_DialBegin;
            manager.DialEnd += Manager_DialEnd;

        }


        private void Manager_DialEnd(object sender, DialEndEvent e)
        {

            Console.WriteLine(e);
            if (keys.ContainsKey(e.UniqueId))
            {
                CallInfo obj = keys[e.UniqueId];
                obj.CallStatusCode = (int)Enum.Parse(typeof(CallStatusCode), e.DialStatus);
                if (e.DialStatus == "ANSWER")
                {
                    obj.CallAnsweredDate = DateTime.Now;
                }
            }
        }

        private async void Manager_DialBegin(object sender, DialBeginEvent e)
        {
            Console.WriteLine(e);
            var destinationNumber = e.DialString.Replace("@asterisk", "").Replace("810", "+");
            var callerNumber = e.CallerIdNum.Replace("810", "+").Replace("@asterisk", "");



            string bitrixInnnerPhone = string.Empty;
            string crmNumber = string.Empty;
            CallType callType = CallType.Outgoing;
            bool localCall = destinationNumber.Length == 3 && callerNumber.Length == 3;
            if (localCall)
            {
                return;
            }

            bool incoming = destinationNumber.Length == 3;
            if (incoming)
            {
                bitrixInnnerPhone = destinationNumber;
                crmNumber = callerNumber;
                callType = CallType.Incoming;
            }
            else
            {
                bitrixInnnerPhone = callerNumber;
                crmNumber = destinationNumber;
            }


            var result = await restApi.SearchCRM(crmNumber);
            var userId = await restApi.GetUserIdByInternalPhone(bitrixInnnerPhone);
            CallInfo obj = new CallInfo
            {
                DestinationNumber = destinationNumber,
                CallerNumber = callerNumber,
                BitrixUserId = userId,
                CallStatusCode = (int)CallStatusCode.NOANSWER
            };
            keys.Add(e.UniqueId, obj);

            var contactExist = result.result.Any();
            obj.BitrixCallId = await restApi.ExternalCallRegister(new B24ExternalCallModel.ExternalCallDataModel
            {
                USER_PHONE_INNER = crmNumber,
                USER_ID = userId,
                SHOW = 1,
                TYPE = (int)callType,
                CRM_CREATE = contactExist ? 0 : 1,
                CRM_ENTITY_TYPE = contactExist ? result.result[0].CRM_ENTITY_TYPE : "CONTACT",
                CRM_ENTITY_ID = contactExist ? result.result[0].CRM_ENTITY_TYPE : null,
                CALL_START_DATE = obj.CallStartDate,
                PHONE_NUMBER = crmNumber,
                PHONE_NUMBER_INTERNATIONAL = crmNumber
            });


        }

        private async void Manager_Hangup(object sender, HangupEvent e)
        {
            Console.WriteLine(e);
            var otv = e.CallerIdNum.Replace("810", "+");
            if (keys.ContainsKey(e.UniqueId))
            {
                await Task.Run(async () =>
                {
                    CallInfo callInfo = keys[e.UniqueId];
                    await restApi.FinishCall(callInfo.BitrixCallId, (int)DateTime.Now.Subtract(callInfo.CallAnsweredDate ?? DateTime.Now).TotalSeconds, callInfo.CallStatusCode, callInfo.BitrixUserId);
                    if (callInfo.CallStatusCode == (int)CallStatusCode.ANSWER)
                    {
                        await restApi.AttachRecord(callInfo.BitrixCallId, $"{options.RecordsBaseAddr}/{callInfo.CallStartDate.Year}/{callInfo.CallStartDate.Month}/{callInfo.CallStartDate.Day}/{e.UniqueId}.wav", $"{otv}-{callInfo.CallStartDate}.wav");

                    }

                    await restApi.HideCall(callInfo.BitrixCallId, callInfo.BitrixUserId);
                    keys.Remove(e.UniqueId);

                }).ContinueWith(t =>
                {
                    Console.WriteLine("Upload record completed");
                });

            }
        }


        public void CallStart(string originate_channel, string originate_callerid, string originate_exten)
        {
            if (!manager.IsConnected())
            {
                throw new AsteriskNotConnectedException("Asterisk not connected. Contact with administrator");
            }

            try
            {
                OriginateResponseEvent a = new OriginateResponseEvent(manager);
                Console.WriteLine(a.Exten);

                OriginateAction oc = new OriginateAction();
                oc.Context = options.OriginateContext;
                oc.Priority = "1";
                oc.Channel = originate_channel;
                oc.CallerId = originate_callerid;
                oc.Exten = originate_exten;
                oc.Timeout = options.OriginateTimeout;
                oc.Async = true;
                oc.ActionCompleteEventClass();
                // oc.Variable = "VAR1=abc|VAR2=def";
                //oc.SetVariable("VAR3", "ghi");
                ManagerResponse originateResponse = manager.SendAction(oc, oc.Timeout);
                Console.WriteLine(originateResponse);




            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
