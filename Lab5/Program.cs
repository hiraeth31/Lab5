using Microsoft.AspNetCore.Authentication.Cookies; // ��� ���������� ����

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

#region ��� ������
builder.Services.AddDistributedMemoryCache(); // ���������� ��� �������� ������ ������ � ������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ����� ����� ������ ��� ����������
    options.Cookie.HttpOnly = true; // ������ cookie ������ ����������� ��� JS �� �������
    options.Cookie.IsEssential = true; // ����� ��� GDPR � ������ ������
});
#endregion

#region ��� ����������� ����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // ��������, ���� ��������������, ���� ������ �������� 
        options.LogoutPath = "/Logout"; // �������� ��� ������
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // ����� ����� cookie ��������������
        options.SlidingExpiration = true; // ���������� ����� ����� ��� ����������
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

#region ��� ����������� ����
app.UseAuthentication(); // �������� ��������������
app.UseAuthorization();  // �������� ����������� (���� ���� ������� �������)
#endregion

#region ��� ������
app.UseSession(); // �������� middleware ��� ������
#endregion

app.MapRazorPages();

app.Run();
