
using API.Extensions;
using API.Middleware;
;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();



//Configure https pipeline
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("https://localhost:4200"));

//middleware, authentification first authorization second

//Authentification checks if the token is valid
app.UseAuthentication();
//Authorization checks what permissions the authentificaton has
app.UseAuthorization();

app.MapControllers();

app.Run();