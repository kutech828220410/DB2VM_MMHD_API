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
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using HIS_DB_Lib;
namespace DB2VM_API
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class OutTakeMedController
    {
        [Route("log")]
        [HttpGet()]
        public string Get_log()
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl("127.0.0.1", "dbvm", "log", "user", "66437068", 3306, MySqlSslMode.None);
            List<object[]> list_value = sQLControl.GetAllRows(null);

            return list_value.JsonSerializationt(true);
        }
        [Route("Sample")]
        [HttpGet()]
        public string Get_Sample()
        {
            string str = Basic.Net.WEBApiGet(@"http://127.0.0.1:4433/api/OutTakeMed/Sample");

            return str;
        }
        [HttpPost]
        public string Post([FromBody] List<class_OutTakeMed_data> data)
        {
            //if (data.Count == 0) return "";
            //string json_out = "";
            //List<class_OutTakeMed_data> data_B1UD = (from temp in data
            //                                         where temp.成本中心.ToUpper() == "1"
            //                                         select temp).ToList();

            //List<class_OutTakeMed_data> data_B2UD = (from temp in data
            //                                         where temp.成本中心.ToUpper() == "2"
            //                                         select temp).ToList();

            //if (data[0].成本中心 == "1")
            //{
            //    returnData returnData = new returnData();
            //    returnData.ServerName = "B1UD";
            //    returnData.Data = data_B1UD;
            //    json_out = Basic.Net.WEBApiPostJson("http://10.13.66.58:4433/api/OutTakeMed/new", returnData.JsonSerializationt());
            //    returnData = json_out.JsonDeserializet<returnData>();
            //    if (returnData == null)
            //    {
            //        return "NG";
            //    }
            //    if (returnData.Code != 200)
            //    {
            //        return "NG";
            //    }
            //}
            //if (data[0].成本中心 == "2")
            //{
            //    returnData returnData = new returnData();
            //    returnData.ServerName = "B2UD";
            //    returnData.Data = data_B2UD;
            //    json_out = Basic.Net.WEBApiPostJson("http://10.13.66.58:4433/api/OutTakeMed/new", returnData.JsonSerializationt());
            //    returnData = json_out.JsonDeserializet<returnData>();
            //    if (returnData == null)
            //    {
            //        return "NG";
            //    }
            //    if (returnData.Code != 200)
            //    {
            //        return "NG";
            //    }
            //}
            Dictionary<string, List<class_OutTakeMed_data>> dic = ToDicByCostCenter(data);
            foreach (string key in dic.Keys)
            {
                List<class_OutTakeMed_data> outTakeMed_Datas = dic[key];
                string 成本中心 = key;
                returnData returnData = new returnData();
                returnData.ServerName = 成本中心;
                returnData.Data = outTakeMed_Datas;
                string json_out = Basic.Net.WEBApiPostJson("http://127.0.0.1:4433/api/OutTakeMed/new", returnData.JsonSerializationt());
                returnData = json_out.JsonDeserializet<returnData>();
                if (returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
            }
            return "OK";
        }

        [Route("storehouse")]
        [HttpPost]
        public string Post_storehouse([FromBody] List<class_OutTakeMed_data> data)
        {
            Logger.Log("OutTakeMed.storehouse", data.JsonSerializationt());

            string[] storehouse_names = new string[] { "3PTS錠1", "3PTS錠2", "3PTS錠3", "3PTS錠4", "3PTS錠5", "3PTS錠6", "3PTS水1", "3PTS水2" };
            //for(int i = 0; i< data.Count)


            for (int i = 0; i < storehouse_names.Length; i++)
            {
                string storehouse_name = storehouse_names[i];
                List<class_OutTakeMed_data> data_buf = (from temp in data
                                                        where temp.成本中心.ToUpper() == storehouse_name
                                                        select temp).ToList();
                storehouse_name = storehouse_name.Replace("3PTS", "");
                storehouse_name = $"{storehouse_name}台";

                
                if (data_buf.Count > 0)
                {
                    returnData returnData = new returnData();
                    returnData.ServerName = storehouse_name;
                    returnData.Data = data_buf;
                    string json_in = returnData.JsonSerializationt();
                    string json_out = Basic.Net.WEBApiPostJson("http://127.0.0.1:4433/api/OutTakeMed/new", returnData.JsonSerializationt());
                    returnData = json_out.JsonDeserializet<returnData>();
                    if (returnData.Code != 200)
                    {
                        string json_err = returnData.JsonSerializationt(true);
                        Logger.Log("OutTakeMed.storehouse_Err", json_err);
                        return json_err;
                    }
                }
            }
         
            return "OK";
        }
        private Dictionary<string, List<class_OutTakeMed_data>> ToDicByCostCenter(List<class_OutTakeMed_data> class_OutTakeMed_data)
        {
            Dictionary<string, List<class_OutTakeMed_data>> dictionary = new Dictionary<string, List<class_OutTakeMed_data>>();
            foreach (var item in class_OutTakeMed_data)
            {
                if (dictionary.TryGetValue(item.成本中心, out List<class_OutTakeMed_data> list))
                {
                    list.Add(item);
                }
                else
                {
                    dictionary[item.成本中心] = new List<class_OutTakeMed_data>() { item };
                }
            }
            return dictionary;
        }
    }
}
