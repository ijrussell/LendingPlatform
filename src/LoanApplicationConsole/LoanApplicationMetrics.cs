using System.Collections.Immutable;
using LoanApplicationConsole.LoanApplication;

namespace LoanApplicationConsole;

public class LoanApplicationMetrics
{
    private readonly LoanApplicationDataStore _store;

    public LoanApplicationMetrics(LoanApplicationDataStore store)
    {
        _store = store;
    }

    public ImmutableList<LoanApplicationStatusSummaryData> SummaryCountByStatus()
    {
        return _store.LoanApplications
            .GroupBy(item => item.Status)
            .Select(item => 
                new LoanApplicationStatusSummaryData(item.Key, (short)item.Count())
            )
            .ToImmutableList();
    }

    public int ApprovedLoanTotalValue()
    {
        var approved = new LoanApplicationStatus.Approved().ToString();
        return _store.LoanApplications
            .Where(item => item.Status.Equals(approved, StringComparison.OrdinalIgnoreCase))
            .Sum(item => item.LoanAmount);
    }

    public short MeanLoanToValueRate()
    {
        return (short)_store.LoanApplications
            .Average(item => item.LoanToValueRate);
    }
}

public record struct LoanApplicationStatusSummaryData(
    string Status, 
    short Count
);

