using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeAPI.Models
{
    public class MessageResponse
    {
        public string Message
        {
            get;
            set;
        }

        public MessageType Type
        {
            get;
            set;
        }
    }
}