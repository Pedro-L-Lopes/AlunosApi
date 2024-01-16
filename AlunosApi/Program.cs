using AlunosApi.Context;
using AlunosApi.Middlewares;
using AlunosApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrando serviços
builder.Services.AddScoped<IAlunoService, AlunosService>();

// Banco de dados
string sqlSeverConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>
                              (options => options.UseSqlServer(sqlSeverConnection));

// Identity || IdentityUser: propriedades do user que vai se autenticar   ||  IdentityRole: Perfis do usuário  
builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Adiciona config padrão ao IdentityUser e Role
    .AddEntityFrameworkStores<AppDbContext>() // Armazenar e recéprar infos dos usuários/perfis registrados
    .AddDefaultTokenProviders(); // Gera token nas operações de conta do user como redefinição de senha/alteração do email

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware(typeof(GlobalErrorHandlingMiddleware));

app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();