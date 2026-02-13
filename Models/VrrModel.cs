using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using BulkUploader.DAL;

namespace BulkUploader.Models
{
    public class VrrModel
    {
        public static DataTable DeleteAll()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "DeleteGP";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable DeleteReportData()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "DeleteGPNew";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable DeleteVrrPdr(string table)
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "sp_DeleteVrrPdr";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                spParam.SetParam("table", SqlDbType.VarChar, table);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable DeleteAll_ZeroRebate()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "DeleteGP_ZeroRebate";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static string InsertTempReportDatda()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "InsertTempReportData";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public static string InsertTempReportDatdaNew()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "InsertTempReportDataNew";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public static string DeleteTempReportDatda()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "DeleteTempReportData";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public static string DeleteTempReportDatdaNew()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "DeleteTempReportDataNew";

                DAL.SPParameters spParam = new DAL.SPParameters();

                //spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public static string InsertExceptionReportdata()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "InsertExceptionReportData";

                DAL.SPParameters spParam = new DAL.SPParameters();

                return objDal.AddData (spParam);

            }

            catch (Exception ex)
            {
                
                return ex.Message;
            }
        }
        public static string InsertExceptionReportdataNew()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "InsertExceptionReportDataNew";

                DAL.SPParameters spParam = new DAL.SPParameters();

                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public static string InsertDifferentialReportdata()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "InsertDifferentialReportData";

                DAL.SPParameters spParam = new DAL.SPParameters();

                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public static string InsertDifferentialReportdataNew()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "InsertDifferentialReportDataNew";

                DAL.SPParameters spParam = new DAL.SPParameters();

                return objDal.AddData(spParam);

            }

            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public static DataTable GetAllCheckVRR()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetReportDynamicData";

                SPParameters spParam = new SPParameters();

                spParam.SetParam("InputID", SqlDbType.Int, "1");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable GetAllCheckPDR()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetReportDynamicData";

                SPParameters spParam = new SPParameters();

                spParam.SetParam("InputID", SqlDbType.Int, "2");
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable GetAdditional(int Report_Id)
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetAdditionalColumnsNew";

                SPParameters spParam = new SPParameters();
                if (Report_Id == 1)
                {
                    spParam.SetParam("reportName", SqlDbType.NVarChar, "CNAAdditionalColumns");
                }
                else if(Report_Id==5)
                {
                    spParam.SetParam("reportName", SqlDbType.NVarChar, "ACCAdditionalColumns");
                }
                else if(Report_Id==6)
                {
                    spParam.SetParam("reportName", SqlDbType.NVarChar, "GPReport");
                }
                else
                {
                    spParam.SetParam("reportName", SqlDbType.NVarChar, "UPGAdditionalColumns");
                }
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);


                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable Filter(int Report_Id)
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetGPFilterRptNew";

                SPParameters spParam = new SPParameters();
                if (Report_Id == 1)
                {
                    spParam.SetParam("reportname", SqlDbType.NVarChar, "CNAFilterReport");
                }
                else if(Report_Id==3)
                {
                    spParam.SetParam("reportname", SqlDbType.NVarChar, "GEOAreaSpiff");
                }
                else if (Report_Id == 4)
                {
                    spParam.SetParam("reportname", SqlDbType.NVarChar, "PDReport");
                }
                else
                {
                    spParam.SetParam("reportname", SqlDbType.NVarChar, "UPGFilterReport");
                }
                //spParam.SetParam("province_code", SqlDbType.VarChar, provincecode);
                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable GetAllDataFromStore()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetStoreInfoNew";

                DAL.SPParameters spParam = new DAL.SPParameters();

                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }
        public static DataTable GetAllDataFromStore_ZeroRebate()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetStoreInfo_ZeroRebate";

                DAL.SPParameters spParam = new DAL.SPParameters();

                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }
        public static string InsertNewCustomers(DataTable CustomerTable)
        {
            try
            {

                DAL.DAL obj = new DAL.DAL();
                SPParameters sp = new SPParameters();
                obj.ProcName = "InsertMTDCustomerNew";
                sp.SetParam("mtdcustomer", SqlDbType.Structured, CustomerTable);
                string response = obj.AddData(sp);
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static string InsertLoginLog(string port, string ipAddress, string userAgenet)
        {
            try
            {
                DAL.DAL obj = new DAL.DAL();
                SPParameters sp = new SPParameters();
                obj.ProcName = "InsertMTDCustomerNew";
                sp.SetParam("IpAddress", SqlDbType.NVarChar, ipAddress);
                sp.SetParam("Port", SqlDbType.NVarChar, port);
                sp.SetParam("UserAgent", SqlDbType.NVarChar, userAgenet);
                string response = obj.AddData(sp);
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public static DataTable GetCustomerCount()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetMTDCustomersCountNew";

                SPParameters spParam = new SPParameters();
                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }

        public static DataTable GetAllEmpNew()
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();

                objDal.ProcName = "GetAllEmpNew";

                SPParameters spParam = new SPParameters();

                return objDal.Getdata(spParam);

            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }
        }
    }
}