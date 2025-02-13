using CollageApp.Data;
using CollageApp.MyLogging;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Microsoft.Extensions.Options;
using System.Diagnostics.Eventing.Reader;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Log/log.txt",rollingInterval: RollingInterval.Minute)
    .CreateLogger();
builder.Logging.AddSerilog();
builder.Logging.ClearProviders();
// connection string :-
builder.Services.AddDbContext<CollegeDBContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeAppDBConnection"));
});


builder.Services.AddControllers(
    options =>options.ReturnHttpNotAcceptable=true).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMyLogger, LogToServerMemory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
