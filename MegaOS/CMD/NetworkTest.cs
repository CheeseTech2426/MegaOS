using CosmosHttp.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmosHttp.Client;
namespace MegaOS.CMD {
    internal class NetworkTest {
        public void Run() {
            HttpRequest request = new();
            request.IP = "34.223.124.45";
            request.Domain = "neverssl.com"; //very useful for subdomains on same IP
            request.Path = "/";
            request.Method = "GET";
            request.Send();
            Console.WriteLine(request.Response.Content);
        }
    }
}
