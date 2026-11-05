using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.DAL
{
    public class UserCRUDDAL : IUserCRUDDAL
    {
        public bool Create(UserDTO user)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if user already exists with the provided email
                    string checkUserQuery = @"SELECT COUNT(*) FROM [User] WHERE Email = @Email;";
                    using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, connection))
                    {
                        checkCmd.Parameters.Add("@Email", SqlDbType.VarChar, 255).Value = user.Email;

                        int userExists = (int)checkCmd.ExecuteScalar();
                        if (userExists > 0)
                        {
                            throw new UserAlreadyExistsException("gebruiker bestaat al");
                        }
                    }

                    // Create the user if email is not taken
                    string createUserQuery = @"INSERT INTO [User] (Name, Password, Email) VALUES (@Name, @Password, @Email);";
                    using (SqlCommand cmd = new SqlCommand(createUserQuery, connection))
                    {
                        // Set the parameters for the SQL command
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                        cmd.Parameters.Add("@Password", SqlDbType.VarChar, 150).Value = user.Password; // Ensure the password is hashed before saving
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 255).Value = user.Email;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0; // Return true if the user was created successfully
                    }
                }
                catch (UserAlreadyExistsException ex)
                {
                    throw;
                }
                catch (SqlException ex)
                {
                    throw new SqlExceptions(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw new UnknownException(ex.ToString());
                }
            }
        }
        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if user exists before attempting to delete
                    string checkUserQuery = @"SELECT COUNT(*) FROM [User] WHERE UserId = @UserId;";
                    using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, connection))
                    {
                        checkCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;

                        int userExists = (int)checkCmd.ExecuteScalar();
                        if (userExists == 0)
                        {
                            throw new UserNotFoundException("Gebruiker niet gevonden");
                        }
                    }

                    // Start a SQL transaction to ensure atomicity
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Delete from the EmailVerification table where the user is involved
                            string deleteVerificationQuery = @"DELETE FROM EmailVerification WHERE UserId = @UserId;";
                            using (SqlCommand cmd = new SqlCommand(deleteVerificationQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;
                                cmd.ExecuteNonQuery();
                            }

                            // Delete from the RelationShip table where the user is involved
                            string deleteRelationshipsQuery = @"DELETE FROM RelationShip WHERE UserIdOne = @UserId OR UserIdTwo = @UserId;";
                            using (SqlCommand cmd = new SqlCommand(deleteRelationshipsQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;
                                cmd.ExecuteNonQuery();
                            }

                            // Delete from UserGroup table where the user is part of a group
                            string deleteUserGroupsQuery = @"DELETE FROM UserGroup WHERE UserId = @UserId;";
                            using (SqlCommand cmd = new SqlCommand(deleteUserGroupsQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;
                                cmd.ExecuteNonQuery();
                            }

                            // Delete the user from the User table
                            string deleteUserQuery = @"DELETE FROM [User] WHERE UserId = @UserId;";
                            using (SqlCommand cmd = new SqlCommand(deleteUserQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    throw new UnknownException("Onbekende fout");
                                }
                            }

                            // Commit the transaction if all commands succeed
                            transaction.Commit();
                            return true; // User deletion succeeded
                        }
                        catch
                        {
                            // Rollback the transaction if any command fails
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new SqlExceptions(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw new UnknownException(ex.ToString());
                }
            }
        }
        public UserDTO Edit(UserDTO user)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if email already exists for a different user
                    string checkEmailQuery = @"SELECT COUNT(*) FROM [User] WHERE Email = @Email AND UserId <> @UserId;";
                    using (SqlCommand checkCmd = new SqlCommand(checkEmailQuery, connection))
                    {
                        checkCmd.Parameters.Add("@Email", SqlDbType.VarChar, 255).Value = user.Email;
                        checkCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user.Id;

                        int emailExists = (int)checkCmd.ExecuteScalar();
                        if (emailExists > 0)
                        {
                            throw new UserAlreadyExistsException("gebruiker bestaat al");
                        }
                    }

                    // SQL query to update the user's information
                    string updateUserQuery = @"UPDATE [User] SET Name = @Name, Password = @Password, Email = @Email WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(updateUserQuery, connection))
                    {
                        // Set the parameters for the SQL command
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                        cmd.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = user.Password; // Ensure the password is hashed before saving
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 255).Value = user.Email;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user.Id;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new UserNotFoundException("Email of wachtwoord incorecct"); // If no rows were affected, the user does not exist.
                        }

                        // Return the updated user object
                        return user;
                    }
                }
                catch (SqlException ex)
                {
                    throw new SqlExceptions(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw new UnknownException(ex.ToString());
                }
            }
        }
        public UserDTO GetUserById(int id)
        {
            UserDTO dto = null;

            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Fetch user details
                    string getUserByIdQuery = "SELECT * FROM [dbo].[User] WHERE [UserId] = @UserId;";
                    using (SqlCommand cmd = new SqlCommand(getUserByIdQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    RelationList = new List<RelationDTO>() // Initialize the friend list
                                };
                            }
                        }
                    }

                    // If the user is not found, throw a UserNotFoundException
                    if (dto == null)
                    {
                        throw new UserNotFoundException("Gebruiker niet gevonden");
                    }

                    // Fetch relationships (friendships) where the user is either UserIdOne or UserIdTwo
                    string getRelationsQuery = @"SELECT r.RelationShipId, 
                                                CASE 
                                                    WHEN r.UserIdOne = @UserId THEN u2.UserId 
                                                    ELSE u1.UserId 
                                                END AS FriendId,                   
                                                CASE 
                                                    WHEN r.UserIdOne = @UserId THEN u2.Name
                                                    ELSE u1.Name 
                                                END AS FriendName,
                                                CASE 
                                                    WHEN r.UserIdOne = @UserId THEN u2.Email
                                                    ELSE u1.Email
                                                END AS FriendEmail,
                                                r.RelationShipStatus
                                            FROM RelationShip r
                                            JOIN [User] u1 ON r.UserIdOne = u1.UserId
                                            JOIN [User] u2 ON r.UserIdTwo = u2.UserId
                                            WHERE r.UserIdOne = @UserId OR r.UserIdTwo = @UserId;";

                    using (SqlCommand cmd = new SqlCommand(getRelationsQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = id;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create and populate the FriendDTO
                                UserDTO friendDTO = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("FriendId")),
                                    Name = reader.GetString(reader.GetOrdinal("FriendName")),
                                    Email = reader.GetString(reader.GetOrdinal("FriendEmail"))
                                };
                                // Create and populate the RelationDTO
                                RelationDTO relation = new RelationDTO
                                {
                                    User = friendDTO,  // Assign the populated UserDTO
                                    Status = reader.GetInt32(reader.GetOrdinal("RelationShipStatus")) // Get the relationship status
                                };

                                // Add the relation to the DTO's list of relations
                                dto.RelationList.Add(relation);
                            }
                        }
                    }
                }
                catch (UserNotFoundException ex)
                {
                    throw new UserNotFoundException(ex.ToString());
                }
                catch (SqlException ex)
                {
                    throw new SqlExceptions(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw new UnknownException(ex.ToString());
                }
            }

            return dto;
        }
        public UserDTO GetUserByEmailAndPassword(string email, string password)
        {
            UserDTO dto = null;

            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Fetch user details using email and password
                    string getUserByEmailPasswordQuery = "SELECT * FROM [dbo].[User] WHERE [Email] = @Email AND [Password] = @Password;";
                    using (SqlCommand cmd = new SqlCommand(getUserByEmailPasswordQuery, connection))
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    RelationList = new List<RelationDTO>() // Initialize the friend list
                                };
                            }
                        }
                    }

                    // If the user is not found, throw a UserNotFoundException
                    if (dto == null)
                    {
                        throw new UserNotFoundException("Email of wachtwoord incorecct");
                    }

                    // Fetch relationships (friendships) where the user is either UserIdOne or UserIdTwo
                    string getRelationsQuery = @"SELECT r.RelationShipId, 
                                        CASE 
                                            WHEN r.UserIdOne = @UserId THEN u2.UserId 
                                            ELSE u1.UserId 
                                        END AS FriendId,                   
                                        CASE 
                                            WHEN r.UserIdOne = @UserId THEN u2.Name
                                            ELSE u1.Name 
                                        END AS FriendName,
                                        CASE 
                                            WHEN r.UserIdOne = @UserId THEN u2.Email
                                            ELSE u1.Email
                                        END AS FriendEmail,
                                        r.RelationShipStatus
                                    FROM RelationShip r
                                    JOIN [User] u1 ON r.UserIdOne = u1.UserId
                                    JOIN [User] u2 ON r.UserIdTwo = u2.UserId
                                    WHERE r.UserIdOne = @UserId OR r.UserIdTwo = @UserId;";

                    using (SqlCommand cmd = new SqlCommand(getRelationsQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = dto.Id;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create and populate the FriendDTO
                                UserDTO friendDTO = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("FriendId")),
                                    Name = reader.GetString(reader.GetOrdinal("FriendName")),
                                    Email = reader.GetString(reader.GetOrdinal("FriendEmail"))
                                };
                                // Create and populate the RelationDTO
                                RelationDTO relation = new RelationDTO
                                {
                                    User = friendDTO,  // Assign the populated UserDTO
                                    Status = reader.GetInt32(reader.GetOrdinal("RelationShipStatus")) // Get the relationship status
                                };

                                // Add the relation to the DTO's list of relations
                                dto.RelationList.Add(relation);
                            }
                        }
                    }
                }
                catch (UserNotFoundException ex)
                {
                    throw;
                }
                catch (SqlException ex)
                {
                    throw new SqlExceptions(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw new UnknownException(ex.ToString());
                }
            }
            return dto;
        }
    }
}
