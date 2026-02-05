using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Collections;

namespace BulkUploader.DAL
{
    public class DAL
    {

        public static string GetConnectionstring()
        {
            string strconnectionstring = ConfigurationManager.ConnectionStrings["APIConnStr"].ToString();
            return strconnectionstring;
        }


        SqlConnection cn;
        SqlCommand cmd;
        SqlDataAdapter adap = new SqlDataAdapter();

        public static string strconnectionstring = GetConnectionstring();

        private string sProcName;
        private string sqlqry;

        public DAL()
        {

            cn = new SqlConnection();
            cmd = new SqlCommand();
            cn.ConnectionString = strconnectionstring;

        }

        public string ProcName
        {
            get
            {
                return sProcName;
            }
            set
            {
                sProcName = value;
            }
        }

        public string SQLQuery
        {
            get
            {
                return sqlqry;
            }
            set
            {
                sqlqry = value;
            }
        }

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
                //return "Error has occured :  " + ex.Message;
                return "0";
            }
        }

        public DataTable GetQryData(string sqlquery)
        {

            DataTable dt = new DataTable();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlquery;
            cmd.Connection = cn;

            adap.SelectCommand = cmd;
            int a = adap.Fill(dt);

            return dt;

        }

        public string AddData(string sqlquery)
        {
            int result = 0;
            string strMessage = "";
            try
            {
                cmd.CommandText = sqlquery;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                cn.Open();

                result = cmd.ExecuteNonQuery();

                if (result == 0)
                    strMessage = "No Record Updated";
                else
                    strMessage = "Operation was successful";
            }
            catch (SqlException ex)
            {
                return @"Error has occured :  " + ex.Message;
            }

            finally
            {
                cn.Close();
            }
            return strMessage;

        }

        public DataTable GetQryData(SPParameters spparam)
        {
            try
            {
                // SPParameters spparam = new SPParameters();

                DataTable dt = new DataTable();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlqry;
                cmd.Connection = cn;

                int i = 0;
                IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ParamData pData = (ParamData)myEnumerator.Current;
                    cmd.Parameters.Add(pData.pName, pData.pDataType);
                    cmd.Parameters[i].Value = pData.pValue;
                    i = i + 1;
                }


                cn.Open();
                cmd.Connection = cn;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();

                    // Fill the DataSet using default values for DataTable names, etc
                    da.Fill(ds);

                    // Detach the SqlParameters from the command object, so they can be used again
                    cmd.Parameters.Clear();

                    cn.Close();

                    int count = ds.Tables[0].Rows.Count;

                    // Return the dataset
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;

            }

        }

        public string AddQryData(SPParameters spparam)
        {

            int result = 0;
            string strMessage = "";
            try
            {
                cmd.CommandText = sqlqry;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                int i = 0;



                IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ParamData pData = (ParamData)myEnumerator.Current;
                    cmd.Parameters.Add(pData.pName, pData.pDataType);

                    if (pData.pValue != null)
                    {
                        cmd.Parameters[i].Value = pData.pValue;
                    }
                    else if (pData.pic != null)
                    {

                        cmd.Parameters[i].Value = pData.pic;
                    }
                    i = i + 1;
                }


                cn.Open();

                result = int.Parse(cmd.ExecuteNonQuery().ToString());

                if (result == 0)
                    strMessage = "No Record Updated";
                else if (result == -1)
                    strMessage = "No Record Updated";
                else
                    strMessage = "Operation was successful";
            }
            catch (SqlException ex)
            {
                return @"Error has occured :  " + ex.Message;
            }

            finally
            {
                cmd.Parameters.Clear();
                cn.Close();
            }
            return result.ToString();

        }

        public DataTable Getdata(SPParameters spparam)
        {
            try
            {
                // SPParameters spparam = new SPParameters();

                DataTable dt = new DataTable();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sProcName;
                cmd.Connection = cn;
                cmd.CommandTimeout = 60000;
                int i = 0;
                IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ParamData pData = (ParamData)myEnumerator.Current;
                    cmd.Parameters.Add(pData.pName, pData.pDataType);
                    cmd.Parameters[i].Value = pData.pValue;
                    i = i + 1;
                }


                cn.Open();
                cmd.Connection = cn;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();

                    // Fill the DataSet using default values for DataTable names, etc
                    da.Fill(ds);

                    // Detach the SqlParameters from the command object, so they can be used again
                    cmd.Parameters.Clear();

                    cn.Close();

                    int count = ds.Tables[0].Rows.Count;

                    // Return the dataset
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                return dt;

            }

        }

        public DataTable GetdataNew(SPParameters spparam)
        {
            DataTable dt = new DataTable();

            string conStr = ConfigurationManager
                            .ConnectionStrings["APIConnStr"]
                            .ConnectionString;

            using (SqlConnection cn = new SqlConnection(conStr))
            using (SqlCommand cmd = new SqlCommand(sProcName, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60000;

                foreach (ParamData pData in spparam.GetParams())
                {
                    cmd.Parameters
                       .Add(pData.pName, pData.pDataType)
                       .Value = pData.pValue;
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);   // adapter handles connection
                }
            }

            return dt;
        }



        public string AddData(SPParameters spparam)
        {

            int result = 0;
            string strMessage = "";
            try
            {
                cmd.CommandText = sProcName;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cn;

                int i = 0;



                IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ParamData pData = (ParamData)myEnumerator.Current;
                    cmd.Parameters.Add(pData.pName, pData.pDataType);

                    if (pData.DTpValue != null)
                    {
                        cmd.Parameters[i].Value = pData.DTpValue;
                    }
                    if (pData.pValue != null)
                    {
                        cmd.Parameters[i].Value = pData.pValue;
                    }
                    else if (pData.pic != null)
                    {

                        cmd.Parameters[i].Value = pData.pic;
                    }
                    i = i + 1;
                }


                cn.Open();

                result = int.Parse(cmd.ExecuteNonQuery().ToString());

                if (result == 0)
                    strMessage = "No Record Updated";
                else if (result == -1)
                    strMessage = "No Record Updated";
                else
                    strMessage = "Operation was successful";
            }
            catch (SqlException ex)
            {
                return @"Error has occured :  " + ex.Message;
            }

            finally
            {
                cmd.Parameters.Clear();
                cn.Close();
            }
            return result.ToString();

        }

        //public string AddData(SPParameters spparam)
        //{
        //    int result = 0;

        //    try
        //    {
        //        cmd.CommandText = sProcName;
        //        cmd.CommandTimeout = 0;
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = cn;

        //        int i = 0;
        //        IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
        //        while (myEnumerator.MoveNext())
        //        {
        //            ParamData pData = (ParamData)myEnumerator.Current;
        //            cmd.Parameters.Add(pData.pName, pData.pDataType);

        //            if (pData.pValue != null)
        //                cmd.Parameters[i].Value = pData.pValue;
        //            else if (pData.DTpValue != null)
        //                cmd.Parameters[i].Value = pData.DTpValue;
        //            else if (pData.pic != null)
        //                cmd.Parameters[i].Value = pData.pic;
        //            else
        //                cmd.Parameters[i].Value = DBNull.Value;

        //            i++;
        //        }

        //        // ADD OUTPUT PARAM
        //        SqlParameter statusParam = new SqlParameter("@Status", SqlDbType.Int)
        //        {
        //            Direction = ParameterDirection.Output
        //        };
        //        cmd.Parameters.Add(statusParam);

        //        cn.Open();
        //        cmd.ExecuteNonQuery();

        //        result = Convert.ToInt32(cmd.Parameters["@Status"].Value);
        //    }
        //    catch (SqlException ex)
        //    {
        //        return "Error has occurred: " + ex.Message;
        //    }
        //    finally
        //    {
        //        cmd.Parameters.Clear();
        //        cn.Close();
        //    }

        //    return result.ToString();
        //}


        public string AddandInsert(SPParameters spparam)
        {

            int result = 0;
            string res = "";
            string strMessage = "";
            try
            {
                cmd.CommandText = sProcName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cn;

                int i = 0;
                SqlParameter output = new SqlParameter();

                IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ParamData pData = (ParamData)myEnumerator.Current;

                    if (pData.pDirection == ParameterDirection.Output)
                    {
                        output = cmd.Parameters.Add(pData.pName, pData.pDataType);
                    }
                    else
                    {
                        cmd.Parameters.Add(pData.pName, pData.pDataType);
                    }

                    if (pData.pValue != null)
                    {
                        cmd.Parameters[i].Value = pData.pValue;
                        cmd.Parameters[i].Direction = pData.pDirection;

                    }
                    else if (pData.pic != null)
                    {

                        cmd.Parameters[i].Value = pData.pic;
                    }
                    else if (pData.pDirection == ParameterDirection.Output)
                    {
                        cmd.Parameters[i].Direction = pData.pDirection;
                    }

                    i = i + 1;
                }


                cn.Open();

                result = int.Parse(cmd.ExecuteNonQuery().ToString());

                res = output.Value.ToString();

                if (result == 0)
                    strMessage = "No Record Updated";
                else if (result == -1)
                    strMessage = "No Record Updated";
                else
                    strMessage = "Operation was successful";
            }
            catch (SqlException ex)
            {
                return "0";
            }

            finally
            {
                cmd.Parameters.Clear();
                cn.Close();
            }
            return res;

        }
        public string AddDataRetScalar(SPParameters spparam)
        {

            string result = "";
            string strMessage = "";
            try
            {
                cmd.CommandText = sProcName;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cn;

                int i = 0;



                IEnumerator myEnumerator = spparam.GetParams().GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    ParamData pData = (ParamData)myEnumerator.Current;
                    cmd.Parameters.Add(pData.pName, pData.pDataType);

                    if (pData.pValue != null)
                    {
                        cmd.Parameters[i].Value = pData.pValue;
                    }
                    else if (pData.pic != null)
                    {

                        cmd.Parameters[i].Value = pData.pic;
                    }
                    i = i + 1;
                }


                cn.Open();

                result = Convert.ToString(cmd.ExecuteScalar());

                //if (result == "")
                //    strMessage = "No Record Updated";
                //else if (result == "")
                //    strMessage = "No Record Updated";
                //else
                //    strMessage = "Operation was successful";
            }
            catch (SqlException ex)
            {
                return @"Error has occured :  " + ex.Message;
            }

            finally
            {
                cmd.Parameters.Clear();
                cn.Close();
            }
            return result.ToString();

        }

    }

    struct ParamData
    {
        public DataTable DTpValue;
        public SqlDbType pDataType;
        public string pName, pValue;
        public byte[] pic;
        public ParameterDirection pDirection;

        public ParamData(string pName, SqlDbType pDataType, DataTable DTpValue)
        {
            this.pName = pName;
            this.pDataType = pDataType;
            this.DTpValue = DTpValue;
            this.pic = null;
            this.pValue = null;
            this.pDirection = ParameterDirection.Input;
        }

        public ParamData(string pName, SqlDbType pDataType, string pValue)
        {
            this.pName = pName;
            this.pDataType = pDataType;
            this.pValue = pValue;
            this.DTpValue = null;
            this.pic = null;
            this.pDirection = ParameterDirection.Input;

        }

        public ParamData(string pName, SqlDbType pDataType, byte[] pValue)
        {

            this.pName = pName;
            this.pDataType = pDataType;
            this.pic = pValue;
            this.DTpValue = null;
            this.pValue = null;
            this.pDirection = ParameterDirection.Input;

        }

        public ParamData(string pName, SqlDbType pDataType, string pValue, ParameterDirection pDirection)
        {
            this.pName = pName;
            this.pDataType = pDataType;
            this.pValue = pValue;
            this.DTpValue = null;
            this.pic = null;
            this.pDirection = pDirection;
        }

        public ParamData(string pName, SqlDbType pDataType, ParameterDirection pDirection)
        {
            this.pName = pName;
            this.pDataType = pDataType;
            this.pValue = null;
            this.DTpValue = null;
            this.pic = null;
            this.pDirection = pDirection;
        }




    }
    public class SPParameters : System.Collections.CollectionBase
    {

        public ArrayList sParams = new ArrayList();


        public void SetParam(string pName, SqlDbType pDataType, string pValue)
        {

            ParamData pData = new ParamData(pName, pDataType, pValue);
            sParams.Add(pData);
        }

        public void SetParam(string pName, SqlDbType pDataType, string pValue, ParameterDirection pDirection)
        {

            ParamData pData = new ParamData(pName, pDataType, pValue, pDirection);
            sParams.Add(pData);
        }

        public void SetParam(string pName, SqlDbType pDataType, ParameterDirection pDirection)
        {

            ParamData pData = new ParamData(pName, pDataType, pDirection);
            sParams.Add(pData);
        }

        public void SetParam(string pName, SqlDbType pDataType, byte[] pValue)
        {

            ParamData pData = new ParamData(pName, pDataType, pValue);
            sParams.Add(pData);
        }

        public void SetParam(string pName, SqlDbType pDataType, DataTable pValue)
        {
            ParamData pData = new ParamData(pName, pDataType, pValue);
            sParams.Add(pData);
        }

        public ArrayList GetParams()
        {
            if (!(sParams == null))
            {
                return sParams;
            }
            else
            {
                return null;

            }

        }
    }
}