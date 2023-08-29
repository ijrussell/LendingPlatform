namespace LoanApplicationConsole.LoanApplication;

public readonly record struct LoanAmount(int Value);
public readonly record struct AssetValue(int Value);
public readonly record struct CreditScore(short Value);
public readonly record struct LoanToValueRate(byte Value);

public class LoanApplicationProcessor
{
    private readonly IRiskDecider _riskDecider;

    public LoanApplicationProcessor(IRiskDecider riskDecider)
    {
        _riskDecider = riskDecider;
    }
    
    public LoanApplicationResponse Process(LoanApplicationRequest request)
    {
        if (!request.IsValid) 
            return new LoanApplicationResponse.UnableToProcess("Request is invalid.");

        var loanToValueRate = 
            CalculateLoanToValueRate(request.LoanAmount, request.AssetValue);

        if (!loanToValueRate.HasValue)
            return new LoanApplicationResponse.UnableToProcess("Unable to calculate loan to value rate.");
        
        var loanApplicationStatus = 
            _riskDecider.Run(request.LoanAmount, loanToValueRate.Value, request.CreditScore);
        
        return CreateLoanApplicationResponse(request, loanToValueRate.Value, loanApplicationStatus);
    }

    private static LoanToValueRate? CalculateLoanToValueRate(LoanAmount loanAmount, AssetValue assetValue)
    {
        // Assumption: AssetValue must be more than LoanAmount
        if (assetValue.Value == 0 || assetValue.Value <= loanAmount.Value) 
            return null;
        
        // Assumption: Round down (50.99 => 50)
        return new LoanToValueRate((byte)(100 * loanAmount.Value / assetValue.Value));
    }

    private static LoanApplicationResponse CreateLoanApplicationResponse(
        LoanApplicationRequest request,
        LoanToValueRate loanToValueRate, 
        LoanApplicationStatus loanApplicationStatus)
    {
        return new LoanApplicationResponse.Processed(
            request.LoanAmount.Value,
            request.AssetValue.Value,
            request.CreditScore.Value,
            loanToValueRate.Value,
            loanApplicationStatus.ToString()!
        );
    }
}