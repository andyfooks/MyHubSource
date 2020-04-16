using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public class MessageDTO
    {

        public MessageDTOInfoTypeEnum MessageType { get; set; }
        public string MessageTypeString { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }

        public MessageDTO(MessageDTOInfoTypeEnum messageType, string message)
        {
            PopulateMe(messageType, message);
        }
        public MessageDTO(MessageDTOInfoTypeEnum messageType, string message, Exception exception)
        {
            this.ErrorMessage = exception.Message;
            this.ErrorStackTrace = exception.StackTrace;
            PopulateMe(messageType, message);
        }
        private void PopulateMe(MessageDTOInfoTypeEnum messageType, string message)
        {
            this.Message = message;
            this.MessageType = messageType;
            this.MessageTypeString = messageType.ToString();
        }
    }
}
