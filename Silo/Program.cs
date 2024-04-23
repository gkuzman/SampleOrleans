using System.Net;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Domain.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

var host = new HostBuilder()
  .UseOrleans(ConfigureSilo)
  .ConfigureLogging(logging => logging.AddConsole())
  .Build();
await host.RunAsync();

return 0;

static void ConfigureSilo(HostBuilderContext context, ISiloBuilder siloBuilder)
{
  siloBuilder
    .Configure<ClusterOptions>(options =>
    {
      options.ClusterId = Common.ClusterId;
      options.ServiceId = Common.ServiceId;
    })
    .UseAzureStorageClustering(options => options.ConfigureTableServiceClient("UseDevelopmentStorage=true"))
    .AddAzureTableGrainStorage(
      "PubSubStore",
      options => options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true"))
    .ConfigureEndpoints(IPAddress.Loopback, siloPort: 11_111, gatewayPort: 30_000)
    .UseDashboard(options => options.Port = 8080);

  siloBuilder.AddAzureQueueStreams("AzureQueueProvider", configurator =>
  {
    configurator.ConfigureAzureQueue(
      ob => ob.Configure(options =>
      {
        options.QueueServiceClient = new QueueServiceClient("UseDevelopmentStorage=true");
        options.QueueNames = new List<string> { QueueNames.WithdrawalIngest };
      }));
  });
}
