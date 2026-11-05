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
    public class MessageDAL : IMessageDAL
    {
        public bool SendMessage(UserDTO sender, GroupDTO group, MessageDTO message)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    // Start a transaction to ensure atomicity of message sending
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert the message into the Message table (use GETDATE() for Timestamp)
                            string sendMessageQuery = @"
                        INSERT INTO Message (UserId, Content, Timestamp) 
                        OUTPUT INSERTED.MessageId 
                        VALUES (@SenderId, @Content, GETDATE());";

                            int messageId;
                            using (SqlCommand cmd = new SqlCommand(sendMessageQuery, connection, transaction))
                            {
                                // Add parameters
                                cmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = sender.Id;
                                cmd.Parameters.Add("@Content", SqlDbType.VarChar, 280).Value = message.Content;

                                // Execute query and get the generated MessageId
                                messageId = (int)cmd.ExecuteScalar();
                            }

                            // Insert into GroupMessage table using the retrieved MessageId and GroupId
                            string insertGroupMessageQuery = @"
                        INSERT INTO GroupMessage (GroupId, MessageId) 
                        VALUES (@GroupId, @MessageId);";

                            using (SqlCommand cmd = new SqlCommand(insertGroupMessageQuery, connection, transaction))
                            {
                                cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = group.Id;
                                cmd.Parameters.Add("@MessageId", SqlDbType.Int).Value = messageId;

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    transaction.Rollback();
                                    return false; // Group not found
                                }
                            }

                            // Commit transaction if both inserts succeed
                            transaction.Commit();
                            return true; // Message sent successfully
                        }
                        catch (GroupNotFoundException ex)
                        {
                            transaction.Rollback();
                            throw new GroupNotFoundException(ex.ToString());
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new MessageSendException(ex.ToString());
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

    }
}
