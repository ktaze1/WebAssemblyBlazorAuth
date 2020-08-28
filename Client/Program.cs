using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using AuthTest.Shared;

namespace AuthTest.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("AuthTest.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthTest.ServerAPI"));

            builder.Services.AddScoped(sp =>
            {
                var baseAddressMessageHandler = sp.GetRequiredService<BaseAddressAuthorizationMessageHandler>();
                baseAddressMessageHandler.InnerHandler = new HttpClientHandler();
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, baseAddressMessageHandler);
                var channel = GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, new GrpcChannelOptions { HttpHandler = grpcWebHandler });
                return new Greeter.GreeterClient(channel);
            });

            builder.Services.AddApiAuthorization()
                .AddAccountClaimsPrincipalFactory<RolesClaimPrincipalFactory>();


            await builder.Build().RunAsync();
        }
    }
}
