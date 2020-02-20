using AutoMapper;
using Data.Models.Identity;
using Data.Models.Mapping.Helpers;
using Data.Models.Mapping.Requests;
using Data.Models.Mapping.Responses;
using System.Globalization;
using NetTopologySuite.Geometries;
using Data.Models.Common;
using Data.Models.Mapping.Resolvers;
using Data.Models.Mapping.Requests.Common;
using Data.Models.Mapping.Responses.Common;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Data.Models.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<User, UserSimpleResponse>();
            CreateMap<User, UserResponse>()
                .ForMember(t => t.ImageUrl, opt => opt.ConvertUsing<ImageValueConverter, string>(src => src.ImageUrl))
                .IncludeBase<User, UserSimpleResponse>();
            CreateMap<UserRequest, User>();
            CreateMap<RegisterRequest, User>()
                .ForMember(t => t.UserName, opt => opt.MapFrom(src => src.PhoneNumber));
            CreateMap<UserUpdateRequest, User>();

            CreateMap<Role, RoleResponse>();

            #region Common
            CreateMap<CultureInfo, SupportedLanguageResponse>()
                .ForMember(t => t.Code, opt => opt.MapFrom(src => src.Name))
                .ForMember(t => t.Name, opt => opt.MapFrom(src => src.EnglishName));

            CreateMap<UserAgreementRequest, UserAgreement>();
            CreateMap<UserAgreement, UserAgreementResponse>();

            CreateMap<AboutUsRequest, AboutUs>();
            CreateMap<AboutUs, AboutUsResponse>();

            CreateMap<FAQRequest, FAQ>();
            CreateMap<FAQ, FAQResponse>();

            CreateMap<PrivacyPolicyRequest, PrivacyPolicy>();
            CreateMap<PrivacyPolicy, PrivacyPolicyResponse>();
            #endregion

        }
    }
}
