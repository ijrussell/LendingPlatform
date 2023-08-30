namespace LoanApplicationConsole.LoanApplication;

public readonly record struct LoanAmount(int Value);
public readonly record struct AssetValue(int Value);
public readonly record struct CreditScore(short Value);

public readonly record struct LoanToValueRate(byte Value)
{
    public static LoanToValueRate? Calculate(LoanAmount loanAmount, AssetValue assetValue)
    {
        // Assumption: AssetValue must be more than LoanAmount
        if (assetValue.Value == 0 || assetValue.Value <= loanAmount.Value) 
            return null;
        
        // Assumption: Round down (50.99 => 50)
        return new LoanToValueRate((byte)(100 * loanAmount.Value / assetValue.Value));
    }
};

