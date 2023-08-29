namespace LoanApplicationConsole.LoanApplication;

public abstract record LoanApplicationResponse
{
    private LoanApplicationResponse() {}

    public sealed record Processed(int LoanAmount, int AssetValue, short CreditScore, byte LoanToValueRate, string Status) : LoanApplicationResponse;

    public sealed record UnableToProcess(string Reason) : LoanApplicationResponse;
}