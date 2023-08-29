using System.Collections.Immutable;
using LoanApplicationConsole.LoanApplication;

var processor = new LoanApplicationProcessor(
    new LoanApplicationRiskDecider()
);

var dataStore = new LoanApplicationDataStore();

bool shouldContinue;

do
{
    var request = CreateLoanApplicationRequest(
        RequestUserInput("Enter Loan Amount in GBP:"),
        RequestUserInput("Enter Asset Value in GBP:"),
        RequestUserInput("Enter Credit Score [1-999]:")
    );
    
    shouldContinue = AskWantTheyWantToDo() switch 
    {
        ConsoleKey.P => ProcessLoanApplication(processor, dataStore, request),
        ConsoleKey.X => false, // Exit
        _ => true // Continue
    };
} while (shouldContinue);

Console.WriteLine("Finished");
return;

bool ProcessLoanApplication(
    LoanApplicationProcessor loanApplicationProcessor, 
    LoanApplicationDataStore loanApplicationDataStore,
    LoanApplicationRequest loanApplicationRequest)
{
    switch (loanApplicationProcessor.Process(loanApplicationRequest))
    {
        case LoanApplicationResponse.Processed response:
            loanApplicationDataStore.Insert(response);

            DisplayLoanApplicationStatus(response);
            DisplayMetrics(loanApplicationDataStore);
            break;
        case LoanApplicationResponse.UnableToProcess response:
            Console.WriteLine($"{response.Reason}");
            break;
    }

    return AskIfWeShouldContinue();
}


string? RequestUserInput(string message)
{
    Console.WriteLine(message);
    return Console.ReadLine();
}

LoanApplicationRequest CreateLoanApplicationRequest(string? loanAmountInput, string? assetValueInput, string? creditScoreInput)
{
#pragma warning disable CA1806
    // If int.TryParse/int16.TryParse return false, they return the default for out params.
    // For int and short, that is 0
    int.TryParse(loanAmountInput, out var loanAmount);
    int.TryParse(assetValueInput, out var assetValue);
    short.TryParse(creditScoreInput, out var creditScore);
#pragma warning restore CA1806
        
    return new LoanApplicationRequest(loanAmount, assetValue, creditScore);
}


void DisplayLoanApplicationStatus(LoanApplicationResponse.Processed response)
{
    Console.WriteLine($"Your loan application was {response.Status}");
}

void DisplayMetrics(LoanApplicationDataStore store)
{
    DisplayCountByStatusSummary(store.SummaryCountByStatus());
    DisplayTotalLoanValue(store.ApprovedLoanTotalValue());
    DisplayMeanLoanToValue(store.MeanLoanToValueRate());
    return;

    void DisplayCountByStatusSummary(ImmutableList<LoanApplicationStatusSummaryData> summary)
    {
        summary.ForEach(item =>
            Console.WriteLine($"There are {item.Count} {item.Status} applications.")
        );
    }

    void DisplayTotalLoanValue(int totalValue)
    {
        Console.WriteLine($"The total value of approved loans is {totalValue}.");
    }

    void DisplayMeanLoanToValue(int meanValue)
    {
        Console.WriteLine($"The mean LTV rate of all processed loans is {meanValue}%.");
    }
}

ConsoleKey AskWantTheyWantToDo()
{
    Console.WriteLine("Press p to process your loan application, x to exit, and any other key to cancel this application.");

    var key = Console.ReadKey().Key;
    
    Console.WriteLine("");

    return key;
}

bool AskIfWeShouldContinue()
{
    Console.WriteLine("Press x to exit, and any other key to make another loan application.");

    var key = Console.ReadKey().Key;

    Console.WriteLine("");

    return key != ConsoleKey.X;
}
