using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public class ProjectConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly bool _isLocalHost;
        private readonly bool _isDevelopment;
        private readonly bool _isProduction;
        private readonly bool _isStaging;
        private readonly bool _useLocalDb;
        private readonly string _environmentName;

        public ProjectConfiguration(IConfiguration configuration, string environmentName,
            bool localhost, bool development, bool staging, bool production, bool useLocalDb = false)
        {
            _configuration = configuration;
            _isDevelopment = development;
            _isLocalHost = localhost;
            _useLocalDb = useLocalDb;
            _isProduction = production;
            _isStaging = staging;
            _environmentName = environmentName;
        }

        #region Environments

        public bool IsLocalhost => _isLocalHost;
        public bool UseLocalDb => _useLocalDb;
        public bool IsDevelopment => _isDevelopment;
        public bool IsProduction => _isProduction;
        public bool IsStaging => _isStaging;

        public string EnvironmentName => _environmentName;
        #endregion Environments



        #region ConnectionStrings

        public string DefaultConnection => _configuration.GetConnectionString("DefaultConnection");
        public string StorageConnection => _configuration.GetConnectionString("StorageConnection");
        public string LocalStorageConnection => _configuration.GetConnectionString("LocalStorageConnection");
        public string RedisConnection => _configuration.GetConnectionString("RedisConnection");
        public string AzureWebJobsServiceBus => _configuration.GetConnectionString("AzureWebJobsServiceBus");

        #endregion ConnectionStrings

        #region DbSettings

        public bool IsAutoMigrationEnabled => !string.IsNullOrEmpty(_configuration["Data:AutoMigrationEnabled"]) &&
            Convert.ToBoolean(_configuration["Data:AutoMigrationEnabled"]);

        public bool AllowDataLoss => !string.IsNullOrEmpty(_configuration["Data:AllowDataLoss"]) &&
            Convert.ToBoolean(_configuration["Data:AllowDataLoss"]);

        #endregion DbSettings

        #region Environment Urls

        public string ApiBaseUrl => _configuration["EndPoints:ApiBaseUrl"];
        public string WebsiteBaseUrl => _configuration["EndPoints:WebsiteBaseUrl"];
        public string AdminBaseUrl => _configuration["EndPoints:AdminBaseUrl"];
        public string StorageBaseUrl => _configuration["EndPoints:StorageBaseUrl"];

        #endregion Environment Urls

        #region External Api Keys

        public string AutoPaperApiKey => _configuration["AutoPaperApiKey"];
        public string SendGridApiKey => _configuration.GetValue<string>("SendGrid:ApiKey");

        #endregion External Api Keys

        #region SendGridTemplates

        public string SendGridWelcomeTemplateId => _configuration["SendGrid:WelcomeTemplateId"];
        public string SendGridForgotPasswordTemplateId => _configuration["SendGrid:ForgotPasswordTemplateId"];

        #endregion SendGridTemplates

        #region Extra Info

        public string PushNotificationEndpoint => _configuration["PushNotification:Endpoint"];
        public string PushNotificationHubName => _configuration["PushNotification:HubName"];
        public string DefaultProxy => _configuration["DefaultProxy"];

        #endregion Extra Info

        #region Allowed File Types

        public List<string> AllowedMimeTypes =>
            string.IsNullOrEmpty(_configuration["AllowedMimeTypes"])
            ? new List<string>()
            : _configuration["AllowedMimeTypes"].Split(',').ToList();

        public List<string> AllowedVideoMimeTypes =>
           string.IsNullOrEmpty(_configuration["AllowedVideoMimeTypes"])
           ? new List<string>()
           : _configuration["AllowedVideoMimeTypes"].Split(',').ToList();

        #endregion Allowed File Types
    }
}