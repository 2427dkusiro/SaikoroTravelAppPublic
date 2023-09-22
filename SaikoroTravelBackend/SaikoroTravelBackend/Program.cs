using Microsoft.EntityFrameworkCore;

using SaikoroTravelBackend;
using SaikoroTravelBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserContext>(opt =>
{
    opt.UseSqlServer("Server=tcp:saikorotravelbackenddbserver.database.windows.net,1433;Initial Catalog=SaikoroTravelBackend_db;Persist Security Info=False;User ID=kujiro;Password=1187hokureI;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

FCMService.Init();
await DiscordService.Init();
Task.Run(FCMTimer.CheckLoop);

app.Run();
