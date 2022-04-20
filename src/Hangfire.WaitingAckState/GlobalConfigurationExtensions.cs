using Hangfire.Dashboard;

namespace Hangfire.WaitingAckState
{
    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Adds WaitingAckState handler, filter and page to the Hangfire configuration.
        /// </summary>
        public static IGlobalConfiguration UseWaitingAckState(this IGlobalConfiguration configuration)
        {
            configuration.UseFilter(new WaitingAckStateFilter());
            GlobalStateHandlers.Handlers.Add(new WaitingAckState.Handler());

            DashboardRoutes.Routes.AddRazorPage("/my-waitingack", page => new WaitingAckJobsPage());

            NavigationMenu.Items.Add(page => new MenuItem("WaitingAck Jobs", page.Url.To("/my-waitingack"))
            {
                Metric = MyAwaitingCount,
                Active = page.RequestPath.StartsWith("/my-waitingack"),
            });
            
            DashboardMetrics.AddMetric(MyAwaitingCount);

            DashboardRoutes.Routes.AddCommand("/waitingack/(?<JobId>.+)/delete",
                context =>
                {
                    WaitingAckJobClient.MarkAsDeleted(context.UriMatch.Groups["JobId"].Value);
                    return true;
                });
            return configuration;
        }

        private static DashboardMetric MyAwaitingCount = new DashboardMetric("my-waitingack-count", x =>
        {
            var repository = new Repository(x.Storage);
            var count = repository.GetWaitingAckJobCount();
            return new Metric(repository.GetWaitingAckJobCount())
            {
                Highlighted = count > 0,
                Style = count > 0 ? MetricStyle.Warning : MetricStyle.Default,
            };
        });
    }
}