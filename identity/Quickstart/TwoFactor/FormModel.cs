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
    public class FormModel
    {
        [BindProperty]
        public string InputCode {get; set;}
        [BindProperty]
        public string ReturnUrl{get; set;}

        public override string ToString()
        {
            return InputCode;
        }
    }
}