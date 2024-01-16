using AlunosApi.Context;
using AlunosApi.Middlewares;
using AlunosApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrando serviços
builder.Services.AddScoped<IAuthenticate, AuthenticateService>();
builder.Services.AddScoped<IAlunoService, AlunosService>();

// Banco de dados
string sqlSeverConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>
                              (options => options.UseSqlServer(sqlSeverConnection));

// Identity || IdentityUser: propriedades do user que vai se autenticar   ||  IdentityRole: Perfis do usuário  
builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Adiciona config padrão ao IdentityUser e Role
    .AddEntityFrameworkStores<AppDbContext>() // Armazenar e recéprar infos dos usuários/perfis registrados
    .AddDefaultTokenProviders(); // Gera token nas operações de conta do user como redefinição de senha/alteração do email

// JWT
builder.Services.AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme).
        AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
        });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitando middleware de erros
app.UseMiddleware(typeof(GlobalErrorHandlingMiddleware));

// Habilitando cors
app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();