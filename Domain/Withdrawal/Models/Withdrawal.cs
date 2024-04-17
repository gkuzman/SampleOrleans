namespace Domain.Withdrawal.Models;

using Domain.Withdrawal.Enums;

public class Withdrawal(string id, string iban, string playerId, decimal amount)
{
  public string Id { get; init; } = id;

  public string Iban { get; init; } = iban;

  public string PlayerId { get; init; } = playerId;

  public decimal Amount { get; init; } = amount;

  public WithdrawalState State { get; set; } = WithdrawalState.Requested;
}