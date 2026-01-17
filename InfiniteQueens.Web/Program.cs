using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using InfiniteQueens.Components;
using InfiniteQueens.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register services
builder.Services.AddSingleton<GameHistory>();
builder.Services.AddTransient<BoardGenerator>();

await builder.Build().RunAsync();
