using HeavyStringFiltering.Application;
using HeavyStringFiltering.Application.Interfaces;
using HeavyStringFiltering.Application.Services;
using HeavyStringFiltering.Domain.FilteringStrategies;
using HeavyStringFiltering.Domain.FilterWords;
using HeavyStringFiltering.Infrastructure.BackgroundServices;
using HeavyStringFiltering.Infrastructure.Storage;
using MediatR;
;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FilterConfig>(
    builder.Configuration.GetSection("FilterConfig"));

builder.Services.AddSingleton<ISimilarityStrategy, LevenshteinStrategy>();
builder.Services.AddSingleton<ITextFilteringService, TextFilteringService>();
builder.Services.AddSingleton<ITextQueueService, TextQueueService>();

//builder.Services.AddHostedService<TextFilterWorker>();
builder.Services.AddHostedService<ParallelTextFilterWorker>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITextStorageCache, TextStorageCache>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
