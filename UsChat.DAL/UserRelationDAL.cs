using Microsoft.Data.SqlClient;
using System.Data;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.DAL
{
    public class UserRelationDAL : IUserRelationDAL
    {
        public bool AddRelation(UserDTO user, RelationDTO relation)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string addRelationQuery = @"
                INSERT INTO RelationShip (UserIdOne, UserIdTwo, RelationShipStatus) 
                VALUES (@UserIdOne, @UserIdTwo, @RelationStatus);";

                    using (SqlCommand cmd = new SqlCommand(addRelationQuery, connection))
                    {
                        cmd.Parameters.Add("@UserIdOne", SqlDbType.Int).Value = user.Id;
                        cmd.Parameters.Add("@UserIdTwo", SqlDbType.Int).Value = relation.User.Id;
                        cmd.Parameters.Add("@RelationStatus", SqlDbType.Int).Value = relation.Status;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0; // Return true if a row was inserted
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
        public bool RemoveRelation(UserDTO user, RelationDTO relation)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string removeRelationQuery = @"
                DELETE FROM RelationShip 
                WHERE (UserIdOne = @UserIdOne AND UserIdTwo = @UserIdTwo) 
                   OR (UserIdOne = @UserIdTwo AND UserIdTwo = @UserIdOne);";

                    using (SqlCommand cmd = new SqlCommand(removeRelationQuery, connection))
                    {
                        cmd.Parameters.Add("@UserIdOne", SqlDbType.Int).Value = user.Id;
                        cmd.Parameters.Add("@UserIdTwo", SqlDbType.Int).Value = relation.User.Id;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0; // Return true if a row was deleted
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
        public bool BlockRelation(UserDTO user, RelationDTO relation)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string blockRelationQuery = @"
                UPDATE RelationShip 
                SET RelationShipStatus = 2 
                WHERE (UserIdOne = @UserIdOne AND UserIdTwo = @UserIdTwo) 
                   OR (UserIdOne = @UserIdTwo AND UserIdTwo = @UserIdOne);";

                    using (SqlCommand cmd = new SqlCommand(blockRelationQuery, connection))
                    {
                        cmd.Parameters.Add("@UserIdOne", SqlDbType.Int).Value = user.Id;
                        cmd.Parameters.Add("@UserIdTwo", SqlDbType.Int).Value = relation.User.Id;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0; // Return true if a row was updated
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
        public List<UserDTO> AddToGroupSearch(string query, int userId)
        {
            List<UserDTO> users = [];
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string searchUsersQuery = @"SELECT u.UserId, u.Name, u.Password, u.Email
                                                               FROM[User] u WHERE u.Email COLLATE Latin1_General_CI_AS
                                                                        LIKE @Query AND u.UserId != @UserId
                                                                        AND  EXISTS( SELECT 1 FROM RelationShip r
                                                                        WHERE (r.UserIdOne = u.UserId AND r.UserIdTwo = @UserId AND r.RelationShipStatus IN(1, 2)) 
                                                                        OR(r.UserIdTwo = u.UserId AND r.UserIdOne = @UserId AND r.RelationShipStatus IN(1, 2)) );";
                    using (SqlCommand cmd = new SqlCommand(searchUsersQuery, connection))
                    {
                        cmd.Parameters.Add("@Query", SqlDbType.VarChar).Value = "%" + query + "%";
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserDTO user = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                };
                                users.Add(user);
                            }
                        }
                    }
                    return users;
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
        public List<UserDTO> SearchUsers(string query, int userId)
        {
            List<UserDTO> users = [];
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string searchUsersQuery = @"SELECT u.UserId, u.Name, u.Password, u.Email
                                                               FROM[User] u WHERE u.Email COLLATE Latin1_General_CI_AS
                                                                        LIKE @Query AND u.UserId != @UserId
                                                                        AND NOT EXISTS( SELECT 1 FROM RelationShip r
                                                                        WHERE (r.UserIdOne = u.UserId AND r.UserIdTwo = @UserId AND r.RelationShipStatus IN(1, 2)) 
                                                                        OR(r.UserIdTwo = u.UserId AND r.UserIdOne = @UserId AND r.RelationShipStatus IN(1, 2)) );";
                    using (SqlCommand cmd = new SqlCommand(searchUsersQuery, connection))
                    {
                        cmd.Parameters.Add("@Query", SqlDbType.VarChar).Value = "%" + query + "%";
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value=userId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserDTO user = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                };
                                users.Add(user);
                            }
                        }
                    }
                    return users;
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
        public List<UserDTO> GetFriendsWithoutChat(int userId)
        {
            List<UserDTO> users = [];
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string friendsWithoutChatQuery = @"
                                                                        -- retrieves all users who are friends of the logged-in user
                                                                    WITH Friends AS (
                                                                    SELECT DISTINCT u.UserId, u.Name, u.Email
                                                                    FROM RelationShip r
                                                                    INNER JOIN [User] u
                                                                        ON (u.UserId = r.UserIdOne AND r.UserIdTwo = @UserID)
                                                                        OR (u.UserId = r.UserIdTwo AND r.UserIdOne = @UserID)
                                                                    WHERE r.RelationShipStatus = 1
                                                                ), 
                                                                        -- retrieves all users who are in a personal group of the logged-in user
                                                                PersonalGroupUsers AS (
                                                                    SELECT DISTINCT ug2.UserId AS FriendId
                                                                    FROM UserGroup ug1
                                                                    INNER JOIN UserGroup ug2
                                                                        ON ug1.GroupId = ug2.GroupId
                                                                    INNER JOIN [Group] g
                                                                        ON g.GroupId = ug1.GroupId
                                                                    WHERE ug1.UserId = @UserID AND g.PersonalGroup = 1
                                                                ) 
                                                                        -- Retrieve Friends Excluding Personal Group Members
                                                                SELECT f.UserId, f.Name, f.Email
                                                                FROM Friends f
                                                                LEFT JOIN PersonalGroupUsers pgu
                                                                    ON f.UserId = pgu.FriendId
                                                                WHERE pgu.FriendId IS NULL;";
                    using (SqlCommand cmd = new SqlCommand(friendsWithoutChatQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserDTO user = new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                };
                                users.Add(user);
                            }
                        }
                    }
                    return users;
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
    }
}
