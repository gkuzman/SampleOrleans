namespace Grains.StorageQueue;

using Domain.Constants;
using Domain.Withdrawal.Models;
using GrainInterfaces.StorageQueue;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;

[ImplicitStreamSubscription(QueueNames.WithdrawalIngest)]
public class WithdrawalRequestConsumerGrain : Grain, IWithdrawalRequestConsumerGrain, IStreamSubscriptionObserver
{
  private readonly ILogger<IWithdrawalRequestConsumerGrain> _logger;
  private readonly WithdrawalRequestObserver _observer;

  /// <summary>
  /// Class that will process withdrawal events
  /// </summary>
  private class WithdrawalRequestObserver : IAsyncObserver<WithdrawalRequest>
  {
    private readonly ILogger<IWithdrawalRequestConsumerGrain> _logger;

    public WithdrawalRequestObserver(ILogger<IWithdrawalRequestConsumerGrain> logger)
    {
      _logger = logger;
    }

    public Task OnCompletedAsync()
    {
      _logger.LogInformation("OnCompletedAsync");
      return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
      _logger.LogInformation("OnErrorAsync: {Exception}", ex);
      return Task.CompletedTask;
    }

    public async Task OnNextAsync(WithdrawalRequest item, StreamSequenceToken? token = null)
    {
      _logger.LogInformation("OnNextAsync: item: {Item}, token = {Token}", item, token);
      await Task.Delay(1000);
    }
  }

  public WithdrawalRequestConsumerGrain(ILogger<IWithdrawalRequestConsumerGrain> logger)
  {
    _logger = logger;
    _observer = new WithdrawalRequestObserver(_logger);
  }

  // Called when a subscription is added
  public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
  {
    // Plug our WithdrawalRequestObserver to the stream
    var handle = handleFactory.Create<WithdrawalRequest>();
    await handle.ResumeAsync(_observer);
  }

  public override Task OnActivateAsync(CancellationToken token)
  {
    _logger.LogInformation("OnActivateAsync");
    return Task.CompletedTask;
  }
}