using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsteriskWeb.AsteriskOption;
using AsterNET.Manager;
using AsterNET.Manager.Action;
using AsterNET.Manager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AsteriskWeb.Controllers
{
    public class AsteriskManagerController : Controller
    {
        private readonly ManagerConnection manager;
        private readonly AsteriskOptions options;
        public AsteriskManagerController(IOptions<AsteriskOptions> options)
        {
            this.options = options.Value;
            manager = new ManagerConnection(this.options.Host, this.options.Port, this.options.UserName, this.options.Secret);
        }


        public IActionResult Index()
        {
            manager.Login();
            ManagerResponse response = manager.SendAction(new GetConfigAction("manager.conf"));
            List<GetConfigResponse> getConfig = new List<GetConfigResponse>();
            if (response.IsSuccess())
            {
                GetConfigResponse responseConfig = (GetConfigResponse)response;
                getConfig.Add(responseConfig);
                return View(getConfig);
            }
            else
            {
                Console.WriteLine("");
                return View();
            }

            //{
            //    Console.WriteLine("\nUpdateConfig action");
            //    UpdateConfigAction config = new UpdateConfigAction("manager.conf", "manager.conf");
            //    config.AddCommand(UpdateConfigAction.ACTION_NEWCAT, "testadmin");
            //    config.AddCommand(UpdateConfigAction.ACTION_APPEND, "testadmin", "secret", "blabla");
            //    ManagerResponse Newresponse = manager.SendAction(config);
            //    Console.WriteLine(Newresponse);
            //}

            
        }

        public IActionResult CreateNewUser()
        {

            throw new NotImplementedException();
        }

        public IActionResult DeleteUser()
        {

            throw new NotImplementedException();
        }

        public IActionResult FindUser()
        {

            throw new NotImplementedException();
        }

        public IActionResult UpdateUser()
        {

            throw new NotImplementedException();
        }
    }
}