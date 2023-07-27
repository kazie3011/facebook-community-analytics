using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FacebookCommunityAnalytics.Api.Configs;
using FacebookCommunityAnalytics.Api.Core.Const;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.Localization;
using FluentDateTimeOffset;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Http.Client;
using ApiResource = FacebookCommunityAnalytics.Api.Localization.ApiResource;

namespace FacebookCommunityAnalytics.Api.Web.Pages
{
    public enum WebPageActionType
    {
        Create,
        Update,
        Delete,
        Get,
        GetList
    }
    /* Inherit your Page Model classes from this class.
     */
    public abstract class ApiPageModel : AbpPageModel
    {
        protected ApiPageModel()
        {
            LocalizationResourceType = typeof(ApiResource);
        }
        //[Inject] public IJSRuntime JsRuntime { get; set; }
        public GlobalConfiguration GlobalConfiguration { get; set; }
        public IStringLocalizer<ApiDomainResource> LD { get; set; }

        public bool IsPartnerRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Partner);
        }

        public bool IsPartnerTool()
        {
            return GlobalConfiguration.PartnerConfiguration.IsPartnerTool;
        }

        public bool IsAdminRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Admin);
        }

        public bool IsDirectorRole()
        {
            return IsAdminRole() || CurrentUser.IsInRole(RoleConsts.Director);
        }

        public bool IsManagerRole()
        {
            return IsDirectorRole() || CurrentUser.IsInRole(RoleConsts.Manager);
        }

        public bool IsSupervisorRole()
        {
            return IsManagerRole() || CurrentUser.IsInRole(RoleConsts.Supervisor);
        }

        public bool IsNotManagerRole()
        {
            return !IsManagerRole();
        }

        public bool IsSaleAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Sale);
        }

        public bool IsCampAndAboveRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.CampaignViewer) || CurrentUser.IsInRole(RoleConsts.CampaignCreator);
        }

        public bool IsSaleAdminRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.SaleAdmin);
        }

        public bool IsLeaderRole()
        {
            return IsSupervisorRole() || CurrentUser.IsInRole(RoleConsts.Leader);
        }

        public bool IsStaffRole()
        {
            return CurrentUser.IsInRole(RoleConsts.Staff);
        }
        
        protected async Task<bool> Invoke(Func<Task> action, IStringLocalizer L, bool showSuccessMessage = false, WebPageActionType actionType = WebPageActionType.Create)
        {
            try
            {
                await action();

                if (showSuccessMessage)
                {
                    switch (actionType)
                    {
                        case WebPageActionType.Create:
                            Alerts.Success(L["WebPageActionType.Create"]);
                            break;
                        case WebPageActionType.Update:
                            Alerts.Success(L["WebPageActionType.Update"]);
                            break;
                        case WebPageActionType.Delete:
                            Alerts.Success(L["WebPageActionType.Delete"]);
                            break;
                        case WebPageActionType.Get:
                            Alerts.Success(L["WebPageActionType.Get"]);
                            break;
                        case WebPageActionType.GetList:
                            Alerts.Success(L["WebPageActionType.GetList"]);
                            break;
                        default:
                            Alerts.Success(L["WebPageActionType.Success"]);
                            break;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                if (exception is AbpRemoteCallException abpRemoteCallException)
                {
                    if (abpRemoteCallException.Error?.ValidationErrors.IsNotNullOrEmpty() ?? false)
                    {
                        foreach (var ex in abpRemoteCallException.Error.ValidationErrors)
                        {
                            Alerts.Danger(ex.Message);
                        }
                    }
                    else
                    {
                        Alerts.Danger(exception.Message);
                    }
                }
                else
                {
                    Alerts.Danger(exception.Message);
                }

                return false;
            }
        }
    }
}