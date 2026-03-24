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
using System.Globalization;

namespace BulkUploader.Controllers
{
    public class HomeController : Controller
    {
        DataTable DataTbl;
        DataTable Callidus_Dt;
        DataTable Goals_Dt;
        public static DateTime time { get; set; }
        public string Date = "";
        public string CheckBackDateReport = "";

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, HttpPostedFileBase file1)
        {
            try
            {
                string res = "Please Upload PnlStatement File";
                SaveFiles(file, "Pnl_Status");
                res = UploadFile(file, null,null,null,null);
                if (Regex.IsMatch(res, @"[a-zA-Z]"))
                {
                    ViewBag.Message = res; 
                    ViewBag.Style = "red";
                } else { 
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
        
        public ActionResult GPUploaderABO()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GPUploaderABO(HttpPostedFileBase file, HttpPostedFileBase file1)
        {
            try
            {
                string res = "Please Upload Both ABO Files,PDR And VRR";
                if (file != null && file1 != null)
                {
                    SaveFiles(file, "GPUploaderABO");
                    res = UploadFile(file, null, null, null, null);
                }
                ViewBag.Message = res;
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }

        public ActionResult PerformanceEvlUploader()
        {
            global.userID = Request.QueryString["userid"];
            global.decrpedUserId = EncryptionHelper.Decrypt(global.userID);
            DataTable dt = DataStringGp.GetEmpIDList();
            ViewBag.EmpIDList = dt;
            return View();
        }
        [HttpPost]
        public ActionResult PerformanceEvlUploader(PerformanceEvlUploadVM model )
        {
            ViewBag.EmpIDList = DataStringGp.GetEmpIDList();
            model.EmpID = model.EmpID == "All" ? null : model.EmpID;
            model.Month = model.Month == "All" ? null : model.Month;

            try
            {
                string res = "Please Upload PerformanceEvl file";
                //if (file.FileName != null)
                //{
                //    SaveFiles(file, "PerformanceEvl");
                //    res = UploadFile(file, UploadDate);
                //}
                if (model.File.FileName != null)
                {
                    SaveFiles(model.File, "PerformanceEvl");
                    res = UploadFile(model.File, global.decrpedUserId, model.EmpID, model.Month, model.Year);
                }
                //ViewBag.Message = res;
                //ViewBag.Style = "green";
                int result = Convert.ToInt32(res);
                ViewBag.Style = result > 0 ? "green" : "red";
                ViewBag.Message = result > 0 ? "File uploaded successfully" : "File not Uploaded";

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }
        
        public ActionResult PerformanceEvlSepUploader()
        {
            global.userID = Request.QueryString["userid"];
            global.decrpedUserId = EncryptionHelper.Decrypt(global.userID);
            DataTable dt = DataStringGp.GetEmpIDList();
            ViewBag.EmpIDList = dt;
            return View();
        }
        [HttpPost]
        public ActionResult PerformanceEvlSepUploader(PerformanceEvlUploadVM model)
        {
            ViewBag.EmpIDList = DataStringGp.GetEmpIDList();
            model.EmpID = model.EmpID == "All" ? null : model.EmpID;
            model.Month = model.Month == "All" ? null : model.Month;

            try
            {
                string res = "Please Upload PerformanceEvl file";
                //if (file.FileName != null)
                //{
                //    SaveFiles(file, "PerformanceEvl");
                //    res = UploadFile(file, UploadDate);
                //}
                if (model.File.FileName != null)
                {
                    SaveFiles(model.File, "PerformanceEvl");
                    res = UploadFile(model.File, global.decrpedUserId, model.EmpID, model.Month, model.Year);
                }
                //ViewBag.Message = res;
                //ViewBag.Style = "green";
                int result = Convert.ToInt32(res);
                ViewBag.Style = result > 0 ? "green" : "red";
                ViewBag.Message = result > 0 ? "File uploaded successfully" : "File not Uploaded";

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }
        
        public ActionResult Inventoryuploader()
        {
            global.userID = Request.QueryString["userid"];
            global.decrpedUserId = EncryptionHelper.Decrypt(global.userID);
            DataTable dt = DataStringGp.GetEmpIDList();
            ViewBag.EmpIDList = dt;
            return View();
        }
        [HttpPost]
        public ActionResult Inventoryuploader(InventoryModel model)
        {
            ViewBag.EmpIDList = DataStringGp.GetEmpIDList();
            model.EmpID = model.EmpID == "All" ? null : model.EmpID;
            model.Month = model.Month == "All" ? null : model.Month;

            try
            {
                string res = "Please Upload Inventory file";
                if (model.File.FileName != null)
                {
                    SaveFiles(model.File, "Inventory");
                    res = UploadFile(model.File, global.decrpedUserId, model.EmpID, model.Month, model.Year);
                }
                int result = Convert.ToInt32(res);
                ViewBag.Style = result > 0 ? "green" : "red";
                ViewBag.Message = result > 0 ? "File uploaded successfully" : "File not Uploaded";

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }

        public ActionResult USHRUploader()
        {
            global.userID = Request.QueryString["userid"];
            global.decrpedUserId = EncryptionHelper.Decrypt(global.userID);
            DataTable dt = DataStringGp.GetEmpIDList();
            ViewBag.EmpIDList = dt;
            return View();
        }
        [HttpPost]
        public ActionResult USHRUploader(USHRModel model)
        {
            try
            {
                string res = "Please Upload HR file";
                if (model.File.FileName != null)
                {
                    SaveFiles(model.File, "PEDataUSHR");
                    res = UploadFile(model.File, global.decrpedUserId, null,null,null);
                }
                int result = Convert.ToInt32(res);
                ViewBag.Style = result > 0 ? "green" : "red";
                ViewBag.Message = result > 0 ? "File uploaded successfully" : "File not Uploaded";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }

        public ActionResult KPIUploader()
        {
            global.userID = Request.QueryString["userid"];
            global.decrpedUserId = EncryptionHelper.Decrypt(global.userID);
            DataTable dt = DataStringGp.GetEmpIDList();
            ViewBag.EmpIDList = dt;
            return View();
        }
        [HttpPost]
        public ActionResult KPIUploader(InventoryModel model)
        {
            ViewBag.EmpIDList = DataStringGp.GetEmpIDList();
            model.EmpID = model.EmpID == "All" ? null : model.EmpID;
            model.Month = model.Month == "All" ? null : model.Month;

            try
            {
                string res = "Please Upload Inventory file";
                if (model.File.FileName != null)
                {
                    SaveFiles(model.File, "KPIUploader");
                    res = UploadFile(model.File, global.decrpedUserId, model.EmpID, model.Month, model.Year);
                }
                int result = Convert.ToInt32(res);
                ViewBag.Style = result > 0 ? "green" : "red";
                ViewBag.Message = result > 0 ? "File uploaded successfully" : "File not Uploaded";

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }

        public ActionResult WirelessActivityUploader()
        {
            global.userID = Request.QueryString["userid"];
            global.decrpedUserId = EncryptionHelper.Decrypt(global.userID);
            DataTable dt = DataStringGp.GetEmpIDList();
            ViewBag.EmpIDList = dt;
            return View();
        }
        [HttpPost]
        public ActionResult WirelessActivityUploader(InventoryModel model)
        {
            ViewBag.EmpIDList = DataStringGp.GetEmpIDList();
            model.EmpID = model.EmpID == "All" ? null : model.EmpID;
            //model.Month = model.Month == "All" ? null : model.Month;
            model.Month = model.UploadDate;
            try
            {
                string res = "Please Upload WirelessActivity file";
                if (model.File.FileName != null)
                {
                    SaveFiles(model.File, "WirelessActivity");
                    res = UploadFile(model.File, global.decrpedUserId, model.EmpID, model.Month, model.Year);
                }
                int result = Convert.ToInt32(res);
                ViewBag.Style = result > 0 ? "green" : "red";
                ViewBag.Message = result > 0 ? "File uploaded successfully" : "File not Uploaded";

                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Message = "File not Uploaded!";
                ViewBag.Style = "red";
                return View();
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult ChargebackRawUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChargebackRawUploader(HttpPostedFileBase ChargeBackUploader, string date)
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {

                    { "ChargeBackUploader", (ChargeBackUploader,"Temp_ChargebackRaw") },
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
                        SaveFiles(file);
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
                    status = DataStringGp.ChargebackRawUpdateSTP(date);
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
                return View("ChargebackRawUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("ChargebackRawUploader");
            }
        }
        // =====================Fraud Transaction Raw Uploader End======== //

        //Excel Save file: Begin
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
        //Excel Save file: End

        //Excel Upload file: Begin
        public string UploadFile(HttpPostedFileBase file, string UserId, string EmpID, string Month, string Year)
        {
            string res = "";
            DataTable Header;
            List<string> ExcelColNameList = new List<string>();
            List<string> fixedColumns = new List<string>();
            List<string> lstFieldsRequired = new List<string>();
            List<string> lstFieldsMissing = new List<string>();
            string RepTime = DataStringGp.createRepTime();

            if (Request.Files.Count > 0)
            {
                try
                {
                    object[,] Obj = null;
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
                            Obj = new object[noOfRow, noOfCol];
                            Obj = (object[,])workSheet.Cells.Value;
                        }

                        string result = file.FileName.Substring(0, file.FileName.IndexOf("."));

                        //================== SEPARATER====================//
                        if (result == "Temp_PnlStatement")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames(DataTbl.TableName);

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }
                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in PnlStatement file:";
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
                                res = DataStringGp.BulkOperationDB_Level(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }

                            if (res != null)
                            {
                                res = DataStringGp.InsertPnlStatus();
                                return DataTbl.Rows.Count.ToString();
                            }
                            else
                            {
                                return "Error occur during Insertion of PnlStatement data";
                            }
                        }
                        //================== SEPARATER====================//
                        else if (result == "Temp_PerformanceEvl")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("Temp_PerformanceEvl");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in PerformanceEvl file:";
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
                                res = DataStringGp.BulkOperationDB_PerformanceEvlReport(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }
                            if (res != null)
                            {
                                res = DataStringGp.InsertPerformanceEvlStatus(UserId, EmpID, Month, Year);
                                //return DataTbl.Rows.Count.ToString();
                                return res;
                            }
                            else
                            {
                                return "Error occur during Insertion of Temp_PerformanceEvl Data";
                            }
                        }
                        //================== SEPARATER====================//
                        else if (result == "temp_PerformEvlmissing")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("temp_PerformEvlmissing");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in PerformEvlmissing file:";
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
                                res = DataStringGp.BulkOperationDB_PerformanceEvlSepReport(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }
                            if (res != null)
                            {
                                res = DataStringGp.InsertPerformanceEvlSepStatus(UserId, EmpID, Month, Year);
                                //return DataTbl.Rows.Count.ToString();
                                return res;
                            }
                            else
                            {
                                return "Error occur during Insertion of temp_PerformEvlmissing Data";
                            }
                        }
                        //================== SEPARATER====================//
                        else if (result == "Temp_Inventory")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("Temp_Inventory");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in inventory file:";
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
                                res = DataStringGp.BulkOperationDB_InventoryReport(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }
                            if (res != null)
                            {
                                res = DataStringGp.InsertInventoryStatus(UserId, EmpID, Month, Year);
                                //return DataTbl.Rows.Count.ToString();
                                return res;
                            }
                            else
                            {
                                return "Error occur during Insertion of Temp_Inventory Data";
                            }
                        }
                        //================== SEPARATER====================//
                        else if (result == "Temp_PEdata_HR")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("Temp_PEdata_HR");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in HR file:";
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
                                res = DataStringGp.BulkOperationDB_HRReport(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }
                            if (res != null)
                            {
                                res = DataStringGp.InsertHRStatus(UserId, EmpID, Month, Year);
                                //return DataTbl.Rows.Count.ToString();
                                return res;
                            }
                            else
                            {
                                return "Error occur during Insertion of Temp_USHR Data";
                            }
                        }
                        //================== SEPARATER====================//
                        else if (result == "Temp_Daily_MTD_kpi")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("Temp_Daily_MTD_kpi");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in HR file:";
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
                                res = DataStringGp.BulkOperationDB_KPIReport(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }
                            if (res != null)
                            {
                                res = DataStringGp.InsertKPIStatus(UserId, EmpID, Month, Year);
                                //return DataTbl.Rows.Count.ToString();
                                return res;
                            }
                            else
                            {
                                return "Error occur during Insertion of Temp_USHR Data";
                            }
                        }
                        //================== SEPARATER====================//
                        else if (result == "Temp_Wirelessactivity")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("Temp_Wirelessactivity");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList.Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns.Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                if (lstFieldsMissing.Count > 0)
                                {
                                    if (res != "")
                                    {
                                        res = res + "<br />and<br />";
                                    }
                                    res += "Following columns are missing in HR file:";
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
                                res = DataStringGp.BulkOperationDB_WirelessActivityReport(DataTbl, RepTime, Header);
                            }
                            else
                            {
                                return res;
                            }
                            if (res != null)
                            {
                                res = DataStringGp.InsertWirelessActivityStatus(UserId, EmpID, Month, Year);
                                //return DataTbl.Rows.Count.ToString();
                                return res;
                            }
                            else
                            {
                                return "Error occur during Insertion of Temp_USHR Data";
                            }
                        }  //================== SEPARATER====================//
                        else if (result == "Temp_ChargebackRaw")
                        {
                            DataTbl = UploadExcel.GetDataTable(Obj);
                            DataTbl.TableName = result;

                            foreach (DataColumn col in DataTbl.Columns)
                            {
                                ExcelColNameList.Add(col.ColumnName);
                            }

                            Header = DataStringGp.GetTableColumnNames("Temp_ChargebackRaw");

                            foreach (DataRow row in Header.Rows)
                            {
                                fixedColumns.Add(row["COLUMN_NAME"].ToString());
                            }

                            lstFieldsRequired = ExcelColNameList
                                .Where(a => fixedColumns.Any(x => x.ToString().ToUpper() == a.ToString().ToUpper())).ToList();
                            lstFieldsMissing = fixedColumns
                                .Where(a => ExcelColNameList.All(x => x.ToString().ToUpper() != a.ToString().ToUpper())).ToList();

                            if (lstFieldsMissing.Count > 0)
                            {
                                res += "Following columns are missing in ChargebackRaw file:";
                                int a = 1;
                                foreach (string field in lstFieldsMissing)
                                {
                                    res += "<br />" + a.ToString() + ") " + field;
                                    a++;
                                }
                                return res;
                            }

                            if (res != null)
                            {
                                res = DataStringGp.BulkOperationDB_ChargebackRaw(DataTbl, Header);
                            }
                            else
                            {
                                return res;
                            }

                            if (res != null)
                            {
                                res = DataStringGp.InsertChargebackRawStatus(Month); // Month param carries UploadDate
                                return res;
                            }
                            else
                            {
                                return "Error occurred during Insertion of Temp_ChargebackRaw Data";
                            }
                        }
                        //================== SEPARATER====================//
                        else
                        {
                            return "Please upload the file!";
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
        //Excel Upload file: End

        public void SaveFiles(HttpPostedFileBase file)
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

        // COMMON UPLOAD METHOD
        // =============================
        private string UploadToTable(HttpPostedFileBase file, string tableName)
        {
            if (file == null || file.ContentLength <= 0)
                throw new Exception("File was not selected.");

            DataTable dt;
            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension == ".csv")
            {
                dt = CsvToDataTable(file);
                //CleanDataTable(dt);
                //ValidateColumns(dt, tableName);
            }
            else
            {
                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets[1];

                     dt = ExcelHelper.ExcelToDataTable(worksheet);

                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (row[col] == null || string.IsNullOrWhiteSpace(row[col].ToString()))
                            {
                                row[col] = DBNull.Value;
                            }
                            else
                            {
                                string value = row[col].ToString().Trim();

                                // ✅ 1. Handle Scientific Notation (e.g., 5.56E+11)
                                if (value.Contains("E") || value.Contains("e"))
                                {
                                    if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal sciVal))
                                    {
                                        row[col] = sciVal;
                                        continue;
                                    }
                                }

                                // ✅ 2. Handle Currency, %, Parentheses
                                if (Regex.IsMatch(value, @"^[\(\)\d\$\.,%]+$"))
                                {
                                    // Remove parentheses → ignoring negative sign
                                    value = value.Replace("(", "").Replace(")", "");

                                    // Remove symbols
                                    value = value.Replace("$", "")
                                                 .Replace(",", "")
                                                 .Replace("%", "");

                                    if (decimal.TryParse(value, out decimal num))
                                    {
                                        row[col] = num;
                                    }
                                    else
                                    {
                                        row[col] = value;
                                    }
                                }
                                else
                                {
                                    // ✅ Keep non-numeric text unchanged
                                    row[col] = value;
                                }
                            }
                        }
                    }

                    return BulkInsert(dt, tableName);
                }
            }
            return BulkInsert(dt, tableName);
        }

