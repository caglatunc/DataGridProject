﻿using Bogus;
using Bogus.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataGridServer.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class ValuesController(AppDbContext context) : ControllerBase
{
    public static List<Book> Books = new();

    [HttpGet]
    public IActionResult SeedData()
    {
        
        for(int i = 0; i < 1000; i++)
        {
            Faker faker = new();
            Book book = new()
            {
                Name = faker.Parse("{{name.lastName}} {{name.firstName}}"),
                Author = faker.Name.FullName(),
                PublishDate = faker.Date.BetweenDateOnly(DateOnly.Parse("01.01.1990"), DateOnly.Parse("01.01.2024")),
                Summary = faker.Lorem.Paragraph(2)
            };
            
           context.Add(book);
           context.SaveChanges();
        }
        return NoContent();
    }

    [HttpGet]
    [EnableQuery]
    public IActionResult GetAll()
    {
        var books = context.Books.AsQueryable();
        return Ok(books);
    }

    [HttpPost]
    public IActionResult Update(Book book)
    {
        context.Update(book);
        context.SaveChanges();
        return NoContent();
    }
}


public sealed class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=CAGLA\\SQLEXPRESS;Initial Catalog=DataGridDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        optionsBuilder.LogTo(Console.WriteLine, new[] {DbLoggerCategory.Database.Command.Name}, LogLevel.Information);
    }

    public DbSet<Book> Books { get; set; } 
}


public sealed record PaginationResponse<T>
    where T:class
    {
        public T? Data { get; init; }
        public int TotalCount { get; init; } 
     }


public sealed class Book
{
    public Book() 
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateOnly PublishDate { get; set; } =DateOnly.FromDateTime(DateTime.Now);
}
