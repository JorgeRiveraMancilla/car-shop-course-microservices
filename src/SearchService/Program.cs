using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq(
        (context, configuration) =>
        {
            configuration.Host(
                "localhost",
                "/",
                h =>
                {
                    h.Username("rabbitmq");
                    h.Password("rabbitmq");
                }
            );

            configuration.ReceiveEndpoint(
                "search-auction-created",
                e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));

                    e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                }
            );
            configuration.ConfigureEndpoints(context);
        }
    );
});

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

try
{
    await DatabaseInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
static IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
