package org.example;

import com.azure.core.credential.AzureKeyCredential;
import com.azure.core.util.BinaryData;
import com.azure.messaging.eventgrid.EventGridEvent;
import com.azure.messaging.eventgrid.EventGridPublisherClient;
import com.azure.messaging.eventgrid.EventGridPublisherClientBuilder;

// Press Shift twice to open the Search Everywhere dialog and type `show whitespaces`,
// then press Enter. You can now see whitespace characters in your code.
public class Main {
    public static void main(String[] args) {
        String endpoint = "YOUR_EVENT_GRID_ENDPOINT";
        String topicKey = "YOUR_TOPIC_KEY";

        EventGridPublisherClient publisherClient = new EventGridPublisherClientBuilder()
                .endpoint(endpoint)
                .credential(new AzureKeyCredential(topicKey))
                .buildEventGridEventPublisherClient();

        EventGridEvent event = new EventGridEvent(
                "sampleEventType",
                "sampleEventSubject",
                BinaryData.fromString("{ \"data\": \"sampleEventData\" }"),
                "test"
        );

        publisherClient.sendEvent(event);
        System.out.println("Event published!");
    }
}