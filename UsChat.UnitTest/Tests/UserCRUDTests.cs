using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Container;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.UnitTest.Mocks;

namespace UsChat.UnitTest.Tests
{
    [TestClass]
    public class UserCrudTests
    {
        private readonly User user1 = new(1, "John", "JohnDoe1!", "Johndoe@example.com");
        private readonly User user2 = new(1, "Jane", "JaneDoe2!", "Janedoe@example.com");

        private UserCRUDContainer? container;
        private UserCRUDMock? mockDAL;

        [TestInitialize]
        public void Initialize()
        {
            mockDAL = new UserCRUDMock();
            container = new UserCRUDContainer(mockDAL);
        }

        [TestMethod]
        [DataRow(1, true)]
        [DataRow(2, false)]
        public void GetUserById_Test(int Id, bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            // Act
            Result<User> result = container!.GetUserById(Id);

            // Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(Id, result.Data.Id);
                Assert.AreEqual(mockDAL.user1.Id, result.Data.Id);
                Assert.AreEqual(mockDAL.user1.Name, result.Data.Name);
                Assert.AreEqual(mockDAL.user1.Email, result.Data.Email);
                Assert.AreEqual(mockDAL.user1.Password, result.Data.Password);
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
        [DataRow("Johndoe@example.com", "JohnDoe1!", true)]
        [DataRow("Johndoe@example.com", "",false)]
        [DataRow("","Johndoe1!",false)]
        public void GetUserByEmailAndPassword_Test(string email, string password, bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;

            //Act
            Result<User> result = container!.GetUserByEmailAndPassword(email, password);

            //Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(email, result.Data.Email);
                Assert.AreEqual(password, result.Data.Password);
                Assert.AreEqual(mockDAL.user1.Id, result.Data.Id);
                Assert.AreEqual(mockDAL.user1.Name, result.Data.Name);
                Assert.AreEqual(mockDAL.user1.Email, result.Data.Email);
                Assert.AreEqual(mockDAL.user1.Password, result.Data.Password);
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
        [DataRow(1, "John", "Johndoe@example.com", "JohnDoe1!", true)]
        [DataRow(0, "John", "Johndoe@example.com", "Johndoe1!", false)]
        [DataRow(1, "", "Johndoe@example.com", "JohnDoe1!", false)]
        [DataRow(1, "John", "", "JohnDoe1!", false)]
        [DataRow(1, "John", "Johndoe@example.com", "", false)]
        public void Create_Test(int Id, string Name, string Email, string Password, bool MockReturn)
        {
            // Arrange
            mockDAL.returnvalue = MockReturn;
            User user1 = new(Id, Name, Password, Email);

            // Act
            Result result = container.Create(user1);

            // Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id, user1.Id);
                Assert.AreEqual(mockDAL.user1.Name, user1.Name);
                Assert.AreEqual(mockDAL.user1.Email, user1.Email);
                Assert.AreEqual(mockDAL.user1.Password, user1.Password);
            }
            else
            {
                Assert.IsNotNull(result);
                Assert.IsFalse(result.IsSuccess);
                Assert.IsNotNull(result.ErrorMessage);
            }
        }
        
        [TestMethod]
        [DataRow(1, "John", "Johnde@example.com", "JohnDoe1!", true)]
        [DataRow(2, "John", "Johnde@example.com", "JohnDoe1!", false)]
        [DataRow(1, "John", "Johndoe@example.com", "JohnDoe1!", false)]
        public void Edit_Test(int Id, string Name, string Email, string Password, bool MockReturn)
        {
            //Arrange
            mockDAL.returnvalue = MockReturn;
            User user1 = new(Id, Name, Password, Email);

            //Act
            Result<User> result = container.Edit(user1);
            //Assert
            if (MockReturn)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(user1.Id, result.Data.Id);
                Assert.AreEqual(user1.Name, result.Data.Name);
                Assert.AreEqual(user1.Email, result.Data.Email);
                Assert.AreEqual(user1.Password, result.Data.Password);
                Assert.AreEqual(mockDAL.user1.Id, result.Data.Id);
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
        [DataRow(1, true)]
        [DataRow(2,false)]
        public void Delete_Test(int Id, bool MockReturn)
        {
            //Arrange
            mockDAL.returnvalue = MockReturn;

            //Act
            Result result = container.Delete(Id);
            //Assert
            if (MockReturn)
            {
                Assert.IsTrue(result.IsSuccess);
                Assert.IsNull(result.ErrorMessage);
                Assert.AreEqual(mockDAL.user1.Id, user1.Id);
                Assert.AreEqual(mockDAL.user1.Name, user1.Name);
                Assert.AreEqual(mockDAL.user1.Email, user1.Email);
                Assert.AreEqual(mockDAL.user1.Password, user1.Password);
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
