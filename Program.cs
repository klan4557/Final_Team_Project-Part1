
// 웹 애플리케이션의 실행을 시작하고, 필요한 설정을 담은 builder 객체를 생성합니다.
using Microsoft.EntityFrameworkCore;
using MVCTest1.Data;
using MVCTest1.Services;
using Org.BouncyCastle.Tls;
using MVCTest1.TcpIp;
using MVCTest1;
using Microsoft.AspNetCore.SignalR;
//var builder = WebApplication.CreateBuilder(args);

//// 서비스 컨테이너에 컨트롤러와 뷰를 사용하는 서비스를 추가합니다.
//// 이는 MVC 패턴을 사용하여 애플리케이션을 구성할 때 필요합니다.
//builder.Services.AddControllersWithViews();
//// 세션을 사용하기위해서 추가
//// Program.cs 또는 Startup.cs 파일 내에서


//// IHttpContextAccessor 서비스를 등록합니다.
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<idcard_detection_interface, idcard_detection_implement>();
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//});

//builder.Services.AddSession(options =>
//{
//    // Idle timeout 값을 설정합니다. 여기서는 30분으로 설정합니다.
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//});

//// 위에서 설정된 builder를 사용하여 app 객체를 생성합니다.
//// 이 객체는 애플리케이션의 생명주기를 관리합니다.
//var app = builder.Build();



//// HTTP 요청 파이프라인을 구성합니다.
//// 개발 환경이 아닌 경우, 오류 페이지를 처리하는 미들웨어를 사용합니다.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error"); // 예외 처리기를 사용하여 에러 페이지로 리다이렉트합니다.
//    app.UseHsts(); // HTTP Strict Transport Security 프로토콜을 사용합니다. 이는 웹 애플리케이션 보안을 강화합니다.
//}

//app.UseHttpsRedirection(); // HTTPS로의 자동 리다이렉션을 활성화합니다.
//app.UseStaticFiles(); // 정적 파일을 제공하는 미들웨어를 활성화합니다. 이는 이미지, CSS, JavaScript 파일 등을 처리합니다.

//app.UseRouting(); // 라우팅을 활성화합니다. 이는 URL을 애플리케이션의 엔드포인트에 매핑하는 역할을 합니다.

//app.UseAuthentication(); // 인증 미들웨어 추가
//app.UseSession();   //세션 활성화
//app.UseAuthorization(); // 권한 부여 미들웨어를 활성화합니다. 이는 사용자의 인증 및 권한을 처리합니다.

//// 인증 정보 파일 경로를 환경 변수에 설정합니다.
//// 이 경로는 Google Cloud에서 생성한 서비스 계정 키 파일의 위치입니다.
//Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\JHG\\c#\\MVCTest1\\ocr-test-project-416305-80f44c1d7331.json");

//// 애플리케이션의 라우팅 규칙을 정의합니다.
//// 기본적으로 'Home' 컨트롤러의 'Index' 액션 메서드로 라우팅됩니다.
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//var tcpServer = new Tcp_ip(13000); // 예시 포트 번호: 13000
//Task.Run(() => tcpServer.StartAsync());
//// 애플리케이션을 실행합니다.
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
app.MapHub<NotificationHub>("/notificationHub"); // SignalR 허브 매핑

// Start the TCP server on a separate task.
var tcpServer = new Tcp_ip(13000, app.Services.GetRequiredService<IHubContext<NotificationHub>>(), app.Services.GetRequiredService<ILogger<Tcp_ip>>()); // Example port number: 13000
Task.Run(() => tcpServer.StartAsync());

// Run the app.
app.Run();
