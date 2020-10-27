using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTaskDemo
{
    class Program
    {
		[DllImport("Kernel32.dll"), SuppressUnmanagedCodeSecurity]
		public static extern int GetCurrentProcessorNumber();
		static void Main(string[] args)
        {			
			var listA = new List<Task>();
			var listB = new List<Task>();
			long timeA, timeB;
			//Establecemos el numero maximo de tareas
			var maxNumberOfTask = 1000;
			var sw = new Stopwatch();

			sw.Start();
			//generamos las tareas y las encolamos para su ejecucion
			for (int i = 1; i <= maxNumberOfTask; i++)
			{
				var task = new Task(() => MyTask());
				task.Start();
				listA.Add(task);
			}
			//esperamos a que terminen
			Task.WaitAll(listA.ToArray());
			sw.Stop();
			timeA = sw.ElapsedMilliseconds;
			//incrementamos el numero minimo de hilos simultaneos
			int minimumWorkerThreads = 100;
			SetDegreeOfParallelism(minimumWorkerThreads);
			//repetimos el proceso
			sw.Reset();
			sw.Start();
			for (int i = 1; i <= maxNumberOfTask; i++)
			{
				var task = new Task(() => MyTask());
				task.Start();
				listB.Add(task);
			}

			Task.WaitAll(listB.ToArray());
			sw.Stop();
			timeB = sw.ElapsedMilliseconds;
			sw.Reset();
			//mostramos las estadisticas 
			Console.WriteLine($@"Lista A termino en {timeA} milliseconds");
			Console.WriteLine($@"Lista B termino en {timeB} milliseconds ");
		}
		private static Random _random = new Random(DateTime.Now.Millisecond);
		private static int taskCount = 0;
		private static void MyTask()
		{
			var taskId = taskCount++;
			var taskTime = 100;
			Thread.Sleep(taskTime);
			Console.WriteLine($@"A la tarea {taskId} le tomo {taskTime} milliseconds, core {GetCurrentProcessorNumber()} ");
		}

		/// <summary>
		/// Establece el numero minimo de hilos en ejecucion simultanea para el ThreadPool por defecto 
		/// https://msdn.microsoft.com/en-us/library/system.threading.threadpool(v=vs.110).aspx
		/// </summary>
		/// <param name="MinimumWorkerThreads">set the number of simultaneous parallel task</param>
		public static void SetDegreeOfParallelism(int MinimumWorkerThreads)
		{
			int minimumWorkerThreads, completationPortsThreads, maximumWorkerThreads;
			ThreadPool.GetMaxThreads(out maximumWorkerThreads, out completationPortsThreads);
			ThreadPool.GetMinThreads(out minimumWorkerThreads, out completationPortsThreads);
			if (minimumWorkerThreads <= Environment.ProcessorCount &&
				MinimumWorkerThreads > Environment.ProcessorCount &&
				MinimumWorkerThreads <= maximumWorkerThreads)
			{
				minimumWorkerThreads = MinimumWorkerThreads;
				completationPortsThreads = MinimumWorkerThreads;
				ThreadPool.SetMinThreads(minimumWorkerThreads, completationPortsThreads);
			}
		}

	}
}
