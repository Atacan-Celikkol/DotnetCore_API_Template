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
    [Route("privacy-policies")]
    [ApiController]
    public class PrivacyPoliciesController : BaseController
    {
        private readonly PrivacyPolicyService _privacyPolicyService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="pps"></param>
        public PrivacyPoliciesController(IMapper mp, PrivacyPolicyService pps) : base(mp)
        {
            _privacyPolicyService = pps;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">For Admin</param>
        /// <returns></returns>
        [Route("~/common/privacy-policy")]
        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<Response<PrivacyPolicyResponse>> GetItemAsync(string id = null)
        {
            if (User.IsInRole("admin") && id != null)
            {
                var policy = MapResponse<PrivacyPolicyResponse>(await _privacyPolicyService.GetItemAsync(id));
                return policy;
            }
            var policys = await _privacyPolicyService.GetItemsAsync(0, 1, x => !x.IsDeleted, "Version", Data.Models.Enums.OrderType.Desc);
            if (policys.Data.Count == 0)
            {
                throw new Exception("Aktif gizlilik sözleşmesi bulunmamaktadır.");
            }
            return MapResponse<PrivacyPolicyResponse>(policys.Data[0]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [ApiAuthorize(Roles = "admin")]
        public async Task<Response<PrivacyPolicyResponse>> CreateItemAsync(PrivacyPolicyRequest request)
        {
            var model = Map<PrivacyPolicy>(request);
            var policy = await _privacyPolicyService.CreateItemAsync(model);
            var result = await _privacyPolicyService.GetItemAsync(policy.Id);
            return MapResponse<PrivacyPolicyResponse>(result);
        }
    }
}