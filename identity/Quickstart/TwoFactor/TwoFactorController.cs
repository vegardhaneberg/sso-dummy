using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Google.Authenticator;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using identity.Quickstart.UI;
using System.ComponentModel.DataAnnotations;

namespace identity.Quickstart.UI
{
    [AllowAnonymous]
    public class TwoFactorController : Controller 
    {
        public TwoFactorController()
        {
        }
        [HttpGet]
        public IActionResult TwoFactor(string ReturnUrl)
        {
            var user = HttpContext.User;
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo = twoFactor.GenerateSetupCode("Beest APP", "andresa.dybvik.kaasen@bredvid.no", "SuperSecretKey", false, 3);
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;

            var model = new TwoFactorModel();
            model.ReturnUrl = ReturnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(TwoFactorModel model)
        {
            var user = HttpContext.User;

            Console.WriteLine("Two Factor Autentication in progress");
            Console.WriteLine("Model code: " + model.InputCode);
            Console.WriteLine("ReturnUrl: " + model.ReturnUrl);
            Console.WriteLine("User claims: " + user.Identity.Name);

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN("SuperSecretKey", model.InputCode);
            if (!isValid)
            {
                //TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                var setupInfo = twoFactor.GenerateSetupCode("Best APP", "andresa.dybvik.kaasen@bredvid.no", "SuperSecretKey", false, 3);
                ViewBag.SetupCode = setupInfo.ManualEntryKey;
                ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
                return View();
            }

            // TODO: store the updated user in database
            
            
            //await HttpContext.SignInAsync(isuser, props);
            
            // request for a local page
            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else if (string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect("~/");
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
        }
    }

     public class TwoFactorModel
    {
        [Required]
        public string InputCode { get; set; }
        public string ReturnUrl { get; set; }

    }
}