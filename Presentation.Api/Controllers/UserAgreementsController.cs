using AutoMapper;
using Data.Models.Common;
using Data.Models.Mapping.Requests.Common;
using Data.Models.Mapping.Responses.Common;
using Data.Models.Mapping.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Filters;
using Services;
using System;
using System.Threading.Tasks;

namespace Presentation.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("user-agreements")]
    [ApiController]
    public class UserAgreementsController : BaseController
    {
        private readonly IUserAgreementService _userAgreementService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="uas"></param>
        public UserAgreementsController(IMapper mp, IUserAgreementService uas) : base(mp)
        {
            _userAgreementService = uas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">For Admin</param>
        /// <returns></returns>
        [Route("~/common/user-agreement")]
        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<Response<UserAgreementResponse>> GetItemAsync(string id = null)
        {
            if (User.IsInRole("admin") && id != null)
            {
                var agreement = MapResponse<UserAgreementResponse>(await _userAgreementService.GetItemAsync(id));
                return agreement;
            }
            var agreements = await _userAgreementService.GetItemsAsync(0, 1, x => !x.IsDeleted, "Version", Data.Models.Enums.OrderType.Desc);
            if (agreements.Data.Count == 0)
            {
                throw new Exception("Aktif kullanıcı sözleşmesi bulunmamaktadır.");
            }
            return MapResponse<UserAgreementResponse>(agreements.Data[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [ApiAuthorize(Roles = "admin")]
        public async Task<Response<UserAgreementResponse>> CreateItemAsync(UserAgreementRequest request)
        {
            var model = Map<UserAgreement>(request);
            var agreement = await _userAgreementService.CreateItemAsync(model);
            var result = await _userAgreementService.GetItemAsync(agreement.Id);
            return MapResponse<UserAgreementResponse>(result);
        }

    }
}
