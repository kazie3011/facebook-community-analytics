using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace FacebookCommunityAnalytics.Api.Pages.Account
{
    public class CustomRegisterModel : Volo.Abp.Account.Public.Web.Pages.Account.RegisterModel
    {
        private IdentityUserManager _identityUserManager;

        [BindProperty]
        public PostInputExtra InputExtra { get; set; }

        public CustomRegisterModel(IAccountAppService accountAppService, IdentityUserManager identityUserManager)
        {
            _identityUserManager = identityUserManager;
        }

        public override Task<IActionResult> OnGetAsync()
        {
            return base.OnGetAsync();
        }

        public override async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await CheckSelfRegistrationAsync();

                if (IsExternalLogin)
                {
                    var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        Logger.LogWarning("External login info is not available");
                        return RedirectToPage("./Login");
                    }

                    await RegisterExternalUserAsync(externalLoginInfo, Input.EmailAddress);
                }
                else
                {
                    var result = await RegisterLocalUserAsync();
                    if (result.Id == Guid.Empty)
                    {
                        return Page();
                    }
                }

                return Redirect(ReturnUrl ?? "~/"); //TODO: How to ensure safety? IdentityServer requires it however it should be checked somehow!
            }
            catch (BusinessException e)
            {
                Alerts.Danger(GetLocalizeExceptionMessage(e));
                return Page();
            }
        }

        protected override async Task<IdentityUser> RegisterLocalUserAsync()
        {
            ValidateModel();

            var identityUser = new IdentityUser(Guid.Empty, Input.UserName, Input.EmailAddress)
            {
                Name = InputExtra.Name,
                Surname = InputExtra.Surname,
            };
            identityUser.SetPhoneNumber(InputExtra.PhoneNumber, false);

            var result = await _identityUserManager.CreateAsync(identityUser, Input.Password);
            if (result.Succeeded)
            {
                var user = await UserManager.GetByIdAsync(identityUser.Id);
                await SignInManager.SignInAsync(user, isPersistent: true);
                return user;
            }

            await HandleRegisterError(result);

            return identityUser;
        }

        private Task HandleRegisterError(IdentityResult result)
        {
            if (result.Errors.Any())
            {
                if (result.Errors.Any(_ => _.Code.Contains("Password")))
                {
                    var errorMessage = string.Join("\n",
                        result.Errors.Where(_ => _.Code.Contains("Password")).Select(_ => _.Description));
                    ModelState.AddModelError("Input.Password", errorMessage);
                }

                if (result.Errors.Any(_ => _.Code.Contains("Email")))
                {
                    var errorMessage = string.Join("\n",
                        result.Errors.Where(_ => _.Code.Contains("Email")).Select(_ => _.Description));
                    ModelState.AddModelError("Input.EmailAddress", errorMessage);
                }

                if (result.Errors.Any(_ => _.Code.Contains("UserName")))
                {
                    var errorMessage = string.Join("\n",
                        result.Errors.Where(_ => _.Code.Contains("UserName")).Select(_ => _.Description));
                    ModelState.AddModelError("Input.UserName", errorMessage);
                }

                if (result.Errors.Any(_ => _.Code.Contains("PhoneNumber")))
                {
                    var errorMessage = string.Join("\n",
                        result.Errors.Where(_ => _.Code.Contains("PhoneNumber")).Select(_ => _.Description));
                    ModelState.AddModelError("InputExtra.PhoneNumber", errorMessage);
                }

            }
            return Task.CompletedTask;
        }
    }

    public class PostInputExtra
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }

        //[RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter valid phone no.")]
        public string PhoneNumber { get; set; }
    }
}
