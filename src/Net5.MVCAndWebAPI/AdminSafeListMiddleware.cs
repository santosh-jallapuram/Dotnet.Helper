// credits : https://docs.microsoft.com/en-us/aspnet/core/security/ip-safelist

namespace Net5.MVCAndWebAPI
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private readonly byte[][] _safelist;

        public AdminSafeListMiddleware(
            RequestDelegate next,
            ILogger<AdminSafeListMiddleware> logger,
            string safelist)
        {
            string[] ips = safelist.Split(';').Where(a => IPAddress.TryParse(a, out _)).ToArray();
            _safelist = new byte[ips.Length][];
            for (var i = 0; i < ips.Length; i++)
            {
                _safelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
            }

            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Host.Host != "localhost")
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in _safelist)
                {
                    if (address.SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                }

                if (badIp)
                {
                    _logger.LogWarning(
                        "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync($"Forbidden Request from Remote IP address: {remoteIp}. Please whitelist your IP if your are admin.");
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
