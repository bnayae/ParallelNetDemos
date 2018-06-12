using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiSample.Controllers
{
    // use it from browser: http://localhost:5287/api/download

    public class DownloadController : ApiController
    {
        private const string URL = "http://www.imdb.com/";

        public async Task<string> Get()
        {
            Thread.Sleep(1500);
            return "X";
        }
    }
}
