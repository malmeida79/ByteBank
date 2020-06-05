using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        private UserManager<UsuarioAplicacao> _userManager;
        private SignInManager<UsuarioAplicacao, string> _signManager;

        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        public SignInManager<UsuarioAplicacao, string> SignInManager
        {
            get
            {
                if (_signManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _signManager = contextOwin.GetUserManager<SignInManager<UsuarioAplicacao, string>>();
                }
                return _signManager;
            }
            set
            {
                _signManager = value;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                var contextoOwin = Request.GetOwinContext();
                return contextoOwin.Authentication;
            }
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(ContaLoginViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var usuario = await UserManager.FindByEmailAsync(modelo.Email);

                if (usuario == null)
                {
                    return SenhaOuUsuarioInvalidos();
                }

                if (usuario != null)
                {
                    var signResultado = await SignInManager.PasswordSignInAsync(usuario.UserName, modelo.Senha, isPersistent: modelo.ContinuarLogado, shouldLockout: true);

                    switch (signResultado)
                    {
                        case SignInStatus.Success:

                            // caso email nao tenha sido confirmado, usuario nao acessa aplicacao
                            if (!usuario.EmailConfirmed)
                            {
                                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                return RedirectToAction("AguardandoConfirmacao");
                            }

                            // caso confirmado usuario acessa normalmente
                            return RedirectToAction("Index", "Home");
                        case SignInStatus.LockedOut:
                            // verificamos a senha nesse ponto apenas para informar ao usuário dono da conta
                            // que sua conta esta bloqueada e para o demais (invasor tentando acesso) irei
                            // retornar apenas a mensagem default para nao dar pistas de que o usuario exite
                            // e esta apenas bloqueado.
                            var senhaCorreta = await UserManager.CheckPasswordAsync(usuario, modelo.Senha);
                            if (senhaCorreta)
                            {
                                ModelState.AddModelError("", "A conta esta bloqueada !");
                            }
                            else
                            {
                                return SenhaOuUsuarioInvalidos();
                            }
                            break;
                        default:
                            return SenhaOuUsuarioInvalidos();
                    }
                }
            }

            return View(modelo);
        }

        public ActionResult EsqueciSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EsqueciSenha(ContaEsqueciSenhaViewModel modelo)
        {
            if (ModelState.IsValid)
            {

                // gerar token para reset da senha
                // gerar url para o usuário 
                // enviar o email ao usuário
                var usuario = await UserManager.FindByEmailAsync(modelo.Email);

                if (usuario != null)
                {

                    var token = UserManager.GeneratePasswordResetTokenAsync(usuario.Id);
                    // criando uma view para enviar ao usuário para que esse ao clicar seja automaticamente validado sem o mesmo ter que 
                    // acessar uma pagina, copiar e colar token, etc ... 
                    var linkDeCallBack = Url.Action("ConfirmacaoAlteracaoSenha", "conta", new { usurioID = usuario.Id, token = token }, Request.Url.Scheme);

                    // enviando email para o usuário com o token para esse poder validar email
                    await UserManager.SendEmailAsync(usuario.Id, "Forum Byte Bank - Alteração de senha", $"Clique aqui {linkDeCallBack} para alterar sua senha.");
                }

                // independente de o usuario estar na base ou nao, iremos mostrar a mensagem de senha enviada
                // para previnir novamente de um hacker saber que a conta é valida ou não.
                return View("EmailAlteracaoSenha");

            }

            return View();
        }

        public async Task<ActionResult> ConfirmacaoAlteracaoSenha(string usuarioID, string token)
        {
            var modelo = new ContaConfirmacaoAlteracaoSenhaViewModel
            {
                UsuarioId = usuarioID,
                Token = token
            };
            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmacaoAlteracaoSenha(ContaConfirmacaoAlteracaoSenhaViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var resultadoAlteracao = await UserManager.ResetPasswordAsync(modelo.UsuarioId, modelo.Token, modelo.NovaSenha);

                if (resultadoAlteracao.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AdicionaErro(resultadoAlteracao);
                }
            }
            return View();
        }

        private ActionResult SenhaOuUsuarioInvalidos()
        {
            ModelState.AddModelError("", "Credenciais inválidas!");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {

                var novoUsuario = new UsuarioAplicacao();

                novoUsuario.Email = modelo.UserName;
                novoUsuario.UserName = modelo.UserName;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;

                var usuario = await UserManager.FindByEmailAsync(modelo.Email);

                if (usuario != null)
                {
                    // Para proteger o usuairo, quando oemail ja estier cadastrado então redirecionaremos
                    // para entrada a fim de não cadastrarmos o mesmo com email duplicado e não iremos
                    // tambem fornecer aos hacckers a validação desse email cadastrado em nosso sistema.
                    return RedirectToAction("AguardandoConfirmacao");
                }

                var resultado = await UserManager.CreateAsync(novoUsuario, modelo.Senha);

                if (resultado.Succeeded)
                {
                    // enviando email para usuario
                    await EnviarEmailConfirmacao(novoUsuario);

                    // incluir usuario
                    return RedirectToAction("AguardandoConfirmacao");
                }
                else
                {
                    AdicionaErro(resultado);
                }
            }

            // algo errado aconteceu
            return View(modelo);
        }


        public async Task<ActionResult> ConfirmacaoEmail(string usuarioID, string token)
        {
            if (usuarioID == null || token == null)
            {
                return View("Error");
            }

            // chegando token
            var resultado = await UserManager.ConfirmEmailAsync(usuarioID, token);

            // se token valido
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }


        private async Task EnviarEmailConfirmacao(UsuarioAplicacao usuario)
        {
            // criando um token para o usuário se registrar na aplicação
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(usuario.Id);

            // criando uma view para enviar ao usuário para que esse ao clicar seja automaticamente validado sem o mesmo ter que 
            // acessar uma pagina, copiar e colar token, etc ... 
            var linkDeCallBack = Url.Action("ConfirmacaoEmail", "conta", new { usurioID = usuario.Id, token = token }, Request.Url.Scheme);

            // enviando email para o usuário com o token para esse poder validar email
            await UserManager.SendEmailAsync(usuario.Id, "Forum Byte Bank - Confirmação de e-mail", $"Bem vindo ao forum ByteBak clique aqui {linkDeCallBack} para confirmar seu endereço de e-mail.");

        }

        private void AdicionaErro(IdentityResult resultado)
        {
            // recebendo e tratando os erros
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro);
            }
        }
    }
}