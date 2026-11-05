using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.DTO.Interfaces
{
    public interface IMessageDAL
    {
        public bool SendMessage(UserDTO sender, GroupDTO group, MessageDTO message);
    }
}
