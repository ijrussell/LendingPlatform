namespace LoanApplicationConsole.LoanApplication;

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
            LoanToValueRate.Calculate(request.LoanAmount, request.AssetValue);

        if (!loanToValueRate.HasValue)
            return new LoanApplicationResponse.UnableToProcess("Unable to calculate loan to value rate.");
        
        var loanApplicationStatus = 
            _riskDecider.Run(request.LoanAmount, loanToValueRate.Value, request.CreditScore);
        
        return CreateLoanApplicationResponse(request, loanToValueRate.Value, loanApplicationStatus);
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