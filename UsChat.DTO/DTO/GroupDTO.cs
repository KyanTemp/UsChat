using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.DTO
{
    public class GroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PersonalGroup { get; set; }
        public List<UserDTO> Participents { get; set; }
        public List<MessageDTO> Messages { get; set; }
        public List<UserDTO> Admins { get; set; }

        public GroupDTO() { }
    }
}
