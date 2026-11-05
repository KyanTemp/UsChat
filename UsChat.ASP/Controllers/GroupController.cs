using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UsChat.ASP.Models.Group;
using UsChat.ASP.Models.User;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.ASP.Controllers
{
    public class GroupController : Controller
    {
        public readonly GroupContainer groupContainer;
        public readonly UserCRUDContainer userContainer;
        public GroupController(IGroupDAL GroupInterface, IUserCRUDDAL UserInterface)
        {
            groupContainer = new(GroupInterface);
            userContainer = new(UserInterface);
        }
        [HttpGet("GroupCreate")]
        public IActionResult GroupCreate(int friendId, bool personal)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            var users = new List<User> { };
            if (friendId == 0)
            {
                users.Add(userContainer.GetUserById(userId).Data);
            }
            else
            {
                users.Add(userContainer.GetUserById(userId).Data);
                users.Add(userContainer.GetUserById(friendId).Data);
            }
            
            var group = new Group(users);
            group.PersonalGroup = personal;
            Result result = groupContainer.CreateNewGroup(group);
            if (!result.IsSuccess)
            {
                ViewBag.errorMessage = result.ErrorMessage;
                return View();
            }
            // Redirect to the FriendList action in the FriendController
            return RedirectToAction("Chatpage", "Chat");
        }
        public IActionResult DeleteGroup()
        {
            int UserId = (int)HttpContext.Session.GetInt32("UserId");
            int selectedGroupId = (int)HttpContext.Session.GetInt32("GroupId");
            Result<List<Group>> groups = groupContainer.GetGroupsByUserId(UserId);
            if (groups.IsSuccess)
            {
                Group group = groups.Data.FirstOrDefault(g => g.Id == selectedGroupId);
                Result result = groupContainer.DeleteGroup(group);
                return RedirectToAction("Chatpage", "Chat", new { id = UserId }, null);
            }
            else
            {
                ViewBag.errormessage = groups.ErrorMessage;
                return View();
            }
        }
        public IActionResult Index()
        {
            int groupId = (int)HttpContext.Session.GetInt32("GroupId");

            if (groupId == 0)
            {
                return RedirectToAction("Chatpage", "Chat");
            }
            var group = groupContainer.GetGroupByGroupId(groupId);
            if (group.IsSuccess)
            {
                GroupViewModel groupViewModel = new()
                {
                    GroupId = group.Data.Id,
                    GroupName = group.Data.Name,
                    Participants = group.Data.Participents.Select(user => new UserViewModel
                    {
                        UserId = user.Id,
                        Username = user.Name,
                        Email = user.Email,
                        Password = user.Password
                    }).ToList(),
                    Admins = group.Data.Admins.Select(user => new UserViewModel
                    {
                        UserId = user.Id,
                        Username = user.Name,
                        Email = user.Email,
                        Password = user.Password
                    }).ToList()
                };
                return View(groupViewModel);
            }
            else
            {
                ViewBag.errorMessage = group.ErrorMessage;
                return View();
            }
            
        }
        public IActionResult AddUserToGroup(int groupId, int userId)
        {
            Result<bool> useradded = groupContainer.AddUserToGroup(groupId, userId); // Replace with your actual logic
            //return Json(new { id = user.UserId, name = user.Name });
            return RedirectToAction("Index");
        }
    }
}
