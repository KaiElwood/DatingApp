
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
  public class RegisterDto
  {

// this is a good place to do validation because:
// 1. automatically binds to accountcontroller -- no need to search for file
    [Required]
		public string username { get; set;} 

    [Required] 
    public string password { get; set; } 
        
  }
}