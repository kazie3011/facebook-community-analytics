using System;
using System.Collections.Generic;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FacebookCommunityAnalytics.Api
{
    /* Inherit your application services from this class.
     */
    public abstract class ApiAppService : ApplicationService
    {
        public GlobalConfiguration GlobalConfiguration { get; set; }
        public IStringLocalizer<ApiDomainResource> LD { get; set; }

        protected ApiAppService()
        {
            LocalizationResource = typeof(ApiResource);
        }

        protected bool IsPartnerRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Partner);
        }

        protected bool IsAdminRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Admin);
        }

        protected bool IsDirectorRole()
        {
            return IsAdminRole() || CurrentUser.IsInRole(RoleConsts.Director);
        }

        protected bool IsManagerRole()
        {
            return IsDirectorRole() || CurrentUser.IsInRole(RoleConsts.Manager);
        }

        protected bool IsSupervisorRole()
        {
            return IsManagerRole() || CurrentUser.IsInRole(RoleConsts.Supervisor);
        }

        protected bool IsSaleAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Sale);
        }

        protected bool IsCampAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.CampaignCreator);
        }

        protected bool IsSaleAdminRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.SaleAdmin);
        }

        protected bool IsLeaderRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Leader);
        }

        protected bool IsStaffRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Staff);
        }

        protected KeyValuePair<DateTime, DateTime> GetPayrollDateTime(int year, int month)
        {
            var date = DateTime.SpecifyKind 
            ( 
                new DateTime 
                ( 
                    year, 
                    month, 
                    GlobalConfiguration.GlobalPayrollConfiguration.PayrollStartDay, 
                    0, 
                    0, 
                    0 
                ), 
                DateTimeKind.Utc 
            ); 
             
            var toDate = DateTime.SpecifyKind 
            ( 
                new DateTime 
                ( 
                    year, 
                    month, 
                    GlobalConfiguration.GlobalPayrollConfiguration.PayrollEndDay, 
                    23, 
                    59, 
                    59 
                ), 
                DateTimeKind.Utc 
            );

            var fromDate = date.AddMonths(-1);

            var fromDateTime = fromDate.AddHours(GlobalConfiguration.GlobalPayrollConfiguration.PayrollTimeZone);
            var toDateTime = toDate.AddHours(GlobalConfiguration.GlobalPayrollConfiguration.PayrollTimeZone);

            return new KeyValuePair<DateTime, DateTime>(fromDateTime, toDateTime);
        }
    }

    public abstract class BaseCrudApiAppService<TEntity, TEntityDto, TKey, TGetInput, TCreateUpdateDto> :
        CrudAppService<TEntity, TEntityDto, TKey, TGetInput, TCreateUpdateDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        public GlobalConfiguration GlobalConfiguration { get; set; }
        public IStringLocalizer<ApiDomainResource> LD { get; set; }

        protected BaseCrudApiAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
            LocalizationResource = typeof(ApiResource);
        }

        protected bool IsPartnerRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Partner);
        }

        protected bool IsAdminRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Admin);
        }

        protected bool IsDirectorRole()
        {
            return IsAdminRole() || CurrentUser.IsInRole(RoleConsts.Director);
        }

        protected bool IsManagerRole()
        {
            return IsDirectorRole() || CurrentUser.IsInRole(RoleConsts.Manager);
        }

        protected bool IsSupervisorRole()
        {
            return IsManagerRole() || CurrentUser.IsInRole(RoleConsts.Supervisor);
        }

        protected bool IsSaleAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Sale);
        }

        protected bool IsCampAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.CampaignViewer) || CurrentUser.IsInRole(RoleConsts.CampaignCreator);
        }

        protected bool IsSaleAdminRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.SaleAdmin);
        }

        protected bool IsLeaderRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Leader);
        }

        protected bool IsStaffRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Staff);
        }
    }
}