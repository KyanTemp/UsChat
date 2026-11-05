using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.DTO.Exceptions;
using UsChat.UnitTest.Mocks;

namespace UsChat.UnitTest.Tests
{
    [TestClass]
    public class GroupTests
    {
        private readonly User user1 = new(1, "John", "JohnDoe1!", "Johndoe@example.com");
        private readonly User user2 = new(2, "Jane", "JaneDoe2!", "Janedoe@example.com");

        private readonly Message message1 = new(1, 2, "hallo", (new DateTime(2021, 11, 9, 9, 12, 12)));
        private readonly Message message2 = new(2, 1, "hallo terug", (new DateTime(2021, 11, 12, 12, 12, 12)));

        private GroupContainer? container;
        private GroupMock? mockDAL;

        [TestInitialize]
        public void Initialize()
        {
            mockDAL = new GroupMock();
            container = new GroupContainer(mockDAL);
        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void GetMessages(bool MockReturn)
        {
            // Arrange
            List<User> participents = new List<User>();
            participents.Add(user1);
            participents.Add(user2);
            
            List<User> admins = new List<User>();

            List<Message> messages = new List<Message>();
            messages.Add(message1);
            messages.Add(message2);

            Group group = new Group(1, "Group1", true, participents, messages,admins);

            mockDAL.returnvalue = MockReturn;

            // Act
            Result<List<Message>> result = container.GetMessages(group);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(result.Data[0].Id, mockDAL.message1.Id);
                Assert.AreEqual(result.Data[0].Content, mockDAL.message1.Content);
                Assert.AreEqual(result.Data[0].UserId, mockDAL.message1.UserId);
                Assert.AreEqual(result.Data[0].Timestamp, mockDAL.message1.Timestamp);
                Assert.AreEqual(result.Data[1].Id, mockDAL.message2.Id);
                Assert.AreEqual(result.Data[1].Content, mockDAL.message2.Content);
                Assert.AreEqual(result.Data[1].UserId, mockDAL.message2.UserId);
                Assert.AreEqual(result.Data[1].Timestamp, mockDAL.message2.Timestamp);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNull(result.Data);
                Assert.IsNotNull(result.ErrorMessage);
            }

        }
        [TestMethod]
        [DataRow(1,true)]
        [DataRow(2,false)]
        public void GetGroupsByUserId(int id,bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            // Act
             Result<List<Group>> result = container.GetGroupsByUserId(id);

            // Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(id, result.Data[0].Id);
                Assert.AreEqual(mockDAL.user1.Id, result.Data[0].Participents[0].Id);
                Assert.AreEqual(mockDAL.message1.Id, result.Data[0].Messages[0].Id);
                Assert.AreEqual(mockDAL.message1.Content, result.Data[0].Messages[0].Content);
                Assert.AreEqual(mockDAL.message1.UserId, result.Data[0].Messages[0].UserId);
                Assert.AreEqual(mockDAL.user2.Id, result.Data[0].Participents[1].Id);
                Assert.AreEqual(mockDAL.message2.Id, result.Data[0].Messages[1].Id);
                Assert.AreEqual(mockDAL.message2.Content, result.Data[0].Messages[1].Content);
                Assert.AreEqual(mockDAL.message2.UserId, result.Data[0].Messages[1].UserId);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNull(result.Data);
                Assert.IsNotNull(result.ErrorMessage);
            }
        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateNewGroup(bool MockReturn)
        {
            // Arrange
            List<User> participants = new List<User>();
            participants.Add(user1);
            participants.Add(user2);
            List<Message> messages = new List<Message>();
            messages.Add(message1);
            messages.Add(message2);
            List<User> admins = new List<User>();
            admins.Add(user1);

            Group group = new Group(1,"groep",true,participants,messages, admins);

            mockDAL.returnvalue = MockReturn;

            // Act
            Result result = container.CreateNewGroup(group);


            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id ,user1.Id);
                Assert.AreEqual(mockDAL.user2.Id, user2.Id);
                Assert.AreEqual(mockDAL.user1.Name, user1.Name);
                Assert.AreEqual(mockDAL.user2.Name, user2.Name);
                Assert.AreEqual(mockDAL.user1.Email, user1.Email);
                Assert.AreEqual(mockDAL.user2.Email, user2.Email);
                Assert.AreEqual(mockDAL.user1.Password, user1.Password);
                Assert.AreEqual(mockDAL.user2.Password, user2.Password);
                Assert.AreEqual(mockDAL.user1.Id, group.Participents[0].Id);
                Assert.AreEqual(mockDAL.user2.Id, group.Participents[1].Id);
                Assert.AreEqual(mockDAL.message1.Id, message1.Id);
                Assert.AreEqual(mockDAL.message2.Id, message2.Id);
                Assert.AreEqual(mockDAL.message1.Content, message1.Content);
                Assert.AreEqual(mockDAL.message2.Content, message2.Content);
                Assert.AreEqual(mockDAL.message1.UserId, message1.UserId);
                Assert.AreEqual(mockDAL.message2.UserId, message2.UserId);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.ErrorMessage);
            }

        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void DeleteGroup(bool MockReturn)
        {
            // Arrange
            List<User> participants = new List<User>();
            participants.Add(user1);
            participants.Add(user2);
            List<Message> messages = new List<Message>();
            messages.Add(message1);
            messages.Add(message2);
            List<User> admins = new List<User>();
            admins.Add(user1);

            Group group = new Group(1, "groep", true, participants, messages, admins);

            mockDAL.returnvalue = MockReturn;

            // Act
            Result result = container.DeleteGroup(group);

            // Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.ErrorMessage);
            }

        }
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsGroupOpen(bool MockReturn)
        {
            // Arrange
            List<User> participants = new List<User>();
            participants.Add(user1);
            participants.Add(user2);
            List<Message> messages = new List<Message>();
            messages.Add(message1);
            messages.Add(message2);
            List<User> admins = new List<User>();
            admins.Add(user1);

            Group group = new Group(1, "groep", true, participants, messages, admins);

            mockDAL.returnvalue = MockReturn;

            // Act
            Result result = container.IsGroupOpen(group);

            // Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(MockReturn, mockDAL.returnvalue);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.ErrorMessage);
            }

        }
        [TestMethod]
        [DataRow(1,true)]
        [DataRow(2,false)]
        public void GetGroupByGroupId(int groupId,bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            // Act
            Result<Group> result = container.GetGroupByGroupId(groupId);

            // Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(MockReturn, mockDAL.returnvalue);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNull(result.Data);
                Assert.IsNotNull(result.ErrorMessage);
            }
        }
    }
}
