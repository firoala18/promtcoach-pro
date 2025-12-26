// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectsWebApp.DataAccsess.Services.Interfaces;

namespace ProjectsWebApp.Areas.Identity.Pages.Account
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [AllowAnonymous]
    public class LockoutModel : PageModel
    {
        private readonly IContactEmailService _contactEmailService;

        /// <summary>
        /// True when the view is showing a manual/permanent lockout message
        /// instead of the standard temporary lockout.
        /// </summary>
        public bool IsManualLockout { get; set; }

        /// <summary>
        /// Contact email address that can be used to reach support.
        /// </summary>
        public string SupportEmail { get; set; }

        public LockoutModel(IContactEmailService contactEmailService)
        {
            _contactEmailService = contactEmailService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public void OnGet()
        {
            // Load configured contact email (fallback if missing)
            try
            {
                SupportEmail = _contactEmailService?.GetEmail() ?? "kontakt@example.com";
            }
            catch
            {
                SupportEmail = "kontakt@example.com";
            }

            // Optional flag: treat ?manual=true as a manual/permanent lock indicator
            var manual = Request?.Query["manual"].ToString();
            IsManualLockout = !string.IsNullOrEmpty(manual) && manual.Trim().ToLowerInvariant() == "true";
        }
    }
}
