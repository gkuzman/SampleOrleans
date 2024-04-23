using Azure.Data.Tables;
using Azure.Storage.Queues;
using Domain.Constants;
using Orleans.Configuration;

const int maxAttempts = 5;
var attempt = 0;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddConsole();
builder.Host.UseOrleansClient(ConfigureClient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureClient(HostBuilderContext context, IClientBuilder clientBuilder)
{
  clientBuilder
    .Configure<ClusterOptions>(options =>
    {
      options.ClusterId = Common.ClusterId;
      options.ServiceId = Common.ServiceId;
    })
    .UseAzureStorageClustering(options =>
    {
      options.TableServiceClient = new TableServiceClient("UseDevelopmentStorage=true");
    })
    .UseConnectionRetryFilter(RetryFilter);

  clientBuilder.AddAzureQueueStreams("AzureQueueProvider", configurator =>
  {
    configurator.ConfigureAzureQueue(
      ob => ob.Configure(options =>
      {
        options.QueueServiceClient = new QueueServiceClient("UseDevelopmentStorage=true");
        options.QueueNames = new List<string> { QueueNames.WithdrawalIngest };
      }));
  });
}

async Task<bool> RetryFilter(Exception exception, CancellationToken cancellationToken)
{
  attempt++;
  Console.WriteLine($"Cluster client attempt {attempt} of {maxAttempts} failed to connect to cluster.  Exception: {exception}");
  if (attempt > maxAttempts)
  {
    return false;
  }
  await Task.Delay(TimeSpan.FromSeconds(4), cancellationToken);
  return true;
}