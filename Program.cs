using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CheckListAppDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

app.Run();

static async Task<IResult> GetAllTodos(CheckListAppDb db)
{
    return TypedResults.Ok(await db.CheckLists.Select(x => new CheckListAppDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(CheckListAppDb db) {
    return TypedResults.Ok(await db.CheckLists.Where(t => t.IsComplete).Select(x => new CheckListAppDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, CheckListAppDb db)
{
    return await db.CheckLists.FindAsync(id)
        is CheckListApp todo
            ? TypedResults.Ok(new CheckListAppDTO(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(CheckListAppDTO todoItemDTO, CheckListAppDb db)
{
    var todoItem = new CheckListApp
    {
        IsComplete = todoItemDTO.IsComplete,
        Name = todoItemDTO.Name
    };

    db.CheckLists.Add(todoItem);
    await db.SaveChangesAsync();

    todoItemDTO = new CheckListAppDTO(todoItem);

    return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
}

static async Task<IResult> UpdateTodo(int id, CheckListAppDTO todoItemDTO, CheckListAppDb db)
{
    var todo = await db.CheckLists.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, CheckListAppDb db)
{
    if (await db.CheckLists.FindAsync(id) is CheckListApp todo)
    {
        db.CheckLists.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}