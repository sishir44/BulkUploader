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

namespace BulkUploader.Controllers
{
    public class BulkUploaderController : Controller
    {
        public ActionResult Upload()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult UploadExcel(
            HttpPostedFileBase Temp_Location,
            HttpPostedFileBase salesFile,
            HttpPostedFileBase commissionFile,
            HttpPostedFileBase storeFile,
            HttpPostedFileBase file5,
            HttpPostedFileBase file6,
            HttpPostedFileBase file7,
            HttpPostedFileBase file8,
            HttpPostedFileBase file9,
            HttpPostedFileBase file10)
        {
            try
            {
                UploadToTable(Temp_Location, "Temp_Location");
                UploadToTable(salesFile, "SalesTable");
                UploadToTable(commissionFile, "CommissionTable");
                UploadToTable(storeFile, "StoreTable");
                UploadToTable(file5, "Table5");
                UploadToTable(file6, "Table6");
                UploadToTable(file7, "Table7");
                UploadToTable(file8, "Table8");
                UploadToTable(file9, "Table9");
                UploadToTable(file10, "Table10");

                ViewBag.Message = "All files uploaded successfully!";
            }
            catch (System.Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            return View("Upload");
        }


        // =============================
        // COMMON UPLOAD METHOD
        // =============================
        private void UploadToTable(HttpPostedFileBase file, string tableName)
        {
            if (file.FileName == null || file.ContentLength == 0)
                return;

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
                BulkInsert(dt, tableName);
            }
        }


        // =============================
        // BULK INSERT METHOD
        // =============================
        private void BulkInsert(DataTable dt, string tableName)
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
        }
    }
}