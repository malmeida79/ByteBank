using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

namespace ByteBank.Forum.App_Start.Identity
{
    public class SenhaValidador : IIdentityValidator<string>
    {
        public int TamanhoRequerido { get; set; }

        public bool ObrigatorioCaracteresEspeciais { get; set; }

        public bool ObrigatorioLowerCase { get; set; }

        public bool ObrigatorioUpperCase { get; set; }

        public bool ObrigatorioDigitos { get; set; }

        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var erros = new List<string>();

            if (ObrigatorioCaracteresEspeciais && !VerificaCaracteresEspeciaos(item))
            {
                erros.Add("A senha deve conter caracteres especiais.");
            }

            if (ObrigatorioDigitos && !VerificaDigitos(item))
            {
                erros.Add("A senha deve conter digitos.");
            }

            if (ObrigatorioLowerCase && !VerificaLowerCase(item))
            {
                erros.Add("A senha deve conter caracteres minusculos.");
            }

            if (ObrigatorioUpperCase && !VerificaUpperCase(item))
            {
                erros.Add("A senha deve conter caracteres maisculos");
            }

            if (!VerificaTamanhoRequerido(item))
            {
                erros.Add($"A senha deve conter no minimo {TamanhoRequerido} caracteres.");
            }

            if (erros.Any())
            {
                return IdentityResult.Failed(erros.ToArray());
            }
            else
            {
                return IdentityResult.Success;
            }
        }

        // usando a interragação, na senha se ela for nula entao sera devolvido false e tambem podemos
        // usar assim uma função arrow, por ter uma unica devolução.
        private bool VerificaTamanhoRequerido(string senha) => senha?.Length >= TamanhoRequerido;

        private bool VerificaCaracteresEspeciaos(string senha) => Regex.IsMatch(senha, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        private bool VerificaLowerCase(string senha) => senha.Any(char.IsLower);

        private bool VerificaUpperCase(string senha) => senha.Any(char.IsUpper);

        private bool VerificaDigitos(string senha) => senha.Any(char.IsDigit);
    }
}