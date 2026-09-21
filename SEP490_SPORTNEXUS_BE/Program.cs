using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đọc chuỗi kết nối và khai báo dùng PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// (Tùy chọn) Cấu hình CORS để frontend có thể gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Bật Swagger UI trong môi trường dev
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Bật CORS
app.UseCors("AllowAll");

// Khai báo xác thực (Authentication) trước phân quyền (Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

