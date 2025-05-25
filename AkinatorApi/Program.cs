using Microsoft.EntityFrameworkCore;
using AkinatorApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ������������ DbContext � �������������� ������ ����������� �� appsettings.json
builder.Services.AddDbContext<AkinatorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS ��� ������
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") // ����� ���� � �����
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// ���������� CORS �� MapControllers
app.UseCors("AllowFrontend");

app.MapControllers();
app.Run();
