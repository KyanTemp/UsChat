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
    public class UserCRUDMock : IUserCRUDDAL
    {
        public readonly UserDTO user1 = new(1,"John","JohnDoe1!","Johndoe@example.com");

        public UserDTO userDTO = new();
        public bool returnvalue;
        public UserDTO GetUserById(int id)
        {
            if (returnvalue)
            {
                return user1; // Valid user
            }
            else
            {
                throw new UserNotFoundException("Gebruiker niet gevonden");
            }
        }
        public bool Create(UserDTO user)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UserAlreadyExistsException("gebruiker bestaat al");
            }
        }
        public bool Delete(int id)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                if (id != user1.Id)
                {
                    throw new UserNotFoundException("Gebruiker niet gevonden");
                }
                else
                {
                    throw new UnknownException("Onbekende fout");
                }
            }
        }
        public UserDTO Edit(UserDTO user)
        {
            if (returnvalue)
            {
                return user;
            }
            else
            {
                if (user.Email == user1.Email)
                {
                    throw new UserAlreadyExistsException("gebruiker bestaat al");
                }
                else if (user.Id != user1.Id)
                {
                    throw new UserNotFoundException("Gebruiker niet gevonden");
                }
                else
                {
                    throw new UnknownException("Onbekende fout");
                }
            }
        }
        public UserDTO GetUserByEmailAndPassword(string email, string password)
        {
            if (returnvalue)
            {
                return user1; // Valid user
            }
            else
            {
                throw new UserNotFoundException("Gebruiker niet gevonden");
            }
        }
        public UserDTO Login(UserDTO user)
        {
            return user1;
        }
    }
}
