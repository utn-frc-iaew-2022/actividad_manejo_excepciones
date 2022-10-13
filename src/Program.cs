
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.AspNetCore.Authorization;

// Nuevo Comentario Local.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Agregar URL de su cuenta de Auth0 de cada uno.
    options.Authority = "https://dev-utn-frc-iaew.auth0.com/";
    // Agregar audiencia de la API.
    options.Audience = "https://api.example.com";
});

//Configuracion para Validar Autorización. 
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("read:todoitems", policy => policy.Requirements.Add(new HasScopeRequirement("read:todoitems", "https://dev-utn-frc-iaew.auth0.com/")));
        options.AddPolicy("write:todoitems", policy => policy.Requirements.Add(new HasScopeRequirement("write:todoitems", "https://dev-utn-frc-iaew.auth0.com/")));
    });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    // app.UseSwagger();
    // app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

//Habilitamos Autenticacion.
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
