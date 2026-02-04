using BulkUploader.DAL;
using BulkUploader.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace BulkUploader.Models
{
    public class DataStringGp
    {
        public static string BulkOperationDB_Level(DataTable dt, string RepTime, DataTable Header)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    VrrModel.DeleteVrrPdr(dt.TableName);
                }

                Header.Rows.Add("RepTime");
                DataColumn newColumn = new DataColumn("RepTime", typeof(String));
                newColumn.DefaultValue = RepTime;
                dt.Columns.Add(newColumn);

                // Newly added begin
                var skipColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Month Date", "RepTime" };
                var columnsToConvert = dt.Columns.Cast<DataColumn>().Where(c => !skipColumns.Contains(c.ColumnName) && c.DataType == typeof(string)).ToList();
                foreach (DataColumn col in columnsToConvert)
                {
                    // Create new float column
                    string floatColName = col.ColumnName + "_float";
                    DataColumn floatCol = new DataColumn(floatColName, typeof(float));
                    dt.Columns.Add(floatCol);
                    // Convert string values to float (0 if invalid/null)
                    foreach (DataRow row in dt.Rows)
                    {
                        string valStr = row[col.ColumnName]?.ToString();

                        if (float.TryParse(valStr, out float f))
                        {
                            row[floatCol] = f;
                        }
                        else
                        {
                            // You can set DBNull.Value here if your DB column allows NULLs instead of 0
                            row[floatCol] = 0f;
                        }
                    }

                    // Remove old string column
                    dt.Columns.Remove(col.ColumnName);

                    // Rename new float column to original name
                    floatCol.ColumnName = col.ColumnName;
                }

                // Fix header table if necessary
                if (!Header.Rows.Cast<DataRow>().Any(r => r["COLUMN_NAME"].ToString() == "RepTime"))
                {
                    Header.Rows.Add("RepTime");
                }
                // Newly added end

                string strconnectionstring = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();

                SqlBulkCopy objbulk = new SqlBulkCopy(strconnectionstring, SqlBulkCopyOptions.FireTriggers);
                objbulk.BulkCopyTimeout = 6000;

                objbulk.DestinationTableName = dt.TableName;

                //foreach (DataRow row in Header.Rows) // commented
                //{
                //    objbulk.ColumnMappings.Add(row["COLUMN_NAME"].ToString(), row["COLUMN_NAME"].ToString());
                //}

                // newly added begin
                foreach (DataRow row in Header.Rows)
                {
                    string colName = row["COLUMN_NAME"].ToString();
                    objbulk.ColumnMappings.Add(colName, colName);
                }
                // newly added end

                objbulk.WriteToServer(dt);
                return "Data has been Uploaded Successfully";
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string BulkOperationDB_PerformanceEvlReport(DataTable dt, string repTime, DataTable header)
        {
            try
            {
                if (dt.Rows.Count == 0) return "No data to upload";

                VrrModel.DeleteVrrPdr(dt.TableName);

                // Add RepTime column if not exists
                if (!dt.Columns.Contains("RepTime"))
                {
                    DataColumn repTimeCol = new DataColumn("RepTime", typeof(string));
                    repTimeCol.DefaultValue = repTime;
                    dt.Columns.Add(repTimeCol);
                }

                // Ensure header includes RepTime
                if (!header.Rows.Cast<DataRow>().Any(r => r["COLUMN_NAME"].ToString() == "RepTime"))
                {
                    header.Rows.Add("RepTime");
                }

                var skipCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RepTime" };

                foreach (var col in dt.Columns.Cast<DataColumn>().Where(c => c.DataType == typeof(string) && !skipCols.Contains(c.ColumnName)).ToList())
                {
                    // Check if column is mostly numeric (allow $, %, ,)
                    bool isNumeric = dt.AsEnumerable().All(row =>
                    {
                        string val = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                        return string.IsNullOrWhiteSpace(val) || float.TryParse(val, out _);
                    });

                    if (isNumeric)
                    {
                        string floatColName = col.ColumnName + "_float";
                        DataColumn floatCol = new DataColumn(floatColName, typeof(float));
                        dt.Columns.Add(floatCol);

                        foreach (DataRow row in dt.Rows)
                        {
                            string valStr = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                            row[floatCol] = float.TryParse(valStr, out float f) ? f : 0f;
                        }

                        dt.Columns.Remove(col.ColumnName);
                        floatCol.ColumnName = col.ColumnName;
                    }
                }

                string connStr = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.FireTriggers))
                {
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.BulkCopyTimeout = 6000;

                    foreach (DataRow row in header.Rows)
                    {
                        string colName = row["COLUMN_NAME"].ToString();
                        if (dt.Columns.Contains(colName))
                            bulkCopy.ColumnMappings.Add(colName, colName);
                    }
                    bulkCopy.WriteToServer(dt);
                }
                return "Data has been Uploaded Successfully";
            }
            catch
            {
                return null;
            }
        }
        public static string BulkOperationDB_InventoryReport(DataTable dt, string repTime, DataTable header)
        {
            try
            {
                if (dt.Rows.Count == 0) return "No data to upload";

                VrrModel.DeleteVrrPdr(dt.TableName);

                // Add RepTime column if not exists
                if (!dt.Columns.Contains("RepTime"))
                {
                    DataColumn repTimeCol = new DataColumn("RepTime", typeof(string));
                    repTimeCol.DefaultValue = repTime;
                    dt.Columns.Add(repTimeCol);
                }

                // Ensure header includes RepTime
                if (!header.Rows.Cast<DataRow>().Any(r => r["COLUMN_NAME"].ToString() == "RepTime"))
                {
                    header.Rows.Add("RepTime");
                }

                var skipCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RepTime" };

                foreach (var col in dt.Columns.Cast<DataColumn>().Where(c => c.DataType == typeof(string) && !skipCols.Contains(c.ColumnName)).ToList())
                {
                    // Check if column is mostly numeric (allow $, %, ,)
                    bool isNumeric = dt.AsEnumerable().All(row =>
                    {
                        string val = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                        return string.IsNullOrWhiteSpace(val) || float.TryParse(val, out _);
                    });

                    if (isNumeric)
                    {
                        string floatColName = col.ColumnName + "_float";
                        DataColumn floatCol = new DataColumn(floatColName, typeof(float));
                        dt.Columns.Add(floatCol);

                        foreach (DataRow row in dt.Rows)
                        {
                            string valStr = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                            row[floatCol] = float.TryParse(valStr, out float f) ? f : 0f;
                        }

                        dt.Columns.Remove(col.ColumnName);
                        floatCol.ColumnName = col.ColumnName;
                    }
                }

                string connStr = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.FireTriggers))
                {
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.BulkCopyTimeout = 6000;

                    foreach (DataRow row in header.Rows)
                    {
                        string colName = row["COLUMN_NAME"].ToString();
                        if (dt.Columns.Contains(colName))
                            bulkCopy.ColumnMappings.Add(colName, colName);
                    }
                    bulkCopy.WriteToServer(dt);
                }
                return "Data has been Uploaded Successfully";
            }
            catch
            {
                return null;
            }
        }

        public static string BulkOperationDB_PerformanceEvlSepReport(DataTable dt, string repTime, DataTable header)
        {
            try
            {
                if (dt.Rows.Count == 0) return "No data to upload";

                VrrModel.DeleteVrrPdr(dt.TableName);

                // Add RepTime column if not exists
                if (!dt.Columns.Contains("RepTime"))
                {
                    DataColumn repTimeCol = new DataColumn("RepTime", typeof(string));
                    repTimeCol.DefaultValue = repTime;
                    dt.Columns.Add(repTimeCol);
                }

                // Ensure header includes RepTime
                if (!header.Rows.Cast<DataRow>().Any(r => r["COLUMN_NAME"].ToString() == "RepTime"))
                {
                    header.Rows.Add("RepTime");
                }

                var skipCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RepTime" };

                foreach (var col in dt.Columns.Cast<DataColumn>().Where(c => c.DataType == typeof(string) && !skipCols.Contains(c.ColumnName)).ToList())
                {
                    // Check if column is mostly numeric (allow $, %, ,)
                    bool isNumeric = dt.AsEnumerable().All(row =>
                    {
                        string val = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                        return string.IsNullOrWhiteSpace(val) || float.TryParse(val, out _);
                    });

                    if (isNumeric)
                    {
                        string floatColName = col.ColumnName + "_float";
                        DataColumn floatCol = new DataColumn(floatColName, typeof(float));
                        dt.Columns.Add(floatCol);

                        foreach (DataRow row in dt.Rows)
                        {
                            string valStr = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                            row[floatCol] = float.TryParse(valStr, out float f) ? f : 0f;
                        }

                        dt.Columns.Remove(col.ColumnName);
                        floatCol.ColumnName = col.ColumnName;
                    }
                }

                string connStr = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.FireTriggers))
                {
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.BulkCopyTimeout = 6000;

                    foreach (DataRow row in header.Rows)
                    {
                        string colName = row["COLUMN_NAME"].ToString();
                        if (dt.Columns.Contains(colName))
                            bulkCopy.ColumnMappings.Add(colName, colName);
                    }
                    bulkCopy.WriteToServer(dt);
                }
                return "Data has been Uploaded Successfully";
            }
            catch
            {
                return null;
            }
        }
        public static string BulkOperationDB_GPReport(DataTable dt, string repTime, DataTable header)
        {
            try
            {
                if (dt.Rows.Count == 0) return "No data to upload";

                VrrModel.DeleteVrrPdr(dt.TableName);

                // Add RepTime column if not exists
                if (!dt.Columns.Contains("RepTime"))
                {
                    DataColumn repTimeCol = new DataColumn("RepTime", typeof(string));
                    repTimeCol.DefaultValue = repTime;
                    dt.Columns.Add(repTimeCol);
                }

                // Ensure header includes RepTime
                if (!header.Rows.Cast<DataRow>().Any(r => r["COLUMN_NAME"].ToString() == "RepTime"))
                {
                    header.Rows.Add("RepTime");
                }

                var skipCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "RepTime" };

                foreach (var col in dt.Columns.Cast<DataColumn>().Where(c => c.DataType == typeof(string) && !skipCols.Contains(c.ColumnName)).ToList())
                {
                    // Check if column is mostly numeric (allow $, %, ,)
                    bool isNumeric = dt.AsEnumerable().All(row =>
                    {
                        string val = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                        return string.IsNullOrWhiteSpace(val) || float.TryParse(val, out _);
                    });

                    if (isNumeric)
                    {
                        string floatColName = col.ColumnName + "_float";
                        DataColumn floatCol = new DataColumn(floatColName, typeof(float));
                        dt.Columns.Add(floatCol);

                        foreach (DataRow row in dt.Rows)
                        {
                            string valStr = row[col.ColumnName]?.ToString().Replace("%", "").Replace("$", "").Replace(",", "").Trim();
                            row[floatCol] = float.TryParse(valStr, out float f) ? f : 0f;
                        }

                        dt.Columns.Remove(col.ColumnName);
                        floatCol.ColumnName = col.ColumnName;
                    }
                }

                string connStr = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connStr, SqlBulkCopyOptions.FireTriggers))
                {
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.BulkCopyTimeout = 6000;

                    foreach (DataRow row in header.Rows)
                    {
                        string colName = row["COLUMN_NAME"].ToString();
                        if (dt.Columns.Contains(colName))
                            bulkCopy.ColumnMappings.Add(colName, colName);
                    }
                    bulkCopy.WriteToServer(dt);
                }
                return "Data has been Uploaded Successfully";
            }
            catch
            {
                return null;
            }
        }

        public static string InsertPnlStatus()
        {
            try
            {
                string Pnl = "";

                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "updateFCT_MY_PnlStatement";
                DAL.SPParameters spParam = new DAL.SPParameters();
                Pnl = objDal.AddData(spParam);

                return Pnl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string InsertPerformanceEvlStatus(string UserId, string EmpID, string Month, string Year)
        {
            try
            {
                string Evl = "";
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "updateFct_my_MTD_PerformanceEvl";
                DAL.SPParameters spParam = new DAL.SPParameters();
                spParam.SetParam("@Year", SqlDbType.VarChar, Year);
                spParam.SetParam("@UserID", SqlDbType.VarChar, UserId);
                spParam.SetParam("@month", SqlDbType.VarChar, Month);
                spParam.SetParam("@EmpId", SqlDbType.VarChar, EmpID);
                Evl = objDal.AddData(spParam);

                return Evl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string InsertInventoryStatus(string UserId, string EmpID, string Month, string Year)
        {
            try
            {
                string Evl = "";
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "updatefct_my_Inventorydata";
                DAL.SPParameters spParam = new DAL.SPParameters();
                //spParam.SetParam("@Year", SqlDbType.VarChar, Year);
                //spParam.SetParam("@UserID", SqlDbType.VarChar, UserId);
                //spParam.SetParam("@month", SqlDbType.VarChar, Month);
                //spParam.SetParam("@EmpId", SqlDbType.VarChar, EmpID);
                Evl = objDal.AddData(spParam);

                return Evl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string InsertPerformanceEvlSepStatus(string UserId, string EmpID, string Month, string Year)
        {
            try
            {
                string Evl = "";
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "updatefct_my_mtdPerformmissingcolumns";
                DAL.SPParameters spParam = new DAL.SPParameters();
                spParam.SetParam("@UserID", SqlDbType.VarChar, UserId);
                spParam.SetParam("@Year", SqlDbType.VarChar, Year);
                spParam.SetParam("@EmpId", SqlDbType.VarChar, EmpID);
                spParam.SetParam("@month", SqlDbType.VarChar, Month);
                Evl = objDal.AddData(spParam);

                return Evl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static DataTable GetEmpIDList()
        {
            DataTable dt = new DataTable();

            try
            {
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "GetEmpIDPR";

                SPParameters spParam = new SPParameters(); // no params, but kept for consistency
                dt = objDal.Getdata(spParam);
            }
            catch (Exception ex)
            {
                // optionally log exception
            }

            return dt;
        }



        public static string InsertGPStatus()
        {
            try
            {
                string GP = "";
                DateTime InputDate = DateTime.Now.Date;
                string formattedDate = InputDate.ToString("yyyy-MM-dd");
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "UpdateFct_my_GPStatus";
                DAL.SPParameters spParam = new DAL.SPParameters();
                spParam.SetParam("@datekey", SqlDbType.Date, formattedDate);
                GP = objDal.AddData(spParam);

                return GP;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable GetTableColumnNames(string table)
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "GetTableColumnNames";
                DAL.SPParameters spParam = new DAL.SPParameters();
                spParam.SetParam("tableName", SqlDbType.NVarChar, table);
                return objDal.Getdata(spParam);
            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }
        public static DataTable GetTableColumnName(string table)
        {
            try
            {
                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "GetTableColumnNames";
                DAL.SPParameters spParam = new DAL.SPParameters();
                spParam.SetParam("tableName", SqlDbType.NVarChar, table);
                return objDal.GetdataNew(spParam);
            }

            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;
            }

        }

        

        public static string createRepTime()
        {
            string dtime = DateTime.Now.ToString("MM-dd-yyyy");

            return dtime;
        }

        public static string BulkOperationReportData1(DataTable GP, DataTable AddCloumns)
        {
            try
            {
                string RepTime = createRepTime();
                // GP.Columns.Remove("RepTime");

                DataColumn newColumn = new DataColumn("RepTime", typeof(String));
                newColumn.DefaultValue = RepTime;
                GP.Columns.Add(newColumn);

                if (GP.Rows.Count > 0)
                {
                    string insRes = VrrModel.InsertTempReportDatdaNew();
                    VrrModel.DeleteReportData();
                }

                string strconnectionstring = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();

                SqlBulkCopy objbulk = new SqlBulkCopy(strconnectionstring, SqlBulkCopyOptions.FireTriggers);
                objbulk.BulkCopyTimeout = 6000;

                objbulk.DestinationTableName = "GPStatus";

                foreach (DataRow coldr in AddCloumns.Rows)
                {
                    if (coldr["Attribute_Id"].ToString() != "790")
                    {
                        objbulk.ColumnMappings.Add(coldr["Attribute_Name"].ToString(), coldr["Attribute_Name"].ToString());
                    }
                }
                objbulk.ColumnMappings.Add("RepTime", "RepTime");

                objbulk.WriteToServer(GP);

                string ExceptionsData = VrrModel.InsertExceptionReportdataNew();
                string DifferentialData = VrrModel.InsertDifferentialReportdataNew();

                return "Data has been Uploaded Successfully";
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public string DataString2(DataTable arrs1, int a,string date, string CheckBackDateReport, DataTable Goals_Dt, string Result)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    string data = "";
        //    int loopend = 1;
        //    int i = 0;
        //    foreach (DataRow dr in arrs1.Rows)
        //    {
        //        sb.Append("~");
        //        sb.Append(((loopend) + i).ToString());
        //        sb.Append("^");
        //            for (int j = 0; j < 58; j++)
        //            {
        //            if (j != 30 && j != 31)
        //            {
        //                sb.Append(dr[j]);
        //                sb.Append("`");
        //            }
        //            }
        //        //data = data.TrimStart('~', ' ');
        //        data = sb.ToString().TrimStart('~', ' ');
        //        data = data.TrimEnd('`', ' ');
        //        i++;
        //    }
        //    data = data.Replace("`~", "~");
        //    //data = data.Replace("-", "");
        //    data = data.Replace("/", "");
        //    data = data.Replace(",", "");
        //    data = data.Replace("#", "");
        //    //data = data.Replace(":", "");
        //    data = data.Replace("(", "");
        //    data = data.Replace(")", "");
        //    if (data.IndexOf('1') == 0 && data.IndexOf('^') == 1)
        //    { 

        //            data = InsertDataForGPReport("GPReport", data, date, CheckBackDateReport, Goals_Dt, Result);

        //    }
        //    else
        //    {
        //        data ="GP STATUS Generation Failed. Please Upload Files Again";

        //    }
        //    return data;
        //}

        //public string DataString3(DataTable arrs1, int a, string date, string CheckBackDateReport, DataTable Goals_Dt, string Result, DataTable AddCloumns)
        //{
        //    string data = "";
        //    try
        //    {

        //        foreach (DataRow coldr in AddCloumns.Rows)
        //        {

        //            if (coldr["Attribute_Id"].ToString() == "869" || coldr["Attribute_Id"].ToString() == "870")
        //            {
        //                coldr.Delete();
        //            }

        //        }
        //        AddCloumns.AcceptChanges();

        //        DataTable ReportData = new DataTable();
        //        ReportData = AddReportDataHeaders(ReportData);
        //        string Reptime = createRepTime();
        //        int RowIndex = 0;

        //        foreach (DataRow GPdr in arrs1.Rows)
        //        {
        //            RowIndex++;

        //            foreach (DataRow addColdr in AddCloumns.Rows)
        //            {

        //                    ReportData.ImportRow(addColdr);
        //                    ReportData.Rows[ReportData.Rows.Count - 1]["RowIndex"] = RowIndex;
        //                    ReportData.Rows[ReportData.Rows.Count - 1]["RepTime"] = Reptime;
        //                    ReportData.Rows[ReportData.Rows.Count - 1]["RepData"] = GPdr[addColdr["Attribute_Name"].ToString()].ToString();

        //            }
        //        }

        //        data = BulkOperationReportData(ReportData);

        //    }
        //    catch(Exception ex)
        //    {
        //        data = null;
        //    }
        //    return data;
        //}

        //public static string InsertDataForGPReport(string reportName, string strData, string date, string CheckBackDateReport, DataTable Goals_Dt, string Result)
        //{
        //    //strData = "1^727: MFK LLC - 10th Street`70144727`North Area`Holly Mcconnell`OH-PA Region`Nassim Dissi`Vannie Deonarine`Ohio - North`10th Street`0`0`0`0`0`2`2`4`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`No SSD`0`0`0`0`0`0`0`0~2^950: MFK LLC - Niles`70144950`North Area`Sven Verbeet`Chicago Region`No SD`No TM`Indiana - North`11th St Niles`0`0`0`0`0`2`1`3`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`Todd Chinn`0`0`0`0`0`0`0`0~3^731: MFK LLC - 12th Street`70144731`North Area`Holly Mcconnell`OH-PA Region`Nassim Dissi`Vannie Deonarine`Ohio - North`12th Street`0`0`0`0`0`2`1`3`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`No SSD`0`0`0`0`0`0`0`0~4^535: MFK LLC - 31st St`70144535`South Area`Sven Verbeet`South Central Region`David Brod`Shah Khan`Tulsa`31st St`0`0`0`0`0`2`1`3`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`No SSD`0`0`0`0`0`0`0`0~5^773: MFK LLC - Cleveland`70144773`South Area`Holly Mcconnell`Central Texas Region`Nassim Dissi`Vannie Deonarine`PQR`65th Street`0`0`0`0`0`2`2`4`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`0`No SSD`0`0`0`0`0`0`0`0";

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(date) && CheckBackDateReport == "1" && Goals_Dt != null && Result != "PDRZ")
        //        {
        //            DateTime date_temp_from = DateTime.Parse(date); //from.value" is input by user (dd/MM/yyyy)
        //            date = date_temp_from.ToString("yyyyMMdd");
        //            DAL.DAL objDal = new DAL.DAL();

        //            objDal.ProcName = "InsertReportDataBackDate";

        //            DAL.SPParameters spParam = new DAL.SPParameters();

        //            spParam.SetParam("strdata", SqlDbType.VarChar, strData);
        //            spParam.SetParam("reportname", SqlDbType.VarChar, reportName);
        //            spParam.SetParam("reptime", SqlDbType.VarChar, date);

        //            return objDal.AddData(spParam);
        //        }
        //        else if (!String.IsNullOrEmpty(date) && CheckBackDateReport == "2" && Goals_Dt == null && Result != "PDRZ")
        //        {
        //            DateTime date_temp_from = DateTime.Parse(date); //from.value" is input by user (dd/MM/yyyy)
        //            date = date_temp_from.ToString("yyyyMMdd");
        //            DAL.DAL objDal = new DAL.DAL();

        //            objDal.ProcName = "InsertSTRDataBackDate";

        //            DAL.SPParameters spParam = new DAL.SPParameters();

        //            spParam.SetParam("strdata", SqlDbType.VarChar, strData);
        //            spParam.SetParam("reportname", SqlDbType.VarChar, reportName);
        //            spParam.SetParam("reptime", SqlDbType.VarChar, date);

        //            return objDal.AddData(spParam);
        //        }
        //        else if(String.IsNullOrEmpty(date) && string.IsNullOrEmpty(CheckBackDateReport) && Goals_Dt == null && Result != "PDRZ")
        //        {
        //            string delRes = VrrModel.DeleteTempReportDatda();
        //            string insRes = VrrModel.InsertTempReportDatda();

        //            VrrModel.DeleteAll();


        //            DAL.DAL objDal = new DAL.DAL();

        //            objDal.ProcName = "InsertReportDataNew";

        //            DAL.SPParameters spParam = new DAL.SPParameters();
        //            spParam.SetParam("strdata", SqlDbType.VarChar, strData);
        //            spParam.SetParam("reportname", SqlDbType.VarChar, reportName);
        //            spParam.SetParam("reptime", SqlDbType.VarChar, createRepTime());
        //            string gp = objDal.AddData(spParam);


        //            string ExceptionsData = VrrModel.InsertExceptionReportdata();
        //            string DifferentialData = VrrModel.InsertDifferentialReportdata();

        //            return gp;
        //        }
        //        else if (String.IsNullOrEmpty(date) && string.IsNullOrEmpty(CheckBackDateReport) && Goals_Dt == null && Result == "PDRZ")
        //        {

        //            VrrModel.DeleteAll_ZeroRebate();


        //            DAL.DAL objDal = new DAL.DAL();

        //            objDal.ProcName = "InsertReportData_ZeroRebate";

        //            DAL.SPParameters spParam = new DAL.SPParameters();
        //            spParam.SetParam("strdata", SqlDbType.VarChar, strData);
        //            spParam.SetParam("reportname", SqlDbType.VarChar, reportName);
        //            spParam.SetParam("reptime", SqlDbType.VarChar, createRepTime());
        //            string gp = objDal.AddData(spParam);


        //            return gp;
        //        }
        //        else {

        //            return "Missing Backdate Fields";
        //        }


        //    }

        //    catch (Exception ex)
        //    {

        //        return ex.ToString();
        //    }

        //}

        //public static string InsertDataForGPReportNew(string Result, DataTable ReportData)
        //{
        //    string gp = "";
        //    try
        //    {

        //        if (Result != "PDRZ")
        //        {
        //            string delRes = VrrModel.DeleteTempReportDatdaNew();
        //            string insRes = VrrModel.InsertTempReportDatdaNew();

        //            //VrrModel.DeleteAll();

        //            DAL.DAL objDal = new DAL.DAL();
        //            objDal.ProcName = "InsertReportDataNew";
        //            DAL.SPParameters spParam = new DAL.SPParameters();
        //            spParam.SetParam("@reportData", SqlDbType.Structured, ReportData);
        //            gp = objDal.AddData(spParam);


        //            string ExceptionsData = VrrModel.InsertExceptionReportdataNew();
        //            string DifferentialData = VrrModel.InsertDifferentialReportdataNew();

        //            return gp;
        //        }
        //    }

        //    catch (Exception ex)
        //    {

        //        return ex.ToString();
        //    }
        //    return gp;

        //}

        //public static string BulkOperationReportData(DataTable ReportData)
        //{
        //    try
        //    {


        //        if (ReportData.Rows.Count > 0)
        //        {
        //            string delRes = VrrModel.DeleteTempReportDatdaNew();
        //            string insRes = VrrModel.InsertTempReportDatdaNew();

        //            VrrModel.DeleteReportData();
        //        }

        //        string strconnectionstring = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();

        //        SqlBulkCopy objbulk = new SqlBulkCopy(strconnectionstring, SqlBulkCopyOptions.FireTriggers);
        //        objbulk.BulkCopyTimeout = 600;

        //        objbulk.DestinationTableName = "ReportDataNew";

        //        objbulk.ColumnMappings.Add("Report_Id", "Report_Id");
        //        objbulk.ColumnMappings.Add("Attribute_Id", "Attribute_Id");
        //        objbulk.ColumnMappings.Add("RowIndex", "RowIndex");
        //        objbulk.ColumnMappings.Add("BreakCount", "BreakCount");
        //        objbulk.ColumnMappings.Add("RepData", "RepData");
        //        objbulk.ColumnMappings.Add("RepTime", "RepTime");

        //        objbulk.WriteToServer(ReportData);

        //        string ExceptionsData = VrrModel.InsertExceptionReportdataNew();
        //        string DifferentialData = VrrModel.InsertDifferentialReportdataNew();

        //        return "Data has been Uploaded Successfully";

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        //public static string BulkOperationDB_Level(DataTable dt, string RepTime)
        //{
        //    try
        //   {


        //        DataColumn newColumn = new DataColumn("RepTime", typeof(String));
        //        newColumn.DefaultValue = RepTime;
        //        dt.Columns.Add(newColumn);

        //        //if (GP.Rows.Count > 0)
        //        //{
        //        //    string insRes = VrrModel.InsertTempReportDatdaNew();

        //        VrrModel.DeleteVrrPdr(dt.TableName);
        //        //}

        //        string strconnectionstring = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();

        //        SqlBulkCopy objbulk = new SqlBulkCopy(strconnectionstring, SqlBulkCopyOptions.FireTriggers);
        //        objbulk.BulkCopyTimeout = 6000;
        //        if (dt.TableName == "vrr")
        //        {
        //            objbulk.DestinationTableName = "VRR";

        //            objbulk.ColumnMappings.Add("Invoice #", "Invoice #");
        //            objbulk.ColumnMappings.Add("Tracking #", "Tracking #");
        //            objbulk.ColumnMappings.Add("Qty", "Qty");
        //            objbulk.ColumnMappings.Add("Product SKU", "Product SKU");
        //            objbulk.ColumnMappings.Add("Product Name", "Product Name");
        //            objbulk.ColumnMappings.Add("Unit Rebate", "Unit Rebate");
        //            objbulk.ColumnMappings.Add("Partial CB", "Partial CB");
        //            objbulk.ColumnMappings.Add("Total Rebate", "Total Rebate");
        //            objbulk.ColumnMappings.Add("Collected", "Collected");
        //            objbulk.ColumnMappings.Add("Balance", "Balance");
        //            objbulk.ColumnMappings.Add("Tax Amount", "Tax Amount");
        //            objbulk.ColumnMappings.Add("Carrier Price", "Carrier Price");
        //            objbulk.ColumnMappings.Add("Related Product", "Related Product");
        //            objbulk.ColumnMappings.Add("Related SKU", "Related SKU");
        //            objbulk.ColumnMappings.Add("Related SN", "Related SN");
        //            objbulk.ColumnMappings.Add("Related Cost", "Related Cost");
        //            objbulk.ColumnMappings.Add("Related Price", "Related Price");
        //            objbulk.ColumnMappings.Add("Rate Plan", "Rate Plan");
        //            objbulk.ColumnMappings.Add("Rate Plan 2", "Rate Plan 2");
        //            objbulk.ColumnMappings.Add("Term Code", "Term Code");
        //            objbulk.ColumnMappings.Add("Customer", "Customer");
        //            objbulk.ColumnMappings.Add("Sales Person", "Sales Person");
        //            objbulk.ColumnMappings.Add("Sales Person ID", "Sales Person ID");
        //            objbulk.ColumnMappings.Add("Sold On", "Sold On");
        //            objbulk.ColumnMappings.Add("Invoiced By", "Invoiced By");
        //            objbulk.ColumnMappings.Add("Invoiced At", "Invoiced At");
        //            objbulk.ColumnMappings.Add("Original Invoice", "Original Invoice");
        //            objbulk.ColumnMappings.Add("Original Sales Date", "Original Sales Date");
        //            objbulk.ColumnMappings.Add("Flagged", "Flagged");
        //            objbulk.ColumnMappings.Add("Reconciled", "Reconciled");
        //            objbulk.ColumnMappings.Add("Reconciled By", "Reconciled By");
        //            objbulk.ColumnMappings.Add("Reconciled On", "Reconciled On");
        //            objbulk.ColumnMappings.Add("Adjusted", "Adjusted");
        //            objbulk.ColumnMappings.Add("Charge Back", "Charge Back");
        //            objbulk.ColumnMappings.Add("Charge Back #", "Charge Back #");
        //            objbulk.ColumnMappings.Add("Charge Back Code", "Charge Back Code");
        //            objbulk.ColumnMappings.Add("Journal #", "Journal #");
        //            objbulk.ColumnMappings.Add("Contract #", "Contract #");
        //            objbulk.ColumnMappings.Add("Customer Identifier", "Customer Identifier");
        //            objbulk.ColumnMappings.Add("Comments", "Comments");
        //            objbulk.ColumnMappings.Add("Comments 2", "Comments 2");
        //            objbulk.ColumnMappings.Add("SOC Code", "SOC Code");
        //            objbulk.ColumnMappings.Add("SOC Code 2", "SOC Code 2");
        //            objbulk.ColumnMappings.Add("Extra Field", "Extra Field");
        //            objbulk.ColumnMappings.Add("ZIP Code", "ZIP Code");
        //            objbulk.ColumnMappings.Add("Region", "Region");
        //            objbulk.ColumnMappings.Add("District", "District");
        //            objbulk.ColumnMappings.Add("Vendor Account Name", "Vendor Account Name");
        //            objbulk.ColumnMappings.Add("Vendor #", "Vendor #");
        //            objbulk.ColumnMappings.Add("EmpID", "EmpID");
        //            objbulk.ColumnMappings.Add("RepTime", "RepTime");
        //    }
        //    else
        //    {
        //        objbulk.DestinationTableName = "PDR";

        //        objbulk.ColumnMappings.Add("Invoice #", "Invoice #");
        //        objbulk.ColumnMappings.Add("Invoiced By", "Invoiced By");
        //        objbulk.ColumnMappings.Add("Invoiced At", "Invoiced At");
        //        objbulk.ColumnMappings.Add("Sold By", "Sold By");
        //        objbulk.ColumnMappings.Add("Tendered By", "Tendered By");
        //        objbulk.ColumnMappings.Add("Sold On", "Sold On");
        //        objbulk.ColumnMappings.Add("Invoice Comments", "Invoice Comments");
        //        objbulk.ColumnMappings.Add("Customer", "Customer");
        //        objbulk.ColumnMappings.Add("Product SKU", "Product SKU");
        //        objbulk.ColumnMappings.Add("Tracking #", "Tracking #");
        //        objbulk.ColumnMappings.Add("Sold As Used", "Sold As Used");
        //        objbulk.ColumnMappings.Add("Contract #", "Contract #");
        //        objbulk.ColumnMappings.Add("Product Name", "Product Name");
        //        objbulk.ColumnMappings.Add("Refund", "Refund");
        //        objbulk.ColumnMappings.Add("Quantity", "Quantity");
        //        objbulk.ColumnMappings.Add("Unit Cost", "Unit Cost");
        //        objbulk.ColumnMappings.Add("Total Cost", "Total Cost");
        //        objbulk.ColumnMappings.Add("List Price", "List Price");
        //        objbulk.ColumnMappings.Add("Selling Price", "Selling Price");
        //        objbulk.ColumnMappings.Add("Original Price", "Original Price");
        //        objbulk.ColumnMappings.Add("Adjusted Price", "Adjusted Price");
        //        objbulk.ColumnMappings.Add("Net Profit ", "Net Profit ");
        //        objbulk.ColumnMappings.Add("Carrier Price", "Carrier Price");
        //        objbulk.ColumnMappings.Add("Net Sales", "Net Sales");
        //        objbulk.ColumnMappings.Add("Pricing Discounts", "Pricing Discounts");
        //        objbulk.ColumnMappings.Add("Total Product Coupons", "Total Product Coupons");
        //        objbulk.ColumnMappings.Add("Region", "Region");
        //        objbulk.ColumnMappings.Add("District", "District");
        //        objbulk.ColumnMappings.Add("Category", "Category");
        //        objbulk.ColumnMappings.Add("Location Type", "Location Type");
        //        objbulk.ColumnMappings.Add("EmpID", "EmpID");
        //        objbulk.ColumnMappings.Add("RepTime", "RepTime");
        //    }

        //        objbulk.WriteToServer(dt);

        //        //string ExceptionsData = VrrModel.InsertExceptionReportdataNew();
        //        //string DifferentialData = VrrModel.InsertDifferentialReportdataNew();

        //        return "Data has been Uploaded Successfully";

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}


        //public static DataTable AddReportDataHeaders(DataTable dt)
        //{
        //    try
        //    {
        //        dt.Columns.Add("Report_Id").DefaultValue = 0;
        //        dt.Columns.Add("Attribute_Id").DefaultValue = 0;
        //        dt.Columns.Add("RowIndex").DefaultValue = 0;
        //        dt.Columns.Add("BreakCount").DefaultValue = 0;
        //        dt.Columns.Add("RepData").DefaultValue = 0;
        //        dt.Columns.Add("RepTime").DefaultValue = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return dt;
        //}

        //public static DataTable GetALLStoreRQName()
        //{
        //    try
        //    {


        //        DAL.DAL objDal = new DAL.DAL();
        //        objDal.ProcName = "GetALLStoreRQName";
        //        DAL.SPParameters spParam = new DAL.SPParameters();
        //        DataTable dt = objDal.Getdata(spParam);
        //        return dt;

        //    }
        //    catch (Exception ex)
        //    {
        //        DataTable dt = new DataTable();
        //        return dt;
        //    }

        //}

        //public static string ActivationValidation(DataTable Vrr_Dt)
        //{
        //    string res = "";
        //    var ActivationValidation = Vrr_Dt.AsEnumerable().Where(x => (x.Field<string>("Product Name") == "New Activation" ||
        //                            x.Field<string>("Product Name") == "Add a Line Activation" ||
        //                            x.Field<string>("Product Name") == "Ported Activation") &&
        //                            string.IsNullOrWhiteSpace(x.Field<string>("Rate Plan")));
        //    int NoRatePlanCount = ActivationValidation.Count();
        //    if (NoRatePlanCount > 0)
        //    {
        //        res = "Upload Failed due to Activations given with no Rate Plan";
        //    }

        //    return res;
        //}

        //public static string RQFullNameValidationVrr(DataTable Vrr_Dt)
        //{
        //    string res = "";
        //    DataTable ALLStoreRQName = GetALLStoreRQName();
        //    var RQNameValidation = Vrr_Dt.AsEnumerable()
        //    .Where(x1 => !ALLStoreRQName.AsEnumerable().Any(x2 => x2.Field<string>("RQFullName").Trim().ToUpper() == x1.Field<string>("Invoiced At").Trim().ToUpper()))
        //     .Select(r => new
        //     {
        //         InvoicedAt = r.Field<string>("Invoiced At")
        //     })
        //     .Distinct()
        //     .ToList();

        //    int NoRQNameCount = RQNameValidation.Count();
        //    if (NoRQNameCount > 0)
        //    {
        //        res = "Upload Failed. <br /> Following unrecognized RQ Full Names found in VRR.";
        //        int a = 1;
        //        foreach (var field in RQNameValidation)
        //        {
        //            res += "<br />" + a.ToString() + ") " + field.InvoicedAt.ToString();
        //            a++;
        //        }
        //    }

        //    return res;
        //}

        //public static string PdrCategoryValidation(DataTable Pdr_Dt)
        //{
        //    string res = "";
        //    var ActivationValidation = Pdr_Dt.AsEnumerable().Where(x => (x.Field<string>("Product Name") == "New Activation" ||
        //                            x.Field<string>("Product Name") == "Add a Line Activation" ||
        //                            x.Field<string>("Product Name") == "Ported Activation") &&
        //                            string.IsNullOrWhiteSpace(x.Field<string>("Rate Plan")));
        //    int NoRatePlanCount = ActivationValidation.Count();
        //    if (NoRatePlanCount > 0)
        //    {
        //        res = "Upload Failed due to Activations given with no Rate Plan";
        //    }

        //    return res;
        //}

        //public static string RQFullNameValidationPdr(DataTable Pdr_Dt)
        //{
        //    string res = "";
        //    DataTable ALLStoreRQName = GetALLStoreRQName();
        //    var RQNameValidation = Pdr_Dt.AsEnumerable()
        //    .Where(x1 => !ALLStoreRQName.AsEnumerable().Any(x2 => x2.Field<string>("RQFullName").Trim().ToUpper() == x1.Field<string>("Invoiced At").Trim().ToUpper()))
        //     .Select(r => new
        //     {
        //         InvoicedAt = r.Field<string>("Invoiced At")
        //     })
        //     .Distinct()
        //     .ToList();

        //    int NoRQNameCount = RQNameValidation.Count();
        //    if (NoRQNameCount > 0)
        //    {
        //        res = "Upload Failed. <br /> Following unrecognized RQ Full Names found in PDR.";
        //        int a = 1;
        //        foreach (var field in RQNameValidation)
        //        {
        //            res += "<br />" + a.ToString() + ") " + field.InvoicedAt.ToString();
        //            a++;
        //        }
        //    }

        //    return res;
        //}



        //public static DataTable GetMTDEmailContent(string ReportName, string Status)
        //{
        //    try
        //    {
        //        DAL.DAL objDal = new DAL.DAL();
        //        objDal.ProcName = "GetMTDEmailContent";
        //        DAL.SPParameters spParam = new DAL.SPParameters();
        //        spParam.SetParam("ReportName", SqlDbType.NVarChar, ReportName);
        //        spParam.SetParam("Status", SqlDbType.BigInt, Status);
        //        DataTable dt = objDal.Getdata(spParam);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        StackTrace st = new StackTrace(ex, true);
        //        StackFrame frame = st.GetFrame(0);

        //        string line = frame.GetFileLineNumber().ToString();
        //        Common.recorderror("GP_ReportUploader.DataStringGp/GetEmailsforMTD", ex.Message, "", line);
        //        DataTable dt = new DataTable();
        //        return dt;
        //    }
        //}
    }
}