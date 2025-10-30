using Microsoft.OpenApi.Models;
using sso_back.Middlewares;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Swagger Bookini.CRM.Api",
        Description = "Bookini APIs Project"
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
    }
}); 


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();



app.Run();

