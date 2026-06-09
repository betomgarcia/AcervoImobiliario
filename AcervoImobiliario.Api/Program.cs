using AcervoImobiliario.Api.Middleware;
using AcervoImobiliario.Application;
using AcervoImobiliario.Infrastructure;
using AcervoImobiliario.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Acervo Imobiliário API",
        Version = "v1",
        Description = "API para cadastro de imóveis por endereço único e histórico de eventos imobiliários."
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongoDbInitializer = scope.ServiceProvider.GetRequiredService<MongoDbInitializer>();
    await mongoDbInitializer.InitializeAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Acervo Imobiliário API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
