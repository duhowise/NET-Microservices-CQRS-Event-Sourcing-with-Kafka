using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Handlers;
using Microsoft.EntityFrameworkCore;
using Post.Common.Events;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Action<DbContextOptionsBuilder>configureDbContext=(options=>options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

var context=builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
context.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepository,PostRepository>();
builder.Services.AddScoped<ICommentRepository,CommentRepository>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddSingleton<IEventStrategy, EventStrategy>();
builder.Services.AddScoped<IEventHandler<CommentAddedEvent>, CommentAddedEventHandler>();
builder.Services.AddScoped<IEventHandler<CommentRemovedEvent>, CommentRemovedEventHandler>();
builder.Services.AddScoped<IEventHandler<CommentUpdatedEvent>, CommentUpdatedEventHandler>();
builder.Services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<PostLikedEvent>, PostLikedEventHandler>();
builder.Services.AddScoped<IEventHandler<PostRemovedEvent>, PostRemovedEventHandler>();
builder.Services.AddScoped<IEventHandler<MessageUpdatedEvent>, MessageUpdatedEventHandler>();

builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
