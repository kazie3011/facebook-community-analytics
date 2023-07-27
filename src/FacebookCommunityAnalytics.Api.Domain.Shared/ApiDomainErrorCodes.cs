namespace FacebookCommunityAnalytics.Api
{
    /// <summary>
    /// The localized messages are defined in Domain.Shared > Localization>ApiValidation > en.json
    /// </summary>
    public static class ApiDomainErrorCodes
    {
        /* You can add your business exception error codes here, as constants */
        public class Sample
        {
            public const string Error = "ApiDomain:SampleError";
        }

        public class Posts
        {
            public const string InvalidUrl = "ApiDomain:InvalidUrl";
            public const string DuplicatedUrl = "ApiDomain:DuplicatedUrl";
            public const string InvalidOrExistingUrl = "ApiDomain:InvalidOrExistingUrl";
            public const string PostSourceTypeMustSame = "ApiDomain:PostSourceTypeMustSame";
            public const string MaxUrlReached = "ApiDomain:MaxUrlReached";
            public const string PostNullData = "ApiDomain:PostNullData";
        }

        public class Categories
        {
            public const string NotExist = "ApiDomain:CategoryNotExist";
        }

        public class Groups
        {
            public const string NoUrl = "ApiDomain:NoUrl";
            public const string InvalidGroupUrl = "ApiDomain:InvalidGroupUrl";
            public const string DuplicateGroup = "ApiDomain:DuplicateGroup";
            public const string NotExist = "ApiDomain:GroupNotExist";
            public const string InsMustHaveGroup = "ApiDomain:InsMustHaveGroup";
        }

        public class ScheduledPosts
        {
            public const string InValidContent = "ApiDomain:InvalidContent";
            public const string InValidGroup = "ApiDomain:InvalidGroup";
            public const string InValidDateTime = "ApiDomain:InValidDateTime";
            public const string SyncFailError = "ApiDomain: SyncFailError";
        }

        public class UserAffiliates
        {
            public const string ShortlinkCreationFailure = "ApiDomain:UserAffiliate.ShortlinkCreationFailure";
            public const string InvalidUrl = "ApiDomain:UserAffiliate.InvalidUrl";
            public const string InvalidShortlink = "ApiDomain:InvalidShortlink";
            public const string InvalidLazadaUrl = "ApiDomain:InvalidLazadaUrl";
            public const string InvalidTikiUrl = "ApiDomain:UserAffiliate.InvalidUrl.Tiki";
        }

        public class UserInfo
        {
            public const string InvalidFacebookId = "ApiDomain:UserInfo:InvalidFacebookId";
            public const string DuplicatedSeedingAccountFid = "ApiDomain:UserInfo:DuplicatedSeedingAccountFid";
        }

        public class Campaign
        {
            public const string InvalidName = "ApiDomain:InvalidName";
            public const string InvalidEmails = "ApiDomain:InvalidEmails";
            public const string PermissionNotGranted = "ApiDomain:Campaign:PermissionNotGranted";
            public const string NameExisted = "ApiDomain:Campaign:NameExisted";
            public const string PrizeExisted = "ApiDomain:Campaign:PrizeExisted";
        }

        public class TiktokCrawl
        {
            public const string NoChannelFound = "ApiDomain:TiktokCrawl.NoChannelFound";
            public const string EmptyData = "ApiDomain:TiktokCrawl.EmptyData";
        }

        public class StaffEvaluation
        {
            public const string InvalidUser = "ApiDomain:InvalidUser";
            public const string DuplicateCriteria = "ApiDomain:DuplicateCriteria";
            public const string InvalidCriteria = "ApiDomain:InvalidCriteria";
            public const string WrongCriteriaPoint = "ApiDomain:WrongCriteriaPoint";
        }

        public class Contract
        {
            public const string InvalidTransaction = "ApiDomain:Contract.InvalidTransaction";
            public const string InvalidValue = "ApiDomain:Contract.InvalidValue";
            public const string InvalidContractCode = "ApiDomain:Contract.InvalidContractCode";
            public const string PartnerIsRequired = "ApiDomain:PartnerIsRequired";
            public const string ContractTypeRequired = "ApiDomain:ContractTypeIsRequired";
            public const string ContractRequiresTnxAmount = "ApiDomain:ContractRequiresTnxAmount";
            public const string ContractAmountRequired = "ApiDomain:ContractAmountRequired";
            public const string ContractNotFound = "ApiDomain:ContractNotFound";
            public const string DoNotGroupNameSync = "ApiDomain:Cotract.DoNotGroupNameSync";
        }

        public class GroupCost
        {
            public const string ExistedName = "ApiDomain.GroupCost.ExistedName";
        }

        public class Transaction
        {
            public const string TransactionAmountInvalid = "ApiDomain:TransactionAmountInvalid";
            public const string TransactionNotFound = "ApiDomain:TransactionNotFound";
            public const string InvalidTransaction = "ApiDomain:Transaction.InvalidTransaction";
            public const string TransactionAmountRequired = "ApiDomain:Transaction.AmountRequired";
        }
    }
}
