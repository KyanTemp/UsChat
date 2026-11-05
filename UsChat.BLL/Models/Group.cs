using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.BLL.Models
{
    public class Group
    {
        //properties
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PersonalGroup { get; set; }
        public List<User> Participents { get; set; }
        public List<Message> Messages { get; set; }
        public List<User> Admins { get; set; }

        //constructors
        public Group(int Id, string Name, bool PersonalGroup, List<User> Participents, List<Message> Messages, List<User> admins)
        {
            this.Id = Id;
            this.Name = Name;
            this.PersonalGroup = PersonalGroup;
            this.Participents = Participents;
            this.Messages = Messages;
            this.Admins = admins;
        }
        public Group(List<User> Participents)
        {
            this.Participents = Participents;
        }

        public Group(GroupDTO DTO)
        {
            Id = DTO.Id;
            Name = DTO.Name;
            PersonalGroup = DTO.PersonalGroup;
            Participents = DTO.Participents.Select(p => new User(p)).ToList();
            this.Messages = DTO.Messages?.Select(m => new Message(m)).ToList();
            Admins = DTO.Admins?.Select(a => new User(a)).ToList();
        }

        public GroupDTO ToDTO()
        {
            GroupDTO DTO = new()
            {
                Id = this.Id,
                Name = this.Name,
                PersonalGroup = this.PersonalGroup,
                Participents = this.Participents.Select(p => p.ToDTO()).ToList(),
                Messages = (this.Messages ?? new List<Message>()).Select(m => m.ToDTO()).ToList(),
                Admins = (this.Admins ?? new List<User>()).Select(a => a.ToDTO()).ToList()
            };
            return DTO;
        }
    }
}
