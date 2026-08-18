using MediaDownloader.Application;
using MediaDownloader.Youtube;
using System.Text.Json.Serialization;

var webRootPath = Path.GetFullPath(
  Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "wwwroot")
);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
  Args = args,
  WebRootPath = webRootPath,
});

builder.Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  });
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<YoutubeDownloader>();
builder.Services.AddScoped<YtdlpController>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
