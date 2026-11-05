using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using UsChat.ASP.Models.User;
using UsChat.BLL.Container;
using UsChat.DTO.Interfaces;
using System.Reflection;
using UsChat.BLL.Models;

namespace UsChat.ASP.Controllers
{
    public class UserController : Controller
    {
        private readonly UserCRUDContainer userContainer;
        private readonly EmailVerificationContainer emailVerificationContainer;

        public UserController(IUserCRUDDAL userInterface, IEmailVerificationDAL EmailInterface)
        {
            var emailSender = new EmailSenderContainer();
            userContainer = new UserCRUDContainer(userInterface);
            emailVerificationContainer = new EmailVerificationContainer(EmailInterface, emailSender);
        }
        [HttpGet]
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                // Redirect to login page if the UserId is not found in session
                return RedirectToAction("Index", "Login");
            }

            var result = userContainer.GetUserById(userId.Value);

            if (result.IsSuccess)
            {
                var model = new UserViewModel
                {
                    
                    UserId = result.Data.Id,
                    Username = result.Data.Name,
                    Email = result.Data.Email,
                    Password = result.Data.Password
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public IActionResult Edit()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                // Redirect to login page if the UserId is not found in session
                return RedirectToAction("Index", "Login");
            }
            
            var result = userContainer.GetUserById(userId.Value);
            if (result.IsSuccess)
            {
                var model = new EditViewModel
                {
                    UserId = result.Data.Id,
                    Username = result.Data.Name,
                    Email = result.Data.Email,
                    Password = result.Data.Password,
                    NewPassword = "",
                    VerifyPassword = ""
                };
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = result.ErrorMessage;
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(EditViewModel edit)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                // Redirect to login if session is missing
                return RedirectToAction("Login", "Account");
            }
            
            var user = userContainer.GetUserById(userId.Value);
            if (!user.IsSuccess)
            {
                ViewBag.ErrorMessage = user.ErrorMessage;
            }
            if (string.IsNullOrEmpty(edit.NewPassword))
            {
                // Update the username only
                user.Data.Name = edit.Username;
                if (userContainer.Edit(user.Data).IsSuccess)
                {
                    return RedirectToAction("Logout","Login");
                }
                else
                {
                    ViewBag.ErrorMessage = user.ErrorMessage;
                    return View(edit);
                }
            }
            else
            {
                // Verify the old password matches
                if (user.Data.Password != edit.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                }

                // Check if the new password matches the verification password
                if (edit.NewPassword != edit.VerifyPassword)
                {
                    ModelState.AddModelError("VerifyPassword", "New passwords do not match.");
                }
                // If there are any errors, return the view with the current model
                if (!ModelState.IsValid)
                {
                    return View(edit);
                }
                // Update the username and password
                user.Data.Name = edit.Username;
                user.Data.Password = edit.NewPassword;
                if (userContainer.Edit(user.Data).IsSuccess)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else 
                { 
                    ViewBag.ErrorMessage = user.ErrorMessage; 
                    return View(edit); 
                }
            }
        }
        [HttpGet]
        public IActionResult Delete()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                if (userContainer.Delete((int)userId).IsSuccess)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.ErrorMessage = "Gebruiker kon niet verwijderd worden";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }


        }
        
    }
}
