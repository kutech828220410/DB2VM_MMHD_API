using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBM.Data.DB2.Core;
using System.Data;
using System.Configuration;
using Basic;
using Oracle.ManagedDataAccess.Client;
namespace DB2VM
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return Basic.Net.WEBApiGet($"http://10.13.66.58:4433/api/test");
        }


    }
}
