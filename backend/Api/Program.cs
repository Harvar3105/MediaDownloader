using MediaDownloader.Youtube;

var webRootPath = Path.GetFullPath(
  Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "wwwroot")
);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
  Args = args,
  WebRootPath = webRootPath,
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<YoutubeDownloader>();

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
