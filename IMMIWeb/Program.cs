using IMMIWeb;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Service.Appointment;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.Favourite;
using IMMIWeb.Service.Service.Setting;
using IMMIWeb.Service.Service.User;
using IMMIWeb.Service.Service.Retains;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IMMIWeb.Service.Service.Retains;
using AutoMapper;
using IMMIWeb.Service.Service.CMS;
using Stripe;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Text;
using IMMIWeb.Service.Service.ReviewRating;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IMMIWeb.Service.Models;

var builder = WebApplication.CreateBuilder(args);

var connetionString = builder.Configuration.GetConnectionString("dbConn");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DbA976eeImmitestContext>(options => options.UseSqlServer(connetionString));
builder.Services.AddScoped<IConsultantRepository, ConsultantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITypeOfServiceRepository, TypeOfServiceRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IAppointmentPaymentRepository, AppointmentPaymentRepository>();
builder.Services.AddScoped<IOtherRepository, OtherRepository>();
builder.Services.AddScoped<IRetainRepository, RetainRepository>();
builder.Services.AddScoped<ICMSRepository, CMSRepository>();
builder.Services.AddScoped<IHelpRepository, HelpRepository>();
builder.Services.AddScoped<IReviewRatingRepository, ReviewRatingRepository>();
builder.Services.AddScoped<IAppLanguageRepository, AppLanguageRepository>();
builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews().AddCookieTempDataProvider();
builder.Services.AddTransient<EmailTemplateMaker>();


//Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");

    var cultures = new CultureInfo[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR")
    };

    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});


//Login Start
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = "apple";
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(option =>
{
    option.ExpireTimeSpan = TimeSpan.FromHours(24);
    option.LoginPath = "/UserAccount/Login";
    option.AccessDeniedPath = "/UserAccount/Login";
})
.AddGoogle(options =>
{
    options.ClientId = "414499700146-nlol8cik3n3ea13ke0vara7pqt2mdagi.apps.googleusercontent.com";
    options.ClientSecret = "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";
});
//.AddOpenIdConnect(options =>
//  {
//      options.Authority = "https://appleid.apple.com"; // disco doc: https://appleid.apple.com/.well-known/openid-configuration

//      options.ClientId = "JRN5W5EA66.com.app.advenuss"; // Service ID
//      //options.CallbackPath = "/UserAccount/AppleSignInCallback"; // corresponding to your redirect URI


//      options.ResponseType = "code id_token"; // hybrid flow due to lack of PKCE support
//      options.ResponseMode = "form_post"; // form post due to prevent PII in the URL
//      options.DisableTelemetry = true;

//      options.Scope.Clear(); // apple does not support the profile scope
//      options.Scope.Add("openid");
//      options.Scope.Add("email");
//      options.Scope.Add("name");

//      // custom client secret generation - secret can be re-used for up to 6 months
//      options.Events.OnAuthorizationCodeReceived = context =>
//      {
//          context.TokenEndpointRequest.ClientSecret = TokenGenerator.CreateNewToken();
//          return Task.CompletedTask;
//      };

//      options.UsePkce = false; // apple does not currently support PKCE (April 2021)
//  });




//Login End

// Add Stripe Infrastructure
builder.Services.AddStripeInfrastructure(builder.Configuration);

builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();


//website translator

//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


var app = builder.Build();


//For session factory start
builder.Services.AddHttpContextAccessor();
SessionFactory.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
//For session factory end

static async Task Main(string[] args)
{
    await FetchAndDisplayExchangeRates();
}

