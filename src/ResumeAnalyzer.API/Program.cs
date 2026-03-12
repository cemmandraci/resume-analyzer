using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.API.Middleware;
using ResumeAnalyzer.Application.Commands.AnalyzeResume;
using ResumeAnalyzer.Infrastructure;
using ResumeAnalyzer.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {Title =  "ResumeAnalyzer API", Version = "v1"});
});

builder.Services.AddMediatR(typeof(AnalyzeResumeCommand).Assembly);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

//Auto Migrate
/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ResumeDbContext>();
    db.Database.Migrate();
}
*/

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();

