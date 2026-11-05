using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.DAL
{
    public class EmailVerificationDAL : IEmailVerificationDAL
    {
        public EmailVerificationTokenDTO GetToken(string token)
        {
            EmailVerificationTokenDTO verificationToken = null;

            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string getTokenQuery = @"SELECT VerificationId, UserId, VerificationToken, TokenExpiryDate 
                                     FROM EmailVerification 
                                     WHERE VerificationToken = @VerificationToken;";

                    using (SqlCommand cmd = new SqlCommand(getTokenQuery, connection))
                    {
                        // Set parameters
                        cmd.Parameters.Add("@VerificationToken", SqlDbType.NVarChar, 256).Value = token;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                verificationToken = new EmailVerificationTokenDTO
                                {
                                    userId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    token = reader.GetString(reader.GetOrdinal("VerificationToken")),
                                    expiryDate = reader.GetDateTime(reader.GetOrdinal("TokenExpiryDate"))
                                };
                            }
                        }
                    }
                    if (verificationToken == null)
                    {
                        throw new TokenNotFoundException("Verificatie Token niet gevonden");
                    }
                }
                catch (TokenNotFoundException ex)
                {
                    throw new TokenNotFoundException(ex.ToString());
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
            return verificationToken;
        }
        public bool MarkUserAsVerified(int userId)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string selectQuery = @"SELECT TokenExpiryDate, u.UserId, Name, Password, Email 
                                   FROM [User] u
                                   INNER JOIN [EmailVerification] ev ON u.UserId = ev.UserId
                                   WHERE u.UserId = @UserId;";
                    using (SqlCommand cmd = new SqlCommand(selectQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime tokenExpiryDate = reader.GetDateTime(reader.GetOrdinal("TokenExpiryDate"));
                                if (tokenExpiryDate < DateTime.UtcNow)
                                {
                                    UserDTO user = new UserDTO
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Password = reader.GetString(reader.GetOrdinal("Password")),
                                        Email = reader.GetString(reader.GetOrdinal("Email"))
                                    };
                                    throw new VerificationLinkExpiredException(user, "Link buiten datum");
                                }
                            }
                            else
                            {
                                throw new VerificationLinkExpiredException(new UserDTO { Id = userId }, "Link buiten datum");
                            }
                        }
                    }

                    // Update IsVerified if the token is still valid
                    string updateQuery = @"UPDATE [User] SET IsVerified = 1 WHERE UserId = @UserId;";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (VerificationLinkExpiredException ex)
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
        public bool SaveToken(EmailVerificationTokenDTO token)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();

                    string saveTokenQuery = @"INSERT INTO EmailVerification (UserId, VerificationToken, TokenExpiryDate) 
                                      VALUES (@UserId, @VerificationToken, @TokenExpiryDate);";

                    using (SqlCommand cmd = new SqlCommand(saveTokenQuery, connection))
                    {
                        // Set parameters
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = token.userId;
                        cmd.Parameters.Add("@VerificationToken", SqlDbType.VarChar, 256).Value = token.token;
                        cmd.Parameters.Add("@TokenExpiryDate", SqlDbType.DateTime).Value = token.expiryDate;

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
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
        public bool IsUserVerified(UserDTO userDTO)
        {
            using (SqlConnection connection = new SqlConnection(SQLConnection.connectionString))
            {
                try
                {
                    connection.Open();
                    string getEmailVerificationQuery = "SELECT isVerified FROM [dbo].[User] WHERE [UserId] = @UserId;";
                    using (SqlCommand cmd = new SqlCommand(getEmailVerificationQuery, connection))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userDTO.Id;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool isVerified = reader.GetBoolean(reader.GetOrdinal("isVerified"));

                                if (isVerified)
                                {
                                    return true;
                                }
                                else
                                {
                                    throw new UserNotVerifiedException("Het emailadres is nog niet geverifieerd. Controleer je inbox voor de verificatielink.");
                                }
                            }
                        }
                    }

                    return false; // If no record was found
                }
                catch (UserNotVerifiedException ex)
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
    }
}
