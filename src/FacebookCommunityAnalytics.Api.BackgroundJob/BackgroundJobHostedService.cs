using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace FacebookCommunityAnalytics.Api.BackgroundJob
{
    public class BackgroundJobHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        private readonly IServiceProvider _serviceProvider;
        private readonly PayrollBackgroundJobService _payrollBackgroundJobService;

        public BackgroundJobHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            IServiceProvider serviceProvider,
            PayrollBackgroundJobService payrollBackgroundJobService)
        {
            _application = application;
            _serviceProvider = serviceProvider;
            _payrollBackgroundJobService = payrollBackgroundJobService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);

            await _payrollBackgroundJobService.Execute();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();

            return Task.CompletedTask;
        }
    }
}
