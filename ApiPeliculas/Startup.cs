using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApiPeliculas.Data;
using ApiPeliculas.Helpers;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiPeliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICategoryRepository, CategoriaRepository>();//inyecto la dependencia.
            services.AddScoped<IPeliculaRepository, PeliculaRepository>();//inyecto la dependencia.
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            //dependencia del token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });


            services.AddAutoMapper(typeof(PeliculasMappers));

            //para poder usar swash documentacion para la API

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("APIPeliculasCategorias", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Categorias Peliculas",
                    Description = "Backend Peliculas",
                    Version = "1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "admin@pablo.com",
                        Name = "Pablo",
                        Url = new Uri("https://www.pablocastillo.me")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT",
                         Url = new Uri("https://wiki")
                    }
                });



                options.SwaggerDoc("APIPeliculas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Peliculas",
                    Description = "Backend Peliculas",
                    Version = "1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "admin@pablo.com",
                        Name = "Pablo",
                        Url = new Uri("https://www.pablocastillo.me")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("https://wiki")
                    }
                });


                options.SwaggerDoc("APIPeliculasUsuarios", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "API Usuarios Peliculas",
                    Description = "Backend Peliculas",
                    Version = "1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "admin@pablo.com",
                        Name = "Pablo",
                        Url = new Uri("https://www.pablocastillo.me")
                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("https://wiki")
                    }
                });



                //configuramos el archivo xml de documentacion

                var archivoXMLComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXMLComentarios);
                options.IncludeXmlComments(rutaApiComentarios);


                //primero definimos el esquema de seguridad.

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
                    {
                        Description = "Autenticacion JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement() 
                {
                    {
                       new OpenApiSecurityScheme{
                             Reference = new OpenApiReference
                             {
                                 Id = "Bearer",
                                 Type = ReferenceType.SecurityScheme
                             }
                        }, new List<string>()
                    }
                });

            });

            


            services.AddControllers();

            // habilitamos soporte para CORS
            services.AddCors();

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler(builder=> {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }

                    });
                });
            }

            app.UseHttpsRedirection();

            //linea para configurar la documentacion de la API
            app.UseSwagger();
            app.UseSwaggerUI(options=>
            {
                options.SwaggerEndpoint("/apiPeliculas/swagger/APIPeliculasCategorias/swagger.json","API CAtegorias Peliculas");
                options.SwaggerEndpoint("/apiPeliculas/swagger/APIPeliculas/swagger.json", "API Peliculas");
                options.SwaggerEndpoint("/apiPeliculas/swagger/APIPeliculasUsuarios/swagger.json", "API Usuarios Peliculas");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            //para la autorizacion y autenticacion

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //soporte para CORS

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
