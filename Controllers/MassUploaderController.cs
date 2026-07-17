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
using System.Diagnostics;
using System.IO;
using DevExpress.Security;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BulkUploader.Controllers
{
    public class MassUploaderController : BaseController
    {
        // =====================UploadExcel Uploader Start======== //
        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
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
            HttpPostedFileBase RepDataDayWise,
            string date,
            string IsFinal = "0"
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
                    { "RepDataDayWise", (RepDataDayWise, "Temp_Daily_MTD_RepdataDayWise") },
                    
                 
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
                    status = DataStringGp.UploaderUpdateSTP(date, IsFinal);
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
        // =====================UploadExcel Uploader End======== //

        // =====================Commission Uploader Start======== //
        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
        public ActionResult CommissionUploader()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult CommissionUploader(
            HttpPostedFileBase CommissionDetails,
            HttpPostedFileBase CommissionAccessories,
            HttpPostedFileBase SMFBBDetail,
            HttpPostedFileBase SMFBBSummary,
            HttpPostedFileBase SMFDetail,
            HttpPostedFileBase SMFSummary,
            HttpPostedFileBase ARCompensationOffset,
            HttpPostedFileBase DemoDevices,
            HttpPostedFileBase IntangibleSKUs,
            HttpPostedFileBase Jline,
            HttpPostedFileBase ManualDiscount,
            HttpPostedFileBase Restocking,
            HttpPostedFileBase Returns,
            //HttpPostedFileBase InventoryShrink,
            HttpPostedFileBase RLONotReceived,
            HttpPostedFileBase SerializedSold,
            HttpPostedFileBase Shrink1,
            HttpPostedFileBase Shrink2,
            HttpPostedFileBase TradeIns,
            HttpPostedFileBase Treasury,
            HttpPostedFileBase WirelessBillCreds,
            HttpPostedFileBase Wiredbillcredits,
            //HttpPostedFileBase ProfitLossStatement,
            string date
            )
        {
            try
            {             
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                    {
                        { "CommissionDetails", (CommissionDetails, "Temp_my_mtdommissionDetail") },
                        { "CommissionAccessories", (CommissionAccessories,"Temp_my_mtdcommissionAccessories") },
                        { "SMFBBDetail", (SMFBBDetail,"Temp_SMFBBDetail") },
                        { "SMFBBSummary", (SMFBBSummary,"Temp_SMFBBSummary") },
                        { "SMFDetail", (SMFDetail,"Temp_SMFDetail") },
                        { "SMFSummary", (SMFSummary,"Temp_SMFSummary") },
                        { "ARCompensationOffset", (ARCompensationOffset,"Temp_ARCompensation_Offset") },
                        { "DemoDevices", (DemoDevices,"temp_demodevices") },
                        { "IntangibleSKUs", (IntangibleSKUs,"Temp_intangible") },
                        { "Jline", (Jline,"temp_jline") },
                        { "ManualDiscount", (ManualDiscount,"temp_manualdiscounts") },
                        { "Restocking", (Restocking,"Temp_Restocking") },
                        { "Returns", (Returns,"Temp_Returns") },
                        { "RLONotReceived", (RLONotReceived,"Temp_RLONotRecieved") },
                        { "SerializedSold", (SerializedSold,"Temp_SerializedSold") },
                        { "Shrink1", (Shrink1,"Temp_Shrink1") },
                        { "Shrink2", (Shrink2,"Temp_Shrink2") },
                        { "TradeIns", (TradeIns,"temp_tradein") },
                        { "Treasury", (Treasury,"temp_treasury") },
                        { "WirelessBillCreds", (WirelessBillCreds,"Temp_WirelessBillCreds") },
                        { "Wiredbillcredits", (Wiredbillcredits,"Temp_Wiredbillcredits") },
                        //{ "ProfitLossStatement", (ProfitLossStatement,"Temp_ProfitLossStatement") },
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
                    status = DataStringGp.CommissionUpdateSTP(date);
                    //if (status == "1" || Convert.ToInt32(status) > 0)
                    //{
                    //    ViewBag.Success = "Uploaded Successfully!";
                    //}
                    //else
                    //{
                    //    //ViewBag.Warning = ViewBag.Warning + "\n" + "Not Uploaded Successfully ❌";
                    //    ViewBag.Error = status;
                    //}
                    if (status == "1" || (int.TryParse(status, out int result) && result > 0))
                    {
                        ViewBag.Success = "Uploaded Successfully!";
                    }
                    else
                    {
                        ViewBag.Error = status;
                    }
                }
                return View("CommissionUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("CommissionUploader");
            }
        }
        // =====================Commission Uploader End======== //

        // =====================Fraud Transaction Raw Uploader Start======== //
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult FraudTransactionUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FraudTransactionUploader(HttpPostedFileBase ManualCC,HttpPostedFileBase MAPARHistoricalAnalysis, string date)
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
            {

                { "ManualCC", (ManualCC,"Temp_ManualCC") },
                { "MAPARHistoricalAnalysis", (MAPARHistoricalAnalysis, "Temp_MAPARHistoricalAnalysis") },
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
                    status = DataStringGp.FraudTransactionUpdateSTP(date);
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
                return View("FraudTransactionUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("FraudTransactionUploader");
            }
        }
        // =====================Fraud Transaction Raw Uploader End======== //


        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
        public ActionResult MTDUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MTDUploader(
            HttpPostedFileBase KPINational,
            HttpPostedFileBase KPINational1,
            HttpPostedFileBase MISLocation,
            HttpPostedFileBase KPILocation1,
            HttpPostedFileBase MTDNational1,
            HttpPostedFileBase MTDNational2,
            HttpPostedFileBase MTDLocation3,
            HttpPostedFileBase MTDLocation4,
            HttpPostedFileBase MTDLocation5,
            HttpPostedFileBase EmployeeRepData,
            HttpPostedFileBase EmployeeDetailforTime,
            HttpPostedFileBase ATTUIDDetailsMIS,
            HttpPostedFileBase TATotalHoursSummary,
            HttpPostedFileBase RepDataDayWise,
            string date,
            string IsFinal = "0"
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
                    { "MTDNational1", (MTDNational1,"Temp_Daily_MTD_MTDNational1") },
                    { "MTDNational2", (MTDNational2,"Temp_Daily_MTD_MTDNational2") },
                    { "MTDLocation3", (MTDLocation3,"Temp_Daily_MTD_MTDLocation3") },
                    { "MTDLocation4", (MTDLocation4,"Temp_Daily_MTD_MTDLocation4") },
                    { "MTDLocation5", (MTDLocation5,"Temp_Daily_MTD_MTDLocation5") },
                    { "EmployeeRepData", (EmployeeRepData, "Temp_Daily_MTD_Repdata") },
                    { "EmployeeDetailforTime", (EmployeeDetailforTime, "Temp_Daily_MTD_EmployeeDetailforTime") },
                    { "ATTUIDDetailsMIS", (ATTUIDDetailsMIS, "Temp_Daily_others_ATTUID") },
                    { "TATotalHoursSummary", (TATotalHoursSummary, "Temp_Daily_MTD_TotalHours") },
                    { "RepDataDayWise", (RepDataDayWise, "Temp_Daily_MTD_RepdataDayWise") },
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
                    status = DataStringGp.MTDUploaderUpdateSTP(date, IsFinal);
                    if (status == "1")
                    {
                        ViewBag.Success = "Uploaded Successfully!";
                    }
                    else
                    {
                        //ViewBag.Warning = ViewBag.Warning + "\n" + "Not Uploaded Successfully ❌";
                        ViewBag.Error = status;
                    }
                }

                return View("MTDUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("MTDUploader");
                //ViewBag.Message = "Error: " + ex.Message;
            }
        }


        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
        public ActionResult DailyUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DailyUploader(
            HttpPostedFileBase LeadsCalled,
            HttpPostedFileBase Marketing,
            HttpPostedFileBase RequisitionsTable,
            //HttpPostedFileBase Wiredraw,
            //HttpPostedFileBase Wirelessraw,
            //HttpPostedFileBase WirelessActivity,
            HttpPostedFileBase ChangeOrderInformation,
            //HttpPostedFileBase DTSAllInformation,
            string date
            )
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {

            
                    { "LeadsCalled", (LeadsCalled, "Temp_Daily_others_Leadscalled") },
                    { "Marketing", (Marketing, "Temp_Daily_others_Marketing") },
                    { "RequisitionsTable", (RequisitionsTable, "Temp_Daily_others_Requisitions") },
                    //{ "Wiredraw", (Wiredraw, "Temp_Daily_others_Wiredraw") },
                    //{ "Wirelessraw", (Wirelessraw, "Temp_Daily_others_Wirelessraw") },
                    //{ "WirelessActivity", (WirelessActivity, "Temp_Wirelessactivity") },
                    { "ChangeOrderInformation", (ChangeOrderInformation, "Temp_ChangeOrderInformation") },
                    //{ "DTSAllInformation", (DTSAllInformation, "Temp_DTSAllInformation") },
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
                    status = DataStringGp.DailyUploaderUpdateSTP(date);
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

                return View("DailyUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("DailyUploader");
                //ViewBag.Message = "Error: " + ex.Message;
            }
        }


        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
        public ActionResult WireDrawUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult WireDrawUploader(
            HttpPostedFileBase Wiredraw,
            string date
            )
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {
                    { "Wiredraw", (Wiredraw, "Temp_Daily_others_Wiredraw") },
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
                    status = DataStringGp.WiredrawUpdateSTP(date);
                    //if (status == "1" || Convert.ToInt32(status) > 0)
                    //{
                    //    ViewBag.Success = "Uploaded Successfully!";
                    //}
                    //else
                    //{
                    //    //ViewBag.Warning = ViewBag.Warning + "\n" + "Not Uploaded Successfully ❌";
                    //    ViewBag.Error = status;
                    //}
                    if (status == "1" || (int.TryParse(status, out int result) && result > 0))
                    {
                        ViewBag.Success = "Uploaded Successfully!";
                    }
                    else
                    {
                        ViewBag.Error = status;
                    }
                }

                return View("WireDrawUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("WireDrawUploader");
                //ViewBag.Message = "Error: " + ex.Message;
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
        public ActionResult WirelessrawUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult WirelessrawUploader(
            HttpPostedFileBase Wirelessraw,
            string date
            )
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {
                    { "Wirelessraw", (Wirelessraw, "Temp_Daily_others_Wirelessraw") },
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
                    status = DataStringGp.WirelessRawUpdateSTP(date);
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

                return View("WirelessrawUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("WirelessrawUploader");
                //ViewBag.Message = "Error: " + ex.Message;
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        [OverrideAuthentication]
        public ActionResult WirelessActivityUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult WirelessActivityUploader(
            HttpPostedFileBase WirelessActivity,
            string date
            )
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {
                    { "WirelessActivity", (WirelessActivity, "Temp_Wirelessactivity") },
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
                    status = DataStringGp.WirelessActivityUpdateSTP(date);
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

                return View("WirelessActivityUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("WirelessActivityUploader");
                //ViewBag.Message = "Error: " + ex.Message;
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult VABTargetUploader()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VABTargetUploader(HttpPostedFileBase Targets, HttpPostedFileBase Tiers, string date)
        {
            try
            {
                var files = new Dictionary<string, (HttpPostedFileBase File, string Table)>
                {

                    { "Targets", (Targets,"Temp_targets") },
                    { "Tiers", (Tiers, "Temp_tiers") },
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
                    status = DataStringGp.VABTargetUpdateSTP(date);
                    if (status == "1" || (int.TryParse(status, out int result) && result > 0))
                    {
                        ViewBag.Success = "Uploaded Successfully!";
                    }
                    else
                    {
                        //ViewBag.Warning = ViewBag.Warning + "\n" + "Not Uploaded Successfully ❌";
                        ViewBag.Error = status;
                    }
                }
                return View("VABTargetUploader");
            }
            catch (System.Exception ex)
            {
                ViewBag.Warning = ex.ToString() + "\n\n" + ex.StackTrace;
                return View("VABTargetUploader");
            }
        }


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
                Common.recorderror("BukhUploader/MassUploaderController/SaveFiles", ex.Message, "", line);
            }
        }

        // COMMON UPLOAD METHOD
        // =============================
        private string UploadToTable(HttpPostedFileBase file, string tableName)
        {
            try
            {
                if (file == null || file.ContentLength <= 0)
                {
                    throw new Exception("File was not selected.");
                }

                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets[1];

                    DataTable dt = ExcelHelper.ExcelToDataTable(worksheet);

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

                                // Scientific notation
                                if (value.Contains("E") || value.Contains("e"))
                                {
                                    if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal sciVal))
                                    {
                                        row[col] = sciVal;
                                        continue;
                                    }
                                }

                                // Currency, %, commas
                                if (Regex.IsMatch(value, @"^[\(\)\d\$\.,%]+$"))
                                {
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
                                    row[col] = value;
                                }
                            }
                        }
                    }

                    return BulkInsert(dt, tableName);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
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