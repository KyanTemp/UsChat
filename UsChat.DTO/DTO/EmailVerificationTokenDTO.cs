using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.DTO
{
    public class EmailVerificationTokenDTO
    {
        public int userId { get; set; }
        public string token { get; set; }
        public DateTime expiryDate { get; set; }
    }
}
