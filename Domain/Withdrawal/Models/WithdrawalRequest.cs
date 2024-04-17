namespace Domain.Withdrawal.Models;

public record WithdrawalRequest(string Id, string Iban, string PlayerId, decimal Amount);