using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.DTO
{
    public class RelationDTO
    {
        public UserDTO User { get; set; }
        public int Status { get; set; }

        public RelationDTO(UserDTO user, int status)
        {
            this.User = user;
            this.Status = status;
        }
        public RelationDTO() { }
    }
}
