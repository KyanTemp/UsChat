using UsChat.DTO.DTO;
using UsChat.DTO.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using UsChat.DTO.Exceptions;
using UsChat.BLL.Models;

namespace UsChat.DAL
{
    public class GroupDAL : IGroupDAL
    {
        public List<UserDTO> GetGroupParticipants(int Id)
        {
            List<UserDTO> participants = new List<UserDTO>();

            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string getParticipantsQuery = "SELECT u.UserId, u.Name, u.Email FROM [User] u INNER JOIN UserGroup ug ON u.UserId = ug.UserId WHERE ug.GroupId = @GroupId";

                    using (SqlCommand cmd = new SqlCommand(getParticipantsQuery, connection))
                    {
                        cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = Id;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                participants.Add(new UserDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email"))
                                });
                            }
                        }
                    }

                    if (participants.Count == 0)
                    {
                        throw new UserNotInGroupException("Gebruik niet in de groep");
                    }
                }
                catch (UserNotInGroupException ex)
                {
                    throw new UserNotInGroupException(ex.ToString());
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
            return participants;
        }

        public List<MessageDTO> GetGroupMessages(int GroupId)
        {
            List<MessageDTO> messages = new List<MessageDTO>();

            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string getMessagesQuery = "SELECT m.MessageId, m.UserId, m.Content, m.Timestamp FROM [Message] m INNER JOIN GroupMessage gm ON m.MessageId = gm.MessageId WHERE gm.GroupId = @GroupId";

                    using (SqlCommand cmd = new SqlCommand(getMessagesQuery, connection))
                    {
                        cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = GroupId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                messages.Add(new MessageDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("MessageId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Content = reader.GetString(reader.GetOrdinal("Content")),
                                    Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                                });
                            }
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
            return messages;
        }

        public List<GroupDTO> GetGroupsByUserId(int UserId)
        {
            List<GroupDTO> groupList = new List<GroupDTO>();

            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string getGroupsQuery = "SELECT g.GroupId, g.Name, g.PersonalGroup FROM [Group] g INNER JOIN UserGroup ug ON g.GroupId = ug.GroupId WHERE ug.UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(getGroupsQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int groupId = reader.GetInt32(reader.GetOrdinal("GroupId"));
                                string groupName = reader.GetString(reader.GetOrdinal("Name"));
                                bool personalGroup = reader.GetBoolean(reader.GetOrdinal("PersonalGroup"));

                                List<UserDTO> participants = GetGroupParticipants(groupId);
                                List<MessageDTO> messages = GetGroupMessages(groupId);

                                groupList.Add(new GroupDTO
                                {
                                    Id = groupId,
                                    Name = groupName,
                                    Participents = participants,
                                    Messages = messages,
                                    PersonalGroup = personalGroup
                                });
                            }
                        }
                    }

                    // If no groups are found, return an empty list
                    return groupList;
                }
                catch (GroupNotFoundException ex)
                {
                    throw new GroupNotFoundException(ex.ToString());
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

        public bool CreateNewGroup(GroupDTO groupDTO)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Insert the group and get the generated GroupId
                    string insertGroupQuery = "INSERT INTO [Group] (Name, PersonalGroup) VALUES (@GroupName, @PersonalGroup); SELECT SCOPE_IDENTITY();";
                    int groupId;

                    using (SqlCommand cmd = new SqlCommand(insertGroupQuery, connection))
                    {
                        cmd.Parameters.Add("@GroupName", SqlDbType.VarChar, 100).Value = groupDTO.Name;
                        cmd.Parameters.Add("@PersonalGroup", SqlDbType.Bit).Value = groupDTO.PersonalGroup;
                        groupId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (groupId == 0)
                    {
                        return false; // Group creation failed
                    }

                    // Add the users to the UserGroup table
                    string insertUserGroupQuery;
                    if (groupDTO.PersonalGroup)
                    {
                        insertUserGroupQuery = "INSERT INTO UserGroup (UserId, GroupId, Role) VALUES (@UserId, @GroupId ,1);";
                    }
                    else
                    {
                        insertUserGroupQuery = "INSERT INTO UserGroup (UserId, GroupId) VALUES (@UserId, @GroupId);";
                    }
                    
                    using (SqlCommand cmd = new SqlCommand(insertUserGroupQuery, connection))
                    {
                        foreach (var user in groupDTO.Participents)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = user.Id;
                            cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    return true; // Group creation succeeded
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

        public bool DeleteGroup(GroupDTO groupDTO)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Start a transaction to ensure atomicity
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Delete associated UserGroups
                            string deleteUserGroupsQuery = "DELETE FROM UserGroup WHERE GroupId = @GroupId;";
                            using (SqlCommand cmd = new SqlCommand(deleteUserGroupsQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupDTO.Id;
                                cmd.ExecuteNonQuery();
                            }

                            string DeleteMessageGroupQuery = "DELETE FROM [GroupMessage] WHERE GroupId = @GroupId";

                            using (SqlCommand cmd = new SqlCommand(DeleteMessageGroupQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupDTO.Id;
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false; // Group not found
                                }
                            }

                            // Delete the group itself
                            string deleteGroupQuery = "DELETE FROM [Group] WHERE GroupId = @GroupId;";
                            using (SqlCommand cmd = new SqlCommand(deleteGroupQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupDTO.Id;
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false; // Group not found
                                }
                            }

                            transaction.Commit();
                            return true; // Deletion succeeded
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw; // Re-throw the exception
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

        public bool IsGroupOpen(GroupDTO groupDTO)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if a group containing both users exists
                    string query = @"SELECT ug1.GroupId 
                             FROM UserGroup ug1 
                             JOIN UserGroup ug2 ON ug1.GroupId = ug2.GroupId 
                             WHERE ug1.UserId = @UserId1 AND ug2.UserId = @UserId2";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.Add("@UserId1", SqlDbType.Int).Value = groupDTO.Participents[0].Id;
                        cmd.Parameters.Add("@UserId2", SqlDbType.Int).Value = groupDTO.Participents[1].Id;

                        object result = cmd.ExecuteScalar();

                        return result == null; // Return true if no group exists
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

        public GroupDTO GetGroupByGroupId(int GroupId)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    GroupDTO dto = new ();
                    dto.Participents = new List<UserDTO>();
                    dto.Admins = new List<UserDTO>();

                    string getGroupQuery = @"SELECT g.GroupId AS Id, g.Name AS GroupName, g.PersonalGroup, u.UserId, u.Name AS UserName, u.Email, ug.Role 
                                                                FROM [Group] g 
                                                                LEFT JOIN UserGroup ug 
                                                                    ON g.GroupId = ug.GroupId 
                                                                LEFT JOIN [User] u 
                                                                    ON ug.UserId = u.UserId 
                                                                WHERE g.GroupId = @GroupId";

                    using (SqlCommand cmd = new SqlCommand(getGroupQuery, connection))
                    {
                        cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = GroupId;
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.GetInt32(reader.GetOrdinal("Role")) == 1)
                                {
                                    dto.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                    dto.Name = reader.GetString(reader.GetOrdinal("GroupName"));
                                    dto.PersonalGroup = reader.GetBoolean(reader.GetOrdinal("PersonalGroup"));
                                    dto.Admins.Add(new UserDTO
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        Name = reader.GetString(reader.GetOrdinal("UserName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email"))
                                    });
                                }
                                else
                                {
                                    dto.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                    dto.Name = reader.GetString(reader.GetOrdinal("GroupName"));
                                    dto.PersonalGroup = reader.GetBoolean(reader.GetOrdinal("PersonalGroup"));
                                    dto.Participents.Add(new UserDTO
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        Name = reader.GetString(reader.GetOrdinal("UserName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email"))
                                    });
                                }
                            }
                        }
                    }
                    return dto;
                }
                catch (GroupNotFoundException ex)
                {
                    throw new GroupNotFoundException(ex.ToString());
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

        public bool AddUserToGroup(int groupId, int userId)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the group exists
                    string checkGroupQuery = "SELECT COUNT(*) FROM [Group] WHERE GroupId = @GroupId";
                    using (SqlCommand checkCmd = new SqlCommand(checkGroupQuery, connection))
                    {
                        checkCmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;

                        int groupExists = (int)checkCmd.ExecuteScalar();
                        if (groupExists == 0)
                        {
                            throw new GroupNotFoundException($"Group does not exist.");
                        }
                    }

                    // Check if the user already exists in the group
                    string checkUserInGroupQuery = "SELECT COUNT(*) FROM UserGroup WHERE GroupId = @GroupId AND UserId = @UserId";
                    using (SqlCommand checkUserCmd = new SqlCommand(checkUserInGroupQuery, connection))
                    {
                        checkUserCmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        checkUserCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        int userInGroup = (int)checkUserCmd.ExecuteScalar();
                        if (userInGroup > 0)
                        {
                            throw new UnknownException($"User is already in the group.");
                        }
                    }

                    // Add the user to the group with role 0
                    string insertUserQuery = @"INSERT INTO UserGroup (GroupId, UserId, Role) 
                                          VALUES (@GroupId, @UserId, 0)";
                    using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, connection))
                    {
                        insertCmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        insertCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        int rowsAffected = insertCmd.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (GroupNotFoundException ex)
                {
                    throw new GroupNotFoundException(ex.ToString());
                }
                catch (UnknownException ex)
                {
                    throw new UnknownException(ex.ToString());
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
