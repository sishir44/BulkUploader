using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using BulkUploader.Models;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace BulkUploader.Controllers
{
    public class GPReportUploaderController : Controller
    {
        DataTable GPReportUploader_Dt;
        DataTable Callidus_Dt;
        DataTable Goals_Dt;
        public static DateTime time { get; set; }
        public string Date = "";
        public string CheckBackDateReport = "";

        public ActionResult GPUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GPUploader(HttpPostedFileBase file, HttpPostedFileBase file1)
        {
            try
            {
                string res = "Please Upload GP Report File";
                SaveFiles(file, "GP_Report");
                res = UploadFile(file);
                if (Regex.IsMatch(res, @"[a-zA-Z]"))
                {
                    ViewBag.Message = res;
                    ViewBag.Style = "red";
                }
                else
                {
                    ViewBag.Message = "Uploaded " + res + " records successfully";
                    ViewBag.Style = "green";
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Something Went Wrong";
                return View();
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult BudGetGoalUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BudGetGoalUploader(HttpPostedFileBase MobilyBudGet, string date)
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
            {

                { "MobilyBudGet", (MobilyBudGet,"Temp_BudGetGoal") },
            };
                var uploadedFiles = new List<string>();
                var missingFiles = new List<string>();
                string res = "";
                string status = "";
                foreach (var item in files)
                {
                    var file = item.Value.File;

                    if (file != null && file.ContentLength > 0)
                    {
                        SaveFile(file);
                        res = UploadToTable(file, item.Value.Table);
                        if (res != "1")
                        {
                            //ViewBag.Warning = "Data is not uploaded on temp table for: " + item.Key;
                            ViewBag.Warning = "Data is not uploaded on temp table for: " + item.Key + "\n" + res;
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
                    status = DataStringGp.BudGetGoalUpdateSTP(date);
                    if (status == "1" || Convert.ToInt32(status) > 0)
                    {
                        ViewBag.Success = "Uploaded Successfully!";
                    }
                    else
                    {
                        //ViewBag.Warning = ViewBag.Warning + "\n" + "Not Uploaded Successfully ❌";
                        ViewBag.Error = status;
                    }
                }
                return View("BudGetGoalUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("BudGetGoalUploader");
            }
        }

        public void SaveFiles(HttpPostedFileBase file, string ReportName)
        {

            try
            {
                List<HttpPostedFileBase> FileEnum = new List<HttpPostedFileBase> { file };

                string date = DateTime.Now.ToString("yyyyMMdd");
                string dateTime = DateTime.Now.ToString("yyyyMMdd_hhmmss");

                foreach (HttpPostedFileBase files in FileEnum)
                {
                    if (files != null)
                    {
                        var root = Server.MapPath("~/UploadedFiles/" + date);
                        bool exists = Directory.Exists(root);
                        if (!exists)
                        {
                            Directory.CreateDirectory(root);
                        }

                        var ReportRoot = Server.MapPath("~/UploadedFiles/" + date + "/" + ReportName);
                        bool isexists = Directory.Exists(ReportRoot);
                        if (!isexists)
                        {
                            Directory.CreateDirectory(ReportRoot);
                        }

                        string FileName = files.FileName;
                        FileName = FileName.Replace(" ", "");

                        string[] NameArray = FileName.Split('.');

                        FileName = NameArray.FirstOrDefault();
                        string FileType = NameArray.LastOrDefault();

                        var NewFileName = FileName + "_" + dateTime + "." + FileType;
                        files.SaveAs(Path.Combine(ReportRoot, NewFileName));
                    }
                }
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(0);
                string line = frame.GetFileLineNumber().ToString();
                Common.recorderror("GP_ReportUploader/HomeController/SaveFiles", ex.Message, "", line);
            }
        }
        public string UploadFile(HttpPostedFileBase file, [Optional] HttpPostedFileBase file2)
        {
            string res = "";

            if (Request.Files.Count > 0)
            {
                try
                {
                    string RepTime = DataStringGp.createRepTime();

                    object[,] GPReportUploaderObj = null;
                    int noOfCol = 0;
                    int noOfRow = 0;

                    if ((file != null) && (file.ContentLength > 0))
                    {
                        byte[] fileBytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            noOfCol = workSheet.Dimension.End.Column;
                            noOfRow = workSheet.Dimension.End.Row;
                            GPReportUploaderObj = new object[noOfRow, noOfCol];
                            GPReportUploaderObj = (object[,])workSheet.Cells.Value;
                        }

                        string result = file.FileName.Substring(0, file.FileName.IndexOf("."));

                        if (result == "temp_gpstatus")
                        {
                            GPReportUploader_Dt = UploadExcel.GetDataTable(GPReportUploaderObj);
                            GPReportUploader_Dt.TableName = result;

                            List<string> XLColumnNames = new List<string>();
                            foreach (DataColumn col in GPReportUploader_Dt.Columns)
                            {
                                XLColumnNames.Add(col.ColumnName);
                            }
                            DataTable Header = DataStringGp.GetTableColumnNames(GPReportUploader_Dt.TableName);
                            List<string> fixedColumns = new List<string>();
                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }
                            List<string> lstFieldsRequired = XLColumnNames.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            List<string> lstFieldsMissing = fixedColumns.Where(a => XLColumnNames.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in GP Report file:";
                                    int a = 1;
                                    foreach (string field in lstFieldsMissing)
                                    {
                                        res += "<br />" + a.ToString() + ") " + field;
                                        a++;
                                    }
                                }
                                if (res != "")
                                {
                                    return res;
                                }
                            }

                            if (res != null)
                            {
                                res = DataStringGp.BulkOperationDB_GPReport(GPReportUploader_Dt, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }

                            if (res != null)
                            {
                                res = DataStringGp.InsertGPStatus();
                                return GPReportUploader_Dt.Rows.Count.ToString();
                            }
                            else
                            {
                                return "Something Went Wrong during Insertion of GP Report";
                            }
                        } 
                        else
                        {
                            return "Please upload the file";
                        }
                    }
                }
                catch (Exception ex)
                {
                    StackTrace st = new StackTrace(ex, true);
                    StackFrame frame = st.GetFrame(0);
                    string line = frame.GetFileLineNumber().ToString();
                    Common.recorderror("GP_ReportUploader/HomeController/UploadFile", ex.Message, "", line);
                    return ex.Message;
                }
            }
            return res;
        }


        /* New */
        public void SaveFile(HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return;

                string date = DateTime.Now.ToString("yyyyMMdd");
                string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                // Create folder paths
                string rootPath = Server.MapPath($"~/UploadedFiles/{date}");
                string reportPath = Path.Combine(rootPath, file.FileName);

                // Ensure directories exist
                Directory.CreateDirectory(reportPath);

                // Safe filename handling
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "");
                string extension = Path.GetExtension(file.FileName);

                string newFileName = $"{fileNameWithoutExt}_{dateTime}{extension}";

                string fullPath = Path.Combine(reportPath, newFileName);

                file.SaveAs(fullPath);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(0);
                string line = frame?.GetFileLineNumber().ToString();
                Common.recorderror("BukhUploader/BulkUploaderController/SaveFiles", ex.Message, "", line);
            }
        }

        // ===========  COMMON UPLOAD METHOD ================== //
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
        // ===========  BULK INSERT METHOD  ================== //
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