using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.UnitTest.Mocks
{
    public class GroupMock : IGroupDAL
    {
        public readonly UserDTO user1 = new(1, "John", "JohnDoe1!", "Johndoe@example.com");
        public readonly UserDTO user2 = new(2, "Jane", "JaneDoe2!", "Janedoe@example.com");

        public MessageDTO message1 = new MessageDTO
        {
            Id = 1,
            UserId = 2,
            Content = "hallo",
            Timestamp = new DateTime(2021, 11, 9, 9, 12, 12)
        };

        public MessageDTO message2 = new MessageDTO
        { 
            Id = 2, 
            UserId = 1, 
            Content = "hallo terug", 
            Timestamp = new DateTime(2021, 11, 12, 12, 12, 12) 
        };

        public GroupDTO GroupDto = new GroupDTO();

        public bool returnvalue;
        public bool CreateNewGroup(GroupDTO groupDTO)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("Onbekende fout");
            }
        }

        public bool DeleteGroup(GroupDTO groupDTO)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("Onbekende fout");
            }
        }

        public GroupDTO GetGroupByGroupId(int GroupId)
        {
            if (returnvalue)
            {

                List<UserDTO> participents = new List<UserDTO>();
                participents.Add(user1);
                participents.Add(user2);

                List<UserDTO> admins = new List<UserDTO>();

                List<MessageDTO> messages = new List<MessageDTO>();
                messages.Add(message1);
                messages.Add(message2);

                GroupDto.Messages = messages;
                GroupDto.Participents = participents;
                GroupDto.Admins = admins;
                return GroupDto;
            }
            else
            {
                throw new UnknownException("Onbekende fout");
            }
        }

        public List<MessageDTO> GetGroupMessages(int GroupId)
        {
            if (returnvalue)
            {
                List<MessageDTO> messages = new List<MessageDTO>();
                messages.Add(message1);
                messages.Add(message2);
                return messages;
            }
            else
            {
                throw new UnknownException("Onbekende fout");
            }
        }

        public List<GroupDTO> GetGroupsByUserId(int UserId)
        {
            if (returnvalue)
            {

                List<UserDTO> participents = new List<UserDTO>();
                participents.Add(user1);
                participents.Add(user2);

                List<UserDTO> admins = new List<UserDTO>();

                List<MessageDTO> messages = new List<MessageDTO>();
                messages.Add(message1);
                messages.Add(message2);

                GroupDto.Id = 1;
                GroupDto.Messages = messages;
                GroupDto.Participents = participents;
                GroupDto.Admins = admins;
                GroupDto.Name = "Group1";
                GroupDto.PersonalGroup = true;

                List<GroupDTO> groups = new List<GroupDTO>();
                groups.Add(GroupDto);

                return groups;
            }
            else
            {
                throw new UnknownException("Onbekende fout");
            }
        }

        public bool IsGroupOpen(GroupDTO groupDTO)
        {
            if (returnvalue)
            {
                return true;
            }
            else
            {
                throw new UnknownException("Onbekende fout");
            }
        }
    }
}
