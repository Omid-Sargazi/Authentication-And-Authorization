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

    public sealed class PayPalProcessor : IPaymentProcessor
    {
        public Task ProcessAsync(decimal amount, string currency)
        {
            Console.WriteLine($"[PayPal] Paid {amount} {currency}");
            return Task.CompletedTask;
        }
    }

    public sealed class PayPalReceipt : IReceiptGenerator
    {
        public string Generate(decimal amount, string currency)
        {
            return $"[PayPal] Receipt for {amount} {currency}";
        }
    }

    public sealed class StripeProcessor : IPaymentProcessor
    {
        public Task ProcessAsync(decimal amount, string currency)
        {
            Console.WriteLine($"[Stripe] Paid {amount} {currency}");
            return Task.CompletedTask;
        }
    }

    public sealed class StripeReceipt : IReceiptGenerator
    {
        public string Generate(decimal amount, string currency)
        {
            return $"[Stripe] Receipt for {amount} {currency}";
        }
    }

    public sealed class PayPalSuiteFactory : IPaymentSuiteFactory
    {
        public IPaymentProcessor CreateProcessor()
        {
            return new PayPalProcessor();
        }

        public IReceiptGenerator CreateReceipt()
        {
            return new PayPalReceipt();
        }
    }

    public sealed class StripeSuiteFactory : IPaymentSuiteFactory
    {
        public IPaymentProcessor CreateProcessor()
        {
            return new StripeProcessor();
        }

        public IReceiptGenerator CreateReceipt()
        {
            return new StripeReceipt();
        }
    }

    public sealed class CheckoutService
    {
        private readonly IPaymentProcessor _processor;
        private readonly IReceiptGenerator _receipt;

        public CheckoutService(IPaymentSuiteFactory factory)
        {
            _processor = factory.CreateProcessor();
            _receipt = factory.CreateReceipt();
        }
    }
}