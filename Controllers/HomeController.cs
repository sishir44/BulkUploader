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
    }
}