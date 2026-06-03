using System.ComponentModel.DataAnnotations;

namespace GCTL.UI.Core.ViewModels
{
    public class LoginPageViewModel
    {
        [Required(ErrorMessage = "Please Enter User Name", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
