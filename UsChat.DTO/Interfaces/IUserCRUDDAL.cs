using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.DTO.Interfaces
{
    public interface IUserCRUDDAL
    {
        public UserDTO GetUserById(int id);
        public UserDTO GetUserByEmailAndPassword(string email, string password);
        public bool Create(UserDTO user);
        public UserDTO Edit(UserDTO user);
        public bool Delete(int id);
    }
}
