namespace GrainInterfaces.StorageQueue;

using Domain.Withdrawal.Models;
using Orleans;

public interface IWithdrawalRequestProducerGrain : IGrainWithStringKey
{
  Task Publish(WithdrawalRequest withdrawalRequest, Guid key);
}