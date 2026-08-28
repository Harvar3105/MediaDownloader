using System.Text.Json.Serialization;
using MediaDownloader.Application;
using MediaDownloader.Runners;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy => policy.WithOrigins(
    "http://localhost:5173",
    "https://localhost:5173",
    "http://localhost:5003",
    "http://127.0.0.1:5003",
    "http://localhost:5000",
    "http://127.0.0.1:5000"));
});

builder.Services.AddScoped<VideoAndAudioDownloader>();
builder.Services.AddScoped<YtdlpController>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
