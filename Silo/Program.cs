using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
  .UseOrleans(ConfigureSilo)
  .ConfigureLogging(logging => logging.AddConsole())
  .Build();
await host.RunAsync();

return 0;

static void ConfigureSilo(HostBuilderContext context, ISiloBuilder siloBuilder)
{
  siloBuilder.UseAzureStorageClustering(options => options.ConfigureTableServiceClient("UseDevelopmentStorage=true"));
}