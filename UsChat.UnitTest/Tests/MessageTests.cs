using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.UnitTest.Mocks;

namespace UsChat.UnitTest.Tests
{
    [TestClass]
    public class MessageTests
    {
        private readonly User user1 = new(1, "John", "JohnDoe1!", "Johndoe@example.com");
        private readonly User user2 = new(1, "Jane", "JaneDoe2!", "Janedoe@example.com");

        private MessageContainer? container;
        private MessageMock? mockDAL;

        [TestInitialize]
        public void Initialize()
        {
            mockDAL = new MessageMock();
            container = new MessageContainer(mockDAL);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SendMessage(bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            List<User> participents = new();
            participents.Add(user1);
            participents.Add(user2);
            List<Message> messages = new();
            Message message = new(1,1, "Hello", DateTime.Now);
            messages.Add(message);
            List<User> Admins = new();

            Group group = new(1, "Group1", true,participents,messages, Admins);

            // Act
            Result result = container.SendMessage(user1, group, message);

            // Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.IsSuccess);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.ErrorMessage);
            }
        }
    }
}
