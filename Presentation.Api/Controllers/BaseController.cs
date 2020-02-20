using AutoMapper;
using Data.Extensions;
using Data.Models.Mapping.Wrappers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Presentation.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/Base")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IMapper Mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mp"></param>
        public BaseController(IMapper mp)
        {
            Mapper = mp;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserId
        {
            get
            {
                if (User == null) return null;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims;
                return claims.Where(t => t.Type.Equals("UserId")).Select(t => t.Value).FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Culture
        {
            get
            {
                var rqf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                // Culture contains the information of the requested culture
                var culture = rqf.RequestCulture.Culture;
                return culture.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Email
        {
            get
            {
                if (User == null) return null;
                var identity = (ClaimsIdentity)User.Identity;
                var claims = identity.Claims;
                return claims.Where(t => t.Type.Equals("Email")).Select(t => t.Value).FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Response<T> MapResponse<T>(object value)
        {
            return Response<T>.Create(Mapper.Map<T>(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Response<T> CreateResponse<T>(T value)
        {
            return Response<T>.Create(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        protected TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        protected TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="paginatedData"></param>
        /// <returns></returns>
        protected PaginatedResponse<T> PaginatedMapResponse<TInput, T>(PaginatedData<TInput> paginatedData)
        {

            return PaginatedResponse<T>.Create(Mapper.Map<List<T>>(paginatedData.Data),
                paginatedData.TotalItemCount,
                paginatedData.CurrentPage,
                paginatedData.Size,
                paginatedData.NextPage,
                paginatedData.PreviousPage);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Response EmptyResponse()
        {
            return Data.Models.Mapping.Wrappers.Response.Create();
        }

    }
}
