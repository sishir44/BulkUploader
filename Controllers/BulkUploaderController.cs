using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Configuration;
using System.Data.SqlClient;
using DevExpress.Data.Utils;
using DevExpress.XtraReports.UI;
using BulkUploader.DAL;
using BulkUploader.Models;

namespace BulkUploader.Controllers
{
    public class BulkUploaderController : Controller
    {
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult UploadExcel()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult UploadExcel(
            HttpPostedFileBase KPINational,
            HttpPostedFileBase KPINational1,
            HttpPostedFileBase MISLocation,
            HttpPostedFileBase KPILocation1,
            //HttpPostedFileBase MISNational,
            HttpPostedFileBase MTDNational1,
            HttpPostedFileBase MTDNational2,
            HttpPostedFileBase MTDLocation3,
            HttpPostedFileBase MTDLocation4,
            HttpPostedFileBase LeadsCalled,
            HttpPostedFileBase EmployeeRepData,
            HttpPostedFileBase Marketing,
            HttpPostedFileBase RequisitionsTable,
            HttpPostedFileBase Wiredraw,
            HttpPostedFileBase Wirelessraw,
            HttpPostedFileBase EmployeeDetailforTime,
            HttpPostedFileBase ATTUIDDetailsMIS,
            HttpPostedFileBase TATotalHoursSummary,
            string date
            )
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {
                    
                    { "KPINational", (KPINational, "Temp_Daily_MTD_KPINational") },
                    { "KPINational1", (KPINational1,"Temp_Daily_MTD_KPINational1") },
                    { "MISLocation", (MISLocation, "Temp_Daily_MTD_MISLocation") },
                    { "KPILocation1", (KPILocation1,"Temp_Daily_MTD_KPILocation") },
                    //{ "MISNational", (MISNational, "") },
                    { "MTDNational1", (MTDNational1,"Temp_Daily_MTD_MTDNational1") },
                    { "MTDNational2", (MTDNational2,"Temp_Daily_MTD_MTDNational2") },
                    { "MTDLocation3", (MTDLocation3,"Temp_Daily_MTD_MTDLocation3") },
                    { "MTDLocation4", (MTDLocation4,"Temp_Daily_MTD_MTDLocation4") },
                    { "LeadsCalled", (LeadsCalled, "Temp_Daily_others_Leadscalled") },
                    { "EmployeeRepData", (EmployeeRepData, "Temp_Daily_MTD_Repdata") },
                    { "Marketing", (Marketing, "Temp_Daily_others_Marketing") },
                    { "RequisitionsTable", (RequisitionsTable, "Temp_Daily_others_Requisitions") },
                    { "Wiredraw", (Wiredraw, "Temp_Daily_others_Wiredraw") },
                    { "Wirelessraw", (Wirelessraw, "Temp_Daily_others_Wirelessraw") },
                    { "EmployeeDetailforTime", (EmployeeDetailforTime, "Temp_Daily_MTD_EmployeeDetailforTime") },
                    { "ATTUIDDetailsMIS", (ATTUIDDetailsMIS, "Temp_Daily_others_ATTUID") },
                    { "TATotalHoursSummary", (TATotalHoursSummary, "Temp_Daily_MTD_TotalHours") },
                    
                 
                };
                //var missingFiles = files.Where(f => f.Value.File == null || f.Value.File.ContentLength == 0).Select(f => f.Key).ToList();
                var uploadedFiles = new List<string>();
                var missingFiles = new List<string>();
                string res = "";
                string status = "";
                foreach (var item in files)
                {
                    var file = item.Value.File;

                    if (file != null && file.ContentLength > 0)
                    {
                        res = UploadToTable(file, item.Value.Table);
                        if(res != "1")
                        {
                            //ViewBag.Warning = "Data is not uploaded on temp table for: " + item.Key;
                            ViewBag.Warning = "Data is not uploaded on temp table for: " + item.Key +"\n" + res;
                            continue;
                        }
                        uploadedFiles.Add(item.Key);
                    }
                    else
                    {
                        missingFiles.Add(item.Key);
                    }
                }

                if (uploadedFiles.Any() && res != "" && res != null)
                {
                    ViewBag.Success = "Data Uploaded to temp table: " + string.Join(", ", uploadedFiles);
                } 

                if (missingFiles.Any())
                    ViewBag.Warning = ViewBag.Warning + "\n" + "Not Selected Files: " + string.Join(", ", missingFiles);

                if (res == "1")
                {
                    status = DataStringGp.UploaderUpdateSTP(date);
                    if (status == "1")
                    {
                        ViewBag.Success = "Uploaded Successfully!";
                    }
                    else
                    {
                        //ViewBag.Warning = ViewBag.Warning + "\n" + "Not Uploaded Successfully ❌";
                        ViewBag.Error =  status;
                    }
                }

                return View("UploadExcel");

                //UploadToTable(KPILocation, "Temp_KPILocation_MTD");
                //UploadToTable(KPINational, "Temp_KPINational_MTD");
                //UploadToTable(MISLocation, "Temp_MISLocation_MTD");
                //UploadToTable(MTDLocation3, "Temp_MTDLocation3_MTD");
                //UploadToTable(MTDLocation4, "Temp_MTDLocation4_MTD");
                //UploadToTable(MTDNational1, "Temp_MTDNational1_MTD");
                //UploadToTable(MTDNational2, "Temp_MTDNational2_MTD");
                //UploadToTable(file8, "Table8");
                //UploadToTable(file9, "Table9");
                //UploadToTable(file10, "Table10");

                //ViewBag.Message = "All files uploaded successfully!";
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("UploadExcel");
                //ViewBag.Message = "Error: " + ex.Message;
            }

            //return View("Upload");
        }


        // =============================
        // COMMON UPLOAD METHOD
        // =============================
        private string UploadToTable(HttpPostedFileBase file, string tableName)
        {
            if (file == null || file.ContentLength <= 0)
            {
                throw new Exception("File was not selected.");
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var worksheet = package.Workbook.Worksheets[1];

                DataTable dt = ExcelHelper.ExcelToDataTable(worksheet);

                // 🔹 Convert empty cells to NULL
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (row[col] == null || string.IsNullOrWhiteSpace(row[col].ToString()))
                        {
                            row[col] = DBNull.Value;
                        }
                    }
                }
                return BulkInsert(dt, tableName);
            }
        }


        // =============================
        // BULK INSERT METHOD
        // =============================
        public string BulkInsert(DataTable dt, string tableName)
        {
            try
            {
                string conStr = ConfigurationManager.ConnectionStrings["APIConnStr"].ConnectionString;

                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    // 🔹 Step 1: Delete old records
                    using (SqlCommand cmd = new SqlCommand($"DELETE FROM [{tableName}]", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // 🔹 Step 2: Bulk insert new records
                    using (SqlBulkCopy bulk = new SqlBulkCopy(con))
                    {
                        bulk.DestinationTableName = tableName;
                        bulk.WriteToServer(dt);
                    }
                }
                return "1";
            }
            catch (Exception ex)
            {
                return "Error has occured :  " + ex.Message;
                //return "0";
            }
        }
    }
}