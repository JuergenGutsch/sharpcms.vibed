using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Sharpcms.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapSharpcms();

app.Run();
