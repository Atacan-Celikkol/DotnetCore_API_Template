using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Extensions;
using Core.Utils;
using Data.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Data.Models.Identity
{
    public class User : IdentityUser, IAuditedEntity<string>, IBaseEntity
    {
        public User()
        {
            Id = SequentialGuid.NewGuid().ToString();
            CreateDate = DateTimeOffset.UtcNow;
            IsActive = true;
            VoucherId = VoucherCodeGenerator.GenerateVoucher(10);
        }

        #region Base Properties
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        #endregion

        #region Personal Information
        [MaxLength(15)]
        public string VoucherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string ImageUrl { get; set; }
        public Gender Gender { get; set; }
        #endregion

        #region Login
        public int LoginAttempts { get; set; }
        public DateTimeOffset? LastCodeSendDate { get; set; }
        #endregion

        #region Advertising
        public bool IsSmsAdvertisingPermitted { get; set; }
        public bool IsEmailAdvertisingPermitted { get; set; }
        #endregion

        [NotMapped]
        public string FullName => FirstName + ' ' + LastName;
    }
}