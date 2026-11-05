using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Models;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.BLL.Container
{
    public class UserCRUDContainer
    {
        //readonly means that the value can only be assigned 1 times
        private readonly IUserCRUDDAL userCRUDDAL;
        public UserCRUDContainer(IUserCRUDDAL userCRUDDAL)
        {
            this.userCRUDDAL = userCRUDDAL ?? throw new ArgumentNullException(nameof(userCRUDDAL));
        }
        public Result Create(User user)
        {
            try
            {
                this.userCRUDDAL.Create(user.ToDTO());
                return Result.Success();
            }
            catch (UserAlreadyExistsException ex)
            {
                return Result.Failure(ex.Message.ToString());
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
        public Result Delete(int id)
        {
            try
            {
                this.userCRUDDAL.Delete(id);
                return Result.Success();
            }
            catch (UserNotFoundException ex)
            {
                return Result.Failure(ex.Message.ToString());
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
        public Result<User> Edit(User user)
        {
            try
            {
                User edituser = new(this.userCRUDDAL.Edit(user.ToDTO()));
                return Result<User>.Success(edituser);
            }
            catch (UserAlreadyExistsException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (UserNotFoundException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (SqlExceptions ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }

        }
        public Result<User> GetUserById(int Id)
        {
            try
            {
                var user = new User(this.userCRUDDAL.GetUserById(Id));
                return Result<User>.Success(user);
            }
            catch (UserNotFoundException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (SqlExceptions ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
        }
        public Result<User> GetUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var user = new User(this.userCRUDDAL.GetUserByEmailAndPassword(email, password));
                return Result<User>.Success(user);
            }
            catch (UserNotFoundException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (SqlExceptions ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<User>.Failure(ex.Message.ToString());
            }
        }
    }
}
