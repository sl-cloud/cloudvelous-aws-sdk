using Amazon.SQS.Model;
using System.Text.Json;

namespace Cloudvelous.Aws.Sqs;

/// <summary>
/// SQS message wrapper with additional metadata
/// </summary>
/// <typeparam name="T">Message body type</typeparam>
public class SqsMessage<T>
{
    /// <summary>
    /// Message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Receipt handle for message deletion
    /// </summary>
    public string ReceiptHandle { get; set; } = string.Empty;

    /// <summary>
    /// Message body
    /// </summary>
    public T Body { get; set; } = default!;

    /// <summary>
    /// Message attributes
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();

    /// <summary>
    /// Message system attributes
    /// </summary>
    public Dictionary<string, string> SystemAttributes { get; set; } = new();

    /// <summary>
    /// Approximate receive count
    /// </summary>
    public int ApproximateReceiveCount { get; set; }

    /// <summary>
    /// Approximate first receive timestamp
    /// </summary>
    public DateTime? ApproximateFirstReceiveTimestamp { get; set; }

    /// <summary>
    /// Sent timestamp
    /// </summary>
    public DateTime? SentTimestamp { get; set; }

    /// <summary>
    /// Creates an SQS message wrapper from an Amazon SQS message
    /// </summary>
    /// <param name="message">Amazon SQS message</param>
    /// <returns>SQS message wrapper</returns>
    public static SqsMessage<T> FromAmazonSqsMessage(Message message)
    {
        var sqsMessage = new SqsMessage<T>
        {
            MessageId = message.MessageId,
            ReceiptHandle = message.ReceiptHandle,
            Attributes = message.Attributes,
            SystemAttributes = message.Attributes
        };

        // Parse message body
        if (!string.IsNullOrEmpty(message.Body))
        {
            try
            {
                sqsMessage.Body = JsonSerializer.Deserialize<T>(message.Body) ?? default!;
            }
            catch
            {
                // If deserialization fails, try to cast directly if T is string
                if (typeof(T) == typeof(string))
                {
                    sqsMessage.Body = (T)(object)message.Body;
                }
            }
        }

        // Parse system attributes
        if (message.Attributes.TryGetValue("ApproximateReceiveCount", out var receiveCount) && int.TryParse(receiveCount, out var count))
        {
            sqsMessage.ApproximateReceiveCount = count;
        }

        if (message.Attributes.TryGetValue("ApproximateFirstReceiveTimestamp", out var firstReceiveTimestamp) && long.TryParse(firstReceiveTimestamp, out var firstTimestamp))
        {
            sqsMessage.ApproximateFirstReceiveTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(firstTimestamp).DateTime;
        }

        if (message.Attributes.TryGetValue("SentTimestamp", out var sentTimestamp) && long.TryParse(sentTimestamp, out var sentTs))
        {
            sqsMessage.SentTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(sentTs).DateTime;
        }

        return sqsMessage;
    }
}
