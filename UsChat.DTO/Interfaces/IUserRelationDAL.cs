using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.DTO.Interfaces
{
    public interface IUserRelationDAL
    {
        public bool AddRelation(UserDTO user, RelationDTO relation);
        public bool RemoveRelation(UserDTO user, RelationDTO relation);
        public bool BlockRelation(UserDTO user, RelationDTO relation);
        public List<UserDTO> AddToGroupSearch(string query, int userId);
        public List<UserDTO> SearchUsers(string query, int userId);
        public List<UserDTO> GetFriendsWithoutChat(int userId);


    }
}
