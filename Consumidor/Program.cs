using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Consumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            Consumir().GetAwaiter().GetResult();
        }


        //basado en el ejemplo de https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send
        static async Task Consumir()
        {
            // Establece el grupo de consumo por defecto
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Crea un cliente para el blob storage que se usara para los checkpoints
            BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Crea un cliente para suscribirse al EH, recibir los mensajes y procesarlos
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, connectionString, eventHubName);

            // registro de las funciones de procesamiento y manejo de errores
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Inicia el procesamiento
            await processor.StartProcessingAsync();

            // Espera antes de terminar            
            Console.ReadLine();

            // Detiene el procesamiento
            await processor.StopProcessingAsync();
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // En esta seccion se realiza el procesamiento del mensaje 
            Console.WriteLine("\tEvento recibido, mensaje: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Se actualiza el checkpoint para el control de mensajes
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // En esta seccion se realiza el manejo de errores de procesamiento
            Console.WriteLine($"\tParticion '{ eventArgs.PartitionId}': ha ocurrido un error.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }

        private const string connectionString = "aqui va la cadena de conexion a EH";
        private const string eventHubName = "ehdemo27102020";
        private const string blobStorageConnectionString = "aqui va la cadena de conexion al BS";
        private const string blobContainerName = "ehblobdemo27102020";
    }
}
