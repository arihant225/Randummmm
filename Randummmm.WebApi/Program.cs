using Microsoft.Extensions.Configuration;
using Randummmm.WebApi.HelperFunctions;
using Randummmm.WebApi.HelperFunctions.Interface;
using Randummmm.WebApi.Hubs;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHostedService<LiveHelper>();

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(policyBuilder=>
policyBuilder.AddPolicy(ConfigurationHelper.PolicyName, _builder =>
{
    string[] origins = builder.Configuration.GetSection("AllowedHosts").Get<string[]>() ?? new string[] { };
    _builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();    
})
);

var app = builder.Build();

app.MapHub<LiveHub>("/live");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ConfigurationHelper.PolicyName);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
