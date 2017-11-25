using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ProjetoModeloDDD.MVC.ViewModels
{
    public class Usuario
    {
        [Required(ErrorMessage = "O nome do usuário deve ser informado")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A senha deve ser informada.")]
        public string Senha { get; set; }

        [Remote("ValidaUsuario", "Home", ErrorMessage = "Usuário Invalido!")]
        public string ValidaUsuario { get; set; }

        [Remote("ValidaSenha", "Home", ErrorMessage = "Senha Invalida!")]
        public string ValidaSenha { get; set; }

        
    }
}