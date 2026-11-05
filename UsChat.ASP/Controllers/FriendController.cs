using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UsChat.ASP.Models.User;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.ASP.Controllers
{
    public class FriendController : Controller
    {
        public readonly UserCRUDContainer userCRUDContainer;
        public readonly UserRelationContainer userRelationContainer;

        public FriendController(IUserCRUDDAL UserInterface, IUserRelationDAL UserRelation)
        {
            userCRUDContainer = new(UserInterface);
            userRelationContainer = new(UserRelation);
        }
        public IActionResult FriendList()
        {
            int Id = (int)HttpContext.Session.GetInt32("UserId");
            ListUserViewModel listUserViewModel = new();
            Result<User>  user = userCRUDContainer.GetUserById(Id);
            if (user.IsSuccess)
            {
                foreach (Relation Friend in user.Data.RelationList)
                {
                    Result<User> CurrentFriend = userCRUDContainer.GetUserById(Friend.User.Id);
                    if (CurrentFriend.IsSuccess)
                    {
                        listUserViewModel.Users.Add(new UserViewModel
                        {
                            Username = CurrentFriend.Data.Name,
                            UserId = CurrentFriend.Data.Id,
                            Email = CurrentFriend.Data.Email
                        });
                    }
                    else
                    {
                        ViewBag.errorMessage = CurrentFriend.ErrorMessage;
                        return View();
                    }
                    
                }
            }
            return View(listUserViewModel);
        }
        [HttpGet]
        public IActionResult SearchFriendsForGroup(string searchTerm)
        {
            int userId = ((int)HttpContext.Session.GetInt32("UserId"));
            var users = userRelationContainer.AddToGroupSearch(searchTerm, userId);
            if (users.IsSuccess)
            {
                return Json(users.Data);
            }
            else
            {
                ViewBag.ErrorMessage = users.ErrorMessage;
                return Json(null);
            }

        }
        [HttpGet]
        public IActionResult SearchFriends(string searchTerm)
        {
            int userId = ((int)HttpContext.Session.GetInt32("UserId"));
            var users = userRelationContainer.SearchUsers(searchTerm, userId);
            if (users.IsSuccess)
            {
                return Json(users.Data);
            }
            else
            {
                ViewBag.ErrorMessage = users.ErrorMessage;
                return Json(null);
            }

        }
        [HttpPost]
        public IActionResult AddFriend(int userId)
        {
            var user = userCRUDContainer.GetUserById((int)HttpContext.Session.GetInt32("UserId"));
            if (user != null)
            {
                Result<User> relationUser = userCRUDContainer.GetUserById(userId);
                if (relationUser.IsSuccess)
                {
                    Relation relation = new(relationUser.Data, 1);
                    userRelationContainer.AddRelation(user.Data, relation);
                    return Json(relationUser);
                }
               
            }
            return Json(null);
        }
        public IActionResult RemoveFriend(int userId)
        {
            var user = userCRUDContainer.GetUserById((int)HttpContext.Session.GetInt32("UserId"));
            if (user != null)
            {
                Result<User> relationUser = userCRUDContainer.GetUserById(userId);
                if (relationUser.IsSuccess)
                {
                    Relation relation = new(relationUser.Data, 0);
                    userRelationContainer.RemoveRelation(user.Data, relation);
                }
                else
                {
                    ViewBag.errorMessage = relationUser.ErrorMessage;
                }
            }
            return RedirectToAction("FriendList");
        }
        public IActionResult BlockFriend(int userId)
        {
            var user = userCRUDContainer.GetUserById((int)HttpContext.Session.GetInt32("UserId"));
            if (user != null)
            {
                Result<User> relationUser = userCRUDContainer.GetUserById(userId);
                if (relationUser.IsSuccess)
                {
                    Relation relation = new(relationUser.Data, 2);
                    userRelationContainer.BlockRelation(user.Data, relation);
                }
            }
            return RedirectToAction("FriendList");
        }
    }
}
