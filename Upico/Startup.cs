using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Upico.Persistence;
using Microsoft.EntityFrameworkCore;
using Upico.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Upico.Core.Repositories;
using Upico.Persistence.Repositories;
using Upico.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Upico.Core.Services;
using Upico.Persistence.Service;
using Upico.Core.StaticValues;
using Microsoft.AspNetCore.Mvc;
using Upico.Controllers.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Upico
{
    public class Startup
    {
        private readonly string _myAllowSpecificOrigins = "AllowSpecficOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {

                options.AddPolicy(name: _myAllowSpecificOrigins,
                    builder => {
                        builder.WithOrigins("http://localhost:5000", "http://localhost:3000", "http://localhost:3001")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                            .WithExposedHeaders("Content-Range");

                    });
            });

            //Disable Automatic Model State Validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //MailService
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            //For identity.entityframworkcore.
            services.AddDbContext<UpicODbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default")));

            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
            })
                .AddEntityFrameworkStores<UpicODbContext>()
                .AddDefaultTokenProviders();

            //Injecting interface
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAdminService, AdminService>();

            // the context that pass to AvatarRepository and UnitOfWork in runtime is the same object.
            services.AddScoped<IAvatarRepository, AvatarRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostedImageRepository, PostedImageRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IReportedPostRepository, ReportedPostRepository>();
            services.AddScoped<IMessageHubRepository, MessageHubRepository>();

            //For auto mapper
            services.AddAutoMapper(typeof(Startup));

            //For signalR
            services.AddSignalR();

            //Default
            services.AddControllers();

            ////Authenticate in swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Upico", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.<br>
                                   Enter 'Bearer' [space] and then your token in the text input below.<br>
                                   Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Scheme="oauth2",
                            Name="Bearer",
                            In=ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });


            //For authentication and authorization
            string issuer = Configuration.GetValue<string>("Tokens:Issuer");
            string signingKey = Configuration.GetValue<string>("Tokens:Key");
            byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

            var key = new SymmetricSecurityKey(signingKeyBytes);


            /*
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            */


            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            
            /*
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = issuer,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = System.TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
            });
            */

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upico v1"));
            }
            
            app.UseCors(_myAllowSpecificOrigins);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.Use(async (context, next) => {
                await next.Invoke();
                //handle response
                //you may also need to check the request path to check whether it requests image
                if (context.User.Identity.IsAuthenticated)
                {
                    var userName = context.User.Identity.Name;
                    //retrieve uer by userName
                    using (var dbContext = context.RequestServices.GetRequiredService<UpicODbContext>())
                    {
                        var user = dbContext.Users.Where(u => u.UserName == userName).FirstOrDefault();
                        var now = DateTime.Now;

                        if((now - user.LastAccessed).TotalSeconds > 60)
                        {
                            user.AccessCount += 1;

                            //log access
                            var accessLog = new AccessLog();
                            accessLog.UserId = user.Id;
                            accessLog.LogTime = now;

                            dbContext.AccessLogs.Add(accessLog);

                            /*var lastAccess = await dbContext.AccessLogs
                                .Where(a => a.UserId == user.Id)
                                .OrderByDescending(a => a.LogTime)
                                .FirstOrDefaultAsync();

                            if(lastAccess != null)
                            {
                                var distance = (now - lastAccess.LogTime).TotalSeconds;
                                if(distance > 1)
                                    dbContext.AccessLogs.Add(accessLog);

                            }
                            else
                            {
                                dbContext.AccessLogs.Add(accessLog);
                            }*/
                        }

                        user.LastAccessed = now;
                        dbContext.Update(user);
                        dbContext.SaveChanges();
                    }
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
