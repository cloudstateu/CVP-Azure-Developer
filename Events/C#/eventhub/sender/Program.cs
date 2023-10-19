using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

const string connectionString = "";
const string eventHubName = "";

// Create a producer client
await using var producerClient = new EventHubProducerClient(connectionString, eventHubName);

// Sample events in a list
var allEvents = new List<EventData>
{
    new EventData("Foo"),
    new EventData("Bar")
};

// More data
for (int i = 0; i < 10; i++)
{
    allEvents.Add(new EventData("Event-" + i));
}

// Create a batch
using EventDataBatch eventDataBatch = await producerClient.CreateBatchAsync();

foreach (EventData eventData in allEvents)
{
    // Try to add the event from the list to the batch
    if (!eventDataBatch.TryAdd(eventData))
    {
        // If the batch is full, send it and then create a new batch
        await producerClient.SendAsync(eventDataBatch);
        
        eventDataBatch.Dispose();

        // Try to add the event that couldn't fit before
        if (!eventDataBatch.TryAdd(eventData))
        {
            throw new ArgumentException(
                "Event is too large for an empty batch. Max size: " + eventDataBatch.MaximumSizeInBytes);
        }
    }
}

// Send the last batch of remaining events
if (eventDataBatch.Count > 0)
{
    Console.WriteLine($"Sending {eventDataBatch.Count} events");
    await producerClient.SendAsync(eventDataBatch);
}