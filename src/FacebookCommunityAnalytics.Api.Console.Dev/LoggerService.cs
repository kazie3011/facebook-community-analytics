using Volo.Abp.DependencyInjection;

namespace FacebookCommunityAnalytics.Api.Console.Dev
{
    public class LoggerService : ITransientDependency
    {
        public void Trace(string value)
        {
            var s = $"===========================CONSOLE DEV: "
                    + value;
            
            System.Console.WriteLine(s);
        }
    }
}