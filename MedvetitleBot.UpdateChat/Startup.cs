using System;
using Azure.Identity;
using MedvetitleBot.UpdateChat.Options;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

[assembly: FunctionsStartup(typeof(MedvetitleBot.UpdateChat.Startup))]
namespace MedvetitleBot.UpdateChat
{
	public class Startup : FunctionsStartup
    {
        private IConfigurationRoot _functionConfig;
        private UpdateChatOptions _updateChatOptions = new();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _functionConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddAzureClients(clientBuilder => {
                clientBuilder.UseCredential(new DefaultAzureCredential());

                if (Uri.TryCreate(_updateChatOptions.TableServiceConnection, UriKind.Absolute, out var tableServiceUri))
                    clientBuilder.AddTableServiceClient(tableServiceUri);
                else
                    clientBuilder.AddTableServiceClient(_updateChatOptions.TableServiceConnection);
            });

            builder.Services.AddScoped<ITelegramBotClient>(factory => new TelegramBotClient(_updateChatOptions.TelegramBotToken));
        }
    }
}

