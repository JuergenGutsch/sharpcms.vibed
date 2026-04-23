using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Sharpcms.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

string legacyBinPath = Path.Combine(app.Environment.ContentRootPath, "Bin");
Directory.CreateDirectory(legacyBinPath);
foreach (string assemblyPath in Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
{
    string assemblyName = Path.GetFileName(assemblyPath);
    if (!assemblyName.StartsWith("Sharpcms.", StringComparison.OrdinalIgnoreCase))
    {
        continue;
    }

    string destinationPath = Path.Combine(legacyBinPath, assemblyName);
    File.Copy(assemblyPath, destinationPath, true);
}

app.MapGet("/", () => Results.Text(Core.Send(), "text/html"));
app.MapGet("/api/sharpcms", () => Results.Text(Core.Send(), "text/html"));

app.Run();
