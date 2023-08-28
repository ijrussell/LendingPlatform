# Lending Platform

This is the code from a coding exercise written in C#. It contains a console application and a test project.

The requirements are shown below.

## Planning

### Logic Pseudocode

- ProcessLoanApplication(Request) : Response =>
    - ValidateRequest(Request) : Result<ValidatedRequest,List<string>> =>
    - Ok validatedRequest =>
        - CalculateLoanToValueRate(LoanAmount, AssetValue) : Option<LoanToValueRate> =>
        - Some loanToValueRate =>
            - RunRiskDecider(LoanAmount, LoanToValueRate, CreditScore) : Status =>
            - Approved =>
                - Processed(data, Approved) // Approved
            - Declined reason =>
                - Processed(data, Declined) // Declined
        - None =>
            - UnableToProcess(reason) // Unable to calculate LTV
    - Error reasons =>
        - UnableToProcess(reasons) // Failed input validation

### ValueTypes

- LoanAmount [int] [ > 0 ]
- AssetValue [int] [ > 0 ]
- CreditScore [short] [ >= 1 && <= 999 ]
- LoanToValueRate [short] [ % -> 0-100?]

Types

- Status [Approved/Declined]
- Response [Processed/UnableToProcess]
- RiskDecider [ Name? ]
- LoanToValueRateCalculator [function?]

## Requirements

Build a simple Console application using the technology of your choice (preferably C#) that enables the writing and reporting of loans as per the requirements below. This should be approached as a way that can demonstrate your process to solving problems (any required infrastructure can simply be mocked), and does not need to be built to a production standard. Instead the exercise should be timeboxed to no longer than an hour. Notes can be taken of any assumptions made, and also any other considerations or improvements that you might make if this was a production application.

### User inputs that the application should require:

* Amount that the loan is for in GBP
* The value of the asset that the loan will be secured against
* The credit score of the applicant (between 1 and 999)

### Metrics that the application should output:

* Whether or not the applicant was successful
* The total number of applicants to date, broken down by their success status
* The total value of loans written to date
* The mean average Loan to Value of all applications received to date
    * Loan to Value (LTV) is the amount that the loan is for, expressed as a percentage of the value of the asset that the loan will be secured against.

### Business rules used to derive whether or not the applicant was successful:

* If the value of the loan is more than £1.5 million or less than £100,000 then the application must be declined
* If the value of the loan is £1 million or more then the LTV must be 60% or less and the credit score of the applicant must be 950 or more
* If the value of the loan is less than £1 million then the following rules apply:
    * If the LTV is less than 60%, the credit score of the applicant must be 750 or more
    * If the LTV is less than 80%, the credit score of the applicant must be 800 or more
    * If the LTV is less than 90%, the credit score of the applicant must be 900 or more
    * If the LTV is 90% or more, the application must be declined

