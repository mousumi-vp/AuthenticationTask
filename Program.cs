using AuthenticationTask.DatabaseContext;
using AuthenticationTask.Identity;
using AuthenticationTask.Middleware;
using AuthenticationTask.ServiceContracts;
using AuthenticationTask.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
    var policy=new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters();
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddTransient<IJwtService,JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,"api.xml"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>
    (options =>
    {
        options.Password.RequiredLength = 5;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole,
    ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole,
    ApplicationDbContext, Guid>>();

//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
    CookieAuthenticationDefaults.AuthenticationScheme;

})
 .AddJwtBearer(options=>
 {
     options.TokenValidationParameters = new
     Microsoft.IdentityModel.Tokens.TokenValidationParameters()
     {
         ValidateAudience = true,
     ValidAudience = builder.Configuration["Jwt:Audience"],
     ValidateIssuer = true,
     ValidIssuer= builder.Configuration["Jwt:Issuer"],
     ValidateLifetime=true,
     ValidateIssuerSigningKey=true,
     IssuerSigningKey=new SymmetricSecurityKey
     (System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
 };
 });
builder.Services.AddAuthorization(options =>
{

});

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
