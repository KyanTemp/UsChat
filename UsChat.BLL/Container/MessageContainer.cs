using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using UsChat.BLL.Models;
using UsChat.DTO.DTO;
using UsChat.DTO.Exceptions;
using UsChat.DTO.Interfaces;

namespace UsChat.BLL.Container
{
    public class MessageContainer
    {
        private readonly IMessageDAL MessageDAL;
        public MessageContainer(IMessageDAL messageDAL)
        {
            this.MessageDAL = messageDAL ?? throw new ArgumentNullException(nameof(messageDAL));
        }
        public Result SendMessage(User sender, Group group, Message message)
        {
            try
            {
                this.MessageDAL.SendMessage(sender.ToDTO(), group.ToDTO(), message.ToDTO());
                return Result.Success();
            }
            catch (GroupNotFoundException ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch(MessageSendException ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch (SqlExceptions ex)
            {
                return Result.Failure(ex.ToString());
            }
            catch (UnknownException ex)
            {
                return Result.Failure(ex.ToString());
            }

        }
    }
}
