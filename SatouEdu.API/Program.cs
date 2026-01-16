using LMS.Infrastructure.Data; // 1. Thêm dòng này để nhận diện AppDbContext
using Microsoft.EntityFrameworkCore; // 2. Thêm dòng này để dùng SQL Server
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- BẮT ĐẦU ĐOẠN CODE BẠN BỊ THIẾU ---
// Đăng ký AppDbContext và lấy chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// --- KẾT THÚC ĐOẠN CODE BẠN BỊ THIẾU ---

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();