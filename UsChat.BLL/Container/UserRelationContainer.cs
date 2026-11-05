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
    public class UserRelationContainer
    {
        private readonly IUserRelationDAL userRelationDAL;
        public UserRelationContainer(IUserRelationDAL userRelationDAL)
        {
            this.userRelationDAL = userRelationDAL;
        }
        public Result AddRelation(User user, Relation relation)
        {
            try
            {
                userRelationDAL.AddRelation(user.ToDTO(), relation.ToDTO());
                return Result.Success();
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch(UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }
        }
        public Result RemoveRelation(User user, Relation relation)
        {
            try
            {
                userRelationDAL.RemoveRelation(user.ToDTO(), relation.ToDTO());
                return Result.Success();
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch(UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }

        }
        public Result BlockRelation(User user, Relation relation)
        {
            try
            {
                userRelationDAL.BlockRelation(user.ToDTO(), relation.ToDTO());
                return Result.Success();
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }

        }
        public Result<List<UserDTO>> AddToGroupSearch(string query, int userId)
        {
            try
            {
                var users = userRelationDAL.AddToGroupSearch(query, userId);
                return Result<List<UserDTO>>.Success(users);
            }
            catch (SqlExceptions ex)
            {
                return Result<List<UserDTO>>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<List<UserDTO>>.Failure(ex.ToString());
            }
        }
        public Result<List<UserDTO>> SearchUsers(string query, int userId)
        {
            try
            {
                var users = userRelationDAL.SearchUsers(query, userId);
                return Result<List<UserDTO>>.Success(users);
            }
            catch (SqlExceptions ex)
            {
                return Result<List<UserDTO>>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<List<UserDTO>>.Failure(ex.ToString());
            }

        }
        public Result<List<UserDTO>> GetFriendsWithoutChat(int userId)
        {
            try
            {
                var friends = userRelationDAL.GetFriendsWithoutChat(userId);
                return Result<List<UserDTO>>.Success(friends);
            }
            catch (SqlExceptions ex)
            {
                return Result<List<UserDTO>>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<List<UserDTO>>.Failure(ex.ToString());
            }

        }
    }
}
