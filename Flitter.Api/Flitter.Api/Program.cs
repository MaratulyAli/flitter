using AutoMapper;
using Flitter.Api.Data;
using Flitter.Api.Data.Caching;
using Flitter.Api.Models.AuthDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opts =>
    {
        opts.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.Authority = builder.Configuration["Jwt:Authority"];
    o.Audience = builder.Configuration["Jwt:Audience"];
    o.RequireHttpsMetadata = false;

    o.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = c =>
        {
            c.NoResult();

            c.Response.StatusCode = 500;
            c.Response.ContentType = "text/plain";

            if (builder.Environment.IsDevelopment())
            {
                return c.Response.WriteAsync(c.Exception.ToString());
            }

            return c.Response.WriteAsync("An error occured processing your authentication.");
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

var configuration = builder.Configuration;
builder.Services.AddDbContext<FlitterDbContext>(opt =>
{
    opt.UseNpgsql(configuration.GetConnectionString("FlitterDb"));
});
builder.Services.AddDbContext<KeycloakContext>();

builder.Services.AddScoped<IPostsCaching, PostsCaching>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FlitterDbContext>();
    dbContext.Database.Migrate();

    try
    {
        var postsCaching = scope.ServiceProvider.GetRequiredService<IPostsCaching>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        await postsCaching.CreateIndex();
        Console.WriteLine("===> Index created");

        var postDocs = await postsCaching.Search("");
        if (postDocs.Count == 0)
        {
            var posts = await dbContext.Posts.ToListAsync();
            var mappedPostDocs = mapper.Map<List<PostDocument>>(posts);

            if (mappedPostDocs.Count > 0)
            {
                await postsCaching.ReIndex(mappedPostDocs);
                Console.WriteLine("===> Posts added into index");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"===> Failed to create index: {ex.Message}");
    }
}

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
