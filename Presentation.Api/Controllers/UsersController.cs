using AutoMapper;
using Core.Exceptions;
using Core.Extensions;
using Core.Utils;
using Data.Models.Identity;
using Data.Models.Mapping.Requests;
using Data.Models.Mapping.Responses;
using Data.Models.Mapping.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Filters;
using Providers.File;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Presentation.Api.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiAuthorize]
    [Route("users")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly ProjectConfiguration _projectConfiguration;
        private readonly IFileProvider fileProvider;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="um"></param>
        /// <param name="us"></param>
        /// <param name="pc"></param>
        /// <param name="fp"></param>
        public UsersController(IMapper mp, UserManager<User> um, UserService us, ProjectConfiguration pc, IFileProvider fp) : base(mp)
        {
            _userManager = um;
            _userService = us;
            _projectConfiguration = pc;
            fileProvider = fp;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Response<UserResponse>> GetUserAsync(string id)
        {
            id = User.IsInRole("admin") && id != "me" ? id : UserId;

            var user = await _userService.GetItemAsync(id);
            var roles = await _userService.GetUserRolesAsync(id);
            var userResponse = Map<UserResponse>(user);
            userResponse.Roles = Map<List<RoleResponse>>(roles);
            return CreateResponse(userResponse);
        }

        /// <summary>
        /// Searches in Id, PhoneNumber, DisplayName.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [ApiAuthorize(Roles = "admin")]
        public async Task<PaginatedResponse<UserResponse>> SearchUsersAsync([FromQuery]SearchQueryParams search = null)
        {
            search ??= new SearchQueryParams();
            var users = await _userService.SearchAsync(search.page, search.size, search.q, search.sort_by, search.order_by);
            return PaginatedMapResponse<User, UserResponse>(users);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("")]
        [ApiAuthorize(Roles = "admin")]
        public async Task<Response<UserResponse>> CreateUserAsync(UserRequest request)
        {
            var user = Map<User>(request);
            using (var transaction = _userService.BeginTransaction())
            {
                try
                {
                    var password = request.GeneratePassword ? VoucherCodeGenerator.GenerateVoucher(8) : request.Password;
                    var result = await _userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Kayıt sırasında hata oluştu");
                    }
                    user = await _userManager.FindByIdAsync(user.Id);
                    result = await _userManager.AddToRoleAsync(user, "agency");
                    if (!result.Succeeded)
                    {
                        throw new Exception("Kayıt sırasında hata oluştu");
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                transaction.Commit();
                //TODO: Send Email from Here
                return MapResponse<UserResponse>(user);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ApiAuthorize(Roles = "admin")]
        public async Task<Response> DeleteUserAsync(string id)
        {
            await _userService.DeleteItemAsync(id);
            return EmptyResponse();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("me/profile-picture")]
        [ImageValidationFilter(ControlType = ControlType.OnlyMinimums, Height = 200)]
        public async Task<Response<string>> UploadProfilePictureAsync(IFormFile file)
        {
            var user = await _userService.GetItemAsync(UserId);
            if ((!_projectConfiguration.AllowedMimeTypes.Contains(file.ContentType) && !_projectConfiguration.AllowedVideoMimeTypes.Contains(file.ContentType))
            || file.Length == 0)
            {
                throw new ValidationException();
            }

            Uri fileUri;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();
                fileUri = await fileProvider.SaveFileAsync("users", $"{UserId}.jpg", bytes);
            }
            user.ImageUrl = fileUri.AbsolutePath;
            await _userService.UpdateItemAsync(user);
            return CreateResponse(fileUri.ToString());
        }
    }
}