using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkUploader.Models
{
    public class Employee
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
    }
}