using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

_ = builder.Services.AddControllers();
_ = builder.Services.AddEndpointsApiExplorer();

_ = builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth?prompt=consent"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "Open ID" },
                    { "email", "Email" },
                    { "profile", "Profile" },
                    { "https://www.googleapis.com/auth/drive.readonly", "Google Drive Read-Only Access" }
                }
            }
        }
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            ["openid", "email", "profile", "https://www.googleapis.com/auth/drive.readonly"]
        }
    });
});

_ = builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", b => b.SetIsOriginAllowed((h) => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

_ = builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddGoogleOpenIdConnect(o =>
{
    o.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();

    _ = app.UseSwaggerUI(o =>
    {
        o.OAuthClientId(builder.Configuration["Authentication:Google:ClientId"]);
        o.OAuthClientSecret(builder.Configuration["Authentication:Google:ClientSecret"]);
        o.OAuthUsePkce();
    });
}

_ = app.UseHttpsRedirection();
_ = app.UseCors("CorsPolicy");
_ = app.UseAuthentication();
_ = app.UseAuthorization();
_ = app.MapControllers();
app.Run();
