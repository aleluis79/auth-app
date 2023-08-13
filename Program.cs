using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<RouteOptions>(options => {
   options.LowercaseQueryStrings = true;
   options.LowercaseUrls = true;
});

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication().AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.Authority = "http://localhost:8080/realms/prueba";
        cfg.TokenValidationParameters = new TokenValidationParameters {
            ValidAudience = "account",
            ValidIssuer = "http://localhost:8080/realms/prueba",
            ValidateAudience = true,
            ValidateIssuer = false,
        };
        cfg.Events = new JwtBearerEvents()
        {
            OnTokenValidated = (ctx) => {
                var clientName = ctx.Principal!.Claims.First(c => c.Type == "azp").Value;
                if (clientName != "Prueba-Client") {
                    ctx.Fail(new UnauthorizedAccessException("Client name incorrect!"));
                }
                return Task.CompletedTask;
            }
        };        
    }
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg => {

    cfg.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth APP", Version = "v1" });

    cfg.AddSecurityDefinition("oauth2",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow // Si se usa Implicit cambiar AuthorizationCode a Implicit
                {
                    AuthorizationUrl = new Uri("http://localhost:8080/realms/prueba/protocol/openid-connect/auth"),
                    TokenUrl = new Uri("http://localhost:8080/realms/prueba/protocol/openid-connect/token"),
                    Scopes = new Dictionary<string, string>(){ {"profile", "profile"} }
                }
            }
        });

    cfg.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Id = "oauth2", //The name of the previously defined security scheme.
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });

/*  Si solo se quiere ingresar un token JWT en Swagger se puede usar este código:

    cfg.AddSecurityDefinition("Bearer", //Name the security scheme
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
            Scheme = JwtBearerDefaults.AuthenticationScheme //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
        });

    cfg.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Id = JwtBearerDefaults.AuthenticationScheme, //The name of the previously defined security scheme.
                    Type = ReferenceType.SecurityScheme
                }
            },new List<string>()
        }
    });
*/
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(cfg => {
    cfg.OAuthClientId("Prueba-Client");
    cfg.OAuthClientSecret("04W37ykgye3Q2TUDVZyvzY3KqMHT95oG");
    cfg.OAuthUsePkce();
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
