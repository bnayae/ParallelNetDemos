using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiSample.Controllers
{
    [RoutePrefix("api/simple")]
    public class SimpleController : ApiController
    {
        [Route("sync")]
        public string GetSleep()
        {
            Thread.Sleep(1000);
            return "S";
        }

        [Route("run-blocking")]
        public async Task<string> GetRunBlocking()
        {
            var result = await Task.Run(() =>
            {
                Thread.Sleep(1000);
                return "R";
            });
            return result;
        }

        [Route("run-async")]
        public async Task<string> GetRunAsync()
        {
            var result = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                return "RA";
            });
            return result;
        }

        [Route("async")]
        public async Task<string> GetAsync()
        {
            await Task.Delay(1000);
            return "A";
        }
    }
}
