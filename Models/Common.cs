using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Web;
using BulkUploader.DAL;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace BulkUploader.Models
{
    public class Common
    {
        static DAL.DAL obj_dal = new DAL.DAL();

        public static DataTable CheckTabledata()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "GetGPAnomaly";
                DAL.SPParameters spParam = new DAL.SPParameters();
                DataTable dt = objDal.Getdata(spParam);
                return dt;
            }
            catch (Exception ex)
            {

                DataTable dt = new DataTable();
                return dt;
            }
        }
        public static void SendEmailAWS(string from, string to, string ccEmail, string bcemail, string Subject, string Body, string attachPath)
        {
            string smtpUsername = ConfigurationManager.AppSettings["AWSSMTUsername"].ToString();
            string smtpPassword = ConfigurationManager.AppSettings["AWSSMTPPassword"].ToString();
            try
            {

                String HOST = "email-smtp.us-east-1.amazonaws.com";
                int PORT = 587;

                if (to == "")
                {
                    to = "appdev@mobilelinkusa.com";

                }
                if (ccEmail == "")
                {
                    ccEmail = "appdev@mobilelinkusa.com";

                }


                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress(from, from);
                message.To.Add(to);
                message.CC.Add(ccEmail);
                //message.To.Add(to);
                message.Subject = Subject;
                message.Bcc.Add(bcemail);
                message.Body = Body;
                if (attachPath.Trim() != "")
                {
                    if (System.IO.File.Exists(attachPath))
                    {
                        Attachment objAttach = new Attachment(attachPath);
                        objAttach.ContentType = new ContentType("application/octet-stream");
                        ContentDisposition disposition = objAttach.ContentDisposition;
                        disposition.DispositionType = "attachment";
                        disposition.CreationDate = System.IO.File.GetCreationTime(attachPath);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachPath);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachPath);
                        message.Attachments.Add(objAttach);
                    }
                }
                // Comment or delete the next line if you are not using a configuration set
                //message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);

                using (var client = new SmtpClient(HOST, PORT))
                {
                    // Pass SMTP credentials
                    client.Credentials =
                        new NetworkCredential(smtpUsername, smtpPassword);

                    // Enable SSL encryption
                    client.EnableSsl = true;

                    client.Send(message);


                    Common.EmailSentLogs(from, to, ccEmail, from, "", Subject, Body, "210", "", "GPStatusEmail");
                }
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(0);

                string line = frame.GetFileLineNumber().ToString();
                // Common obj = new Common();
                Common.recorderror("OperationIssuesDashboard", ex.Message, "", line);
            }
        }
        public static DataTable GetEmailContent(string ReportName)
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "GetGPEmailContent";
                DAL.SPParameters spParam = new DAL.SPParameters();
                spParam.SetParam("ReportName", SqlDbType.NVarChar, ReportName);
                DataTable dt = objDal.Getdata(spParam);
                return dt;
            }
            catch (Exception ex)
            {

                DataTable dt = new DataTable();
                return dt;
            }
        }

        public static void EmailSentLogs(string from, string to, string CCemail, string Bccemail, string SubmittedBy, string Subject, string Body, string AppID, string RequestID, string Module)
        {

            try
            {
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "EmailSentLogs";
                DAL.SPParameters sp = new DAL.SPParameters();
                sp.SetParam("from", SqlDbType.NVarChar, from);
                sp.SetParam("to", SqlDbType.NVarChar, to);
                sp.SetParam("CCemail", SqlDbType.NVarChar, CCemail);
                sp.SetParam("Bccemail", SqlDbType.NVarChar, Bccemail);
                sp.SetParam("SubmittedBy", SqlDbType.NVarChar, SubmittedBy);
                sp.SetParam("Subject", SqlDbType.NVarChar, Subject);
                sp.SetParam("Body", SqlDbType.NVarChar, Body);
                sp.SetParam("AppID", SqlDbType.NVarChar, AppID);
                sp.SetParam("RequestID", SqlDbType.NVarChar, RequestID);
                sp.SetParam("Module", SqlDbType.NVarChar, Module);
                objDal.AddData(sp);
            }
            catch (Exception ex)
            {
                Common.recorderror("travel", ex.Message.ToString(), "khalid_jamil", "error in procedure insert logs");
            }

        }
        public static void recorderror(string modulename, string exception, string username, string linenumber)
        {

            try
            {

                obj_dal.ProcName = "InsertErrorLogs";
                SPParameters sp = new SPParameters();
                sp.SetParam("module", SqlDbType.NVarChar, modulename);
                sp.SetParam("expmsg", SqlDbType.NVarChar, exception);
                sp.SetParam("userid", SqlDbType.NVarChar, username);
                sp.SetParam("lineno", SqlDbType.NVarChar, linenumber);
                obj_dal.AddData(sp);
            }
            catch (Exception ex)
            {
                Common.recorderror("MTD_Uploader", ex.Message.ToString(), "khalid_jamil", "error in procedure insert logs");
            }

        }
        public static void SaveFiles(HttpPostedFileBase file, string ReportName)
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
                        var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + date);
                        bool exists = Directory.Exists(root);
                        if (!exists)
                        {
                            Directory.CreateDirectory(root);
                        }

                        var ReportRoot = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + date + "/" + ReportName);
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
                Common.recorderror("TD_Uploader.Models.MTD_Uploader/Common/SaveFiles", ex.Message, "", line);
            }
        }

        public static void SaveFiles(HttpPostedFileBase file, HttpPostedFileBase file1, HttpPostedFileBase file2, string ReportName)
        {

            try
            {
                List<HttpPostedFileBase> FileEnum = new List<HttpPostedFileBase> { file, file1, file2 };

                string date = DateTime.Now.ToString("yyyyMMdd");
                string dateTime = DateTime.Now.ToString("yyyyMMdd_hhmmss");

                foreach (HttpPostedFileBase files in FileEnum)
                {
                    if (files != null)
                    {
                        var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + date);
                        bool exists = Directory.Exists(root);
                        if (!exists)
                        {
                            Directory.CreateDirectory(root);
                        }

                        var ReportRoot = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + date + "/" + ReportName);
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
                Common.recorderror("TD_Uploader.Models.MTD_Uploader/Common/SaveFiles", ex.Message, "", line);
            }
        }

        public static void SaveFiles(HttpPostedFileBase file, HttpPostedFileBase file2, string ReportName)
        {

            try
            {
                List<HttpPostedFileBase> FileEnum = new List<HttpPostedFileBase> { file, file2 };

                string date = DateTime.Now.ToString("yyyyMMdd");
                string dateTime = DateTime.Now.ToString("yyyyMMdd_hhmmss");

                foreach (HttpPostedFileBase files in FileEnum)
                {
                    if (files != null)
                    {
                        var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + date);
                        bool exists = Directory.Exists(root);
                        if (!exists)
                        {
                            Directory.CreateDirectory(root);
                        }

                        var ReportRoot = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + date + "/" + ReportName);
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
                Common.recorderror("TD_Uploader.Models.MTD_Uploader/Common/SaveFiles", ex.Message, "", line);
            }
        }
    }
}