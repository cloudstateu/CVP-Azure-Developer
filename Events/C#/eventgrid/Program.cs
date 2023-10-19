using Azure;
using Azure.Messaging.EventGrid;


class Program
{
    static async Task Main()
    {
        string endpoint = "YOUR_EVENT_GRID_ENDPOINT";
        string topicAccessKey = "YOUR_TOPIC_KEY";

        var client = new EventGridPublisherClient(
            new Uri(endpoint), 
            new AzureKeyCredential(topicAccessKey));

        var sampleEvent = new EventGridEvent(
            "sampleEventSubject",
            "sampleEventType",
            "1.0",
            new BinaryData("{ \"data\": \"sampleEventData\" }"));

        await client.SendEventsAsync(new EventGridEvent[] { sampleEvent });

        Console.WriteLine("Event published!");
    }
}