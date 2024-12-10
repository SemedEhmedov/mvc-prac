using System.ComponentModel.DataAnnotations;

namespace Uniqlo.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string EmailorUserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
