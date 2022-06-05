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
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;

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

        [HttpGet]
        public IActionResult PowerBI()
        {
            return View(new PowerBIModel());
        }

        [HttpPost]
        public async Task<IActionResult> PowerBI(PowerBIModel data)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            try
            {
                ValidatePowerBIConfiguration(errors, data);

                if (!errors.Any())
                {
                    IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                    .Create(data.ClientId)
                                                                    .WithClientSecret(data.AppSecret)
                                                                    .WithAuthority($"{data.Authority}{data.TenantId}")
                                                                    .Build();
                    List<string> scopes = new List<string>
                        {
                            data.Scope,
                        };

                    Microsoft.Identity.Client.AuthenticationResult authenticationResult = await clientApp.AcquireTokenForClient(scopes).ExecuteAsync();

                    if (authenticationResult is null)
                    {
                        errors.Add(new ValidationResult("Unable to authenticate the Power BI client"));
                    }
                    if (!errors.Any())
                    {
                        TokenCredentials tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");

                        // 3. Get embedded report info
                        var client = new PowerBIClient(new Uri(data.APIBaseURL), tokenCredentials);

                        Reports reports = client.Reports.GetReports(Guid.Parse(data.WorkSpaceId));

                        ViewBag.Reports = reports;
                    }
                }
            }
            catch (HttpOperationException ex)
            {
                errors.Add(new ValidationResult(ex.Message));
            }
            catch (Exception e)
            {
                errors.Add(new ValidationResult(e.Message));
            }

            errors.ForEach(_ =>
            {
                ModelState.AddModelError(string.Empty, _.ErrorMessage);
            });

            return View(data);
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

        private void ValidatePowerBIConfiguration(ICollection<ValidationResult> errors, PowerBIModel data)
        {
            if (string.IsNullOrWhiteSpace(data.ClientId))
            {
                errors.Add(new ValidationResult("ClientId_Empty"));
            }
            else if (!Guid.TryParse(data.ClientId, out _))
            {
                errors.Add(new ValidationResult("ClientId_Not_Valid"));
            }

            if (string.IsNullOrWhiteSpace(data.WorkSpaceId))
            {
                errors.Add(new ValidationResult("WorkspaceId_Empty"));
            }
            else if (!Guid.TryParse(data.WorkSpaceId, out _))
            {
                errors.Add(new ValidationResult("WorkspaceId_Not_Valid"));
            }

            if (string.IsNullOrWhiteSpace(data.TenantId))
            {
                errors.Add(new ValidationResult("TenantId_Empty"));
            }
            else if (!Guid.TryParse(data.TenantId, out _))
            {
                errors.Add(new ValidationResult("TenantId_Not_Valid"));
            }

            if (string.IsNullOrWhiteSpace(data.AppSecret))
            {
                errors.Add(new ValidationResult("App_Secret_Empty"));
            }
        }
    }
}
