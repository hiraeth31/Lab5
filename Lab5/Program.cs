using Microsoft.AspNetCore.Authentication.Cookies; // для авторизаци куки

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

#region Для Сессий
builder.Services.AddDistributedMemoryCache(); // Необходимо для хранения данных сессии в памяти
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии без активности
    options.Cookie.HttpOnly = true; // Делаем cookie сессии недоступной для JS на клиенте
    options.Cookie.IsEssential = true; // Важно для GDPR и работы сессий
});
#endregion

#region Для авторизации куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Страница, куда перенаправлять, если доступ запрещен 
        options.LogoutPath = "/Logout"; // Страница для выхода
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Время жизни cookie аутентификации
        options.SlidingExpiration = true; // Продлевать время жизни при активности
    });

builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

#region Для авторизации куки
app.UseAuthentication(); // Включаем аутентификацию
app.UseAuthorization();  // Включаем авторизацию (даже если правила простые)
#endregion

#region Для Сессий
app.UseSession(); // Включаем middleware для сессий
#endregion

app.MapRazorPages();

app.Run();
