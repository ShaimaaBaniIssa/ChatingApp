using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string Content { get; set; }
        public string RecipientUserName { get; set; }

    }
}