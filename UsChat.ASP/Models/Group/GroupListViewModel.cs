namespace UsChat.ASP.Models.Group
{
    public class GroupListViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string LastMessage { get; set; }  // Could display the last message sent in the group
        public string LastMessageTimestamp { get; set; }  // Timestamp of the last message
        public bool IsActive { get; set; }  // To indicate if this group is the currently active one
        public bool PersonalGroup { get; set; }  // To indicate if this is a personal group

    }
}
