using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/get", async (int id) =>
{
    LearningFucker.DataContext context = new LearningFucker.DataContext();
    return Newtonsoft.Json.JsonConvert.SerializeObject(await context.GetRow(id));
});

app.MapPost("/write", async (HttpRequest request) =>
{
    StreamReader sr = new StreamReader(request.Body);
    var data = await sr.ReadToEndAsync();
    LearningFucker.DataContext context = new LearningFucker.DataContext();
    return await context.WriteData(Newtonsoft.Json.JsonConvert.DeserializeObject<LearningFucker.Models.Question>(data));
});

app.Urls.Add("http://*:52126");
app.Run();

