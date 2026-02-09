using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using BulkUploader.Models;
using BulkUploader.DAL;

public class LoginController : Controller
{
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(UserModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string hash = HashPassword(model.Password);

        UserModel user = UserDAL.Login(model.Email, hash);

        if (user != null)
        {
            Session["Email"] = user.Email;
            Session["Password"] = user.PasswordHash;
            TempData["LoginSts"] = 1;

            TempData["Success"] = "Login successful";
            return RedirectToAction("Index", "Home");
        }
        TempData["Error"] = "Invalid username or password";
        return View(model);
    }


    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(UserModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string hash = HashPassword(model.Password);

        bool isRegistered = UserDAL.Register(model, hash);

        if (isRegistered)
        {
            TempData["Success"] = "Registration successful";
            return RedirectToAction("Login");
        }
        else
        {
            TempData["Error"] = "Username or Email already exists.";
            return View(model);
        }
    }


    public ActionResult Logout()
    {
        Session.Clear();
        TempData["Success"] = "Logout successful";
        return RedirectToAction("Login");
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha = SHA256.Create())
        {
            return System.Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(password))
            );
        }
    }



    public ActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public ActionResult ForgotPassword(string email)
    {
        // check email in DB
        ViewBag.Message = "If this email exists, a reset link has been sent.";
        return View();
    }


}
