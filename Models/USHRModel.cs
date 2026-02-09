using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkUploader.Models
{
    public class USHRModel
    {
        public HttpPostedFileBase File { get; set; }
        public string Company { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string SalaryPayRate { get; set; }
        public string LastPayChange { get; set; }
        public string AvgMarketRate { get; set; }
    }
}