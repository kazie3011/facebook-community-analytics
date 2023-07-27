using System;
using FacebookCommunityAnalytics.Api.Contracts;
using FacebookCommunityAnalytics.Api.Core.Enums;
using FacebookCommunityAnalytics.Api.Core.Extensions;
using FacebookCommunityAnalytics.Api.GroupCosts;
using FacebookCommunityAnalytics.Api.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace FacebookCommunityAnalytics.Api.Validators
{
    public class ContractValidator<T> : BaseAbstractValidator<T> where T : class
    {
        private readonly IContractRepository _contractRepository;
        protected ContractValidator(IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv, IContractRepository contractRepository) : base(l, lv)
        {
            _contractRepository = contractRepository;
        }

        protected bool ValidContractCode(string contractCode)
        {
            return contractCode.IsNotNullOrEmpty();
        }
        
        protected bool ValidContractType(ContractType contractType)
        {
            return !contractType.IsIn(ContractType.NoSelect);
        }

        protected bool ValidPartialPaymentValue(CreateUpdateContractDto dto)
        {
            return dto.TotalValue >= dto.PartialPaymentValue;
        }

        protected bool ValidTotalValue(CreateUpdateContractDto dto)
        {
            return (dto.ContractType.IsIn(ContractType.Retail, ContractType.Service) && dto.TotalValue > 0) || dto.ContractType.IsIn(ContractType.Unknown, ContractType.Master);
        }
    }
    
    public class ContractCreateUpdateDtoValidator : ContractValidator<CreateUpdateContractDto>
    {
        public ContractCreateUpdateDtoValidator(IContractRepository contractRepository, IStringLocalizer<ApiResource> l, IStringLocalizer<ApiDomainResource> lv) : base(l, lv, contractRepository)
        {
            RuleFor(x => x.ContractCode).Must(ValidContractCode)
                .WithMessage((dto) =>
                {
                    var message = LV[ApiDomainErrorCodes.Contract.InvalidContractCode].Value;
                    return message;
                });
            
            RuleFor(x => x.ContractType).Must(ValidContractType)
                .WithMessage((dto) =>
                {
                    var message = LV[ApiDomainErrorCodes.Contract.ContractTypeRequired].Value;
                    return message;
                });
            
            RuleFor(x => x).Must(ValidPartialPaymentValue)
                .WithMessage((dto) =>
                {
                    var message = LV[ApiDomainErrorCodes.Contract.InvalidValue].Value;
                    return message;
                });
            
            RuleFor(x => x).Must(ValidTotalValue)
                .WithMessage((dto) =>
                {
                    var message = LV[ApiDomainErrorCodes.Contract.ContractAmountRequired].Value;
                    return message;
                });
        }
    }
    
}