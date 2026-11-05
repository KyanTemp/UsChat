using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.UnitTest.Mocks
{
    public class MessageMock : IMessageDAL
    {

        public UserDTO userDTO = new();
        public bool returnvalue;

        public bool SendMessage(UserDTO sender, GroupDTO group, MessageDTO message)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("onbekende fout!");
            }
        }
    }
}
