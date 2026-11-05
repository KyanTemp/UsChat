using UsChat.ASP.Models.User;

namespace UsChat.ASP.Models.User
{
    public class ListUserViewModel
    {
        public ListUserViewModel()
        {
            Users = new List<UserViewModel>();
        }
        public List<UserViewModel> Users { get; set; }
    }
}
