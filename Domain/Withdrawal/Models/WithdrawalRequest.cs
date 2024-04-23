namespace Domain.Withdrawal.Models;

using Orleans;

[GenerateSerializer, Immutable]
public sealed record WithdrawalRequest(string Id, string Iban, string PlayerId, decimal Amount);