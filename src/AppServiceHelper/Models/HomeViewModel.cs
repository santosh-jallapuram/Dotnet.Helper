using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceHelper.Models
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
}
