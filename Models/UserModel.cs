using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkUploader.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }   // plain (input only)

        public string PasswordHash { get; set; }
    }

}