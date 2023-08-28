namespace LoanApplicationConsole.LoanApplication;

public abstract record LoanApplicationStatus
{
    private LoanApplicationStatus() {}

    public sealed record Approved : LoanApplicationStatus
    {
        public override string ToString() => "Approved";
    }

    public sealed record Declined(string Reason) : LoanApplicationStatus    
    {
        public override string ToString() => "Declined";
    }
}