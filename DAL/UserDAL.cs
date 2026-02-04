using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using BulkUploader.DAL;
using BulkUploader.Models;


public class UserDAL : DAL
{
    public static UserModel Login(string Email, string passwordHash)
    {
        using (SqlConnection cn = new SqlConnection(strconnectionstring))
        using (SqlCommand cmd = new SqlCommand("USP_USER_LOGIN", cn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = Email;
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;

            cn.Open();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    return new UserModel
                    {
                        UserId = Convert.ToInt32(dr["UserId"]),
                        Username = dr["Username"].ToString(),
                        Email = dr["Email"].ToString(),
                        Password = dr["PasswordHash"].ToString()
                    };
                }
            }
        }
        return null; // Invalid login
    }


    public static bool Register(UserModel model, string passwordHash)
    {
        try
        {
            using (SqlConnection cn = new SqlConnection(strconnectionstring))
            using (SqlCommand cmd = new SqlCommand("USP_USER_REGISTER", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = model.Username;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = model.Email;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;

                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }
        catch (SqlException)
        {
            return false; // duplicate username/email
        }
    }

}
