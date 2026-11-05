using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.UnitTest.Mocks
{
    public class UserRelationMock : IUserRelationDAL
    {
        public readonly UserDTO user1 = new(1, "John", "JohnDoe1!", "Johndoe@example.com");
        public readonly UserDTO user2 = new(1, "Jane", "JaneDoe2!", "Janedoe@example.com");
        public Relation relation1 => new( new User(1, "Jane", "JaneDoe2!", "Janedoe@example.com"), 1);

        public bool returnvalue;
        public bool AddRelation(UserDTO user, RelationDTO relation)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("onbekende fout.");
            }
        }

        public bool BlockRelation(UserDTO user, RelationDTO relation)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("onbekende fout.");
            }
        }

        public List<UserDTO> GetFriendsWithoutChat(int userId)
        {
            if (returnvalue)
            {
                List<UserDTO> users = new();
                users.Add(user2);
                return users;
            }
            else
            {
                throw new SqlExceptions("onbekende fout");
            }
        }

        public bool RemoveRelation(UserDTO user, RelationDTO relation)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("onbekende fout.");
            }
        }

        public List<UserDTO> SearchUsers(string query, int userId)
        {
            if (returnvalue)
            {
                List<UserDTO> users = new();
                users.Add(user1);
                return users;
            }
            else
            {
                throw new SqlExceptions("onbekende fout");
            }
        }
    }
}
