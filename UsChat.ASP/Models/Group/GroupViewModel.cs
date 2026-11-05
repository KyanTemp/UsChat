using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using UsChat.ASP.Models.User;
using UsChat.DTO.DTO;

namespace UsChat.ASP.Models.Group
{
    public class GroupViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<UserViewModel> Participants { get; set; }
        public List<UserViewModel> Admins { get; set; }
    }
}
