using HiTechStore;

var builder = WebApplication.CreateBuilder(args)
                .ConfigueBuilder();

var app = builder.Build();

await app.ConfigueApp();

app.Run();