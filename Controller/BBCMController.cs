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
using SQLUI;
using System.Xml;
using HIS_DB_Lib;
namespace DB2VM.Controller
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBCMController : ControllerBase
    {


        static string MySQL_server = $"{ConfigurationManager.AppSettings["MySQL_server"]}";
        static string MySQL_database = $"{ConfigurationManager.AppSettings["MySQL_database"]}";
        static string MySQL_userid = $"{ConfigurationManager.AppSettings["MySQL_user"]}";
        static string MySQL_password = $"{ConfigurationManager.AppSettings["MySQL_password"]}";
        static string MySQL_port = $"{ConfigurationManager.AppSettings["MySQL_port"]}";

        private SQLControl sQLControl_藥檔資料 = new SQLControl(MySQL_server, MySQL_database, "medicine_page_cloud", MySQL_userid, MySQL_password, (uint)MySQL_port.StringToInt32(), MySql.Data.MySqlClient.MySqlSslMode.None);


        [HttpGet]
        public string Get(string Code)
        {
            if (Code.StringIsEmpty()) return "[]";
            System.Text.StringBuilder soap = new System.Text.StringBuilder();
            soap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            soap.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            soap.Append("<soap:Body>");
            soap.Append("<Drug_DATA xmlns=\"http://tempuri.org/\">");
            soap.Append("<myhospital>1</myhospital>");
            soap.Append("<myNS></myNS>");
            soap.Append($"<myMCODE>{Code}</myMCODE>");
            soap.Append("<myDB>OPD</myDB>");
            soap.Append("</Drug_DATA>");
            soap.Append("</soap:Body>");
            soap.Append("</soap:Envelope>");
            string Xml = Basic.Net.WebServicePost("https://tpord.mmh.org.tw/ADC_WS_A226/ADCDrugWS.asmx?op=Drug_DATA", soap);
            string[] Node_array = new string[] { "soap:Body", "Drug_DATAResponse", "Drug_DATAResult", "diffgr:diffgram", "NewDataSet", "Temp1"};

            XmlElement xmlElement = Xml.Xml_GetElement(Node_array);
            string MCODE = xmlElement.Xml_GetInnerXml("MCODE");
            string FullName = xmlElement.Xml_GetInnerXml("FULLNAME");
            string ShortName = xmlElement.Xml_GetInnerXml("SHORTNAME");
            string COMPAR2 = xmlElement.Xml_GetInnerXml("COMPAR2");
            string MLEVEL = xmlElement.Xml_GetInnerXml("MLEVEL");
            if (MCODE.StringIsEmpty()) return "[]";
            List<MedClass> medClasses = new List<MedClass>();
            MedClass medClass = new MedClass();
            medClass.藥品碼 = MCODE;
            medClass.藥品名稱 = FullName;
            medClass.藥品學名 = ShortName;
            medClass.警訊藥品 = (COMPAR2 == "Y") ? "True" : "False";
            medClass.管制級別 = MLEVEL;

            medClasses.Add(medClass);

            List<object[]> list_藥檔資料 = sQLControl_藥檔資料.GetRowsByDefult(null, (int)enum_雲端藥檔.藥品碼, MCODE);
            if(list_藥檔資料.Count == 0)
            {
                object[] value = new object[new enum_雲端藥檔().GetLength()];
                value[(int)enum_雲端藥檔.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_雲端藥檔.藥品碼] = medClass.藥品碼;
                value[(int)enum_雲端藥檔.藥品名稱] = medClass.藥品名稱;
                value[(int)enum_雲端藥檔.藥品學名] = medClass.藥品學名;
                value[(int)enum_雲端藥檔.警訊藥品] = medClass.警訊藥品;
                value[(int)enum_雲端藥檔.管制級別] = medClass.管制級別;
                sQLControl_藥檔資料.AddRow(null, value);
            }
            else
            {
                object[] value = list_藥檔資料[0];
                value[(int)enum_雲端藥檔.藥品碼] = medClass.藥品碼;
                value[(int)enum_雲端藥檔.藥品名稱] = medClass.藥品名稱;
                value[(int)enum_雲端藥檔.藥品學名] = medClass.藥品學名;
                value[(int)enum_雲端藥檔.警訊藥品] = medClass.警訊藥品;
                value[(int)enum_雲端藥檔.管制級別] = medClass.管制級別;
                List<object[]> list = new List<object[]>();
                list.Add(value);
                sQLControl_藥檔資料.UpdateByDefulteExtra(null, list);
            }
  

            if (medClasses.Count == 0) return "[]";
            string jsonString = medClasses.JsonSerializationt();
            return jsonString;
        }
    }
}
