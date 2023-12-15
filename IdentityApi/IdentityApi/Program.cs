using IdentityApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<AppDbContext>(
   options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddIdentityCore<User>()
       .AddEntityFrameworkStores<AppDbContext>()
       .AddApiEndpoints();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGroup("identity")
   .MapIdentityApi<User>();

app.MapGet("/", () => "Home")
   .WithName("Home")
   .RequireAuthorization()
   .WithOpenApi();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();