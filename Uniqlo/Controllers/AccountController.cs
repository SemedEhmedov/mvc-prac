﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uniqlo.Abstrcations.EmailServices;
using Uniqlo.DAL;
using Uniqlo.Helpers.Email;
using Uniqlo.Helpers.Enums;
using Uniqlo.Models;
using Uniqlo.ViewModels;

namespace Uniqlo.Controllers
{
    public class AccountController : Controller
    {
            AppDbContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IMailService _mailService;
            public AccountController(AppDbContext appDBContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IMailService mailService)
            {
                _context = appDBContext;
                _userManager = userManager;
                _signInManager = signInManager;
                _roleManager = roleManager;
                _mailService = mailService;
            }



            public IActionResult Register()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Register(RegisterVm vm)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                JsonResult jsonResult = new JsonResult(vm);
                AppUser User = new AppUser();
                {
                    User.UserName = vm.UserName;
                    User.Email = vm.Email;
                }
                var result = await _userManager.CreateAsync(User, vm.Password);
                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View();
                }
                await _userManager.AddToRoleAsync(User, UserRoles.Member.ToString());


                return RedirectToAction(nameof(Login));
            }
            public async Task<IActionResult> SignOut()
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction("Index", "Home");
            }

            public IActionResult Login()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> Login(LoginVM loginVM, string? ReturnUrl)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                AppUser User = await _userManager.FindByEmailAsync(loginVM.EmailorUserName)
                    ?? await _userManager.FindByNameAsync(loginVM.EmailorUserName);
                if (User == null)
                {
                    ModelState.AddModelError("", "Giris melumatlariniz sehvdir");
                    return View();
                }
                var result = await _signInManager.CheckPasswordSignInAsync(User, loginVM.Password, true);
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "banlisiniz");
                    return View();
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Giris melumatlariniz sehvdir");
                    return View();
                }
                await _signInManager.SignInAsync(User, loginVM.Remember);
                if (ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            public async Task<IActionResult> CreateRole()
            {
                foreach (var item in Enum.GetValues(typeof(UserRoles)))
                {
                    await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = item.ToString()
                    });
                }
                return RedirectToAction("Index", "Home");
            }
            public IActionResult ForgetPassword()
            {
                return View();
            }
            [HttpPost]
            public async Task<IActionResult> ForgetPassword(ForgetPasswordVM vm)
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                AppUser User = await _userManager.FindByEmailAsync(vm.Email);
                if (User == null)
                {
                    return NotFound();
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(User);
                object userstat = new
                {
                    userId = User.Id,
                    token = token
                };
                var link = Url.Action("ResetPassword", "Register", userstat, HttpContext.Request.Scheme);
                MailRequest mailRequest = new MailRequest()
                {
                    ToEmail = vm.Email,
                    Subject = "reset password",
                    Body = $"<a href ='{link}'> Reset Password<a/>"
                };
                await _mailService.SendEmailAsync(mailRequest);
                return RedirectToAction(nameof(Login));
            }
            public IActionResult ResetPassword(string userid, string token)
            {
                if (userid == null)
                {
                    return BadRequest();
                }
                ResetPasswordVM resetPasswordVM = new ResetPasswordVM()
                {
                    userid = userid,
                    token = token
                };

                return View(resetPasswordVM);
            }
            [HttpPost]
            public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }
                var user = await _userManager.FindByIdAsync(vm.userid);
                if (user == null)
                {
                    return NotFound();
                }
                var result = await _userManager.ResetPasswordAsync(user, vm.token, vm.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(vm);
                }

                return RedirectToAction(nameof(Login));
            }
        }
    }

