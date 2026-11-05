using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Models;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.BLL.Container
{
    public class GroupContainer
    {
        private readonly IGroupDAL groupDAL;

        public GroupContainer(IGroupDAL groupDAL)
        {
            this.groupDAL = groupDAL ?? throw new ArgumentNullException(nameof(groupDAL));
        }
        public Result<List<Message>> GetMessages(Group group)
        {
            try
            {
                List<Message> messages =  groupDAL.GetGroupMessages(group.Id).Select(dto => new Message(dto)).ToList();
                return Result<List<Message>>.Success(messages);
            }
            catch (SqlExceptions ex)
            {
                return Result<List<Message>>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<List<Message>>.Failure(ex.ToString());
            }
        }
        public Result<List<Group>> GetGroupsByUserId(int UserId)
        {
            try
            {
            List<Group> groups = groupDAL.GetGroupsByUserId(UserId).Select(group => new Group(group)).ToList();
            return Result<List<Group>>.Success(groups);
            }
            catch (GroupNotFoundException ex)
            {
                return Result<List<Group>>.Failure(ex.ToString());
            }
            catch (SqlExceptions ex)
            {
                return Result<List<Group>>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<List<Group>>.Failure(ex.ToString());
            }
        }
        public Result CreateNewGroup(Group group)
        {
            try
            {
                if (group.PersonalGroup)
                {
                    if (IsGroupOpen(group).IsSuccess)
                    {
                        group.Name = $"{group.Participents[0].Name}{group.Participents[1].Name} ";
                        groupDAL.CreateNewGroup(group.ToDTO());
                        return Result.Success();
                    }
                    return Result.Failure("Group already exists");
                }
                else
                {
                    group.Name = $"{group.Participents[0].Name}Group";
                    groupDAL.CreateNewGroup(group.ToDTO());
                    return Result.Success();
                }
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }
        }
        public Result DeleteGroup(Group group)
        {
            try
            {
                groupDAL.DeleteGroup(group.ToDTO());
                return Result.Success();
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }
        }
        public Result IsGroupOpen(Group group)
        {
            try
            {
                groupDAL.IsGroupOpen(group.ToDTO());
                return Result.Success();
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }
        }
        public Result<Group> GetGroupByGroupId(int groupId)
        {
            try
            {
            Group group = new(groupDAL.GetGroupByGroupId(groupId));
            return Result<Group>.Success(group);
            }
            catch (SqlExceptions ex)
            {
                return Result<Group>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<Group>.Failure(ex.ToString());
            }
        } 
        public Result<bool> AddUserToGroup(int groupId, int userId)
        {
            try
            {
                bool Bool = groupDAL.AddUserToGroup(groupId, userId);
                return Result<bool>.Success(Bool);
            }
            catch (SqlExceptions ex)
            {
                return Result<bool>.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result<bool>.Failure(ex.ToString());
            }
        }
        
    }
}
