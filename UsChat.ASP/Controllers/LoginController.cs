using Microsoft.AspNetCore.Mvc;
using UsChat.ASP.Models.User;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.ASP.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserCRUDContainer userContainer;
        private readonly EmailVerificationContainer emailVerificationContainer;

        public LoginController(IUserCRUDDAL userInterface, IEmailVerificationDAL EmailInterface)
        {
            var emailSender = new EmailSenderContainer();
            userContainer = new UserCRUDContainer(userInterface);
            emailVerificationContainer = new EmailVerificationContainer(EmailInterface, emailSender);
        }
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User U = new(model.Username, model.Password, model.Email);
                Result result = userContainer.Create(U);
                if (result.IsSuccess)
                {
                    Result<User> user = userContainer.GetUserByEmailAndPassword(U.Email, U.Password);
                    if (user.IsSuccess)
                    {
                        emailVerificationContainer.GenerateAndSendVerificationEmail(user.Data);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = result.ErrorMessage;
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = result.ErrorMessage;
                    return View();
                }
            }
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(UserViewModel model)
        {
            Result<User> user = null;
            user = userContainer.GetUserByEmailAndPassword(model.Email, model.Password);
            if (user.IsSuccess)
            {
                Result<bool> result = emailVerificationContainer.IsUserVerified(user.Data.ToDTO());
                if (!result.IsSuccess)
                {
                    emailVerificationContainer.GenerateAndSendVerificationEmail(user.Data);
                    ViewBag.ErrorMessage = result.ErrorMessage.ToString();
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = user.ErrorMessage.ToString();
                return View();
            }
            HttpContext.Session.SetInt32("UserId", user.Data.Id);

            // Redirect to the chat
            return RedirectToAction("Chatpage", "Chat");
        }
        [HttpGet]
        [Route("login/verify")]
        public IActionResult VerifyEmail(string token)
        {
            if (!emailVerificationContainer.ValidateToken(token).IsSuccess)
            {
                return View("VerificationFailed"); // View for when the token is invalid or missing
            }

            var emailToken = emailVerificationContainer.GetToken(token); // Get the full token information
            if (!emailToken.IsSuccess)
            {
                ViewBag.ErrorMessage = emailToken.ErrorMessage;
                return View();
            }
            if (emailToken.Data == null || emailToken.Data.expiryDate < DateTime.UtcNow)
            {
                return View("VerificationFailed"); // View for when the token is expired or invalid
            }
            // Mark user as verified
            emailVerificationContainer.ValidateToken(token);

            return View("VerificationSuccess"); // View for when the verification is successful
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}

