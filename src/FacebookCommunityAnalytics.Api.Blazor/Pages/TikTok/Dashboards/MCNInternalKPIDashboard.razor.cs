

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.RichTextEdit;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Groups;
using FacebookCommunityAnalytics.Api.StaffEvaluations;
using FacebookCommunityAnalytics.Api.Tiktoks;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.TikTok.Dashboards
{
    public partial class MCNInternalKPIDashboard
    {
        private IReadOnlyList<TikTokChannelKpiModel> TikTokChannels { get; set; }
        // private Modal EditStaffEvaluationModal { get; set; }
        // private CreateUpdateStaffEvaluationDto EditingStaffEvaluation { get; set; }
        // private bool StaffEvaluationLevelEdit { get; set; }
        // private string CurrentSorting { get; set; }
        // private RichTextEdit richTextEditRef;
        // private RichTextEdit richTextTaskEditRef;
        // private Guid EditingStaffEvaluationId { get; set; }
        // private string ReviewContentEdit { get; set; }
        // private string AssignedContentEdit { get; set; }

        public MCNInternalKPIDashboard()
        {
            TikTokChannels = new List<TikTokChannelKpiModel>();
            //EditingStaffEvaluation = new CreateUpdateStaffEvaluationDto();
        }
        protected override async Task OnInitializedAsync()
        {
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
            }
        }
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TikTokChannelKpiModel> e)
        {
            await DoSearch();
            await InvokeAsync(StateHasChanged);
        }
        
        public async Task DoSearch()
        {
            var (fromDate, toDate) = GetPayrollDateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            TikTokChannels = await GroupsExtendAppService.GetTikTokChannelKpiModels(new GetTikTokChannelKpiRequest()
            {
                Month = toDate.Month,
                Year = toDate.Year
            });
        }
        
        // private void CloseEditStaffEvaluationModal()
        // {
        //     EditStaffEvaluationModal.Hide();
        //     EditingStaffEvaluation = new CreateUpdateStaffEvaluationDto();
        // }
        //
        // public async Task OnDirectorReviewContentChanged()
        // {
        //     ReviewContentEdit = await richTextEditRef.GetTextAsync();
        // }
        //
        // public async Task OnAssignedTasksContentChanged()
        // {
        //     AssignedContentEdit = await richTextTaskEditRef.GetTextAsync();
        // }
        //
        // private void OpenEditStaffEvaluationModal(StaffEvaluationDto input)
        // {
        //     EditingStaffEvaluationId = input.Id;
        //     EditingStaffEvaluation = ObjectMapper.Map<StaffEvaluationDto, CreateUpdateStaffEvaluationDto>(input);
        //     ReviewContentEdit = EditingStaffEvaluation.DirectorReview;
        //     AssignedContentEdit = EditingStaffEvaluation.AssignedTasks;
        //     EditStaffEvaluationModal.Show();
        // }
        //
        // private async Task UpdateStaffEvaluationAsync()
        // {
        //     var success = await Invoke
        //     (
        //         async () =>
        //         {
        //             EditingStaffEvaluation.DirectorReview = ReviewContentEdit;
        //             EditingStaffEvaluation.AssignedTasks = AssignedContentEdit;
        //             EditingStaffEvaluation.TotalPoint = EditingStaffEvaluation.QuantityKPI + EditingStaffEvaluation.QualityKPI + EditingStaffEvaluation.ReviewPoint;
        //             await StaffEvaluationAppService.UpdateAsync(EditingStaffEvaluationId, EditingStaffEvaluation);
        //             await DoSearch();
        //             EditStaffEvaluationModal.Hide();
        //         },
        //         L,
        //         true,
        //         BlazorComponentBaseActionType.Update
        //     );
        // }
    }
}