static async Task FetchAndDisplayExchangeRates()
{
    string apiKey = "3272ae92bd856f2e2aa8fa1a";
    string exchangeRateApiUrl = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/USD";

    using (var client = new HttpClient())
    {
        try
        {
            client.DefaultRequestHeaders.Add("apikey", apiKey); // Add API key to request header

            HttpResponseMessage response = await client.GetAsync(exchangeRateApiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody); // Display the response in console

            // You need to parse the JSON response and work with the data as needed
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapPost("/settimezone", async context =>
    {
        // Parse the incoming JSON data to get the client's timezone
        using var reader = new StreamReader(context.Request.Body);
        var json = await reader.ReadToEndAsync();
        var timezoneData = JsonConvert.DeserializeObject<TimeZoneData>(json);

        // Use the client's timezone as needed
        string clientTimezone = timezoneData.Timezone;
        SessionFactory.TimeZone = clientTimezone;
        //SessionFactory.UserTimeZone = clientTimezone;
        // You can store the timezone in a session or a database if needed
        // For demonstration purposes, we'll just respond with the timezone
        //context.Response.ContentType = "application/json";
        //await context.Response.WriteAsync($"{{ \"timezone\": \"{clientTimezone}\" }}");
    });
});

app.MapAreaControllerRoute(
    name: "ConsultantSignUp",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAccount}/{action=ConsultantSignUp}");

app.MapAreaControllerRoute(
    name: "ConsultantOTPVerification",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAccount}/{action=ConsultantOTPVerification}");

app.MapAreaControllerRoute(
    name: "ConsultantLogin",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAccount}/{action=ConsultantLogin}");

app.MapAreaControllerRoute(
    name: "ConsultantLoginOTPVerification",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAccount}/{action=ConsultantLoginOTPVerification}");

app.MapAreaControllerRoute(
    name: "ManageSlot",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantSlot}/{action=ManageSlot}");

app.MapAreaControllerRoute(
    name: "UserAppointmentRequest",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAppointment}/{action=UserAppointmentRequest}");

app.MapAreaControllerRoute(
    name: "CMSConfigurationConsultant",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantCMS}/{action=CMSConfigurationConsultant}");

app.MapAreaControllerRoute(
    name: "GetServiceByCountry",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAccount}/{action=GetServiceByCountry}");

app.MapAreaControllerRoute(
    name: "Index",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantHome}/{action=Index}");

app.MapAreaControllerRoute(
    name: "TextChatConsultant",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantChat}/{action=TextChatConsultant}");

app.MapAreaControllerRoute(
    name: "ConsultantReceiveCall",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantChat}/{action=ConsultantReceiveCall}");

app.MapAreaControllerRoute(
    name: "AppointmentPendingList",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAppointment}/{action=AppointmentPendingList}");


app.MapAreaControllerRoute(
    name: "AppointmentAcceptedList",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAppointment}/{action=AppointmentAcceptedList}");


app.MapAreaControllerRoute(
    name: "AppointmentRejectedList",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAppointment}/{action=AppointmentRejectedList}");

app.MapAreaControllerRoute(
    name: "Clientlist",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantRetain}/{action=Clientlist}");

app.MapAreaControllerRoute(
    name: "AppointmentDetails",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantAppointment}/{action=AppointmentDetails}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute( 
    name: "RetainUserChat",
    areaName: "Consultant",
    pattern: "Consultant/{controller=ConsultantChat}/{action=RetainUserChat}");


app.MapPost("/fetch-exchange-rates", async (HttpContext context) =>
{
    await FetchAndDisplayExchangeRates();
    await context.Response.WriteAsync("Exchange rates fetched and displayed.");
});


app.Run();

public static class TokenGenerator
{
    public static string CreateNewToken()
    {
        System.IO.File.AppendAllText("CreateNewToken.txt", "goes");
        const string iss = "JRN5W5EA66"; // your accounts team ID found in the dev portal
        const string aud = "https://appleid.apple.com";
        const string sub = "JRN5W5EA66.com.app.advenuss"; // same as client_id
        var now = DateTime.UtcNow;


        string contentRootPath = "h:\\root\\home\\iqlance-001\\www\\immiweb-dev\\appleprivatekey\\";
        string filePath = System.IO.Path.Combine(contentRootPath, "AuthKey_X7D76297QN.p8");

        //System.IO.File.AppendAllText("CreateNewToken.txt", "filePath " + filePath);

        string privateKey = System.IO.File.ReadAllText(filePath);

        //const string privateKey = "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgg61bNRs7CvNy91QUc6uhElt+UB2JIB0aZ4hnm2IZiBagCgYIKoZIzj0DAQehRANCAASwmJCnOeGyAYPAuiB6BqzkKxGgO5rSKeLmXFXe6b/qHLmLY92+WjQLIMCX3G0toCtnwBqEGXm+yzcMkKqbh6qn";

        System.IO.File.AppendAllText("CreateNewToken.txt", "PrivateKey " + privateKey);


        //var ecdsa = ECDsa.Create();
        //ecdsa?.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);



        RSA rSA = RSA.Create();

        bool isPkcsprivateKey = privateKey.Contains("BEGIN PRIVATE KEY");
        System.IO.File.AppendAllText("isPkcsprivateKey.txt", "isPkcsprivateKey " + isPkcsprivateKey);

        if (isPkcsprivateKey)
        {
            var privateKeyValue = privateKey.Replace("-----BEGIN PRIVATE KEY-----", string.Empty).Replace("-----END PRIVATE KEY-----", string.Empty);
            System.IO.File.AppendAllText("privateKeyValue.txt", "privateKeyValue " + privateKeyValue);
            privateKeyValue = privateKey.Replace(Environment.NewLine, string.Empty);
            var privateKeyBytes = Convert.FromBase64String(privateKey);
            rSA.ImportPkcs8PrivateKey(privateKeyBytes, out int _);
            System.IO.File.AppendAllText("privateKeyBytes.txt", "privateKeyBytes " + privateKeyBytes);
        }
        else
        {
            var privateKeyValue = privateKey.Replace("-----BEGIN RSA PRIVATE KEY-----", string.Empty).Replace("-----END RSA PRIVATE KEY-----", string.Empty);
            privateKeyValue = privateKey.Replace(Environment.NewLine, string.Empty);
            var privateKeyBytes = Convert.FromBase64String(privateKey);
            rSA.ImportRSAPrivateKey(privateKeyBytes, out int _);
            System.IO.File.AppendAllText("privateKeyByteselse.txt", "privateKeyBytes else " + privateKeyBytes);
        }

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = iss,
            Audience = aud,
            Claims = new Dictionary<string, object> { { "sub", sub } },
            Expires = now.AddMinutes(5), // expiry can be a maximum of 6 months - generate one per request or re-use until expiration
            IssuedAt = now,
            NotBefore = now,
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rSA), SecurityAlgorithms.EcdsaSha256)
        });
    }
}



