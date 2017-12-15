using ProjetoModeloDDD.Domain.Auxiliar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace GerenciamentoOnline.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string Empresa { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Agree")]
        public bool Concorda { get; set; }
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        public bool IsInRole(string role) { return false; }

        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public CustomPrincipal(GenericIdentity userIdentity, UsuarioAuth usrAuth)
        {
            this.Identity = userIdentity;
            this.IdUsuario = usrAuth.IdUsuario;
            this.IdEstab = usrAuth.IdEstab;
            this.IdEmpresa = usrAuth.IdEmpresa;
            this.UserName = usrAuth.UserName;
        }

        public int IdUsuario { get; set; }
        public int IdEstab { get; set; }
        public int IdEmpresa { get; set; }
        public string UserName { get; set; }
    }

    public class UsuarioAuth
    {
        public int IdUsuario { get; set; }
        public int IdEstab { get; set; }
        public int IdEmpresa { get; set; }
        public string UserName { get; set; }
    }


}

