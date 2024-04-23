namespace Grains.StorageQueue;

using Domain.Constants;
using Domain.Withdrawal.Models;
using GrainInterfaces.StorageQueue;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

public class WithdrawalRequestProducerGrain : Grain, IWithdrawalRequestProducerGrain
{
  private IAsyncStream<WithdrawalRequest>? _stream;

  public async Task Publish(WithdrawalRequest withdrawalRequest, Guid key)
  {
    var streamId = StreamId.Create(QueueNames.WithdrawalIngest, key);
    _stream = this.GetStreamProvider("AzureQueueProvider")
      .GetStream<WithdrawalRequest>(streamId);

    if (_stream is not null)
    {
      await _stream.OnNextAsync(withdrawalRequest);
    }
  }
}