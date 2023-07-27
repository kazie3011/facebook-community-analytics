using System;
using System.Collections.Generic;
using System.Linq;
using FacebookCommunityAnalytics.Api.Core.Enums;
using Volo.Abp.Application.Dtos;

namespace FacebookCommunityAnalytics.Api.UserCompensations
{
    public class GetUserCompensationsInput : PagedAndSortedResultRequestDto
    {
        public string FilterText { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        
        public Guid? PayrollId { get; set; }
    }
    public class UserCompensationDto : FullAuditedEntityDto<Guid>
    {
        public Guid PayrollId { get; set; }
        public Guid UserId { get; set; }
        public string Team { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<UserCompensationBonusDto> Bonuses { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BonusAmount => this.Bonuses.Sum(x => x.BonusAmount);
        public string Description { get; set; }
        public decimal SalaryAmount { get; set; }
        public decimal FinesAmount => this.Bonuses.Sum(x => x.FinesAmount);
        public UserCompensationDto()
        {
            Bonuses = new List<UserCompensationBonusDto>();
        }
    }
    public class CreateUpdateUserCompensationDto
    {
        public Guid PayrollId { get; set; }
        public Guid UserId { get; set; }
        public string Team { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<UserCompensationBonusDto> Bonuses { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
        public CreateUpdateUserCompensationDto()
        {
            Bonuses = new List<UserCompensationBonusDto>();
        }
    }
    
    public class UserCompensationBonusDto
    {
        public Guid UserId { get; set; }
        public BonusType? BonusType { get; set; }
        public decimal BonusAmount { get; set; }
        public string BonusDescription { get; set; }
        public decimal FinesAmount { get; set; }
        public string FinesDescription { get; set; }
    }
}