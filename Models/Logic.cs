using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace BulkUploader.Models
{
    public class Logic
    {
        public static DataTable Sorting(DataTable dt, string Checker, string optional = null)
        {

            DataTable dtsort = dt;
            DataTable Reports = new DataTable();
            try
            {
                if (optional == null)
                {
                    // if (Checker == "Cricket New Activation")
                    if (Checker == "New Activation")
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where (dts.Field<string>("Product Name") == Checker || dts.Field<string>("Product Name") == "Add a Line Activation" ||
                                dts.Field<string>("Product Name") == "Ported Activation") && Convert.ToInt32(dts.Field<string>("Qty")) >= 0 && Convert.ToInt32(dts.Field<string>("Total Rebate")) != 0
                                select dts;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }
                    else if (Checker == "Auto Pay Enrollment")
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where dts.Field<string>("Product Name") == Checker || dts.Field<string>("Product Name") == "Auto Pay Enrollment (New Act)"
                                select dts;

                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }
                    else
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where dts.Field<string>("Product Name") == Checker
                                select dts;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }

                }
                else if (optional != null)
                {
                    if (Checker == "Customer Assistance Fee")
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where dts.Field<string>("Product Name").Contains(Checker)
                                select dts;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }
                    else if (Checker == "Cricket Bill Payment")
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where dts.Field<string>("Product Name").Contains(Checker)
                                select dts;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }
                    else if (Checker == "Progressive Lease Vendor Rebate")
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where dts.Field<string>("Product Name").Contains(Checker)
                                select dts;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }
                    else
                    {
                        var a = from dts in dtsort.AsEnumerable()
                                where dts.Field<string>("Category").Contains(Checker)
                                select dts;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            Reports.Columns.Add(dc.ColumnName, dc.DataType);
                        }
                        foreach (var aa in a.ToList())
                        {
                            DataRow dr = Reports.NewRow();
                            Reports.ImportRow(aa);
                        }
                    }
                }


                return Reports;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DataTable AddColumnsVrr(DataTable dt)
        {
            try
            {
                dt.Columns.Add("Invoice Count");
                dt.Columns.Add("Invoice Key");

                foreach (DataRow dr in dt.Rows)
                {
                    dr["Invoice Count"] = 0.ToString();
                    dr["Invoice Key"] = 0.ToString();

                }
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public static DataTable AddColumnsPdr(DataTable dt)
        {
            try
            {
                //20201231 dt.Columns.Add("Invoice Count");
                //20201231 dt.Columns.Add("Invoice Key");
                dt.Columns.Add("Refund Key");

                foreach (DataRow dr in dt.Rows)
                {
                    //20201231  dr["Invoice Count"] = 0.ToString();
                    //20201231  dr["Invoice Key"] = 0.ToString();
                    dr["Refund Key"] = 0.ToString();

                }
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public static DataTable AdditionalPdr(DataTable Pdr_Dt)
        {
            try
            {

                var devices = Pdr_Dt.AsEnumerable().Where(x => (x["Category"].ToString().Contains("Devices")));
                int a = devices.Count();
                int b = 0;


                var query = (from row in devices
                             group row by new { Key = row.Field<string>("Invoice #") } into g
                             select new
                             {
                                 GroupDescription = g.Key,
                                 Values = (from value in g.ToList()
                                           group value by value["Invoice #"] into valueGroup
                                           select valueGroup.Sum(x => Convert.ToInt32(x["Quantity"].ToString())))
                             }).ToList();

                
                foreach (DataRow dr in devices.ToList())
                {

                    var numberOfRecords = "0";
                    string value = (from pair in query where pair.GroupDescription.Key == dr["Invoice #"].ToString() select pair.Values.FirstOrDefault()).FirstOrDefault().ToString();
                    numberOfRecords = string.IsNullOrEmpty(value) ? "0" : value;
                    dr["Refund Key"] = numberOfRecords;
                    if (dr["Refund Key"].ToString() == "0")
                    {
                        dr["Refund"] = "Refunded/Exchange";
                    }
                    b++;

                }

            }
            catch (Exception ex)
            {
            }
            return Pdr_Dt;
        }

        public static DataTable AdditionalVrr(DataTable Vrr_Dt, DataTable Pdr_Dt)
        {
            try
            {
                DataTable Temp = new DataTable();
                Temp = Vrr_Dt.Clone();

                var filter = Vrr_Dt.AsEnumerable().AsEnumerable().Where(x => (x["Product Name"].ToString()) == "New Activation" ||
                (x["Product Name"].ToString()) == "Add a Line Activation" ||
                (x["Product Name"].ToString()) == "Ported Activation");

                foreach (DataRow dr in filter.ToList())
                {
                    Temp.ImportRow(dr);
                }

                foreach (DataRow dr in filter.ToList())
                {


                    var numberOfRecords = 0;
                    if (Temp != null)
                    {


                        numberOfRecords = Temp.AsEnumerable().Where(x => (x["Invoice #"].ToString()) == dr["Invoice #"].ToString()).Count();



                        for (int i = 0; i < Temp.Rows.Count; i++)
                        {
                            DataRow recRow = Temp.Rows[i];
                            if (recRow["Invoice #"].ToString() == dr["Invoice #"].ToString())
                            {
                                recRow.Delete();
                                Temp.AcceptChanges();
                                break;
                            }
                        }


                        if (numberOfRecords > 0)
                        {
                            dr["Invoice Count"] = numberOfRecords;
                            string invoice_c = dr["Invoice #"].ToString();

                            invoice_c += dr["Invoice Count"].ToString();

                            dr["Invoice Key"] = invoice_c;
                        }

                        foreach (DataRow dr1 in Pdr_Dt.Rows)
                        {

                            if (dr["Invoice Key"].ToString() == dr1["Invoice Key"].ToString())
                            {
                                dr["Rate Plan"] = dr1["Product Name"].ToString();
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Vrr_Dt;
        }

        public static DataTable RemoveNegativeQuantityFromVrr(DataTable Vrr_Dt)
        {
            try
            {
                DataTable Temp = Vrr_Dt.Copy();

                
                int i = 0;
                foreach (DataRow dr in Temp.Rows)
                {

                        if (Convert.ToDecimal(dr["Qty"]) < 0)
                        {
                            int index = Temp.Rows.IndexOf(dr);
                            Vrr_Dt.Rows.RemoveAt(index - i);
                            i++;
                            Vrr_Dt.AcceptChanges();
                            
                        }
                    
                }

            }
            catch (Exception ex)
            {
            }
            return Vrr_Dt;

        }

        public static DataTable RemoveNegativeInvoicesFromVrr(DataTable Vrr_Dt, DataTable Pdr_Dt)
        {
            try
            {
                DataTable Temp = Vrr_Dt.Copy();

                var filter = Pdr_Dt.AsEnumerable().AsEnumerable().Where(x => Convert.ToInt32(x["Quantity"]) < 0);
                int i = 0;
                foreach (DataRow dr in Temp.Rows)
                {

                    foreach (DataRow dr1 in filter.ToList())
                    {
                        if ((dr["Related SN"].ToString() == dr1["Tracking #"].ToString()) && !String.IsNullOrEmpty(dr["Related SN"].ToString()) && (dr1["Refund Key"].ToString() != "0") && (dr1["Refund"].ToString() == "Yes"))
                        {
                            int index = Temp.Rows.IndexOf(dr);
                            Vrr_Dt.Rows.RemoveAt(index - i);
                            i++;
                            Vrr_Dt.AcceptChanges();
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Vrr_Dt;

        }

        public static DataTable AdditionalCNA(DataTable dt, int Report_Id)
        {
            try
            {
                DataTable AddCol = VrrModel.GetAdditional(Report_Id);
                DataTable Filters = VrrModel.Filter(Report_Id);
                DataTable FiltersGeo = VrrModel.Filter(3);
                DataTable FilterCNA = FilterProcessCNA(Filters);
                foreach (DataRow dr in AddCol.Rows)
                {
                    dt.Columns.Add(dr["Attribute_Name"].ToString());
                }
                foreach (DataRow dr in dt.Rows)
                {
                    dr["GP without Spiff"] = 0.ToString();
                    dr["Spiff 22"] = 0.ToString();
                    dr["Spiff 6"] = 0.ToString();
                    dr["Total Spiff"] = 0.ToString();
                    dr["GP with Spiff"] = 0.ToString();
                }
                foreach (DataRow dr in FilterCNA.Rows)
                {
                    /* 
                     Rateplan1	30 GB Simply Data
                     Rateplan2	3 GB Simply Data
                     Rateplan3	10 GB Simply Data
                     Rateplan4	$25 Talk & Text with HD Voice
                     Rateplan5	Unlimited Cricket More
                     Rateplan6	Unlimited Cricket Core
                     Rateplan7	2GB Data
                     Rateplan8	20GB Simply Data
                     Rateplan9	40GB Simply Data
                     Total Rebate1	40
                     GP Without Spiff1	55--53
                     GP Without Spiff2	85--83
                     GP Without Spiff3	60--58
                     GP Without Spiff4	90--88
                     GP Without Spiff5	45
                     GP Without Spiff6	75
                     Spiff 22	22
                     Spiff 6	7
                     Qty	25
                     BYOD	BYOD Phone
                      */

                    var filter = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan6") && Convert.ToInt32(dts.Field<string>("Total Rebate")) < Convert.ToInt32(dr.Field<string>("Total Rebate1"))) select dts;
                    foreach (DataRow dr1 in filter.ToList())
                    {
                        dr1["GP without Spiff"] = Convert.ToDouble(dr["GP without Spiff1"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var filter1 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan6") && Convert.ToInt32(dts.Field<string>("Total Rebate")) >= Convert.ToInt32(dr.Field<string>("Total Rebate1"))) select dts;
                    foreach (DataRow dr1 in filter1.ToList())
                    {
                        dr1["GP without Spiff"] = Convert.ToDouble(dr["GP without Spiff2"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var filter2 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan5") && Convert.ToInt32(dts.Field<string>("Total Rebate")) < Convert.ToInt32(dr.Field<string>("Total Rebate1"))) select dts;
                    foreach (DataRow dr1 in filter2.ToList())
                    {
                        dr1["GP without Spiff"] = Convert.ToDouble(dr["GP without Spiff3"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var filter3 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan5") && Convert.ToInt32(dts.Field<string>("Total Rebate")) >= Convert.ToInt32(dr.Field<string>("Total Rebate1"))) select dts;
                    foreach (DataRow dr1 in filter3.ToList())
                    {
                        dr1["GP without Spiff"] = Convert.ToDouble(dr["GP without Spiff4"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var filter4 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan9") || dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan8") || dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan3") || dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan2") || dts.Field<string>("Rate Plan") == dr.Field<string>("Rate Plan1") || dts.Field<string>("Rate Plan") == "100GB Simply Data") select dts;
                    foreach (DataRow dr1 in filter4.ToList())
                    {
                        dr1["GP without Spiff"] = dr1["Total Rebate"];
                    }
                    var filter5 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan9") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan8") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan6") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan5") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan3") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan2") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan1") && dts.Field<string>("Rate Plan") != "100GB Simply Data" && Convert.ToInt32(dts.Field<string>("Total Rebate")) < Convert.ToInt32(dr.Field<string>("Total Rebate1"))) select dts;
                    foreach (DataRow dr1 in filter5.ToList())
                    {
                        dr1["GP without Spiff"] = Convert.ToDouble(dr["GP without Spiff5"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var filter6 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan9") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan8") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan6") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan5") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan3") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan2") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan1") && dts.Field<string>("Rate Plan") != "100GB Simply Data" && Convert.ToInt32(dts.Field<string>("Total Rebate")) >= Convert.ToInt32(dr.Field<string>("Total Rebate1"))) select dts;
                    foreach (DataRow dr1 in filter6.ToList())
                    {
                        dr1["GP without Spiff"] = Convert.ToDouble(dr["GP without Spiff6"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    // edited by zain 20200211
                    var filter7 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan9") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan8") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan4") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan3") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan2") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan1") && dts.Field<string>("Rate Plan") != "100GB Simply Data") select dts;
                    DataTable dataTable = filter7.CopyToDataTable();
                    foreach (DataRow dr1 in filter7.ToList())
                    {
                        dr1["Spiff 22"] = Convert.ToDouble(dr["Spiff 22"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var spiff6filter = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan9") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan8") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan3") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan2") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan1") && dts.Field<string>("Rate Plan") != "100GB Simply Data") select dts;
                    foreach (DataRow dr1 in spiff6filter.ToList())
                    {

                        dr1["Spiff 6"] = Convert.ToDouble(dr["Spiff 6"]) * Convert.ToDouble(dr1["Qty"]);
                    }
                    var filter11 = from dts in dt.AsEnumerable() where (dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan9") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan8") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan3") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan2") && dts.Field<string>("Rate Plan") != dr.Field<string>("Rate Plan1") && dts.Field<string>("Rate Plan") != "100GB Simply Data") select dts;
                    foreach (DataRow dr1 in FiltersGeo.Rows)
                    {
                        foreach (DataRow dr2 in filter11.ToList())
                        {
                            if ((dr2["Invoiced At"].ToString().ToLower().Replace(",", string.Empty)) == (dr1["Repdata"].ToString().ToLower().Replace(",", string.Empty)) && (dr2["EmpID"].ToString().ToLower()) == (dr1["EmpID"].ToString().ToLower()))
                            {
                                double sum = Convert.ToDouble(dr2["Spiff 6"]);
                                sum += Convert.ToDouble(dr["Qty"]);
                                dr2["Spiff 6"] = sum.ToString();
                            }
                        }
                    }
                    foreach (DataRow d in dt.Rows)
                    {
                        d["Total Spiff"] = (Convert.ToInt32(d["Spiff 22"].ToString()) + Convert.ToInt32(d["Spiff 6"].ToString())).ToString();
                    }

                   
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        dr1["GP with Spiff"] = (Convert.ToInt32(dr1["GP without Spiff"].ToString()) + Convert.ToInt32(dr1["Total Spiff"].ToString())).ToString();
                    }
                    var filter10 = from dts in dt.AsEnumerable() where (dts.Field<string>("Related Product") == dr.Field<string>("BYOD")) select dts;
                    foreach (DataRow dr1 in filter10.ToList())
                    {
                        dr1["Product Name"] = dr["BYOD"].ToString();
                    }

                }

            }
            catch (Exception ex)
            { }
            return dt;
        }

        public static DataTable Additionalupg(DataTable dt, int Report_Id)
        {
            try
            {
                DataTable AddCol = VrrModel.GetAdditional(Report_Id);
                DataTable Filters = VrrModel.Filter(Report_Id);
                DataTable FiltersGeo = VrrModel.Filter(3);
                DataTable FilterUpg = FilterProcessUpgrade(Filters);
                foreach (DataRow dr in AddCol.Rows)
                {
                    dt.Columns.Add(dr["Attribute_Name"].ToString());
                }
                foreach (DataRow dr in dt.Rows)
                {
                    dr["GP without Spiff"] = 0.ToString();
                    dr["Spiff 22"] = 0.ToString();
                    dr["Spiff 6"] = 0.ToString();
                    dr["Total Spiff"] = 0.ToString();
                    dr["GP with Spiff"] = 0.ToString();
                }
                foreach (DataRow FilterRow in FilterUpg.Rows)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["GP without Spiff"] = Convert.ToInt32(FilterRow["GP without Spiff3"]) * (Convert.ToInt32(dr["Qty"]));
                    }
                    var filter8 = from dts in dt.AsEnumerable() where (Convert.ToInt32(dts.Field<string>("Qty")) < Convert.ToInt32(FilterRow.Field<string>("Qty4"))) select dts;
                    foreach (DataRow dr1 in filter8.ToList())
                    {
                        dr1["Total Spiff"] = (Convert.ToInt32(dr1["Spiff 22"].ToString()) + Convert.ToInt32(dr1["Spiff 6"].ToString())).ToString();
                    }
                    foreach (DataRow dr1 in dt.Rows)
                    {
                        dr1["GP with Spiff"] = (Convert.ToInt32(dr1["GP without Spiff"].ToString()) + Convert.ToInt32(dr1["Total Spiff"].ToString())).ToString();
                    }
                }
            }
            catch (Exception ex)
            { }
            return dt;
        }

        public static DataTable AdditionalAcc(DataTable dt, int Report_Id, DataTable working)
        {
            DataTable AddCol = VrrModel.GetAdditional(Report_Id);
            foreach (DataRow dr in AddCol.Rows)
            {
                dt.Columns.Add(dr["Attribute_Name"].ToString());
            }
            foreach (DataRow dr in dt.Rows)
            {
                dr["PPP"] = 0.ToString();
            }
            foreach (DataRow dr in working.Rows)
            {
                foreach (DataRow dr1 in dt.Rows)
                {
                    if (dr["Invoice #"].ToString() == dr1["Invoice #"].ToString())
                    {
                        dr1["PPP"] = dr["Tracking #"].ToString();
                    }
                }
            }

            var filterSoldFor = from dts in dt.AsEnumerable() where dts.Field<string>("Selling Price") == "0" && dts.Field<string>("PPP") != "0" select dts;
            foreach (DataRow dr in filterSoldFor.ToList())
            {

                double listPricce = Convert.ToDouble(dr["List Price"]);
                double totalCost = Convert.ToDouble(dr["Total Cost"]);


                dr["Net Profit "] = (listPricce - totalCost).ToString();
                dr["Pricing Discounts"] = "0";


            }
            return dt;


        }

        public static DataTable FilterProcessCNA(DataTable data)
        {
            DataTable CNA = new DataTable();
            try
            {
                var filter = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 881 select dts;
                int i = 1;
                DataRow dr1 = CNA.NewRow();
                foreach (DataRow dr in filter.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString() + i);
                    i++;
                }
                var filter1 = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 886 select dts;
                int k = 1;
                foreach (DataRow dr in filter1.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString() + k);
                    k++;
                }
                var filter2 = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 882 select dts;
                int m = 1;
                foreach (DataRow dr in filter2.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString() + m);
                    m++;
                }
                var filter3 = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 884 select dts;
                foreach (DataRow dr in filter3.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString());
                }
                var filter4 = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 885 select dts;
                foreach (DataRow dr in filter4.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString());
                }
                var filter5 = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 902 select dts;
                foreach (DataRow dr in filter5.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString());
                }
                var filter6 = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 904 select dts;
                foreach (DataRow dr in filter6.ToList())
                {
                    CNA.Columns.Add(dr["Attribute_Name"].ToString());
                }
                for (int j = 0; j < CNA.Columns.Count; j++)
                {
                    foreach (DataRow dr2 in filter.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    foreach (DataRow dr2 in filter1.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    foreach (DataRow dr2 in filter2.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    foreach (DataRow dr2 in filter3.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    foreach (DataRow dr2 in filter4.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    foreach (DataRow dr2 in filter5.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    foreach (DataRow dr2 in filter6.ToList())
                    {
                        dr1[j] = dr2["Repdata"].ToString();
                        j++;
                    }
                    CNA.Rows.Add(dr1);
                }
            }
            catch (Exception ex)
            { }
            return CNA;
        }

        public static DataTable FilterProcessUpgrade(DataTable data)
        {
            DataTable upg = new DataTable();
            try
            {
                var filter = from dts in data.AsEnumerable() where dts.Field<int>("Attribute_Id") == 889 || dts.Field<int>("Attribute_Id") == 890 || dts.Field<int>("Attribute_Id") == 905 select dts;
                DataRow dr1 = upg.NewRow();
                int i = 0;
                foreach (DataRow dr in filter.ToList())
                {
                    upg.Columns.Add(dr["Attribute_Name"].ToString() + i);
                    i++;
                }
                for (int j = 0; j < upg.Columns.Count; j++)
                {
                    foreach (DataRow dr in filter.ToList())
                    {
                        dr1[j] = dr["Repdata"].ToString();
                        j++;
                    }
                    upg.Rows.Add(dr1);
                }
            }
            catch (Exception ex)
            { }
            return upg;
        }

        public static string GPReportMFR(DataTable Bp, DataTable Ap, DataTable PPP, DataTable CNA, DataTable UPG, DataTable Byod, DataTable Working, DataTable Cricket_Protect, DataTable Cricket_Protect_Plus, DataTable Acc, DataTable Sims, DataTable Fees, DataTable two_gb_data, DataTable CAF, string date, DataTable Goals_Dt, DataTable Pdr_Dt_Full, DataTable ProgLeaseVendorRebate, string CheckBackDateReport, string Result)
        {
            DataTable GP = new DataTable();
            DataTable AddCol = VrrModel.GetAdditional(6);
            foreach (DataRow dr in AddCol.Rows)
            {
                GP.Columns.Add(dr["Attribute_Name"].ToString()).DefaultValue = 0;
                //GP.Columns().DefaultValue = 0;
            }
            //  GP.Columns.Add("2GB Data");

            DataTable StaticData = new DataTable();

            if (Result == "PDRZ")
            {
                StaticData = VrrModel.GetAllDataFromStore_ZeroRebate();
            }
            else
            {
                StaticData = VrrModel.GetAllDataFromStore();
            }

            DataTable CustomerTotalCount = null;
            if (Pdr_Dt_Full != null && Result != "PDRZ")
            {


                var result =  Pdr_Dt_Full.AsEnumerable()
                            .Select(row => new
                            {
                                InvoiceAt = row.Field<string>("Invoiced At"),
                                EmpID = row.Field<string>("EmpID"),
                                Customer = row.Field<string>("Customer")
                            }).Distinct().ToList();



                DataTable CustomerTable = ToDataTable(result);
                string res = VrrModel.InsertNewCustomers(CustomerTable);

                CustomerTotalCount = VrrModel.GetCustomerCount();

              

            }


            var drow = Pdr_Dt_Full != null && Result != "PDRZ" ? from dts in Pdr_Dt_Full.AsEnumerable().Where(r => r.Field<string>("Category") == " >> Cricket Activations >> Devices >> Smart Phone" ||
               r.Field<string>("Category") == " >> Cricket Activations >> Devices >> Feature Phone")
                                                                 select dts : null;



            var query1 = Pdr_Dt_Full != null && Result != "PDRZ" ? (from row in drow
                                                                    group row by new { Key = row.Field<string>("Invoiced At"), Key1 = row.Field<string>("EmpID") } into g
                                                                    orderby g.Key.Key, g.Key.Key1
                                                                    select new
                                                                    {
                                                                        GroupDescription = g.Key.Key,
                                                                        GroupDescription1 = g.Key.Key1,
                                                                        Values = from value in g.ToList()
                                                                                 group value by value["Invoice #"]
                                                                                  into valueGroup
                                                                                 select valueGroup.Sum(x => Convert.ToInt32(x.Field<string>("Quantity")))

                                                                    }).ToList() : null;




            var query = ProgLeaseVendorRebate != null ? (from row in ProgLeaseVendorRebate.AsEnumerable()
                             group row by new { Key = row.Field<string>("Invoiced At"), Key1 = row.Field<string>("EmpID") } into g
                             orderby g.Key.Key, g.Key.Key1
                             select new
                             {
                                 GroupDescription = g.Key.Key,
                                 GroupDescription1 = g.Key.Key1,
                                 Values = (from value in g.ToList()
                                           group value by value["EmpID"] into valueGroup
                                           select valueGroup.Sum(x => Convert.ToInt32(x["Quantity"].ToString())))
                             }).ToList() : null;
            

            foreach (DataRow dr in StaticData.Rows)
            {
                var numberOfRecordsCNA = 0;
                var TwogbCountCNA = 0;
                var TenGbCountCNA = 0;
                var UnlCoreCountCNA = 0;
                var UnlMoreCountCNA = 0;
                var TwentyGBSimplyDataCountCNA = 0;
                var TwentyFiveGBCountCNA = 0;
                var FortyGBSimplyDataCountCNA = 0;
                var HundredGBSimplyDataCountCNA = 0;
                var numberOfRecordsByod = 0;
                var TwogbCountByod = 0;
                var TenGbCountByod = 0;
                var UnlCoreCountByod = 0;
                var UnlMoreCountByod = 0;
                var TwentyGBSimplyDataCountByod = 0;
                var TwentyFiveGBCountByod = 0;
                var FortyGBSimplyDataCountByod = 0;
                var HundredGBSimplyDataCountByod = 0;
                var numberOfRecordsTwoGbData = 0;
                var numberOfRecordsCktPrtct = 0;
                var numberOfRecordsCktPrtctPlus = 0;
                var numberOfRecordsUPG = 0;
                var numberOfRecordsBP = 0;
                var numberOfRecordsAP = 0;
                var numberofRecordsWrk = "0";
                var numberofRecordsWrkTS = "0";
                var numberofRecordsACC = "0";
                var listPriceACC = "0";
                var totalDiscountACC = "0";
                var Acc_CountACC = "0";
                var TotalSalesACC = "0";
                var numberofRecordsSIMS = "0";
                var numberofRecordsFEES = "0";
                var listPriceFEES = "0";
                var totalDiscountFEES = "0";
                var numberofRecordsCAF = "0";
                var numberofRecordsPVR = "0";


                if (Goals_Dt == null)
                {
                    dr["RQ4 Full Name"] = dr["RQ4 Full Name"].ToString().Replace(",", string.Empty);
                    GP.ImportRow(dr);
                }
                else
                {
                    foreach (DataRow dr1 in Goals_Dt.Rows)
                    {

                        if (dr["UID"].ToString() == dr1["UID"].ToString())
                        {

                            dr["RQ4 Full Name"] = dr["RQ4 Full Name"].ToString().Replace(",", string.Empty);
                            dr["GA Goal"] = dr1["GaGoal"].ToString();
                            dr["Upgrade Equipment Goal"] = dr1["UpgradeGoal"].ToString();
                            GP.ImportRow(dr);

                        }
                    }
                }

                if (CNA != null)
                {
                    

                        numberOfRecordsCNA = CNA.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TwogbCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "2 GB")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TenGbCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "10 GB")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        UnlCoreCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "Unlimited Cricket Core")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        UnlMoreCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "Unlimited Cricket More")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TwentyGBSimplyDataCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "20GB Simply Data")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TwentyFiveGBCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "25GB")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        FortyGBSimplyDataCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "40GB Simply Data")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        HundredGBSimplyDataCountCNA = CNA.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "100GB Simply Data")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                    
                }

                if (Byod != null)
                {
                   
                        numberOfRecordsByod = Byod.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TwogbCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "2 GB")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TenGbCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "10 GB")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        UnlCoreCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "Unlimited Cricket Core")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        UnlMoreCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "Unlimited Cricket More")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TwentyGBSimplyDataCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "20GB Simply Data")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        TwentyFiveGBCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "25GB")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        FortyGBSimplyDataCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "40GB Simply Data")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                        HundredGBSimplyDataCountByod = Byod.AsEnumerable().Where(x => ((x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower()) && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower() && (x["Rate Plan"].ToString() == "100GB Simply Data")).Sum(x => Convert.ToInt32(x["Qty"].ToString()));

                   

                }

                if (Working != null)
                {
                    numberofRecordsWrk = Working.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("GP without Spiff"))).ToString();
                    numberofRecordsWrkTS = Working.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Total Spiff"))).ToString();


                }

                if (Result != "PDRZ")
                {
                    if (two_gb_data != null)
                    {
                        numberOfRecordsTwoGbData = two_gb_data.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                    }
                    if (Cricket_Protect != null)
                    {

                        numberOfRecordsCktPrtct = Cricket_Protect.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                    }
                    if (Cricket_Protect_Plus != null)
                    {

                        numberOfRecordsCktPrtctPlus = Cricket_Protect_Plus.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                    }
                    if (UPG != null)
                    {
                        numberOfRecordsUPG = UPG.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));
                    }
                    if (Bp != null)
                    {
                        numberOfRecordsBP = Bp.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Quantity"].ToString()));

                    }
                    if (Ap != null)
                    {
                        numberOfRecordsAP = Ap.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Qty"].ToString()));

                    }
                    if (Acc != null)
                    {
                        numberofRecordsACC = Acc.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Net Profit "))).ToString();
                        listPriceACC = Acc.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("List Price"))).ToString();
                        totalDiscountACC = Acc.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Pricing Discounts"))).ToString();
                        Acc_CountACC = Acc.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToInt32(x["Quantity"])).ToString();
                        TotalSalesACC = Acc.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x["Net Sales"])).ToString();
                    }
                    if (Sims != null)
                    {

                        numberofRecordsSIMS = Sims.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Net Profit "))).ToString();

                    }
                    if (Fees != null)
                    {
                        //if ((dr["RQ4 Full Name"].ToString() == "430: MFK LLC - Troy" && dr["EmpID"].ToString() == "-1") || (dr["RQ4 Full Name"].ToString() == "430: MFK LLC - Troy" && dr["EmpID"].ToString() == "1913705")) {
                            numberofRecordsFEES = Fees.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Net Profit "))).ToString();
                            listPriceFEES = Fees.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("List Price"))).ToString();
                            totalDiscountFEES = Fees.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Pricing Discounts"))).ToString();
                        //}
                    }
                    if (CAF != null)
                    {

                        numberofRecordsCAF = CAF.AsEnumerable().Where(x => (x["Invoiced At"].ToString().ToLower()).Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && x["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower()).Sum(x => Convert.ToDouble(x.Field<string>("Net Profit "))).ToString();
                    }
                    if (ProgLeaseVendorRebate != null)
                    {
                        string value = (from pair in query where pair.GroupDescription.ToLower().Replace(",", string.Empty) == dr["RQ4 Full Name"].ToString().ToLower() && pair.GroupDescription.ToLower() == dr["EmpID"].ToString().ToLower() select pair.Values.FirstOrDefault()).FirstOrDefault().ToString();
                        numberofRecordsPVR = string.IsNullOrEmpty(value) ? "0" : value;
                    }
                }

                foreach (DataRow dr1 in GP.Rows)
                {
                    if (dr1["RQ4 Full Name"].ToString() == dr["RQ4 Full Name"].ToString() && dr1["EmpID"].ToString().ToLower() == dr["EmpID"].ToString().ToLower())
                    {
                        if (CNA != null)
                        {
                            dr1["Achieved New Activation"] = numberOfRecordsCNA;
                            dr1["2 GB Count"] = TwogbCountCNA;
                            dr1["10 GB Count"] = TenGbCountCNA;
                            dr1["Unlimited Cricket Core Count"] = UnlCoreCountCNA;
                            dr1["Unlimited Cricket More Count"] = UnlMoreCountCNA;
                            dr1["20GB Simply Data Count"] = TwentyGBSimplyDataCountCNA;
                            dr1["25GB Count"] = TwentyFiveGBCountCNA;
                            dr1["40GB Simply Data Count"] = FortyGBSimplyDataCountCNA;
                            dr1["100GB Simply Data Count"] = HundredGBSimplyDataCountCNA;
                            
                        }

                        if (Byod != null)
                        {
                            dr1["Achieved BYOD Activation"] = numberOfRecordsByod;
                            dr1["2 GB Count"] = Convert.ToInt32(dr1["2 GB Count"].ToString()) + TwogbCountByod;
                            dr1["10 GB Count"] = Convert.ToInt32(dr1["10 GB Count"].ToString()) + TenGbCountByod;
                            dr1["Unlimited Cricket Core Count"] = Convert.ToInt32(dr1["Unlimited Cricket Core Count"].ToString()) + UnlCoreCountByod;
                            dr1["Unlimited Cricket More Count"] = Convert.ToInt32(dr1["Unlimited Cricket More Count"].ToString()) + UnlMoreCountByod;
                            dr1["20GB Simply Data Count"] = Convert.ToInt32(dr1["20GB Simply Data Count"].ToString()) + TwentyGBSimplyDataCountByod;
                            dr1["25GB Count"] = Convert.ToInt32(dr1["25GB Count"].ToString()) + TwentyFiveGBCountByod;
                            dr1["40GB Simply Data Count"] = Convert.ToInt32(dr1["40GB Simply Data Count"].ToString()) + FortyGBSimplyDataCountByod;
                            dr1["100GB Simply Data Count"] = Convert.ToInt32(dr1["100GB Simply Data Count"].ToString()) + HundredGBSimplyDataCountByod;
                            
                        }

                        if (Working != null)
                        {
                            dr1["Adj Rebate GP (W/O SPIFF)"] = numberofRecordsWrk.ToString();
                            dr1["TOTAL SPIFF"] = numberofRecordsWrkTS.ToString();
                            
                        }

                        if (Result != "PDRZ")
                        {
                            if (two_gb_data != null)
                            {
                                dr1["2 GB Data"] = numberOfRecordsTwoGbData;
                                
                            }

                            if (Cricket_Protect != null)
                            {
                                dr1["Cricket Protect"] = numberOfRecordsCktPrtct;
                                
                            }

                            if (Cricket_Protect_Plus != null)
                            {
                                dr1["Cricket Protect Plus"] = numberOfRecordsCktPrtctPlus;
                                
                            }

                            if (UPG != null)
                            {
                                dr1["Achieved Upgrade Equipment"] = numberOfRecordsUPG;
                                
                            }

                            if (Bp != null)
                            {
                                dr1["Bill Pay Count"] = numberOfRecordsBP;
                                
                            }

                            if (Ap != null)
                            {
                                dr1["Auto Pay Count"] = numberOfRecordsAP;
                                
                            }

                            if (Acc != null)
                            {
                                dr1["Accessories GP"] = numberofRecordsACC.ToString();
                                dr1["Acc List Prices"] = listPriceACC.ToString();
                                dr1["Acc Total Discount"] = totalDiscountACC.ToString();
                                dr1["Accss Count"] = Acc_CountACC;
                                dr1["Accessories Sales"] = TotalSalesACC;
                                
                            }

                            if (Sims != null)
                            {
                                dr1["SIMS GP"] = numberofRecordsSIMS.ToString();
                                

                            }

                            if (Fees != null)
                            {
                                dr1["FEES GP"] = numberofRecordsFEES.ToString();
                                dr1["Fees List Prices"] = listPriceFEES.ToString();
                                dr1["Fees Total Discount"] = totalDiscountFEES.ToString();
                               
                            }

                            if (CAF != null)
                            {
                                double number = Convert.ToDouble(numberofRecordsCAF);
                                double row = Convert.ToDouble(dr1["TOTAL SPIFF"]);
                                row += number;
                                dr1["TOTAL SPIFF"] = row.ToString();
                                

                            }

                            if (ProgLeaseVendorRebate != null)
                            {
                                dr1["Total Application Funded"] = numberofRecordsPVR;
                                

                            }

                            if (Pdr_Dt_Full != null)
                            {
                                foreach (DataRow dr2 in CustomerTotalCount.Rows)
                                {
                                    if (dr1["RQ4 Full Name"].ToString().ToLower() == dr2["RQFullName"].ToString().ToLower().Replace(",", string.Empty) && dr1["EmpID"].ToString().ToLower() == dr2["EmpID"].ToString().ToLower())
                                    {
                                        dr1["MTD Customers Count"] = string.IsNullOrEmpty(dr2["Counts"].ToString()) ? "0" : dr2["Counts"].ToString();
                                        break;
                                    }

                                }

                                dr1["MTD Device Count"] = 0;
                                var value = (from pair in query1 where pair.GroupDescription.ToLower().Replace(",", string.Empty) == dr1["RQ4 Full Name"].ToString().ToLower() && pair.GroupDescription1.ToLower() == dr1["EmpID"].ToString().ToLower() select pair.Values).ToList();

                                foreach (var v in value)
                                {
                                    foreach (var va in v)
                                    {
                                        string b = va.ToString();

                                        if (Int32.Parse(b) != -1 && Int32.Parse(b) != 0 && Int32.Parse(b) != 1)
                                        {
                                            dr1["MTD Device Count"] = Int32.Parse(dr1["MTD Device Count"].ToString()) + Int32.Parse(b);
                                        }

                                    }

                                }
                            }
                        }

                        dr1["GA Achieved"] = (Convert.ToDouble(dr1["Achieved New Activation"].ToString()) + Convert.ToDouble(dr1["Achieved BYOD Activation"].ToString())).ToString();

                        dr1["OPPS Achieved"] = (Convert.ToDouble(dr1["Achieved Upgrade Equipment"].ToString()) + Convert.ToDouble(dr1["GA Achieved"].ToString())).ToString();

                        dr1["Total OPPS Goal"] = (Convert.ToDouble(dr1["GA Goal"].ToString()) + Convert.ToDouble(dr1["Upgrade Equipment Goal"].ToString())).ToString();

                        if (dr1["GA Goal"].ToString() != "0")
                        {
                            dr1["% of GA Achieved"] = ((Convert.ToDouble(dr1["GA Achieved"].ToString()) / Convert.ToDouble(dr1["GA Goal"].ToString())) * 100).ToString();
                        }
                        else
                        {
                            dr1["% of GA Achieved"] = 0.ToString();
                        }
                        if (dr1["Total OPPS Goal"].ToString() != "0")
                        {
                            dr1["% of OPPS Achieved"] = ((Convert.ToDouble(dr1["OPPS Achieved"].ToString()) / Convert.ToDouble(dr1["Total OPPS Goal"].ToString())) * 100).ToString();
                        }
                        else
                        {
                            dr1["% of OPPS Achieved"] = 0.ToString();
                        }

                        if (dr1["OPPS Achieved"].ToString() != "0")
                        {
                            dr1["APO Archived"] = (Convert.ToDouble(dr1["Accessories GP"].ToString()) / Convert.ToDouble(dr1["OPPS Achieved"].ToString())).ToString();
                        }
                        else
                        {
                            dr1["APO Archived"] = 0.ToString();
                        }
                        dr1["Total Achieved GP Without Spiff"] = (Convert.ToDouble(dr1["Adj Rebate GP (W/O SPIFF)"].ToString()) + Convert.ToDouble(dr1["Accessories GP"].ToString()) + Convert.ToDouble(dr1["FEES GP"].ToString())).ToString();

                        dr1["Total Spiff"] = dr1["TOTAL SPIFF"].ToString();

                        dr1["Total Achieved GP With Spiff"] = (Convert.ToDouble(dr1["Total Achieved GP Without Spiff"].ToString()) + Convert.ToDouble(dr1["Total Spiff"].ToString())).ToString();

                    }
                }

            }

            //DataStringGp dsg = new DataStringGp();
            //string data = dsg.DataString4(GP, GP.Rows.Count, date, CheckBackDateReport, Goals_Dt, Result, AddCol);
            string data = DataStringGp.BulkOperationReportData1(GP, AddCol);
            return data;
        }
        
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static DataTable GetALLStoreRQName()
        {
            try
            {


                DAL.DAL objDal = new DAL.DAL();
                objDal.ProcName = "GetALLStoreRQName";
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

        public static string ActivationValidation(DataTable Vrr_Dt)
        {
            string res = "";
            var ActivationValidation = Vrr_Dt.AsEnumerable().Where(x => (x.Field<string>("Product Name") == "New Activation" ||
                                    x.Field<string>("Product Name") == "Add a Line Activation" ||
                                    x.Field<string>("Product Name") == "Ported Activation") &&
                                    string.IsNullOrWhiteSpace(x.Field<string>("Rate Plan")));
            int NoRatePlanCount = ActivationValidation.Count();
            if (NoRatePlanCount > 0)
            {
                res = "Upload Failed due to Activations given with no Rate Plan";
            }

            return res;
        }

        public static string RQFullNameValidationVrr(DataTable Vrr_Dt)
        {
            string res = "";
            DataTable ALLStoreRQName = GetALLStoreRQName();
            var RQNameValidation = Vrr_Dt.AsEnumerable()
            .Where(x1 => !ALLStoreRQName.AsEnumerable().Any(x2 => x2.Field<string>("RQFullName").Trim().ToUpper() == x1.Field<string>("Invoiced At").Trim().ToUpper()))
             .Select(r => new
             {
                 InvoicedAt = r.Field<string>("Invoiced At")
             })
             .Distinct()
             .ToList();

            int NoRQNameCount = RQNameValidation.Count();
            if (NoRQNameCount > 0)
            {
                res = "Upload Failed. <br /> Following unrecognized RQ Full Names found in VRR.";
                int a = 1;
                foreach (var field in RQNameValidation)
                {
                    res += "<br />" + a.ToString() + ") " + field.InvoicedAt.ToString();
                    a++;
                }
            }

            return res;
        }

        public static string PdrCategoryValidation(DataTable Pdr_Dt)
        {
            string res = "";
            int AccValidation = Pdr_Dt.AsEnumerable().Where(x => (x.Field<string>("Category").Contains("Accessories"))).Count();
            int FeesValidation = Pdr_Dt.AsEnumerable().Where(x => (x.Field<string>("Category").Contains("Fees"))).Count();
            int BpValidation = Pdr_Dt.AsEnumerable().Where(x => (x.Field<string>("Category").Contains("Cricket Bill Pay"))).Count();

            if (AccValidation == 0)
            {
                res += "Upload Failed due to Accessories not exists in Category Column.";
            }
            if (FeesValidation == 0)
            {
                res += "<br />Upload Failed due to Fees not exists in Category Column.";
            }
            if (BpValidation == 0)
            {
                res += "<br /> Upload Failed due to Cricket Bill Pay not exists in Category Column.";
            }
            return res;
        }

        public static string RQFullNameValidationPdr(DataTable Pdr_Dt)
        {
            string res = "";
            DataTable ALLStoreRQName = GetALLStoreRQName();
            var RQNameValidation = Pdr_Dt.AsEnumerable()
            .Where(x1 => !ALLStoreRQName.AsEnumerable().Any(x2 => x2.Field<string>("RQFullName").Trim().ToUpper() == x1.Field<string>("Location Name").Trim().ToUpper()))
             .Select(r => new
             {
                 InvoicedAt = r.Field<string>("Location Name")
             })
             .Distinct()
             .ToList();

            int NoRQNameCount = RQNameValidation.Count();
            if (NoRQNameCount > 0)
            {
                res = "Upload Failed. <br /> Following unrecognized RQ Full Names found in PDR.";
                int a = 1;
                foreach (var field in RQNameValidation)
                {
                    res += "<br />" + a.ToString() + ") " + field.InvoicedAt.ToString();
                    a++;
                }
            }

            return res;
        }

    }
}