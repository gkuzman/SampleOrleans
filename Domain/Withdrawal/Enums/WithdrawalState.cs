namespace Domain.Withdrawal.Enums;

public enum WithdrawalState
{
  Requested,
  Approved,
  Rejected,
  AwaitingPayment,
  PaidOut,
  NotPaidOut
}