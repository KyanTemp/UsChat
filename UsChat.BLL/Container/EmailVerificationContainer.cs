using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.BLL.Container
{
    public class EmailVerificationContainer
    {
        private readonly IEmailVerificationDAL verificationDAL;
        private readonly EmailSenderContainer emailSender;

        public EmailVerificationContainer(IEmailVerificationDAL verificationDAL, EmailSenderContainer emailSender)
        {
            this.verificationDAL = verificationDAL;
            this.emailSender = emailSender;
        }
        public Result GenerateAndSendVerificationEmail(User user)
        {
            try
            {
                //Guid: globaly unieke Identifeir
                string token = Guid.NewGuid().ToString();
                var emailToken = new EmailVerificationToken(user.Id, token, DateTime.UtcNow.AddMinutes(30));
                verificationDAL.SaveToken(emailToken.ToDTO());

                // Send verification email
                string verificationLink = $"https://localhost:7238/login/verify?token={token}";
                emailSender.SendEmail(user.Email, "Verify Your Email", $"Click here to verify your email: {verificationLink}");
                return Result.Success();
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.Message.ToString());
            }
            catch (UnknownException ex)
            {
                return Result.Failure(ex.Message.ToString());
            }
        }
        public Result<bool> ValidateToken(string token)
        {
            try
            {
                var emailToken = verificationDAL.GetToken(token);
                if (emailToken == null || emailToken.expiryDate < DateTime.UtcNow)
                {
                    return Result<bool>.Success(false);
                }
                // Mark user as verified
                verificationDAL.MarkUserAsVerified(emailToken.userId);
                return Result<bool>.Success(true);
            }
            catch (SqlExceptions ex)
            {
                return Result<bool>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<bool>.Failure(ex.ToString());
            }
        }
        public Result<EmailVerificationTokenDTO> GetToken(string token)
        {
            try
            {
                return Result<EmailVerificationTokenDTO>.Success(verificationDAL.GetToken(token));
            }
            catch (TokenNotFoundException ex)
            {
                throw;
            }
            catch (SqlExceptions ex)
            {
                return Result<EmailVerificationTokenDTO>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<EmailVerificationTokenDTO>.Failure(ex.ToString());
            }

        }
        public Result<bool> IsUserVerified(UserDTO userDTO)
        {
            try
            {
                return Result<bool>.Success(verificationDAL.IsUserVerified(userDTO));
            }
            catch (UserNotVerifiedException ex)
            {
                return Result<bool>.Failure(ex.Message.ToString());
            }
            catch (SqlExceptions ex)
            {
                return Result<bool>.Failure(ex.Message.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<bool>.Failure(ex.Message.ToString());
            }
        }
    }
}
