using Microsoft.AspNetCore.Mvc;
using UsChat.BLL.Models;
using UsChat.ASP.Models.Chat;
using UsChat.ASP.Models.Group;
using UsChat.ASP.Models.Message;
using UsChat.BLL.Container;
using UsChat.DTO.Interfaces;
using UsChat.DTO.DTO;
using Microsoft.AspNetCore.SignalR;
using UsChat.DTO.Exceptions;
using UsChat.ASP.Models.User;

public class ChatController : Controller
{
    public readonly GroupContainer groupContainer;
    public readonly UserCRUDContainer userCRUDContainer;
    public readonly UserRelationContainer userRelationContainer;
    public readonly MessageContainer messageContainer;
    private readonly IHubContext<ChatHub> _hubContext; // Add SignalR Hub context

    public ChatController(IGroupDAL GroupInterface, IUserCRUDDAL UserInterface, IUserRelationDAL RelationInterface , IMessageDAL MessageInterface, IHubContext<ChatHub> hubContext)
    {
        groupContainer = new(GroupInterface);
        userCRUDContainer = new(UserInterface);
        userRelationContainer = new(RelationInterface);
        messageContainer = new(MessageInterface);
        _hubContext = hubContext;
    }
    [HttpGet]
    public IActionResult Chatpage(int selectedGroupId)
    {
        int id = (int)HttpContext.Session.GetInt32("UserId");

    // Retrieve user and their groups
        var user = userCRUDContainer.GetUserById(id);
        var groups = groupContainer.GetGroupsByUserId(id).Data ?? new List<Group>();

        // If selectedGroupId is -1 and there are available groups, assign the first group's ID
        if (selectedGroupId == 0 && groups.Any())
        {
            selectedGroupId = groups.First().Id;
            HttpContext.Session.SetInt32("GroupId", selectedGroupId);
        }
        if (selectedGroupId != 0)
        {
            HttpContext.Session.SetInt32("GroupId", selectedGroupId);
        }
        // Populate the group list (left side)
        var groupListViewModels = groups.Select(group => new GroupListViewModel
        {
            GroupId = group.Id,
            GroupName = group.Name,
            PersonalGroup = group.PersonalGroup,
            LastMessage = group.Messages?.LastOrDefault()?.Content ?? "No messages",
            LastMessageTimestamp = group.Messages?.LastOrDefault()?.Timestamp.ToString("g") ?? "",
            IsActive = group.Id == selectedGroupId
        }).ToList();

        var activeChat = groups.FirstOrDefault(g => g.Id == selectedGroupId);

        // If no chat is selected, return a viewmodel with no active chat
        if (activeChat == null)
        {
            var viewModel = new GroupChatPageViewModel
            {
                Groups = groupListViewModels,
                ActiveChat = null
            };

            return View(viewModel);
        }

        // Here, we retrieve messages from the group that matches the GroupId
        var groupWithMessages = groups.FirstOrDefault(g => g.Id == activeChat.Id);

        // If chat is selected, populate the ChatViewModel
        var chatViewModel = new ChatViewModel
        {
            GroupId = activeChat.Id,
            GroupName = activeChat.Name,
            PersonalChat = activeChat.PersonalGroup,
            Participants = activeChat.Participents.Select(p => new UserDTO
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email
            }).ToList(),
            Messages = groupWithMessages?.Messages?.Select(msg => new MessageViewModel
            {
                Content = msg.Content,
                Timestamp = msg.Timestamp.ToString("g"),
                IsFromCurrentUser = msg.UserId == id,
                MessageId = msg.Id
            }).ToList() ?? new List<MessageViewModel>(), // Fallback to an empty list if null
        };

        // Populate the full page ViewModel
        var fullViewModel = new GroupChatPageViewModel
        {
            Groups = groupListViewModels,
            ActiveChat = chatViewModel
        };

        return View(fullViewModel);
    }
    [HttpGet("NewChat")]
    public IActionResult NewChat()
    {
        ListUserViewModel listUserViewModel = new();
        int id = (int)HttpContext.Session.GetInt32("UserId");
        var users = userRelationContainer.GetFriendsWithoutChat(id);
        foreach (var item in users.Data)
        {
            listUserViewModel.Users.Add(new UserViewModel
            {
                Username = item.Name,
                UserId = item.Id,
                Email = item.Email
            });
        }
        return View(listUserViewModel);
    }
    // New SendMessage method to save and broadcast messages via SignalR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(int GroupId, string NewMessageContent)
    {
        int UserID = (int)HttpContext.Session.GetInt32("UserId");

        // Retrieve the user from the userContainer
        Result<User> currentUser = userCRUDContainer.GetUserById(UserID);
        if (!currentUser.IsSuccess)
        {
            ViewBag.errorMessage = currentUser.ErrorMessage;
            return View("Error");
        }
        Group currentGroup = groupContainer.GetGroupsByUserId(UserID).Data.FirstOrDefault(g => g.Id == GroupId);

        if (currentGroup == null || currentUser == null)
        {
            // Handle case when group or user is not found
            return NotFound();
        }

        // Create new message DTO
        Message newMessage = new Message(currentGroup.Id, currentUser.Data.Id, NewMessageContent, DateTime.Now);

        // Save the message to the database
            Result result = messageContainer.SendMessage(currentUser.Data, currentGroup, newMessage);
            if (!result.IsSuccess)
            {
                ViewBag.Message = result.ErrorMessage;
                return View("Error");
            }
        // Redirect back to the chat page
        return RedirectToAction("Chatpage", new { id = UserID, selectedGroupId = GroupId });
    }
}
