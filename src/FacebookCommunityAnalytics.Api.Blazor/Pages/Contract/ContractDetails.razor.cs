using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using FacebookCommunityAnalytics.Api.Blazor.Shared;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.ContractTransactions;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Medias;
using FacebookCommunityAnalytics.Api.Shared;
using FacebookCommunityAnalytics.Api.TeamMembers;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Modal = Blazorise.Modal;

namespace FacebookCommunityAnalytics.Api.Blazor.Pages.Contract
{
    public partial class ContractDetails
    {
        [Parameter] public string                                               ContractId { get; set; }
        protected          List<Volo.Abp.BlazoriseUI.BreadcrumbItem>            BreadcrumbItems = new();
        protected          PageToolbar                                          Toolbar      { get; } = new();
        private            ContractDto                                          _contractDto { get; set; }
        private            List<MediaDto>                                       _mediaImages;
        private            List<FileModel>                                      _mediaPDFs;
        private            List<FileModel>                                      _mediaOthers;
        private            List<MediaDto>                                       MediaModelResults            { get; set; }
        private            List<MediaDto>                                       MediaModelTransactionResults { get; set; }
        private            RadzenButton                                         _buttonUpload                { get; set; }
        private            RadzenButton                                         _buttonUploadNewTransaction  { get; set; }
        private            RadzenButton                                         _buttonUploadEditTransaction { get; set; }
        private            Modal                                                NewTransactionModal          { get; set; }
        private            Modal                                                EditTransactionModal         { get; set; }
        private            Guid                                                 EditTransactionId            { get; set; }
        private            string                                               _createTransactionModalTitle { get; set; }
        private            string                                               _editTransactionModelTitle   { get; set; }
        private            CreateUpdateContractTransactionDto                   NewContractTransaction       { get; set; }
        private            CreateUpdateContractTransactionDto                   EditContractTransaction      { get; set; }
        private            List<ContractTransactionWithNavigationPropertiesDto> ContractTransactionDtos      { get; set; }
        private            List<LookupDto<Guid?>>                               SalePersonsLookupDtos        { get; set; }
        private            ContractDto                                          _contractCreationDto         { get; set; }
        private            List<GroupCostDto>                                   ActiveGroupCosts             { get; set; } = new();
        private            IEnumerable<Guid>                                    NewContractGroupCostIds      { get; set; }
        private            List<GroupCostDto>                                   GroupCosts                   { get; set; }
        private            bool                                                 CanEditContract              { get; set; } = true;
        private            bool                                                 CanDeleteContract            { get; set; } = true;
        private            IEnumerable<Guid>                                    EditTransactionGroupCostIds  { get; set; }
        private            List<GroupCostDto>                                   EditGroupCosts               { get; set; }
        private            List<GroupCostDto>                                   ChangedGroupCosts            { get; set; }
        private            bool                                                 _showLoading                 { get; set; }
        private            string                                               _selectedTab;
        private            int                                                  _pdfIndex   = 1;
        private            int                                                  _otherIndex = 1;
        private            bool                                                 _isValidUploadTransactionImage;
        private            bool                                                 _isVATReadonly { get; set; }
        private            DateTime?                                            createPaymentDueDate;
        private            DateTime?                                            editPaymentDueDate;
        private            bool                                                 IsMasterContract { get; set; }
        private            TextEdit                                             NewCostField  = new();
        private            TextEdit                                             EditCostField = new();

        public ContractDetails()
        {
            _contractDto                   = new ContractDto();
            _contractCreationDto           = new ContractDto();
            _mediaImages                   = new List<MediaDto>();
            _mediaPDFs                     = new List<FileModel>();
            _mediaOthers                   = new List<FileModel>();
            _selectedTab                   = "ContractImageTab";
            NewTransactionModal            = new Modal();
            EditTransactionModal           = new Modal();
            MediaModelResults              = new List<MediaDto>();
            SalePersonsLookupDtos          = new List<LookupDto<Guid?>>();
            ContractTransactionDtos        = new List<ContractTransactionWithNavigationPropertiesDto>();
            NewContractTransaction         = new CreateUpdateContractTransactionDto();
            EditContractTransaction        = new CreateUpdateContractTransactionDto();
            _isValidUploadTransactionImage = true;
            GroupCosts                     = new();
            ChangedGroupCosts              = new();
            EditGroupCosts                 = new();
        }

