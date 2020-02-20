using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Models.Mapping.Responses;
using Data.Models.Mapping.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Presentation.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("common")]
    [AllowAnonymous]
    public class CommonController : BaseController
    {
        private readonly IOptions<RequestLocalizationOptions> requestLocalizationOptions;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="rlo"></param>
        public CommonController(IMapper mp, IOptions<RequestLocalizationOptions> rlo) : base(mp)
        {
            requestLocalizationOptions = rlo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("supported-languages")]
        [HttpGet]
        public Response<List<SupportedLanguageResponse>> GetSupportedLanguages()
        {
            return MapResponse<List<SupportedLanguageResponse>>(requestLocalizationOptions.Value.SupportedCultures);
        }
    }
}
