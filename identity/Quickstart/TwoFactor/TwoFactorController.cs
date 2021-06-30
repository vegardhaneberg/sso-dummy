using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using IdentityServer4.Test;
using Google.Authenticator;
using System.Security.Claims;
using IdentityModel;
using System.Collections.Generic;

namespace identity.Quickstart.UI
{
    [AllowAnonymous]
    public class TwoFactorController : Controller 
    {
        private TestUser _user;

        public TwoFactorController()
        {
            _user = new TestUser{
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "scott",
                Password = "password",
                Claims = new List<Claim> {
                    new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                    new Claim(JwtClaimTypes.Role, "admin")
                }
            };
        }

        [HttpGet]
        public IActionResult TwoFactor(string ReturnUrl)
        {
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();

            var user = HttpContext.User;
            var name = user.Identity.AuthenticationType;
            Console.WriteLine(name);
            var setupInfo = twoFactor.GenerateSetupCode("myapp", _user.Username, "SuperSecretKey", false, 3);
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;

            FormModel modelToSend = new FormModel();
            modelToSend.ReturnUrl = ReturnUrl;

            return View(modelToSend);
        }

        [HttpPost]
        public ActionResult TwoFactor(FormModel formModel)
        {
            Console.WriteLine("Calling on send post");
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool validCode = twoFactor.ValidateTwoFactorPIN("SuperSecretKey", formModel.InputCode);
            if (validCode)
            {   
                return Redirect(formModel.ReturnUrl);
            }
            Console.WriteLine("In TwoFactorController.cs: " + "Invalid 2FA Code");
            FormModel wrongFormModel = new FormModel();
            wrongFormModel.ReturnUrl = formModel.ReturnUrl;
            return View(wrongFormModel);
        }
    }
}
