﻿using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Common.Converter
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }

        public override BaseEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader ,out var doc))
            {
                throw new JsonException($"Failed to parse {nameof(JsonDocument)}!");
            }

            if (!doc.RootElement.TryGetProperty("Type",out var type))
            {
                throw new JsonException("could not detect the type of the discriminator property");
            }

            var typeDiscriminator = type.GetString();
            var json = doc.RootElement.GetRawText();
            return typeDiscriminator switch
            {
                nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
                nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json, options),
                nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
                nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
                nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
                nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
                nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
                _=> throw new JsonException($"type discriminator not supported yet {nameof(typeDiscriminator)}")
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case PostCreatedEvent postCreatedEvent:
                    JsonSerializer.Serialize(writer, postCreatedEvent, postCreatedEvent.GetType(), options);
                    break;
                case MessageUpdatedEvent messageUpdatedEvent:
                    JsonSerializer.Serialize(writer, messageUpdatedEvent, messageUpdatedEvent.GetType(), options);
                    break;
                case PostLikedEvent postLikedEvent:
                    JsonSerializer.Serialize(writer, postLikedEvent, postLikedEvent.GetType(), options);
                    break;
                case CommentAddedEvent commentAddedEvent:
                    JsonSerializer.Serialize(writer, commentAddedEvent, commentAddedEvent.GetType(), options);
                    break;
                case CommentUpdatedEvent commentUpdatedEvent:
                    JsonSerializer.Serialize(writer, commentUpdatedEvent, commentUpdatedEvent.GetType(), options);
                    break;
                case CommentRemovedEvent commentRemovedEvent:
                    JsonSerializer.Serialize(writer, commentRemovedEvent, commentRemovedEvent.GetType(), options);
                    break;
                case PostRemovedEvent postRemovedEvent:
                    JsonSerializer.Serialize(writer, postRemovedEvent, postRemovedEvent.GetType(), options);
                    break;
                default:
                    throw new NotSupportedException($"Serialization is not supported for the type: {value.GetType()}");
            }
        }
    }
}
