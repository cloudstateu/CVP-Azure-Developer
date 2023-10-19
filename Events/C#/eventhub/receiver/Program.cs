using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

class Receiver
{
    private const string connectionString = "";
    private const string eventHubName = "";
    private const string storageConnectionString = "";
    private const string storageContainerName = "";

    static async Task Main(string[] args)
    {
        var blobServiceClient = new BlobServiceClient(storageConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(storageContainerName);

        var processorClient = new EventProcessorClient(
            blobContainerClient, 
            EventHubConsumerClient.DefaultConsumerGroupName, 
            connectionString, 
            eventHubName);

        processorClient.ProcessEventAsync += ProcessEventHandler;
        processorClient.ProcessErrorAsync += ProcessErrorHandler;

        Console.WriteLine("Starting event processor");
        await processorClient.StartProcessingAsync();

        Console.WriteLine("Press enter to stop.");
        Console.ReadLine();

        Console.WriteLine("Stopping event processor");
        await processorClient.StopProcessingAsync();

        Console.WriteLine("Event processor stopped.");
        Console.WriteLine("Exiting process");
    }

    private static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        var partitionContext = eventArgs.Partition;
        var eventData = eventArgs.Data;

        Console.WriteLine($"Processing event from partition {partitionContext.PartitionId} with sequence number {eventData.SequenceNumber} with body: {eventData.EventBody}");

        // Every 10 events received, it will update the checkpoint stored in Azure Blob Storage.
        if (eventData.SequenceNumber % 10 == 0)
        {
            await eventArgs.UpdateCheckpointAsync();
        }
    }

    private static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        Console.WriteLine($"Error occurred in partition processor for partition {eventArgs.PartitionId}, {eventArgs.Exception.Message}");
        return Task.CompletedTask;
    }
}
