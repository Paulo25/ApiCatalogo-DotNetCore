using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// registrando servico, transient(a instancia da interface será criada a cada vez que for chamada)
builder.Services.AddTransient<IMeuServico, MeuServico>();

// Add services to the container.
builder.Services.AddControllers().
    AddJsonOptions(options => 
        options.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apiagenda", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header de autorização JWT usando o esquema Bearer.\r\n\r\nInforme 'Bearer'[espaço] e o seu token.\r\n\r\nExamplo: \'Bearer 12345abcdef\'",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
          new string[] {}
       }
    });
});

////definir politica cors via atributo
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("PermitirApiRequest",
//        builder =>
//        builder.WithOrigins("https://wwwapirequest.io/")
//     .WithMethods("GET")
//     );
//});
//definir politica cors via middleware
builder.Services.AddCors();


//configuração conexão com banco de dados
var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(mySqlConnection,
                    ServerVersion.AutoDetect(mySqlConnection)));

//adicionando serviço Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

//JWT
//adiciona o manipulador de autenticacao e define o 
//esquema de autenticacao usado : Bearer
//valida o emissor, a audiencia e a chave
//usando a chave secreta valida a assinatura
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
         ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
     });

//configurando serviço de log filter
builder.Services.AddScoped<ApiLoggingFilter>();

//registrando serviço padrão de unidade de trabalho
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//registrando serviço de automapper
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//registrando serviço de logging
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));


//depois do build vem as configurações de pipeline
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//adiciona o middleware de tratamento de erros
app.ConfigureExceptionHandler();

//adiciona o middleware para redirecionar para https
app.UseHttpsRedirection();

//adiciona o middleware de roteamento
app.UseRouting();

//adiciona o middleware de autenticação
app.UseAuthentication();

//adiciona o middleware que executa o endpoint do request atual
app.UseAuthorization();

//politica CORS restritiva, via middleware
//app.UseCors(opt => opt.
//    WithOrigins("https://wwwapirequest.io/")
//     .WithMethods("GET"));

//via atributo 
//app.UseCors();

//permite qualquer origem, via middleware
app.UseCors(opt => opt.AllowAnyOrigin());


//adiciona o middleware que executa o mapeamento das controllers
app.MapControllers();

app.Run();
