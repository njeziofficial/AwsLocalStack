using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AwsLocalStack.Contracts;
using AwsLocalStack.Data;
using AwsLocalStack.Extensions;
using AwsLocalStack.Helpers;
using AwsLocalStack.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Configure localstack

var serviceUrl = builder.Configuration.GetValue<string>("DatabaseSettings:ServiceUrl");
//var serviceUrl = builder.Configuration["DatabaseSettings:ServiceUrl"];
builder.Services.AddSingleton<IAmazonDynamoDB>(x =>
{
    var dbConfig = new AmazonDynamoDBConfig
    {
        ServiceURL = serviceUrl
    };
    return new AmazonDynamoDBClient(dbConfig);
});

builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.Initialize(builder.Services.BuildServiceProvider()).GetAwaiter().GetResult();
//ends here...

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    //opts.UseSqlServer(builder.Configuration.GetValue<string>("DatabaseSettings:DefaultConnection"));
});

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.ExtendBuilder(app.Services.GetRequiredService<ILoggerFactory>());
app.MapControllers();

app.Run();
