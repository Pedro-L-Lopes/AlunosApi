using AlunosApi.Context;
using AlunosApi.Middlewares;
using AlunosApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
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

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "APICatalogo",
        Version = "v1",
        Description = "Cadastro e edição de alunos",
        TermsOfService = new Uri("https://github.com/Pedro-L-Lopes/AlunosApi"),
        Contact = new OpenApiContact
        {
            Name = "pedro",
            Email = "Pedro123@aaa.com",
            Url = new Uri("https://github.com/Pedro-L-Lopes/AlunosApi"),
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://github.com/Pedro-L-Lopes/AlunosApi"),
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);


    // Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header de autorização JWT usando o esquema Bearer.\r\n\r\nInforme 'Bearer'[espaço]..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
             new string[]{}
        }

    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlunosApi");
});

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