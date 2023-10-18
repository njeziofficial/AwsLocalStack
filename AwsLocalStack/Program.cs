using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AwsLocalStack.Helpers;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
