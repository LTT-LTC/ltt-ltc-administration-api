using CloudinaryDotNet;
using LTC.AdministrationService.EntityFrameworkCore;
using LTC.AdministrationService.Grpc;
using LTC.AdministrationService.MultiTenancy;
using LTC.CustomerManagement.HealthChecks;
using LTC.CustomerManagement.MongoDb;
using LTC.Shared.Hosting.Microservices;
using LTC.Shared.Hosting.Microservices.Authentication;
using LTC.Shared.Hosting.Microservices.OpenApi.Swagger;
using LTC.AdministrationService;
using LTC.AdministrationService.EntityFrameworkCore;
using LTC.AdministrationService.MultiTenancy;
using LTC.Shared.Hosting.Microservices;
using LTC.Shared.Hosting.Microservices.Authentication;
using LTC.Shared.Hosting.Microservices.OpenApi.Swagger;
using MailKit.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using System.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Emailing;
using Volo.Abp.MailKit;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;

namespace LTC.AdministrationService;

[DependsOn(
    typeof(AdministrationServiceHttpApiModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AdministrationServiceApplicationModule),
    typeof(AdministrationServiceEntityFrameworkCoreModule),
    typeof(AdministrationServiceMongoDbModule),
    //typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(LTCSharedHostingMicroservicesModule),
    typeof(AbpEmailingModule),
    typeof(AbpMailKitModule)
    )]
