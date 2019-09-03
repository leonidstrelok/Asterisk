using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsteriskWeb.AsteriskOption;
using AsteriskWeb.Paginations;
using AsterNET.Manager;
using AsterNET.Manager.Action;
using AsterNET.Manager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AsteriskWeb.Controllers
{
    public class AsteriskSipController : Controller
    {
        private readonly ManagerConnection manager;
        private readonly AsteriskOptions options;
        public AsteriskSipController(IOptions<AsteriskOptions> options)
        {
            this.options = options.Value;
            manager = new ManagerConnection(this.options.Host, this.options.Port, this.options.UserName, this.options.Secret);
            manager.Login();
        }


        public IActionResult Index(int page = 1)
        {

            int pageSize = 10;
            int count = 0;
            ManagerResponse response = manager.SendAction(new GetConfigAction("sip.conf"));
            List<GetConfigResponse> getConfig = new List<GetConfigResponse>();
            List<string> getLines = new List<string>();
            List<string> getCategories = new List<string>();

            if (response.IsSuccess())
            {
                GetConfigResponse responseConfig = (GetConfigResponse)response;
                getConfig.Add(responseConfig);
                foreach (var conf in responseConfig.Categories.Keys)
                {
                    count += responseConfig.Categories[conf].Count();
                    getCategories.Add(responseConfig.Categories[conf]);
                    foreach (var keyLine in responseConfig.Lines(conf).Keys)
                    {
                        getLines.Add(responseConfig.Lines(conf)[keyLine]);
                    }
                }

                var items = getCategories.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
                IndexViewModel viewModel = new IndexViewModel
                {
                    PageViewModel = pageViewModel,
                    GetConfigResponses = getConfig,
                    Categories = getCategories,
                    Lines = getLines
                };
                return View(viewModel);
            }
            else
            {
                Console.WriteLine("");
                return View();
            }
        }

        [HttpPost]
        public IActionResult Create(string nameUser, string pass, string fullName)
        {
            ManagerResponse response = manager.SendAction(new GetConfigAction("sip.conf"));
            if (response.IsSuccess())
            {
                GetConfigResponse responseConfig = (GetConfigResponse)response;
                foreach (var user in responseConfig.Categories.Keys)
                {
                    if (nameUser == responseConfig.Categories[user])
                    {
                        return RedirectToAction("Error");
                    }

                }
            }
            UpdateConfigAction config = new UpdateConfigAction("sip.conf", "sip.conf");
            config.AddCommand(UpdateConfigAction.ACTION_NEWCAT, nameUser);
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "secret", pass);
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "context", "dial_out");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "type", "friend");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "host", "dynamic");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "callerid", fullName);
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "pickupgroup", "1");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "canreinvite", "yes");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "nat", "force_rport,comedia");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "directmedia", "nonat");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "languages", "en-us");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "qualifyfreq", "60");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "qualify", "500");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "insecure", "port,invite");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "dtmfmode", "auto");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "disallow", "all");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "allow", "alaw");
            config.AddCommand(UpdateConfigAction.ACTION_APPEND, nameUser, "allow", "gsm");
            config.Reload = "yes";
            ManagerResponse Newresponse = manager.SendAction(config);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteUser(string idUser)
        {
            UpdateConfigAction config = new UpdateConfigAction("sip.conf", "sip.conf", true);
            config.AddCommand(UpdateConfigAction.ACTION_DELCAT, idUser);
            ManagerResponse response = manager.SendAction(config);
            return RedirectToAction("Index"); 
        }

        public IActionResult UpdateConfig(string nameUser, string pass, string fullName)
        {
            UpdateConfigAction config = new UpdateConfigAction("sip.conf", "sip.conf");
            config.AddCommand(UpdateConfigAction.ACTION_UPDATE, nameUser, "secret", pass);
            config.AddCommand(UpdateConfigAction.ACTION_UPDATE, nameUser, "callerid", fullName);
            config.Reload = "yes";
            ManagerResponse response = manager.SendAction(config);

            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}