using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.DTOs
{
    public class UserDto
    {
        public string username { get; set; }
        public string token { get; set; }
    }
}