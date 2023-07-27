using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FacebookCommunityAnalytics.Api.Console.BackgroundJobs
{
    public class SampleJob: BackgroundJobBase
    {
        protected override Task DoExecute()
        {
            Debug.Write($"{this.GetType().Name} is running at {DateTime.UtcNow} UTC \n");
            //throw new NotImplementedException("test exception log");

            return Task.CompletedTask;
        }
    }
}
