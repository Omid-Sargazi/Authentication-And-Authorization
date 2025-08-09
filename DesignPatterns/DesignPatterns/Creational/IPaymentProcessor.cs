namespace DesignPatterns.Creational
{
    public interface IPaymentProcessor
    {
        Task ProcessAsync(decimal amount, string currency);
    }

    public interface IReceiptGenerator
    {
        string Generate(decimal amount, string currency);
    }

    public interface IPaymentSuiteFactory
    {
        IPaymentProcessor CreateProcessor();
        IReceiptGenerator CreateReceipt();
    }


}