        private DataTable CsvToDataTable(HttpPostedFileBase file)
        {
            DataTable dt = new DataTable();

            using (var reader = new StreamReader(file.InputStream))
            {
                bool isHeader = true;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var values = ParseCsvLine(line);
                    if (isHeader)
                    {
                        foreach (var header in values)
                        {
                            dt.Columns.Add(header.Trim());
                        }
                        isHeader = false;
                    }
                    else
                    {
                        while (values.Count < dt.Columns.Count)
                        {
                            values.Add(null);
                        }

                        if (values.Count > dt.Columns.Count)
                        {
                            values = values.Take(dt.Columns.Count).ToList();
                        }

                        dt.Rows.Add(values.ToArray());
                    }
                }
            }

            return dt;
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

        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            string current = "";

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            values.Add(current);
            return values;
        }

        //private void CleanDataTable(DataTable dt)
        //{
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            if (row[col] == null || string.IsNullOrWhiteSpace(row[col].ToString()))
        //            {
        //                row[col] = DBNull.Value;
        //                continue;
        //            }

        //            string value = row[col].ToString().Trim();

        //            // ✅ Detect negative (parentheses)
        //            bool isNegative = value.Contains("(") && value.Contains(")");

        //            value = value.Replace("(", "")
        //                         .Replace(")", "")
        //                         .Replace("$", "")
        //                         .Replace(",", "")
        //                         .Replace("%", "");

        //            // ✅ Scientific notation
        //            if (value.Contains("E") || value.Contains("e"))
        //            {
        //                if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal sciVal))
        //                {
        //                    row[col] = sciVal.ToString("0");
        //                    continue;
        //                }
        //            }

        //            // ✅ Numeric conversion
        //            if (decimal.TryParse(value, out decimal num))
        //            {
        //                row[col] = isNegative ? -num : num;
        //            }
        //            else
        //            {
        //                row[col] = value;
        //            }
        //        }
        //    }
        //}

        //private void ValidateColumns(DataTable dt, string tableName)
        //{
        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["APIConnStr"].ConnectionString))
        //    {
        //        con.Open();

        //        DataTable schema = con.GetSchema("Columns", new string[] { null, null, tableName });

        //        var dbColumns = schema.AsEnumerable()
        //                              .Select(r => r["COLUMN_NAME"].ToString().ToLower())
        //                              .ToList();

        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            if (!dbColumns.Contains(col.ColumnName.ToLower()))
        //            {
        //                throw new Exception($"Column '{col.ColumnName}' does not exist in table '{tableName}'");
        //            }
        //        }
        //    }
        //}

    }
}