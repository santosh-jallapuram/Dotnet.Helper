namespace AppServiceHelper.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using AppServiceHelper.Models;

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            this._config = config;
        }

        public IActionResult Index()
        {
            HomeViewModel vm = new HomeViewModel();

            vm.AppSettings = _config.AsEnumerable()
                .Where(_ => !_.Key.Contains("ConnectionStrings")) // azure connection strings are considered as app setting.
                .Select(_ => new AppServiceHelper.Models.KeyValuePair { Key = _.Key, Value = _.Value })
                .OrderBy(_ => _.Key)
                .ToList();

            vm.ConnectionStrings = _config.GetSection("ConnectionStrings").GetChildren()
                .Select(_ => new AppServiceHelper.Models.KeyValuePair { Key = _.Key, Value = _.Value })
                .OrderBy(_ => _.Key)
                .ToList();
            ViewBag.AppSettings = new SelectList(vm.AppSettings.OrderBy(i => i.Value).Distinct(), "Key", "Value");
            return View(vm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string VerifyConnectionString(string connectionStringName)
        {

            SqlConnection connection = new SqlConnection(_config.GetConnectionString("connectionStringName"));

            try
            {
                connection.Open();
            }
            catch (Exception e)
            {

                return e.ToString();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return "Success";
        }

    }

}
