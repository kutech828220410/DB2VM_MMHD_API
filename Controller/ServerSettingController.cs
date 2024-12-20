using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using HIS_DB_Lib;
namespace DB2VM_API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerSettingController : Controller
    {
        private string url = "http://10.13.66.58:4433/api/ServerSetting";

        [Route("init")]
        [HttpGet]
        public string GET_init()
        {
            return Basic.Net.WEBApiGet($"{url}/init");
        }

        [Route("type")]
        [HttpGet]
        public string GET_type()
        {
            return Basic.Net.WEBApiGet($"{url}/type");
        }

        [Route("program")]
        [HttpGet]
        public string GET_program()
        {
            return Basic.Net.WEBApiGet($"{url}/program");
        }

        [HttpGet]
        public string GET()
        {
            return Basic.Net.WEBApiGet($"{url}");
        }

        [Route("add")]
        [HttpPost]
        public string POST_add([FromBody] returnData returnData)
        {
            return Basic.Net.WEBApiPostJson($"{url}/add", returnData.JsonSerializationt());
        }

        [Route("delete")]
        [HttpPost]
        public string POST_delete([FromBody] returnData returnData)
        {
            return Basic.Net.WEBApiPostJson($"{url}/delete", returnData.JsonSerializationt());
        }
    }
}
