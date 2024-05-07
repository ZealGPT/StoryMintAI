using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Sqlite;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using StoryMint.Container;
using StoryMint.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.private.json", true, true);

var cultureInfo = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
builder.Services.Configure<RouteOptions>(o =>
{
    o.AppendTrailingSlash = false;
    o.LowercaseUrls = true;
    o.SuppressCheckForUnhandledSecurityMetadata = false;
});

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"{connectionString}__{builder.Environment.EnvironmentName}.db"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration.GetValue<string>("Authentication:GitHub:ClientId")!;
        options.ClientSecret = builder.Configuration.GetValue<string>("Authentication:GitHub:ClientSecret")!;
    });

builder.Services.AddTransient(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.Services.AddLogging(l => l
        .SetMinimumLevel(LogLevel.Trace)
    );

    kernelBuilder.Services.AddAzureOpenAITextEmbeddingGeneration(
         configuration.GetValue<string>("AzureOpenAI:EmbeddingGeneration:DeploymentName")!,
         configuration.GetValue<string>("AzureOpenAI:EmbeddingGeneration:Endpoint")!,
         configuration.GetValue<string>("AzureOpenAI:EmbeddingGeneration:ApiKey")!
        );

    kernelBuilder.Services.AddAzureOpenAIChatCompletion(
         configuration.GetValue<string>("AzureOpenAI:ChatCompletion:DeploymentName")!,
         configuration.GetValue<string>("AzureOpenAI:ChatCompletion:Endpoint")!,
         configuration.GetValue<string>("AzureOpenAI:ChatCompletion:ApiKey")!
        );

    kernelBuilder.Services.AddAzureOpenAITextToImage(
         configuration.GetValue<string>("AzureOpenAI:TextToImage:DeploymentName")!,
         configuration.GetValue<string>("AzureOpenAI:TextToImage:Endpoint")!,
         configuration.GetValue<string>("AzureOpenAI:TextToImage:ApiKey")!
        );

    kernelBuilder.Services.AddAzureOpenAITextToAudio(
         configuration.GetValue<string>("AzureOpenAI:TextToAudio:DeploymentName")!,
         configuration.GetValue<string>("AzureOpenAI:TextToAudio:Endpoint")!,
         configuration.GetValue<string>("AzureOpenAI:TextToAudio:ApiKey")!
        );

    kernelBuilder.Services.AddAzureOpenAIAudioToText(
         configuration.GetValue<string>("AzureOpenAI:AudioToText:DeploymentName")!,
         configuration.GetValue<string>("AzureOpenAI:AudioToText:Endpoint")!,
         configuration.GetValue<string>("AzureOpenAI:AudioToText:ApiKey")!
        );

    var kernel = kernelBuilder.Build();

    kernel.PromptRenderFilters.Add(new PromptFilter());

    return kernel;
});
builder.Services.AddSingleton<IPromptRenderFilter, PromptFilter>();
builder.Services.AddTransient<AiService>();
builder.Services.AddMediatR(options => { 
    options.RegisterServicesFromAssemblyContaining<Program>();
});

builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = ctx => ctx.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName));
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseStatusCodePages();
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
