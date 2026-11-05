using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.DTO.Interfaces
{
    public interface IEmailVerificationDAL
    {
        bool SaveToken(EmailVerificationTokenDTO token);
        EmailVerificationTokenDTO GetToken(string token);
        bool MarkUserAsVerified(int userId);
        bool IsUserVerified(UserDTO userDTO);
    }
}
