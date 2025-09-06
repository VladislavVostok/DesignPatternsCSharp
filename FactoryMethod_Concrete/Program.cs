using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FactoryMethod_Concrete
{

    public class PaymentRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ProviderName { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }

    public class RefundRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
    }

    public class RefundResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class StripeConfig
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int TimeoutSeconds { get; set; }
    }

    public class PayPalConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseSandbox { get; set; }
    }

    // Интерфейс платежного шлюза
    public interface IPaymentGateway
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest req);
        Task<RefundResult> ProcessRefundAsync(RefundRequest req);
        bool ValidateCredentials();

    }

    // Конкретная реализация платежных шлюзов

    public class StripeGateway : IPaymentGateway
    {
        private readonly StripeConfig _config;

        public StripeGateway(StripeConfig config)
        {
            _config = config;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest req)
        {

            // Реальная реализация интеграции со Stripe
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
            var response = await client.PostAsJsonAsync(_config.BaseUrl + "/payments", req);
            return await response.Content.ReadFromJsonAsync<PaymentResult>();
        }

        public async Task<RefundResult> ProcessRefundAsync(RefundRequest req)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

            var response = await client.PostAsJsonAsync(_config.BaseUrl + "/refunds", req);
            return await response.Content.ReadFromJsonAsync<RefundResult>();
        }

        public bool ValidateCredentials()
        {
            // Проверка валидности API ключа
            return !string.IsNullOrEmpty(_config.ApiKey) && _config.ApiKey.StartsWith("sk_");
        }
    }


    public class PayPallGateway : IPaymentGateway
    {
        private readonly PayPalConfig _config;

        public PayPallGateway(PayPalConfig config)
        {
            _config = config;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest req)
        {

            // Реальная реализация интеграции со Stripe
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ClientSecret}");

            var response = await client.PostAsJsonAsync(_config.ClientId + "/payments", req);
            return await response.Content.ReadFromJsonAsync<PaymentResult>();
        }

        public async Task<RefundResult> ProcessRefundAsync(RefundRequest req)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ClientSecret}");

            var response = await client.PostAsJsonAsync(_config.ClientId + "/refunds", req);
            return await response.Content.ReadFromJsonAsync<RefundResult>();
        }

        public bool ValidateCredentials()
        {
            // Проверка валидности API ключа
            return !string.IsNullOrEmpty(_config.ClientSecret) && _config.ClientSecret.StartsWith("pl_");
        }
    }


    // Абстрактный создатель

    public abstract class PaymentGatewayFactory
    {
        public abstract IPaymentGateway CreateGateway();


        public async Task<PaymentResult> ProcessPayment(PaymentRequest req)
        {
            var gateway = CreateGateway();

            if (!gateway.ValidateCredentials())
                throw new InvalidOperationException("Не пройдена валидации для шлюза!");

            Console.WriteLine($"Процесс оплаты {req.TransactionId}");

                try
                {
                    return await gateway.ProcessPaymentAsync(req);
                }
                catch (Exception ex)
                {
                Console.WriteLine($"Проццесс оплаты прерван! {ex.Message}");
                    throw;
                }
            
        }


        public class StripeGatewayFactory : PaymentGatewayFactory
        {
            private readonly StripeConfig _config;

            public StripeGatewayFactory(IOptions<StripeConfig> configOptions)
            {
                _config = configOptions.Value;
            }

            public override IPaymentGateway CreateGateway()
            {
                return new StripeGateway(_config);
            }

        }

        public class PayPallGatewayFactory : PaymentGatewayFactory
        {
            private readonly PayPalConfig _config;
            private readonly ILogger _logger;


            public PayPallGatewayFactory(IOptions<PayPalConfig> configOptions, ILogger logger)
            {
                _config = configOptions.Value;
                _logger = logger;
            }

            public override IPaymentGateway CreateGateway()
            {
                return new PayPallGateway(_config);
            }

        }

        public interface IPaymentGatewayFactoryResolver
        {
            PaymentGatewayFactory Resolve(string providerName);
        }

        public class PaymentGatewayFactoryResolver : IPaymentGatewayFactoryResolver
        {

            private readonly IServiceProvider _serviceProvider;

            public PaymentGatewayFactoryResolver(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public PaymentGatewayFactory Resolve(string providerName)
            {
                return providerName.ToLower() switch
                {
                    "stripe" => _serviceProvider.GetRequiredService<StripeGatewayFactory>(),
                    "paypal" => _serviceProvider.GetRequiredService<PayPallGatewayFactory>(),
                    _ => throw new ArgumentException($"Неизвестный платежный провайдер: {providerName}")
                };
            }
        }

        public class PaymentService
        {
            private readonly IPaymentGatewayFactoryResolver _factoryResolver;
            private readonly ILogger _logger;

            public PaymentService(IPaymentGatewayFactoryResolver factoryResolver)
            {
                _factoryResolver = factoryResolver;

            }

            public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest req)
            {
                try
                {
                    var factory = _factoryResolver.Resolve(req.ProviderName);
                    return await factory.ProcessPayment(req);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Процесс оплаты завершился с ошибкой для  провайдера: {req.ProviderName}");
                    throw new Exception("Платеж завалился", ex);
                }
            }

        }


        class Program
        {
            static async Task Main(string[] args)
            {

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();


                var serviceProvider = new ServiceCollection()

                    .AddSingleton<IConfiguration>(configuration)

                    .Configure<StripeConfig>(configuration.GetSection("Stripe"))
                    .Configure<StripeConfig>(configuration.GetSection("PayPal"))

                    .AddLogging(configurare => configurare.AddConsole())

                    .AddTransient<IPaymentGateway, StripeGateway>()
                    .AddTransient<IPaymentGateway, PayPallGateway>()


                    .AddTransient<StripeGatewayFactory>()
                    .AddTransient<PayPallGatewayFactory>()


                    .AddSingleton<IPaymentGatewayFactoryResolver, PaymentGatewayFactoryResolver>()

                    .AddTransient<PaymentService>()

                    .BuildServiceProvider();

                var paymentService = serviceProvider.GetService<PaymentService>();

                var request = new PaymentRequest
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Amount = 100.00m,
                    Currency = "USD",
                    ProviderName = "stripe"
                };

                try
                {
                    var result = await paymentService.ProcessPaymentAsync(request);
                    Console.WriteLine($"Платеж обработан: {result.Success}, ID: {result.TransactionId}");
                }
                catch (Exception ex) { 
                    Console.WriteLine($"Ошибка обработки платежа: {ex.Message}");
                }


            }


        }




    }
}