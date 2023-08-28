namespace LoanApplicationConsole.LoanApplication;

public readonly record struct LoanApplicationRequest
{
    public LoanAmount LoanAmount { get; }
    public AssetValue AssetValue { get; }
    public CreditScore CreditScore { get; }

    public LoanApplicationRequest(int loanAmount, int assetValue, short creditScore)
    {
        LoanAmount = new LoanAmount(loanAmount);
        AssetValue = new AssetValue(assetValue);
        CreditScore = new CreditScore(creditScore);
    }
    
    public bool IsValid => 
        LoanAmount.Value > 0 // Assumption 
        && AssetValue.Value > 0 // Assumption 
        && CreditScore.Value is >= 1 and <= 999;
}