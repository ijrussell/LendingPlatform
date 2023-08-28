using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace LoanApplicationConsole.LoanApplication;

public class LoanApplicationDataStore : Collection<LoanApplicationData>
{
    public void Insert(LoanApplicationResponse response)
    {
        switch (response)
        {
            case LoanApplicationResponse.Processed p:
                Add(new LoanApplicationData(
                    p.LoanAmount, 
                    p.AssetValue, 
                    p.CreditScore, 
                    p.LoanToValueRate, 
                    p.Status
                ));
                break;
        }
    }

    public ImmutableList<LoanApplicationStatusSummaryData> SummaryCountByStatus()
    {
        return Items
            .GroupBy(item => item.Status)
            .Select(item => 
                new LoanApplicationStatusSummaryData(item.Key, (short)item.Count())
            )
            .ToImmutableList();
    }

    public int ApprovedLoanTotalValue()
    {
        var approved = new LoanApplicationStatus.Approved().ToString();
        return Items
            .Where(item => item.Status.Equals(approved, StringComparison.OrdinalIgnoreCase))
            .Sum(item => item.LoanAmount);
    }

    public short MeanLoanToValueRate()
    {
        return (short)Items
            .Average(item => item.LoanToValueRate);
    }
}

public record struct LoanApplicationStatusSummaryData(
    string Status, 
    short Count
);

public record struct LoanApplicationData(
    int LoanAmount, 
    int AssetValue, 
    short CreditScore, 
    short LoanToValueRate, 
    string Status
);