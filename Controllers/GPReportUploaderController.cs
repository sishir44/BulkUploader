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
                            return "Please upload the file named 'TempGPt.xlsx'";
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

    }
}