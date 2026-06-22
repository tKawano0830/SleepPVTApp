using Microsoft.EntityFrameworkCore;
using SleepPvtTracker.Core.Domain.DomainServices;
using SleepPvtTracker.Core.Interfaces;
using SleepPvtTracker.Core.UseCases;
using SleepPvtTracker.Infrastructure.Data;
using SleepPvtTracker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=sleeppvttracker.db"));

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

builder.Services.AddScoped<ISleepRecordRepository, SleepRecordRepository>();
builder.Services.AddScoped<ISleepRecordUseCase, SleepRecordUseCase>();
builder.Services.AddScoped<SleepRecordOverlapChecker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SleepRecord}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}


app.Run();
