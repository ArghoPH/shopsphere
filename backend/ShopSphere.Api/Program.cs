using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopSphere.Api.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT key is missing.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueApp", policy =>
    {
        policy.WithOrigins(
                allowedOrigins.Length > 0
                    ? allowedOrigins
                    : new[]
                    {
                        "http://localhost:5173",
                        "http://localhost:5174",
                        "http://127.0.0.1:5173",
                        "http://127.0.0.1:5174"
                    }
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(30);
    })
    .UseSnakeCaseNamingConvention();
});

var app = builder.Build();

app.UseCors("VueApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>ShopSphere API</title>
    <style>
        body {
            margin: 0;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: Arial, sans-serif;
            background: linear-gradient(135deg, #020617, #1e293b);
            color: white;
        }

        .card {
            max-width: 620px;
            width: 90%;
            padding: 40px;
            border-radius: 28px;
            background: rgba(255, 255, 255, 0.08);
            box-shadow: 0 25px 80px rgba(0, 0, 0, 0.35);
            border: 1px solid rgba(255, 255, 255, 0.14);
            text-align: center;
        }

        .badge {
            display: inline-block;
            padding: 8px 14px;
            border-radius: 999px;
            background: #22c55e;
            color: #052e16;
            font-weight: 700;
            font-size: 13px;
            margin-bottom: 18px;
        }

        h1 {
            margin: 0;
            font-size: 42px;
            letter-spacing: -1px;
        }

        p {
            color: #cbd5e1;
            line-height: 1.7;
            font-size: 16px;
        }

        .links {
            display: flex;
            gap: 12px;
            justify-content: center;
            flex-wrap: wrap;
            margin-top: 28px;
        }

        a {
            color: white;
            text-decoration: none;
            padding: 12px 18px;
            border-radius: 14px;
            background: rgba(255, 255, 255, 0.12);
            font-weight: 700;
            transition: 0.2s;
        }

        a:hover {
            background: #2563eb;
        }

        .footer {
            margin-top: 24px;
            font-size: 13px;
            color: #94a3b8;
        }
    </style>
</head>
<body>
    <div class="card">
        <div class="badge">API LIVE</div>
        <h1>ShopSphere API</h1>
        <p>
            The ShopSphere backend is running successfully.
            This API powers products, categories, cart, checkout, orders,
            authentication, admin management and image uploads.
        </p>

        <div class="links">
            <a href="/api/products">Products API</a>
            <a href="/api/categories">Categories API</a>
            <a href="https://shopsphere-red.vercel.app">Frontend Website</a>
        </div>

        <div class="footer">
            Built with ASP.NET Core Web API, Supabase, Docker and Render.
            <p>© 2024 ShopSphere. All rights reserved.</p>
            <p>Developed by <a href="https://github.com/ArghoPH">Argho Chakma</a></p>
        </div>

        <div class="footer">
           
        </div>



    </div>
</body>
</html>
""", "text/html"));

app.Run();