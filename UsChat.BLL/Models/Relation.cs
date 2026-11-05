using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.BLL.Models
{
    public class Relation
    {
        public User User { get; set; }
        public int Status { get; set; }

        public Relation(User user, int status)
        {
            this.User = user;
            this.Status = status;
        }
        public Relation(RelationDTO relationDTO)
        {
            relationDTO.User = User.ToDTO();
            relationDTO.Status = Status;
        }
        public RelationDTO ToDTO()
        {
            return new RelationDTO(User.ToDTO(), Status);
        }
    }
}
