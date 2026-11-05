using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.DTO.Interfaces
{
    public interface IGroupDAL
    {
        public List<MessageDTO> GetGroupMessages(int GroupId);
        public List<GroupDTO> GetGroupsByUserId(int UserId);
        public bool CreateNewGroup(GroupDTO groupDTO);
        public bool DeleteGroup(GroupDTO groupDTO);
        public bool IsGroupOpen(GroupDTO groupDTO);
        public GroupDTO GetGroupByGroupId(int GroupId);
        public bool AddUserToGroup(int groupId, int userId);
    }
}
