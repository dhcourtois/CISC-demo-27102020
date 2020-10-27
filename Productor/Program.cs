using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Productor
{
    class Program
    {
        static void Main(string[] args)
        {
            Producir().GetAwaiter().GetResult();
        }
        //basado en el ejemplo de https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send
        static async Task Producir()
        {
            
            // Crea un cliente de produccion para enviar mensajes
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                // Crea un lote de mensajes
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                for (int i = 1; i <= 10; i++)
                {
                    // agrega los mensajes al lote  
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Hola EH, mensaje {i}.")));
                }

                // Envia el lote de mensajes
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("Los mensajes se publicaron.");
            }
        }

        private const string connectionString = "aqui va la cadena de conexion";
        private const string eventHubName = "ehdemo27102020";
    }
}
