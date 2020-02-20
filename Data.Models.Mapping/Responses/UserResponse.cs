using Data.Models.Enums;
using System;
using System.Collections.Generic;

namespace Data.Models.Mapping.Responses
{
    public class UserSimpleResponse : BaseResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }

    public class UserResponse : UserSimpleResponse
    {
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string ImageUrl { get; set; }
        public List<RoleResponse> Roles { get; set; }
        public int SweepstakePoints { get; set; }
        public bool IsSurveyAvailable { get; set; }
        public bool IsRegisteredForWifi { get; set; }
        public string Country { get; set; }

        #region Advertising
        public bool IsSmsAdvertisingPermitted { get; set; }
        public bool IsEmailAdvertisingPermitted { get; set; }
        #endregion
    }
}
