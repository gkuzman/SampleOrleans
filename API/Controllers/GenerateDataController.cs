using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

using Azure.Storage.Queues;
using Domain.Constants;
using Domain.Withdrawal.Models;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;

[Route("api/generate-data/[action]")]
[ApiController]
public class GenerateDataController : Controller
{
  private readonly QueueServiceClient _queueServiceClient;

  public GenerateDataController(IAzureClientFactory<QueueServiceClient> queueServiceClientFactory)
  {
    _queueServiceClient = queueServiceClientFactory.CreateClient(QueueNames.StorageQueue);
  }

  [HttpPut]
  public async Task<IActionResult> Withdrawal([FromBody] WithdrawalRequest withdrawal)
  {
    var client = _queueServiceClient.GetQueueClient(QueueNames.WithdrawalIngest);
    await client.CreateIfNotExistsAsync();
    var bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(withdrawal));
    await client.SendMessageAsync(Convert.ToBase64String(bytes));

    // Generate data
    return Ok();
  }
}