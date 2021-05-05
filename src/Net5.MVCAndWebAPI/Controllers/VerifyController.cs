using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net5.MVCAndWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Text;


namespace Net5.MVCAndWebAPI.Controllers
{
    public class VerifyController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;


        public VerifyController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            this._config = config;
        }

        [HttpGet]
        public IActionResult Database()
        {
            SetViewBag();
            return View(new DatabaseVerifyModel());
        }

        [HttpPost]
        public IActionResult Database(DatabaseVerifyModel data)
        {

            if (data.ConnenctionString.Contains("Password"))
            {
                data.Status = ValidateConnectionStringWithPassword(data.ConnenctionString);
            }
            else
            {
                data.Status = ValidateConnectionStringWithoutPassword(data.ConnenctionString, data.ClientId, data.TenantId, data.AppSecret, data.ResourceName);
            }

            data.IsSuccess = data.Status == "Connection success.";

            SetViewBag();
            return View("Database", data);
        }

        private string ValidateConnectionStringWithPassword(string connection)
        {

            if (string.IsNullOrEmpty(connection))
                return "Connection string is empty";

            SqlConnection con = new SqlConnection(connection);
            try
            {
                con.Open();
                return "Connection success.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }

        private string ValidateConnectionStringWithoutPassword(string connection, string clientId, string tenantId, string appSecret, string resourceName)
        {

            if (string.IsNullOrEmpty(connection))
                return "Connection string is empty";

            if (string.IsNullOrEmpty(clientId) || !Guid.TryParse(clientId, out _))
                return "Invalid client/app id.";

            if (string.IsNullOrEmpty(tenantId) || !Guid.TryParse(tenantId, out _))
                return "Invalid tenant id.";

            if (string.IsNullOrEmpty(appSecret))
                return "App secret is empty";

            if (string.IsNullOrEmpty(resourceName))
                return "Resource name is empty";

            SqlConnection con = new SqlConnection(connection);
            try
            {
                var tokenProv = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider($"RunAs=App;AppId={clientId};TenantId={tenantId};AppKey={appSecret}");
                con.AccessToken = tokenProv.GetAccessTokenAsync(resourceName).Result;
                con.Open();
                return "Connection success.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }


        private void SetViewBag()
        {
            var appSettings = _config.AsEnumerable()
                .Where(_ => !_.Key.Contains("ConnectionStrings")) // azure connection strings are considered as app setting.
                .Select(_ => new Net5.MVCAndWebAPI.Models.KeyValuePair { Key = _.Key, Value = _.Value })
                .OrderBy(_ => _.Key)
                .ToList();

            var connectionStrings = _config.GetSection("ConnectionStrings").GetChildren()
                .Select(_ => new Net5.MVCAndWebAPI.Models.KeyValuePair { Key = _.Key, Value = _.Value })
                .OrderBy(_ => _.Key)
                .ToList();


            ViewBag.AppSettings = new SelectList(appSettings.Distinct(), "Value", "Key");
            ViewBag.ConnectionStrings = new SelectList(connectionStrings.OrderBy(i => i.Value).Distinct(), "Value", "Key");
        }
    }
}
