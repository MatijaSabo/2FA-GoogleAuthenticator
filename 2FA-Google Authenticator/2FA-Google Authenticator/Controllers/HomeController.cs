using _2FA_Google_Authenticator.ViewModel;
using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _2FA_Google_Authenticator.Controllers
{
    public class HomeController : Controller
    {
        private const string key = "SisProject"; //Private key for Google Authenticator

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            string message = "";
            bool status = false;

            //Checking username and password 
            if(login.Username == "Admin" && login.Password == "admin")
            {
                status = true;
                Session["username"] = login.Username;

                //"2FA Setup
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                //
                var SetupInfo = tfa.GenerateSetupCode("QR Code", login.Username, key, 300, 300);
                ViewBag.BarcodeImageUrl = SetupInfo.QrCodeSetupImageUrl;
                //ViewBag.SetupCode = SetupInfo.ManualEntryKey;
            }
            else
            {
                message = "Login failed";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;

            return View();
        }

        public ActionResult ProfilePage()
        {
            if(Session["username"] == null || Session["isValid2FA"] == null|| !(bool)Session["isValid2Fa"])
            {
                return RedirectToAction("Login");
            }

            ViewBag.Message = Session["username"].ToString();
            return View();
        }

        public ActionResult Verify2FA()
        {
            var token = Request["passcode"];
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool isValid = tfa.ValidateTwoFactorPIN(key, token);

            if (isValid)
            {
                Session["isValid2FA"] = true;
                return RedirectToAction("ProfilePage", "Home");
            }

            ViewBag.Message = "2FA Failed";
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Logout()
        {
            Session["username"] = null;
            Session["isValid2FA"] = false;
            ViewBag.Status = false;
            ViewBag.Message = "Logout success";
            return RedirectToAction("Login", "Home");
        }

    }
}