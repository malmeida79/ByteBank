using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;
using ByteBank.Forum.App_Start.Identity;
using System;

// definindo que deve ser inicializado pelo ownin
[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]

namespace ByteBank.Forum
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<DbContext>(() =>
                new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<UsuarioAplicacao>>(
                (opcoes, contextoOwin) =>
                {
                    var dbContext = contextoOwin.Get<DbContext>();
                    return new UserStore<UsuarioAplicacao>(dbContext);
                });

            builder.CreatePerOwinContext<UserManager<UsuarioAplicacao>>(
                (opcoes, contextoOwin) =>
                {
                    var userStore = contextoOwin.Get<IUserStore<UsuarioAplicacao>>();
                    var userManager = new UserManager<UsuarioAplicacao>(userStore);
                    var userValidador = new UserValidator<UsuarioAplicacao>(userManager);

                    // validando email nao duplicado
                    userValidador.RequireUniqueEmail = true;
                    userManager.UserValidator = userValidador;

                    // validando segurança da senha
                    userManager.PasswordValidator = new SenhaValidador()
                    {
                        TamanhoRequerido = 6,
                        ObrigatorioCaracteresEspeciais = true,
                        ObrigatorioDigitos = true,
                        ObrigatorioLowerCase = true,
                        ObrigatorioUpperCase = true
                    };

                    // iniciando o serviço de emails
                    userManager.EmailService = new EmailServico();

                    // criando um provedor de tokens
                    var dataProtectionProvider = opcoes.DataProtectionProvider;
                    var dataProtectionProviderCreated = dataProtectionProvider.Create("ByteBank.Forum");

                    // associando o provedor de tokens
                    userManager.UserTokenProvider = new DataProtectorTokenProvider<UsuarioAplicacao>(dataProtectionProviderCreated);

                    // numero maximo de tentativa antes do lockout do usuario
                    userManager.MaxFailedAccessAttemptsBeforeLockout = 3;

                    // tempo de lockout (5 minutos)
                    userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);

                    // Aplicando lockout para todo e qualquer usuário ... 
                    userManager.UserLockoutEnabledByDefault = true;

                    return userManager;
                });

            // Configurando SignIn do usuário
            builder.CreatePerOwinContext<SignInManager<UsuarioAplicacao, string>>(
               (opcoes, contextoOwin) =>
               {
                   var userManager = contextoOwin.Get<UserManager<UsuarioAplicacao>>();
                   var signInManager = new SignInManager<UsuarioAplicacao, string>(userManager, contextoOwin.Authentication);
                   return signInManager;
               });


            // configurando para aplicação ter acesso a cookies Auth
            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }

}