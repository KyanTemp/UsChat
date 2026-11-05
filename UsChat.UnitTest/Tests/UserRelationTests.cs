using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.UnitTest.Mocks;

namespace UsChat.UnitTest.Tests
{
    [TestClass]
    public class UserRelationTests
    {
        private readonly User user1 = new(1, "John", "JohnDoe1!", "Johndoe@example.com");
        private readonly User user2 = new(1, "Jane", "JaneDoe2!", "Janedoe@example.com");
        private Relation relation1 => new(user2, 1);

        private UserRelationContainer? container;
        private UserRelationMock? mockDAL;
        [TestInitialize]
        public void Initialize()
        {
            mockDAL = new UserRelationMock();
            container = new UserRelationContainer(mockDAL);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void AddRelation(bool MockReturn)
        {
            // Arrange
            User user = user1;
            Relation relation = relation1;
            mockDAL.returnvalue = MockReturn;

            // Act
            Result result = container!.AddRelation(user, relation);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id, user.Id);
                Assert.AreEqual(mockDAL.user1.Name, user.Name);
                Assert.AreEqual(mockDAL.user1.Email, user.Email);
                Assert.AreEqual(mockDAL.user1.Password, user.Password);
                Assert.AreEqual(mockDAL.relation1.User.Id, relation.User.Id);
                Assert.AreEqual(mockDAL.relation1.Status, relation.Status);
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
        public void RemoveRelation(bool MockReturn)
        {
            // Arrange
            User user = user1;
            Relation relation = new(user2, 1);
            mockDAL.returnvalue = MockReturn;

            // Act
            Result result = container!.RemoveRelation(user1, relation1);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id, user.Id);
                Assert.AreEqual(mockDAL.user1.Name, user.Name);
                Assert.AreEqual(mockDAL.user1.Email, user.Email);
                Assert.AreEqual(mockDAL.user1.Password, user.Password);
                Assert.AreEqual(mockDAL.relation1.User.Id, relation.User.Id);
                Assert.AreEqual(mockDAL.relation1.Status, relation.Status);
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
        public void BlockRelation(bool MockReturn)
        {
            // Arrange
            User user = user1;
            Relation relation = new(user2, 1);
            mockDAL.returnvalue = MockReturn;

            // Act
            Result result = container!.BlockRelation(user, relation);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id, user.Id);
                Assert.AreEqual(mockDAL.user1.Name, user.Name);
                Assert.AreEqual(mockDAL.user1.Email, user.Email);
                Assert.AreEqual(mockDAL.user1.Password, user.Password);
                Assert.AreEqual(mockDAL.relation1.User.Id, relation.User.Id);
                Assert.AreEqual(mockDAL.relation1.Status, relation.Status);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.ErrorMessage);
            }

        }

        [TestMethod]
        [DataRow("john",1 ,true)]
        [DataRow("jane",2,false)]
        public void SearchUsers(string query,int id,bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            // Act
            Result<List<UserDTO>> result = container!.SearchUsers(query, id);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id, result.Data[0].Id);
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
        public void GetFriendsWithoutChat(int id,bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            // Act
            Result<List<UserDTO>> result = container!.GetFriendsWithoutChat(id);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user2.Id, result.Data[0].Id);
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
