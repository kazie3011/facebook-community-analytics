using FacebookCommunityAnalytics.Api.Core.Enums;

namespace FacebookCommunityAnalytics.Api.Groups
{
    public static class GroupConsts
    {
        private const string DefaultSorting = "{0}Title asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Group." : string.Empty);
        }

        public static string GetGroupDisplayName(string name, GroupSourceType groupType)
        {
            return $"{name} ({groupType})";
        }
    }
}