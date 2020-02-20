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
    [Route("faqs")]
    [ApiController]
    public class FAQsController : BaseController
    {
        private readonly FAQService _faqService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="faqs"></param>
        public FAQsController(IMapper mp, FAQService faqs) : base(mp)
        {
            _faqService = faqs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">For Admin</param>
        /// <returns></returns>
        [Route("~/common/faq")]
        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<Response<FAQResponse>> GetItemAsync(string id = null)
        {
            if (User.IsInRole("admin") && id != null)
            {
                var faq = MapResponse<FAQResponse>(await _faqService.GetItemAsync(id));
                return faq;
            }
            var faqs = await _faqService.GetItemsAsync(0, 1, x => !x.IsDeleted, "Version", Data.Models.Enums.OrderType.Desc);
            if (faqs.Data.Count == 0)
            {
                throw new Exception("Aktif FAQ bulunmamaktadır.");
            }
            return MapResponse<FAQResponse>(faqs.Data[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [ApiAuthorize(Roles = "admin")]
        public async Task<Response<FAQResponse>> CreateItemAsync(FAQRequest request)
        {
            var model = Map<FAQ>(request);
            var faq = await _faqService.CreateItemAsync(model);
            var result = await _faqService.GetItemAsync(faq.Id);
            return MapResponse<FAQResponse>(result);
        }

    }
}
