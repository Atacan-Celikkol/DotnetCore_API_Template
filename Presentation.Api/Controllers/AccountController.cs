using AutoMapper;
using Core.Exceptions;
using Core.Extensions;
using Data.Models.Identity;
using Data.Models.Mapping.Requests;
using Data.Models.Mapping.Wrappers;
using Data.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Filters;
using Presentation.Api.Token;
using Providers.Mail;
using Services;
using System;
using System.Threading.Tasks;

namespace Presentation.Api.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [Route("account")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly UserService userService;
        private readonly JwtHelper jwtHelper;
        private readonly IMailProvider mailProvider;
        private readonly ProjectConfiguration projectConfiguration;

        public AccountController(IMapper mp, UserManager<User> um, UserService us, JwtHelper jh, IMailProvider mpv, ProjectConfiguration pc) : base(mp)
        {
            userManager = um;
            userService = us;
            jwtHelper = jh;
            mailProvider = mpv;
            projectConfiguration = pc;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<Response<string>> RegisterAsync(RegisterRequest model)
        {
            var user = Map<User>(model);
            user.IsEmailAdvertisingPermitted = true;
            user.IsSmsAdvertisingPermitted = true;

            using (var transaction = userService.BeginTransaction())
            {
                try
                {
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        //TODO: User Manager Exception Handling
                        throw new IdentityCreationException();
                    }
                    result = await userManager.AddToRoleAsync(user, "member");
                    if (!result.Succeeded)
                    {
                        //TODO: User Manager Exception Handling
                        throw new IdentityCreationException();
                    }
                    transaction.Commit();
                    //TODO: Add TableStorageService Here for Statistics
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            var credentials = await jwtHelper.GenerateJwtTokenAsync(user, TimeSpan.FromDays(30));
            return Response<string>.Create(credentials);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<Response<string>> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var user = await userManager.FindByNameAsync(loginRequest.Username);
            var correctPassword = user != null && await userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!correctPassword)
            {
                throw new WrongPasswordOrUsernameException();
            }
            var credentials = await jwtHelper.GenerateJwtTokenAsync(user, TimeSpan.FromDays(30));
            return Response<string>.Create(credentials);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("change-password")]
        [HttpPost]
        [ApiAuthorize]
        public async Task<Response> ChangePasswordAsync([FromBody] ChangePasswordRequest model)
        {
            var user = await userManager.FindByIdAsync(UserId);

            if (!await userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                throw new WrongPasswordException();
            }
            if (model.NewPassword != model.NewPasswordAgain)
            {
                throw new PasswordsNotSameException();
            }

            await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return EmptyResponse();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("forgot-password")]
        [HttpPost]
        public async Task<Response> ForgotPasswordAsync([FromBody] ForgotPasswordRequest model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await mailProvider.SendAsync(user.Email, user.FullName,
                projectConfiguration.SendGridForgotPasswordTemplateId, new
                {
                    redirectUrl = $"{projectConfiguration.WebsiteBaseUrl}/account/reset-password?email={user.UserName}&reset-token={token}"
                });
            return EmptyResponse();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("reset-password")]
        [HttpPost]
        public async Task<Response> ResetPasswordAsync([FromBody] ResetPasswordRequest model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return EmptyResponse();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("user-information")]
        [HttpPut]
        [ApiAuthorize(Roles = "member")]
        public async Task<Response> UpdateInformationAsync(UserUpdateRequest model)
        {
            var user = await userService.GetItemAsync(UserId);
            user = Map(model, user);
            await userService.UpdateItemAsync(user);
            return EmptyResponse();
        }
    }
}