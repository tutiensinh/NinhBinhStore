using NinhBinhStore.DAO;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký HttpContextAccessor để dùng được trong View (Layout)
builder.Services.AddHttpContextAccessor();

// 2. CẤU HÌNH SESSION (Quan trọng: Giỏ hàng và Đăng nhập cần cái này)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// 3. Đăng ký Dependency Injection cho DAO
// Nên dùng AddScoped thay vì AddTransient cho các ứng dụng Web
builder.Services.AddScoped<DBContext>();
builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<OrderDAO>();
builder.Services.AddScoped<ContactDAO>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// 4. KÍCH HOẠT SESSION (Phải đặt trước UseRouting)
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
