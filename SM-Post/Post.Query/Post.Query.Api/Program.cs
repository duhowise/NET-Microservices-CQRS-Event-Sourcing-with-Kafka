using System.Text.Json.Serialization;
using Confluent.Kafka;
using CQRS.Core.Infrastructure;
using Messaging.Rabbitmq.Extensions;
using Messaging.Rabbitmq.Implementation;
using Microsoft.EntityFrameworkCore;
using Post.Common.Base;
using Post.Common.Events;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
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
builder.Services.AddScoped<IQueryHandler,QueryHandler>();
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


// register query handler methods
var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
var dispatcher = new QueryDispatcher();
dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);
builder.Services.AddScoped<IQueryDispatcher<PostEntity>>(_ => dispatcher);


builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles;
});
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
