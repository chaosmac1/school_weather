using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace BackendApi; 

public class Startup {
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers();
        services.AddResponseCompression(
            delegate(ResponseCompressionOptions options) {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] {"*/*"});
            });
        services.Configure<GzipCompressionProviderOptions>(option => { option.Level = CompressionLevel.Optimal;  });
        services.Configure<BrotliCompressionProviderOptions>(option => {
            option.Level = System.IO.Compression.CompressionLevel.Optimal;
        });
        services.AddResponseCaching(options => { options.SizeLimit = 200 * 1024 * 1024; });
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "BackendLayer0", Version = "v1"});
        });
        services.AddCors(options => {
            options.AddPolicy("_myAllowSpecificOrigins",
                builder => {
                    builder.WithOrigins("*");
                    builder.WithHeaders("*");
                    builder.WithMethods("*");
                });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackendApi v1"));
        }

        app.UseResponseCaching();
        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseCors("_myAllowSpecificOrigins");

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}