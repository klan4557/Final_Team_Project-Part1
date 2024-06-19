
// �� ���ø����̼��� ������ �����ϰ�, �ʿ��� ������ ���� builder ��ü�� �����մϴ�.
using Microsoft.EntityFrameworkCore;
using MVCTest1.Data;
using MVCTest1.Services;
using Org.BouncyCastle.Tls;
using MVCTest1.TcpIp;
using MVCTest1;
using Microsoft.AspNetCore.SignalR;
//var builder = WebApplication.CreateBuilder(args);

//// ���� �����̳ʿ� ��Ʈ�ѷ��� �並 ����ϴ� ���񽺸� �߰��մϴ�.
//// �̴� MVC ������ ����Ͽ� ���ø����̼��� ������ �� �ʿ��մϴ�.
//builder.Services.AddControllersWithViews();
//// ������ ����ϱ����ؼ� �߰�
//// Program.cs �Ǵ� Startup.cs ���� ������


//// IHttpContextAccessor ���񽺸� ����մϴ�.
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<idcard_detection_interface, idcard_detection_implement>();
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//});

//builder.Services.AddSession(options =>
//{
//    // Idle timeout ���� �����մϴ�. ���⼭�� 30������ �����մϴ�.
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//});

//// ������ ������ builder�� ����Ͽ� app ��ü�� �����մϴ�.
//// �� ��ü�� ���ø����̼��� �����ֱ⸦ �����մϴ�.
//var app = builder.Build();



//// HTTP ��û ������������ �����մϴ�.
//// ���� ȯ���� �ƴ� ���, ���� �������� ó���ϴ� �̵��� ����մϴ�.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error"); // ���� ó���⸦ ����Ͽ� ���� �������� �����̷�Ʈ�մϴ�.
//    app.UseHsts(); // HTTP Strict Transport Security ���������� ����մϴ�. �̴� �� ���ø����̼� ������ ��ȭ�մϴ�.
//}

//app.UseHttpsRedirection(); // HTTPS���� �ڵ� �����̷����� Ȱ��ȭ�մϴ�.
//app.UseStaticFiles(); // ���� ������ �����ϴ� �̵��� Ȱ��ȭ�մϴ�. �̴� �̹���, CSS, JavaScript ���� ���� ó���մϴ�.

//app.UseRouting(); // ������� Ȱ��ȭ�մϴ�. �̴� URL�� ���ø����̼��� ��������Ʈ�� �����ϴ� ������ �մϴ�.

//app.UseAuthentication(); // ���� �̵���� �߰�
//app.UseSession();   //���� Ȱ��ȭ
//app.UseAuthorization(); // ���� �ο� �̵��� Ȱ��ȭ�մϴ�. �̴� ������� ���� �� ������ ó���մϴ�.

//// ���� ���� ���� ��θ� ȯ�� ������ �����մϴ�.
//// �� ��δ� Google Cloud���� ������ ���� ���� Ű ������ ��ġ�Դϴ�.
//Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\JHG\\c#\\MVCTest1\\ocr-test-project-416305-80f44c1d7331.json");

//// ���ø����̼��� ����� ��Ģ�� �����մϴ�.
//// �⺻������ 'Home' ��Ʈ�ѷ��� 'Index' �׼� �޼���� ����õ˴ϴ�.
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//var tcpServer = new Tcp_ip(13000); // ���� ��Ʈ ��ȣ: 13000
//Task.Run(() => tcpServer.StartAsync());
//// ���ø����̼��� �����մϴ�.
//app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IHttpContextAccessor for accessing HttpContext in other classes.
builder.Services.AddHttpContextAccessor();

// Register custom services.
builder.Services.AddSingleton<idcard_detection_interface, idcard_detection_implement>();
builder.Services.AddSingleton<FaceDetection>(provider => new FaceDetection("C:\\JHG\\c#\\MVCTest1\\Models\\model"));

// Configure the database context.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Configure session with a 30-minute idle timeout.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SignalR services.
builder.Services.AddSignalR();

// Build the app.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Error page for non-development environments.
    app.UseHsts(); // Enforce strict transport security.
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS.
app.UseStaticFiles(); // Serve static files.

app.UseRouting(); // Enable routing.

app.UseAuthentication(); // Enable authentication.
app.UseSession(); // Enable session management.
app.UseAuthorization(); // Enable authorization.

// Set the path to the Google Cloud credentials JSON file.
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\JHG\\c#\\MVCTest1\\innate-shell-425109-e7-da4daaf3c8e4.json");

// Define the default routing scheme.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<NotificationHub>("/notificationHub"); // SignalR ��� ����

// Start the TCP server on a separate task.
var tcpServer = new Tcp_ip(13000, app.Services.GetRequiredService<IHubContext<NotificationHub>>(), app.Services.GetRequiredService<ILogger<Tcp_ip>>()); // Example port number: 13000
Task.Run(() => tcpServer.StartAsync());

// Run the app.
app.Run();
