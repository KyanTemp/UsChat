using UsChat.ASP.Models.Chat;
using UsChat.ASP.Models.Group;
namespace UsChat.ASP.Models.Chat
{
    public class GroupChatPageViewModel
    {
        public List<GroupListViewModel> Groups { get; set; }  // The list of groups (left side)
        public ChatViewModel ActiveChat { get; set; }  // The active conversation (right side)
    }
}
