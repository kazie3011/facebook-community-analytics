using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.Contracts
{
    public class PagedResultExtendDto<T> : PagedResultDto<T>
    {
        public List<T> List { get; set; }
    }
}