using Azure.Storage.Queues;
using Domain.Constants;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add storage queue
builder.Services.AddAzureClients(builder =>
{
  builder.AddQueueServiceClient("UseDevelopmentStorage=true").WithName(QueueNames.StorageQueue);

  builder.AddClient<QueueClient, QueueClientOptions>((_, _, provider) =>
      provider
        .GetService<QueueServiceClient>()!
        .CreateQueue(QueueNames.WithdrawalIngest))
    .WithName(QueueNames.WithdrawalIngest);
});

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