public class AdministrationServiceHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        //PreConfigure<OpenIddictBuilder>(builder =>
        //{
        //    builder.AddValidation(options =>
        //    {
        //        options.AddAudiences("AdministrationService");
        //        options.UseLocalServer();
        //        options.UseAspNetCore();
        //    });
        //});

        //if (!hostingEnvironment.IsDevelopment())
        //{
        //    PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
        //    {
        //        options.AddDevelopmentEncryptionAndSigningCertificate = false;
        //    });

        //    PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
        //    {
        //        serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
        //        serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
        //    });
        //}
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var isEnabledNginx = configuration.GetSection("NginxSettings:IsEnabled").Get<bool>();

        // config chia tenant = header
        Configure<AbpAspNetCoreMultiTenancyOptions>(o => { o.TenantKey = "X-Tenant"; });
        Configure<Volo.Abp.AspNetCore.Mvc.AntiForgery.AbpAntiForgeryOptions>(options => { options.AutoValidate = false; });
        context.Services.AddGrpc();
        context.Services.AddGrpcReflection();
        Configure<AbpAuditingOptions>(options => { options.IsEnabled = false; });
        //Configure<AbpMvcLibsOptions>(options =>
        //{
        //    options.CheckLibs = false;
        //});
        context.Services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ApplicationExceptionFilterAttribute));
        });

        ConfigureCloudinary(context);
        ConfigureLocalization(context);
        ConfigureAuthentication(context);
        ConfigureMailKit(context);
        //ConfigureUrls(configuration);
        //ConfigureBundles();
        //ConfigureConventionalControllers();
        //ConfigureHealthChecks(context);
        //ConfigureSwagger(context, configuration);
        if (isEnabledNginx)
        {
            ConfigureForwardedHeader(context);
        }

        context.ConfigureSwaggerServices("LTC Administration Service API Endpoint", "v1");
        ConfigureVirtualFileSystem(context);
        context.Services.AddAdministrationServiceHealthChecks();
        ConfigureCors(context, configuration);
    }

    private void ConfigureMailKit(ServiceConfigurationContext context)
    {
        Configure<AbpMailKitOptions>(options => { options.SecureSocketOption = SecureSocketOptions.Auto; });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.ConfigureAuthenticationJwtBearer();

        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureCloudinary(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<Cloudinary>(provider =>
        {
            var configuration = context.Services.GetConfiguration();
            var cloudName = configuration["CloudinarySettings:CloudName"];
            var apiKey = configuration["CloudinarySettings:ApiKey"];
            var apiSecret = configuration["CloudinarySettings:ApiSecret"];

            var account = new Account(
                cloudName,
                apiKey,
                apiSecret
            );
            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;
            return cloudinary;
        });
    }

    #region ConfigureUrls

    //private void ConfigureUrls(IConfiguration configuration)
    //{
    //    Configure<AppUrlOptions>(options =>
    //    {
    //        options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
    //        options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
    //        options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
    //        options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
    //    });
    //}

    #endregion

    #region ConfigureBundles

    //private void ConfigureBundles()
    //{
    //    Configure<AbpBundlingOptions>(options =>
    //    {
    //        options.StyleBundles.Configure(
    //            LeptonXLiteThemeBundles.Styles.Global,
    //            bundle =>
    //            {
    //                bundle.AddFiles("/global-styles.css");
    //            }
    //        );

    //        options.ScriptBundles.Configure(
    //            LeptonXLiteThemeBundles.Scripts.Global,
    //            bundle =>
    //            {
    //                bundle.AddFiles("/global-scripts.js");
    //            }
    //        );
    //    });
    //}

    #endregion

    private void ConfigureForwardedHeader(ServiceConfigurationContext context)
    {
        context.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            //// Ch? tin t??ng proxy n�y
            //options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));

            //// Ho?c n?u proxy n?m trong 1 subnet
            //options.KnownNetworks.Add(
            //             //// Chỉ tin tưởng proxy này
            //options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));

            //// Hoặc nếu proxy nằm trong 1 subnet
        });
    }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    //options.FileSets.ReplaceEmbeddedByPhysical<AdministrationServiceDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}src/LTC.AdministrationService.Domain.Shared"));
                    //options.FileSets.ReplaceEmbeddedByPhysical<AdministrationServiceDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}src/LTC.AdministrationService.Domain"));
                    //options.FileSets.ReplaceEmbeddedByPhysical<AdministrationServiceApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}src/LTC.AdministrationService.Application.Contracts"));
                    //options.FileSets.ReplaceEmbeddedByPhysical<AdministrationServiceApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}src/LTC.AdministrationService.Application"));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(AdministrationServiceApplicationModule).Assembly);
            });
        }

        private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOidc(
                configuration["AuthServer:Authority"]!,
                ["AdministrationService"],
                [AbpSwaggerOidcFlows.AuthorizationCode],
                null,
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "LTC Administration Service API Endpoint", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                });
        }

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]?
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.Trim().RemovePostFix("/"))
                                .ToArray() ?? Array.Empty<string>()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        private void ConfigureHealthChecks(ServiceConfigurationContext context)
        {
            context.Services.AddHealthChecks()
                .AddDbContextCheck<AdministrationServiceDbContext>("Database")
                .AddCheck("self", () => HealthCheckResult.Healthy());
        }

        private void ConfigureLocalization(ServiceConfigurationContext context)
        {
            Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("vi"),
                    new CultureInfo("en")
                };

                options.DefaultRequestCulture = new RequestCulture("vi");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
            });
        }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        Console.WriteLine($"----------Start OnApplicationInitialization----------");

        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();
        var isEnabledNginx = configuration.GetSection("NginxSettings:IsEnabled").Get<bool>();
        app.UseForwardedHeaders();
        app.UseAbpRequestLocalization();

        if (env.IsDevelopment() || env.EnvironmentName == "LocalDevelopment" || env.EnvironmentName == "LocalHost" ||
            env.EnvironmentName == "InternalDevelopment")
        {
            app.UseCors("CorsPolicyFree");
            app.UseDeveloperExceptionPage();
            string swaggerRoutePrefix = "LTC/administration-service/swagger";
            app.UseSwaggerUI("LTC Administration Service", swaggerRoutePrefix);
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Administration Service API");
                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            });
        }
        else
        {
            app.UseCors();
        }


        app.UseRouting();
        app.UseRequestLocalization();

        //app.MapAbpStaticAssets();
        //app.UseAbpStudioLink();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        app.UseAuthentication();
        //app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseJwtTokenMiddleware();
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        //app.Use(async (context, next) =>
        //{
        //    Console.WriteLine("?>>>> Header Accept-Language: " + context.Request.Headers["Accept-Language"]);
        //    await next();
        //});

        // -------------- GRPC -----------------
        app.UseConfiguredEndpoints(endpoints =>
        {
            // Enable gRPC reflection
            endpoints.MapGrpcReflectionService();
        });
    }
}
