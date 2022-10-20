using ProdutoAPI.Data;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using ProdutoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProdutoContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/produto", async (
            ProdutoContextDb context) =>
        await context.Produtos.ToListAsync())
    .WithName("GetProduto")
    .WithTags("Produto");

app.MapGet("/produto/{id}", async (
            Guid id,
            ProdutoContextDb context) =>

        await context.Produtos.FindAsync(id)
            is Produto produto
                ? Results.Ok(produto)
                : Results.NotFound()
        )
    .Produces<Produto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetProdutoPorId")
    .WithTags("Produto");

app.MapPost("/produto", async (
        ProdutoContextDb context,
        Produto produto) =>
    {
        if (!MiniValidator.TryValidate(produto, out var errors))
            return Results.ValidationProblem(errors);

        context.Produtos.Add(produto);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.Created($"/fornecedor/{produto.Id}",produto)                          /// <-- pode retornar o objeto completo 
            //? Results.CreatedAtRoute("GetProdutoPorId", new { id = produto.Id }, produto)     /// <-- pode retornar a url de consulta
            : Results.BadRequest("Houve um problema ao salvar o registro");
    }).ProducesValidationProblem()
    .Produces<Produto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostProduto")
    .WithTags("Produto");

app.MapPut("/produto", async (
        Guid id,
        ProdutoContextDb context,
        Produto produto) =>
    {
        // // Esta pesquisa pode provocar erro de tracking se houver um Get imediatamente antes do Put
        //var produtoBanco = await context.Produtos.FindAsync(id);
        //if (produtoBanco == null) return Results.NoContent();

        var produtoBanco = await context.Produtos
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(f => f.Id == id);
        if (produtoBanco == null) return Results.NoContent();

        if (!MiniValidator.TryValidate(produto, out var errors))
            return Results.ValidationProblem(errors);

        context.Produtos.Update(produto);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("Houve um problema ao salvar o registro");
    }).ProducesValidationProblem()
    .Produces<Produto>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutProduto")
    .WithTags("Produto");

app.MapDelete("/produto", async (
        Guid id,
        ProdutoContextDb context) =>
    {

        var produto = await context.Produtos.FindAsync(id);
        if (produto == null) return Results.NoContent();    

        context.Produtos.Remove(produto);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.NoContent()
            : Results.BadRequest("Houve um problema ao excluir o registro");
    }).ProducesValidationProblem()
    .Produces<Produto>(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("DeleteProduto")
    .WithTags("Produto");

app.Run();

