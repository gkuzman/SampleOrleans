using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

using Domain.Withdrawal.Models;
using GrainInterfaces.StorageQueue;

[Route("api/generate-data/[action]")]
[ApiController]
public class GenerateDataController : Controller
{
  private readonly IClusterClient _clusterClient;

  public GenerateDataController(IClusterClient clusterClient)
  {
    _clusterClient = clusterClient;
  }

  [HttpPut]
  public async Task<IActionResult> Withdrawal([FromBody] WithdrawalRequest withdrawal)
  {
    var guid = Guid.NewGuid();
    var producer = _clusterClient.GetGrain<IWithdrawalRequestProducerGrain>(guid.ToString());
    await producer.Publish(withdrawal, guid);
    // Generate data
    return Ok();
  }

  [HttpPut("{count:int}")]
  public async Task<IActionResult> BatchWithdrawals(int count)
  {
    // create a batch of withdrawal requests
    var batch = Enumerable.Range(0, count)
      .Select(i => new WithdrawalRequest(
        Guid.NewGuid().ToString(),
        "NL02ABNA0123456789",
        "player1",
        100
      ));

    // publish each withdrawal request in parallel
    await Parallel.ForEachAsync(batch, async (withdrawal, tkn) =>
    {
      var guid = Guid.NewGuid();
      var producer = _clusterClient.GetGrain<IWithdrawalRequestProducerGrain>(guid.ToString());
      await producer.Publish(withdrawal, guid);
    });

    return Ok();
  }
}