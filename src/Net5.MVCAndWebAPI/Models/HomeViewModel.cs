using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Net5.MVCAndWebAPI.Models
{

    public class HomeViewModel
    {
        public List<KeyValuePair> AppSettings { get; set; } = new List<KeyValuePair>();

        public List<KeyValuePair> ConnectionStrings { get; set; } = new List<KeyValuePair>();
    }

    public class KeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class DatabaseVerifyModel
    {
        [DisplayName("Connection string")]
        public string ConnenctionString { get; set; }

        [DisplayName("Client Id or App Id")]
        public string ClientId { get; set; }

        [DisplayName("Tenant Id")]
        public string TenantId { get; set; }

        [DisplayName("Application Secret")]
        public string AppSecret { get; set; }

        [DisplayName("Resource Name (if using azure managed identity)")]
        public string ResourceName { get; set; } = "https://database.windows.net";

        public bool IsSuccess { get; set; }
        public string Status { get; set; }
    }

    public class PowerBIModel
    {

        /// <summary>
        /// Gets or sets the authority URL.
        /// </summary>
        /// <value>
        /// The authority URL.
        /// </value>
        public string Authority { get; set; } = "https://login.microsoftonline.com/";



        /// <summary>
        /// Gets or sets the resource URL.
        /// </summary>
        /// <value>
        /// The resource URL.
        /// </value>
        public string ResourceUrl { get; set; } = "https://analysis.windows.net/powerbi/api";

        /// <summary>
        /// Gets or sets the API base URL.
        /// </summary>
        /// <value>
        /// The API base URL.
        /// </value>
        public string APIBaseURL { get; set; } = "https://api.powerbi.com";

        /// <summary>
        /// Gets or sets the embed base URL.
        /// </summary>
        /// <value>
        /// The embed base URL.
        /// </value>
        public string EmbedBaseURL { get; set; } = "https://app.powerbi.com";



        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>
        /// The scope.
        /// </value>
        public string Scope { get; set; } = "https://analysis.windows.net/powerbi/api/.default";

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; } = "57c69691-22e5-4280-9712-bb3057b71e11";

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        /// <value>
        /// The application secret.
        /// </value>
        public string AppSecret { get; set; } = "~TnlOHe.rY3_kyJtkMg2F_4oe6DWNkf0V~";

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId { get; set; } = "2899ab60-00c1-4206-b226-2ef30e219cb3";

        /// <summary>
        /// Gets or sets the work space identifier.
        /// </summary>
        /// <value>
        /// The work space identifier.
        /// </value>
        public string WorkSpaceId { get; set; } = "f93590dc-2cb9-4f15-a726-dd8a186907ee";

        /// <summary>
        /// Gets or sets the report identifier.
        /// </summary>
        /// <value>
        /// The report identifier.
        /// </value>
        public string ReportId { get; set; }

    }

    public class Temp : Microsoft.PowerBI.Api.Models.Report
    {
    }
}
