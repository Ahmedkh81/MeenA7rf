using DataAccess.Repositories;
using DataAccess.Repositories.IRepositories;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Utility.DBInitializer;

namespace MeenA7rf.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IApplicationUserOTPRepository _applicationUserOTPRepository;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IApplicationUserOTPRepository applicationUserOTPRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser user = new()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Player);

                TempData["success-notification"] = "Registration successful! Please confirm your email address.";

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEmail), "Account",
                    new
                    {
                        userId = user.Id,
                        Token = token,
                        area = "Identity"
                    }, Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user!.Email ?? "",
                    "Confirm Your MeenA7rf Account",
                    $@"<h2>Welcome to MeenA7rf – Football Trivia!</h2>
                       <p>Dear {user.UserName},</p>
                       <p>Thank you for signing up to <b>MeenA7rf</b>, the ultimate football trivia challenge 🎉.</p>
                       <p>To start playing and testing your football knowledge, please confirm your email by clicking the link below:</p>
                       <p><a href='{link}' style='color:#1a73e8;'>Confirm My Account</a></p>
                       <p>If you didn’t sign up for MeenA7rf, please ignore this message.</p>
                       <p>Best regards,<br/>The MeenA7rf Team ⚽</p>"
                );

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);

            if (user is not null)
            {
                var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

                if (result)
                {
                    if (!user.EmailConfirmed)
                    {
                        TempData["error-notification"] = "Please confirm your email address before logging in.";
                        ViewBag.ShowResendButton = true;
                        return View(loginVM);
                    }

                    if (!user.LockoutEnabled)
                    {
                        TempData["error-notification"] = $"You have a block till {user.LockoutEnd}";
                        return View(loginVM);
                    }

                    await _signInManager.SignInAsync(user, loginVM.RememberMe);
                    TempData["success-notification"] = "Login Successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
            }

            ModelState.AddModelError("UserNameOrEmail", "Invalid UserName Or Email");
            ModelState.AddModelError("Password", "Invalid Password");

            return View(loginVM);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                    TempData["success-notification"] = "Email confirmed successfully! You can now log in.";
                else
                    TempData["error-notification"] = "Email confirmation failed. Please try again.";

                return RedirectToAction("index", "Home", new { area = "Customer" });
            }
            return NotFound();
        }

        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            TempData["success-notification"] = "Logout Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);
            }

            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail);

            if (user is not null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEmail), "Account",
                    new
                    {
                        userId = user.Id,
                        Token = token,
                        area = "Identity"
                    }, Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user!.Email ?? "",
                    "Reconfirm Your MeenA7rf Account",
                    $@"<h2>We Miss You at MeenA7rf!</h2>
                       <p>Dear {user.UserName},</p>
                       <p>It seems like your account hasn’t been confirmed yet. To continue enjoying <b>MeenA7rf – Football Trivia</b>, please reconfirm your email by clicking below:</p>
                       <p><a href='{link}' style='color:#1a73e8;'>Reconfirm My Account</a></p>
                       <p>If you already confirmed your account, you can safely ignore this message.</p>
                       <p>Best regards,<br/>The MeenA7rf Team ⚽</p>"
                );

                TempData["success-notification"] = "Email Sent Successfully";

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            return View(resendEmailConfirmationVM);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordVM);
            }

            var user = await _userManager.FindByNameAsync(forgotPasswordVM.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(forgotPasswordVM.UserNameOrEmail);

            if (user is not null)
            {
                var otp = new Random().Next(100000, 999999);

                var totalOTPs = (await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == user.Id
                && DateTime.UtcNow.Day == e.SendDate.Day));

                if (totalOTPs.Count() > 5)
                {
                    TempData["error-notification"] = "Many Requests of OTPs";
                    return View(forgotPasswordVM);
                }

                await _applicationUserOTPRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    OTPNumber = otp,
                    SendDate = DateTime.UtcNow,
                    Status = false,
                    ValidTo = DateTime.UtcNow.AddMinutes(30)
                });

                await _emailSender.SendEmailAsync(
                    user!.Email ?? "",
                    "Reset Your Password – MeenA7rf",
                    $@"<h2>Password Reset Request</h2>
                       <p>Dear {user.UserName},</p>
                       <p>We received a request to reset your password for <b>MeenA7rf – Football Trivia</b>.</p>
                       <p>Use the following One-Time Password (OTP) to reset your account password:</p>
                       <h3 style='color:#d93025;'>{otp}</h3>
                       <p>If you didn’t request this, please ignore this email.</p>
                       <p>Stay sharp and keep testing your football knowledge! ⚽</p>
                       <p>Best regards,<br/>The MeenA7rf Team</p>"
                );

                TempData["success-notification"] = "OTP Sent to your Email Successfully";
                return RedirectToAction("ResetPassword", "Account", new { area = "Identity", userId = user.Id });
            }
            return View(forgotPasswordVM);
        }

        public async Task<IActionResult> ResetPassword(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                return View(new ResetPasswordVM()
                {
                    UserId = userId
                });
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }

            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);

            if (user is not null)
            {
                var lastOTP = (await _applicationUserOTPRepository.GetAsync(e => e.ApplicationUserId == resetPasswordVM.UserId))
                    .OrderBy(e=>e.Id).LastOrDefault();

                if (lastOTP is not null)
                {
                    if (lastOTP.OTPNumber == resetPasswordVM.OTP && (lastOTP.ValidTo - DateTime.UtcNow).TotalMinutes < 30 && !lastOTP.Status)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordVM.Password);

                        if (result.Succeeded)
                            TempData["success-notification"] = "Reset Password Successfully";
                        else
                            TempData["error-notification"] = $"{String.Join(",", result.Errors)}";

                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                }
                TempData["error-notification"] = "Invalid OR Expired OTP";
                return View(resetPasswordVM);
            }
            return NotFound();
        }
    }
}