        protected override async Task OnInitializedAsync()
        {
            BrowserDateTime = await GetBrowserDateTime();
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await InitData();
            await GetSalePersonsLookupAsync();
            _createTransactionModalTitle = L["ContractDetails.NewTransaction.Title"].Value + " - " + _contractDto?.ContractCode;
            _editTransactionModelTitle   = L["ContractDetails.EditTransaction.Title"].Value + " - " + _contractDto?.ContractCode;
            await GetTransactions();
            await GetGroupCosts();
            CheckIsMasterContract();
        }

        private void CheckIsMasterContract()
        {
            IsMasterContract = _contractDto is { ContractType: ContractType.Master };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitPage();
            }
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton
                (
                 L["ContractDetail.CreateTransaction"],
                 async () =>
                 {
                     await OpenNewTransactionModal();
                 },
                 IconName.Add
                );
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Contract"], "/contracts"));
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ContractDetails"]));
            return ValueTask.CompletedTask;
        }

        private async Task InitData()
        {
            _mediaImages = new List<MediaDto>();
            _mediaPDFs = new List<FileModel>();
            _mediaOthers = new List<FileModel>();
            _otherIndex = 1;
            _pdfIndex = 1;
            
            _contractDto   = await ContractAppService.GetAsync(ContractId.ToGuidOrDefault());
            _isVATReadonly = _contractDto.ContractType != ContractType.Master;
            _mediaImages   = _contractDto.MediasDtos.Where(_ => _.FileContentType.IsNotNullOrEmpty() && _.FileContentType.Contains("image")&&_.ThumbnailUrl.IsNotNullOrEmpty()).ToList();

             var medias = await _mediaAppService.GetContentMedias(_mediaImages.Select(x=>x.ThumbnailUrl).ToList());

             foreach (var item in _mediaImages)
             {
                 item.ImagePath = medias.FirstOrDefault(x => x.Key == item.ThumbnailUrl).Value;
             }
             
            foreach (var mediaDto in _contractDto.MediasDtos.Where(_ => _.FileContentType.IsNotNullOrEmpty() && _.FileContentType.Contains("pdf")))
            {
                var clientTime = await ConvertToBrowserDateTime(mediaDto.CreationTime);
                _mediaPDFs.Add
                    (
                     new FileModel
                     {
                         Index        = _pdfIndex++,
                         FileName     = mediaDto.FileName,
                         CreationTime = clientTime.Humanize(),
                         Url          = mediaDto.Url,
                         MediaId      = mediaDto.Id
                     }
                    );
            }

            foreach (var mediaDto in _contractDto.MediasDtos.Where(_ => _.FileContentType.IsNullOrEmpty() || (!_.FileContentType.Contains("image") && !_.FileContentType.Contains("pdf"))))
            {
                var clientTime = await ConvertToBrowserDateTime(mediaDto.CreationTime);
                _mediaOthers.Add
                    (
                     new FileModel
                     {
                         Index        = _otherIndex++,
                         FileName     = mediaDto.FileName,
                         CreationTime = clientTime.Humanize(),
                         Url          = mediaDto.Url,
                         MediaId      = mediaDto.Id
                     }
                    );
            }

            if (_mediaImages.IsNullOrEmpty() && _mediaPDFs.IsNotNullOrEmpty()) _selectedTab = "ContractPDFTab";
            if (_mediaImages.IsNullOrEmpty() && _mediaPDFs.IsNullOrEmpty()) _selectedTab    = "ContractOtherTab";
        }
        private async Task GetTransactions()
        {
            ContractTransactionDtos = await ContractAppService.GetTransactionsByContractId(ContractId.ToGuidOrDefault());
        }

