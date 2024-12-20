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
    public class sessionController : Controller
    {
        public class HIS_login
        {
            public string Hosp { get; set; }
            public string Message { get; set; }
            public string APName { get; set; }
            public string Id { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Idno { get; set; }
            public string email { get; set; }
            public string Authority { get; set; }
            public string CostDP { get; set; }
            public string CostDPName { get; set; }
        }

        static string server = $"{ConfigurationManager.AppSettings["MySQL_server"]}";
        static string database = $"{ConfigurationManager.AppSettings["MySQL_database"]}";
        static string userid = $"{ConfigurationManager.AppSettings["MySQL_user"]}";
        static string password = $"{ConfigurationManager.AppSettings["MySQL_password"]}";
        static uint port = (uint)$"{ConfigurationManager.AppSettings["MySQL_port"]}".StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        [Route("MMHlogin")]
        [HttpPost]
        public string POST_UD1F_login([FromBody] returnData returnData)
        {
            SQLControl sQLControl_person_page = new SQLControl(server, database, "person_page", userid, password, port, SSLMode);
            sessionClass data = returnData.Data.ObjToClass<HIS_DB_Lib.sessionClass>();
            List<object[]> list_person_page = sQLControl_person_page.GetAllRows(null);
            List<object[]> list_person_page_buf = new List<object[]>();
            string ID = data.ID;
            string Password = data.Password;
            HIS_login hIS_Login = new HIS_login();
            hIS_Login.Hosp = "1";
            hIS_Login.Id = ID.ToUpper();
            hIS_Login.Password = Password;
            hIS_Login.APName = "LESHINEDISPENSING";
            string json_result = Net.WEBApiPostJson("https://tpexm.mmh.org.tw/webemrapia232/api/login", hIS_Login.JsonSerializationt());

            hIS_Login = json_result.JsonDeserializet<HIS_login>();
            if(hIS_Login.Message == "登入成功")
            {
                list_person_page_buf = list_person_page.GetRows((int)enum_人員資料.ID, ID);
                object[] value = new object[new enum_人員資料().GetLength()];
                if (list_person_page_buf.Count == 0)
                {
                    value[(int)enum_人員資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_人員資料.ID] = ID;
                    value[(int)enum_人員資料.密碼] = Password;
                    value[(int)enum_人員資料.姓名] = hIS_Login.Name;
                    if(hIS_Login.Authority.StringToInt32() > 0)
                    {
                        value[(int)enum_人員資料.權限等級] = hIS_Login.Authority;
                    }
                    else
                    {
                        value[(int)enum_人員資料.權限等級] = "1";
                    }
                    if (list_person_page.Count % 6 == 0) value[(int)enum_人員資料.顏色] = System.Drawing.Color.Red.ToColorString();
                    if (list_person_page.Count % 6 == 1) value[(int)enum_人員資料.顏色] = System.Drawing.Color.Orange.ToColorString();
                    if (list_person_page.Count % 6 == 2) value[(int)enum_人員資料.顏色] = System.Drawing.Color.Yellow.ToColorString();
                    if (list_person_page.Count % 6 == 3) value[(int)enum_人員資料.顏色] = System.Drawing.Color.Linen.ToColorString();
                    if (list_person_page.Count % 6 == 4) value[(int)enum_人員資料.顏色] = System.Drawing.Color.Blue.ToColorString();
                    if (list_person_page.Count % 6 == 5) value[(int)enum_人員資料.顏色] = System.Drawing.Color.Pink.ToColorString();
                    if (list_person_page.Count % 6 == 6) value[(int)enum_人員資料.顏色] = System.Drawing.Color.PeachPuff.ToColorString();

                    sQLControl_person_page.AddRow(null, value);
                }
                else
                {
                    value = list_person_page_buf[0];
                    value[(int)enum_人員資料.ID] = ID;
                    value[(int)enum_人員資料.密碼] = Password;
                    value[(int)enum_人員資料.姓名] = hIS_Login.Name;
                    if (hIS_Login.Authority.StringToInt32() > 0)
                    {
                        value[(int)enum_人員資料.權限等級] = hIS_Login.Authority;
                    }
                    else
                    {
                        value[(int)enum_人員資料.權限等級] = "1";
                    }
                    List<object[]> list = new List<object[]>();
                    list.Add(value);
                    sQLControl_person_page.UpdateByDefulteExtra(null, list);
                }
            }
            json_result = Net.WEBApiPostJson("http://10.13.66.58:4433/api/session/login", returnData.JsonSerializationt());
            return json_result;
        }
    }
}
