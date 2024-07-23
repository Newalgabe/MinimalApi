using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task API", Version = "v1" });
});

var tasks = new List<Task>
{
    new Task { Id = 1, Title = "Score Goal", Description = "Score a goal in the next match", AssignedTo = "Heung-Min Son" },
    new Task { Id = 2, Title = "Defend", Description = "Keep a clean sheet", AssignedTo = "Guglielmo Vicario" },
    new Task { Id = 3, Title = "Assist", Description = "Provide an assist", AssignedTo = "Timo Werner" }
};

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task API V1");
    });
}

app.UseHttpsRedirection();

app.MapPost("/AddTask", (Task newTask) =>
{
    newTask.Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
    tasks.Add(newTask);
    return Results.Created($"/GetTask/{newTask.Id}", newTask);
})
.WithName("AddTask")
.WithOpenApi();

app.MapGet("/GetAllTasks", () =>
{
    return Results.Ok(tasks);
})
.WithName("GetAllTasks")
.WithOpenApi();

app.MapGet("/GetTask/{id}", (int id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    return task != null ? Results.Ok(task) : Results.NotFound();
})
.WithName("GetTask")
.WithOpenApi();

app.MapPut("/EditTask", (Task updatedTask) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
    if (task == null)
    {
        return Results.NotFound();
    }

    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.AssignedTo = updatedTask.AssignedTo;
    return Results.Ok(task);
})
.WithName("EditTask")
.WithOpenApi();

app.Run();

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string AssignedTo { get; set; }
}
