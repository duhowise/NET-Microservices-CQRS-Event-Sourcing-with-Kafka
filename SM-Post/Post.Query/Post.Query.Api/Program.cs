using Confluent.Kafka;
using Messaging.Rabbitmq.Extensions;
using Messaging.Rabbitmq.Implementation;
using Messaging.Rabbitmq.Interfaces;
using Microsoft.EntityFrameworkCore;
using Post.Cmd.Infrastructure;
using Post.Common.Base;
using Post.Common.Events;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Extensions;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;
using EventHandler = Post.Query.Infrastructure.Handlers.EventHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Action<DbContextOptionsBuilder>configureDbContext=(options=>options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

var context=builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
context.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepository,PostRepository>();
builder.Services.AddScoped<ICommentRepository,CommentRepository>();
builder.Services.AddScoped<IEventHandler,EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddQueueing(new QueueingConfigurationSettings
{
    RabbitMqConsumerConcurrency = 5,
    RabbitMqHostname = "localhost",
    RabbitMqPort = 5672,
    RabbitMqPassword = "guest",
    RabbitMqUsername = "guest"
});
builder.Services.AddQueueMessageConsumer<CommentAddedQueueConsumer, CommentAddedEvent>();
builder.Services.AddQueueMessageConsumer<CommentRemovedQueueConsumer, CommentRemovedEvent>();
builder.Services.AddQueueMessageConsumer<CommentUpdatedQueueConsumer, CommentUpdatedEvent>();
builder.Services.AddQueueMessageConsumer<MessageUpdatedQueueConsumer,MessageUpdatedEvent>();
builder.Services.AddQueueMessageConsumer<PostCreatedQueueConsumer, PostCreatedEvent>();
builder.Services.AddQueueMessageConsumer<PostLikedQueueConsumer, PostLikedEvent>();
builder.Services.AddQueueMessageConsumer<PostRemovedQueueConsumer, PostRemovedEvent>();
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
