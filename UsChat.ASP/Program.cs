using UsChat.BLL.Container;
using UsChat.DAL;
using UsChat.DTO.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


//Dendency injection.
//3 types. use diffrent one depeding on usses and recource mangement.
//Scoped: 1 instances per http request. can use multiple times in same request. usefull for exampe DB acces. it gets dispossed of after request.
//Transient: for a new instance every time you ask for it. only use for lightweight stuff. usefull for Stateless helper(?)
//Singleton: lives from start to finish in application shared across all http request. usefull for caching or logging.
builder.Services.AddScoped<IUserRelationDAL, UserRelationDAL>();
builder.Services.AddScoped<IGroupDAL, GroupDAL>();
builder.Services.AddScoped<IMessageDAL, MessageDAL>();
builder.Services.AddScoped<IUserCRUDDAL, UserCRUDDAL>();
builder.Services.AddTransient<IEmailVerificationDAL, EmailVerificationDAL>();
builder.Services.AddTransient<EmailSenderContainer>();

builder.Services.AddScoped<MessageContainer>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


//Configure SignalR endpoint for the hub
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
