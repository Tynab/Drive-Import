using Blazored.Toast;
using DriveImport;
using DriveImport.Services;
using DriveImport.Services.Implements;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
_ = builder.Services.AddBlazoredToast();
_ = builder.Services.AddTransient<IDriveService, DriveService>();

_ = builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["BackendApiUrl"] ?? "https://localhost:7289/")
});

await builder.Build().RunAsync();
