using LMS.Infrastructure.Data; // 1. Thêm dòng này để nhận diện AppDbContext
using Microsoft.EntityFrameworkCore; // 2. Thêm dòng này để dùng SQL Server
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // Dùng để cấu hình Swagger có nút ổ khóa

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- BẮT ĐẦU ĐOẠN CODE BẠN BỊ THIẾU ---
// Đăng ký AppDbContext và lấy chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// --- KẾT THÚC ĐOẠN CODE BẠN BỊ THIẾU ---

builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;// Xác thực ai đó đăng nhập
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;// yeu cau người dùng cung cấp thông tin đăng nhập
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,// xac thuc nguoi phat hanh token
        ValidateAudience = true,// xac thuc nguoi nhan token
        ValidateLifetime = true,// xac thuc thoi gian ton tai cua token
        ValidateIssuerSigningKey = true,// xac thuc khoa ky token
        ValidIssuer = builder.Configuration["Jwt:Issuer"],// nguoi phat hanh token hop le
        ValidAudience = builder.Configuration["Jwt:Audience"],// nguoi nhan token hop le
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))// khoa ky token
    };
});
//Cau hinh swagger de co nut khoa
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1" });
    //dinh nghia bao mat dung Bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter token in the format: Bearer {your token}",// mo ta cho nguoi dung biet cach nhap token
        Name = "Authorization",// ten header se gui token
        In = ParameterLocation.Header, // token se duoc gui trong header
        Type = SecuritySchemeType.ApiKey,// kieu xac thuc la ApiKey
        Scheme = "Bearer"// ten cua scheme
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();