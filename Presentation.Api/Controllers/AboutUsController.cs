using AutoMapper;
using Data.Models.Common;
using Data.Models.Mapping.Requests;
using Data.Models.Mapping.Responses;
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
    [Route("about-us")]
    [ApiController]
    public class AboutUsController : BaseController
    {
        private readonly IAboutUsService _aboutUsService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="uas"></param>
        public AboutUsController(IMapper mp, IAboutUsService uas) : base(mp)
        {
            _aboutUsService = uas;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">For Admin</param>
        /// <returns></returns>
        [Route("~/common/about-us")]
        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<Response<AboutUsResponse>> GetItemAsync(string id = null)
        {
            if (User.IsInRole("admin") && id != null)
            {
                var agreement = MapResponse<AboutUsResponse>(await _aboutUsService.GetItemAsync(id));
                return agreement;
            }
            var agreements = await _aboutUsService.GetItemsAsync(0, 1, x => !x.IsDeleted, "Version", Data.Models.Enums.OrderType.Desc);
            if (agreements.Data.Count == 0)
            {
                throw new Exception("Aktif 'Hakkımızda' bulunmamaktadır.");
            }
            return MapResponse<AboutUsResponse>(agreements.Data[0]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [ApiAuthorize(Roles = "admin")]
        public async Task<Response<AboutUsResponse>> CreateItemAsync(AboutUsRequest request)
        {
            var model = Map<AboutUs>(request);
            var agreement = await _aboutUsService.CreateItemAsync(model);
            var result = await _aboutUsService.GetItemAsync(agreement.Id);
            return MapResponse<AboutUsResponse>(result);
        }
    }
}