        //**********************************Upload Image*****************************
        int          progress;
        string       info;
        RadzenUpload createUpload;
        private int  OneMb;

        private void OnChange(UploadChangeEventArgs args, string name)
        {
        }

        private void OnProgress(UploadProgressArgs args, string name)
        {
            this.info     = $"% '{name}' / {args.Loaded} of {args.Total} bytes.";
            this.progress = args.Progress;
            if (args.Progress != 100)
            {
                return;
            }
        }

        private async Task CreateUploadAsync()
        {
            _showLoading           = true;
            _buttonUpload.Disabled = true;
            await createUpload.Upload();
        }

        public async Task OnComplete(UploadCompleteEventArgs args)
        {
            try
            {
                MediaModelResults = JsonConvert.DeserializeObject<List<MediaDto>>(args.RawResponse);
                if (MediaModelResults != null)
                {
                    _contractDto.MediaIds.AddRange(MediaModelResults.Select(x => x.Id));
                }

                await UpdateContract();
                await Message.Success(L["ContractDetails.UploadFiles.Success"]);

                //await this.InvokeAsync(StateHasChanged);
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
            catch (Exception)
            {
                // ignored
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
        }

        private void OnError(UploadErrorEventArgs args)
        {
            var x = args.Message;
        }

        private async Task RemoveImage(Guid mediaId)
        {
            await _mediaAppService.DeleteAsync(mediaId);
            _contractDto.MediaIds.Remove(mediaId);
            await UpdateContract();
        }

        private async Task DownloadFile(Guid id)
        {
            var result = await _mediaAppService.DownloadFileAsync(id);
            if (result is null) return;
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", result.FileName, result.ContentType, result.FileBytes);
        }

        //**********************************Upload Image In Transaction Creation*****************************
        int          progressTransaction;
        string       infoTransaction;
        RadzenUpload createUploadNewTransaction;
        RadzenUpload createUploadEditTransaction;
        //private int OneMb;

        private void OnChangeTransaction(UploadChangeEventArgs args, string name)
        {
            _isValidUploadTransactionImage = !args.Files.IsNotNullOrEmpty();
        }

        private void OnProgressTransaction(UploadProgressArgs args, string name)
        {
            this.infoTransaction     = $"% '{name}' / {args.Loaded} of {args.Total} bytes.";
            this.progressTransaction = args.Progress;
            if (args.Progress != 100)
            {
                return;
            }
        }

        private async Task CreateUploadNewTransactionAsync()
        {
            //_showLoading = true;
            // _buttonUploadNewTransaction.Disabled = true;
            _isValidUploadTransactionImage = true;
            await createUploadNewTransaction.Upload();
        }

        private async Task CreateUploadEditTransactionAsync()
        {
            //_showLoading = true;
            // _buttonUploadEditTransaction.Disabled = true;
            _isValidUploadTransactionImage = true;
            await createUploadEditTransaction.Upload();
        }

        public async Task OnCompleteTransaction(UploadCompleteEventArgs args)
        {
            try
            {
                MediaModelTransactionResults = JsonConvert.DeserializeObject<List<MediaDto>>(args.RawResponse);
            }
            catch (Exception)
            {
                // ignored
                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            }
        }

        private void OnErrorTransaction(UploadErrorEventArgs args)
        {
            var x = args.Message;
        }

        private async Task UpdateContract()
        {
            var success = await Invoke
                              (
                               async () =>
                               {
                                   await ContractAppService.UpdateAsync(ContractId.ToGuidOrDefault(), ObjectMapper.Map<ContractDto, CreateUpdateContractDto>(_contractDto));
                                   await InitData();
                               },
                               L,
                               true,
                               BlazorComponentBaseActionType.Update
                              );
        }

        private void OnSelectedTabChanged(string name)
        {
            _selectedTab = name;
        }

        private async Task OpenNewTransactionModal()
        {
            MediaModelTransactionResults = new List<MediaDto>();
            NewContractTransaction       = new CreateUpdateContractTransactionDto() { PaymentDueDate = DateTime.UtcNow };
            if (_contractDto.ContractType != ContractType.Master) NewContractTransaction.VATPercent = _contractDto.VATPercent;
            NewTransactionModal.Show();
            if (IsMasterContract)
            {
                NewContractGroupCostIds = new List<Guid>();
                await GetTransactions();
                await GetGroupCosts();
            }
        }

        private async Task CreateTransaction()
        {
            var success = await Invoke
                              (
                               async () =>
                               {
                                   NewContractTransaction.ContractId   = ContractId.ToGuidOrDefault();
                                   NewContractTransaction.CreatedAt    = DateTime.UtcNow;
                                   NewContractTransaction.SalePersonId = _contractDto.SalePersonId;
                                   await ContractAppService.CreateTransaction(NewContractTransaction);
                                   await InitData();
                                   await GetTransactions();
                                   NewTransactionModal.Hide();
                               },
                               L,
                               true,
                               BlazorComponentBaseActionType.Create
                              );
        }

        private void CloseNewTransactionModal()
        {
            NewTransactionModal.Hide();
        }

        private async Task OpenEditTransactionModal(ContractTransactionDto input)
        {
            MediaModelTransactionResults = new List<MediaDto>();
            EditTransactionId            = input.Id;
            EditContractTransaction      = ObjectMapper.Map<ContractTransactionDto, CreateUpdateContractTransactionDto>(input);
            if (input.PaymentDueDate.HasValue)
            {
                editPaymentDueDate = await ConvertUniversalToBrowserDateTime(input.PaymentDueDate.Value);
            }

            if (_contractDto.ContractType != ContractType.Master) EditContractTransaction.VATPercent = _contractDto.VATPercent;
            if (IsMasterContract)
            {
                await GetGroupCosts();
                EditGroupCosts = ActiveGroupCosts.Clone();
                foreach (var item in EditGroupCosts)
                {
                    var contractGroupCostInfo = EditContractTransaction.GroupCostInfos.FirstOrDefault(_ => _.Id == item.Id);
                    if (contractGroupCostInfo is null) continue;
                    ChangedGroupCosts.Add(item);
                    item.Cost = contractGroupCostInfo.Cost;
                }

                foreach (var item in EditContractTransaction.GroupCostInfos)
                {
                    if (EditGroupCosts.Select(_ => _.Id).Contains(item.Id)) continue;
                    var groupCost = GroupCosts.FirstOrDefault(_ => _.Id == item.Id);
                    if (groupCost is null) continue;
                    groupCost.Cost = item.Cost;
                    EditGroupCosts.Add(groupCost);
                }

                EditTransactionGroupCostIds = EditContractTransaction.GroupCostInfos.Select(_ => _.Id).ToList();
            }

            EditTransactionModal.Show();
        }

        private async Task UpdateTransaction()
        {
            var success = await Invoke
                              (
                               async () =>
                               {
                                   await ContractAppService.UpdateTransaction(EditTransactionId, EditContractTransaction);
                                   await GetTransactions();
                                   EditTransactionModal.Hide();
                               },
                               L,
                               true,
                               BlazorComponentBaseActionType.Update
                              );
        }

        private void CloseEditTransactionModal()
        {
            EditContractTransaction = new CreateUpdateContractTransactionDto();
            EditTransactionModal.Hide();
        }

        private async Task DeleteTransaction(Guid transactionId)
        {
            var confirmResult = await _uiMessageService.Confirm(L["DeleteConfirmationMessage"]);
            if (confirmResult)
            {
                var success = await Invoke
                                  (
                                   async () =>
                                   {
                                       await ContractAppService.DeleteTransaction(transactionId);
                                       await GetTransactions();
                                   },
                                   L,
                                   true,
                                   BlazorComponentBaseActionType.Delete
                                  );
            }
        }

        private async Task GetSalePersonsLookupAsync()
        {
            SalePersonsLookupDtos = await ContractAppService.GetAppUserLookupAsync(new GetMembersApiRequest { TeamName = TeamMemberConsts.Sale });
        }

        private Task OnNewTransactionValueChanged(string transactionValue)
        {
            NewContractTransaction.PartialPaymentValue = transactionValue.ToDecimalOrDefault();
            return Task.CompletedTask;
        }

        private Task OnEditTransactionValueChanged(string transactionValue)
        {
            EditContractTransaction.PartialPaymentValue = transactionValue.ToDecimalOrDefault();
            return Task.CompletedTask;
        }

        private async Task CreatePaymentDueDateAtChange(DateTime? value)
        {
            if (value != null)
            {
                createPaymentDueDate                  = value;
                NewContractTransaction.PaymentDueDate = await ConvertBrowserToUniversalDateTime(createPaymentDueDate.Value);
            }
        }

        private async Task EditPaymentDueDateAtChange(DateTime? value)
        {
            if (value != null)
            {
                editPaymentDueDate                     = value;
                EditContractTransaction.PaymentDueDate = await ConvertBrowserToUniversalDateTime(editPaymentDueDate.Value);
            }
        }

        private async Task GetGroupCosts()
        {
            GroupCosts       = await ContractAppService.GetGroupCosts();
            ActiveGroupCosts = GroupCosts.Where(_ => !_.Disable).ToList();
        }

        private void OnSelectedNewGroupCost(object value)
        {
            var groupCostIds = ((IEnumerable<Guid>)value).ToList();
            var groupCosts   = ActiveGroupCosts.Where(x => x.Id.IsIn(groupCostIds)).ToList();
            NewContractGroupCostIds               = groupCostIds;
            NewContractTransaction.GroupCostInfos = ObjectMapper.Map<List<GroupCostDto>, List<GroupCostInfoDto>>(groupCosts);
            NewContractTransaction.Cost           = groupCosts.Sum(item => item.Cost);
        }

        private void OnSelectedEditingGroupCost(object value)
        {
            var groupCostIds     = ((IEnumerable<Guid>)value).ToList();
            var changedGroupCost = ChangedGroupCosts.FirstOrDefault(_ => !_.Id.IsIn(groupCostIds));
            if (changedGroupCost is not null)
            {
                EditGroupCosts = EditGroupCosts.Select
                                                (
                                                 _ =>
                                                 {
                                                     if (_.Id == changedGroupCost.Id) _.Cost = changedGroupCost.Cost;
                                                     return _;
                                                 }
                                                )
                                               .ToList();
                ChangedGroupCosts.Remove(changedGroupCost);
            }

            var groupCosts = EditGroupCosts.Where(x => x.Id.IsIn(groupCostIds)).ToList();
            EditTransactionGroupCostIds            = groupCostIds;
            EditContractTransaction.GroupCostInfos = ObjectMapper.Map<List<GroupCostDto>, List<GroupCostInfoDto>>(groupCosts);
            EditContractTransaction.Cost           = groupCosts.Sum(item => item.Cost);
        }

        private void OnNewContractCostAmountChange(string amount)
        {
            NewContractTransaction.Cost = amount.ToDecimalOrDefault();
        }

        private void OnChangedNewManualCost(bool value)
        {
            NewContractTransaction.IsManualCost = value;
            if (value)
            {
                NewContractGroupCostIds               = null;
                NewContractTransaction.GroupCostInfos = new List<GroupCostInfoDto>();
            }

            NewContractTransaction.Cost = 0;
        }

        private void OnChangedEditingManualCost(bool value)
        {
            EditContractTransaction.IsManualCost = value;
            if (value)
            {
                EditTransactionGroupCostIds            = null;
                EditContractTransaction.GroupCostInfos = new List<GroupCostInfoDto>();
            }

            EditContractTransaction.Cost = 0;
        }

        private void OnEditContractCostAmountChange(string amount)
        {
            EditContractTransaction.Cost = amount.ToDecimalOrDefault();
        }
    }

    public record FileModel
    {
        public int    Index;
        public string FileName;
        public string CreationTime;
        public string Url;
        public Guid   MediaId;
    }
}