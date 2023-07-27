using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.JQuery;
using Volo.Abp.Modularity;

namespace FacebookCommunityAnalytics.Api.Web.Bundling
{
    [DependsOn(
        typeof(JQueryScriptContributor)
    )]
    public class ApiExtendScriptContributor : BundleContributor
    { 
        public override void ConfigureBundle(BundleConfigurationContext context)
        {
            context.Files.AddIfNotContains("/datetimerangepicker/js/moment.min.js");
            //Data table
            context.Files.AddIfNotContains("/libs/datatables.net-select/js/dataTables.select.js");
            context.Files.AddIfNotContains("/libs/jquery-datatables-checkboxes/js/dataTables.checkboxes.js");
            context.Files.AddIfNotContains("/libs/datatables.net-select/js/dataTables.select.js");
            context.Files.AddIfNotContains("/libs/datatables.net-select-bs4/js/select.bootstrap4.js");
            
            //Datetime range picker
            context.Files.AddIfNotContains("/datetimerangepicker/js/daterangepicker.min.js");
            
            context.Files.AddIfNotContains("/scripts/global.js");
        }
    
    }
}