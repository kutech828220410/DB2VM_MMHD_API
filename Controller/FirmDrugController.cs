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
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace DB2VM_API
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class FirmDrugController : Controller
    {
        //台北：https://tpord.mmh.org.tw/ADC_WEBAPI_A226/api/FirmDrug

        //淡水：https://tsord.mmh.org.tw/ADC_WEBAPI_A226/api/FirmDrug

        //欄位說明：

        //HOSPITAL 院區：台北: 1、淡水: 2、台東: 3、新竹:4、竹兒:5
        //DB 資料庫：正式：OPD、測試:TEST
        //FirmName 廠商名稱
        //GroupCode 場域：1:台北B1住院藥局、2:台北B2住院藥局、3:淡水門診藥局
        //Type 類型：調劑台、冰箱標籤、ADC藥櫃
        //MCODE 馬偕碼：多筆用,隔開 Ex.25001,25002,25003,25004
        //OPER 操作者：員工代號
        //回覆資料欄位：

        //isSuccess 是否成功：成功: Y；失敗: N
        //Message 失敗原因

        public class class_儲位總庫存表
        {
            [JsonPropertyName("storage_name")]
            public string 儲位名稱 { get; set; }
            [JsonPropertyName("Code")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("Neme")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("package")]
            public string 單位 { get; set; }
            [JsonPropertyName("inventory")]
            public string 庫存 { get; set; }
            [JsonPropertyName("storage_type")]
            public string 儲位型式 { get; set; }

        }

        public class class_FirmDrug
        {
            public string HOSPITAL { get; set; }
            public string DB { get; set; }
            public string FirmName { get; set; }
            public string GroupCode { get; set; }
            public string Type { get; set; }
            public string MCODE { get; set; }
            public string OPER { get; set; }

        }

        [Route("B1UD")]
        [HttpGet()]
        public string Get_B1UD(string? ID)
        {
            if (ID.StringIsEmpty()) ID = "";
            medicine_pageController medicine_PageController = new medicine_pageController();
            string str = medicine_PageController.Get_storage_list("1");
            List<class_儲位總庫存表> list_storage_list = str.JsonDeserializet<List<class_儲位總庫存表>>();
            List<class_儲位總庫存表> list_storage_list_buf = new List<class_儲位總庫存表>();
            List<string> Codes = (from value in list_storage_list
                                 where value.藥品碼.StringIsEmpty() == false
                                 select value.藥品碼).Distinct().ToList();
            class_FirmDrug class_FirmDrug = new class_FirmDrug();

            class_FirmDrug.HOSPITAL = "1";
            class_FirmDrug.DB = "OPD";
            class_FirmDrug.FirmName = "力翔";
            class_FirmDrug.GroupCode = "1";
            class_FirmDrug.Type = "調劑台";
            class_FirmDrug.OPER = ID;
            for(int i = 0; i < Codes.Count; i++)
            {
                class_FirmDrug.MCODE += Codes[i];
                if (i != Codes.Count - 1) class_FirmDrug.MCODE += ",";
            }
            string FirmDrug_Json = class_FirmDrug.JsonSerializationt();
            string result = Basic.Net.WEBApiPostJson("https://tpord.mmh.org.tw/ADC_WEBAPI_A226/api/FirmDrug", FirmDrug_Json);

            return result;
        }

        [Route("B2UD")]
        [HttpGet()]
        public string Get_B2UD(string? ID)
        {
            if (ID.StringIsEmpty()) ID = "";
            medicine_pageController medicine_PageController = new medicine_pageController();
            string str = medicine_PageController.Get_storage_list("2");
            List<class_儲位總庫存表> list_storage_list = str.JsonDeserializet<List<class_儲位總庫存表>>();
            List<class_儲位總庫存表> list_storage_list_buf = new List<class_儲位總庫存表>();
            List<string> Codes = (from value in list_storage_list
                                  where value.藥品碼.StringIsEmpty() == false
                                  select value.藥品碼).Distinct().ToList();
            class_FirmDrug class_FirmDrug = new class_FirmDrug();

            class_FirmDrug.HOSPITAL = "1";
            class_FirmDrug.DB = "OPD";
            class_FirmDrug.FirmName = "力翔";
            class_FirmDrug.GroupCode = "2";
            class_FirmDrug.Type = "調劑台";
            class_FirmDrug.OPER = ID;
            for (int i = 0; i < Codes.Count; i++)
            {
                class_FirmDrug.MCODE += Codes[i];
                if (i != Codes.Count - 1) class_FirmDrug.MCODE += ",";
            }
            string FirmDrug_Json = class_FirmDrug.JsonSerializationt();
            string result = Basic.Net.WEBApiPostJson("https://tpord.mmh.org.tw/ADC_WEBAPI_A226/api/FirmDrug", FirmDrug_Json);

            return result;
        }

    }
}
