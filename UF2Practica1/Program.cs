using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace UF2Practica1
{
	class MainClass
	{

        #region Const values
        //Valors constants
       

        //Variable que determina el numero de caixers en funcionament.
        const int maxCax = 5;
        public static ConcurrentQueue<Client> cua = new ConcurrentQueue<Client>();

        /* Cua concurrent
		 	Dos mètodes bàsics: 
		 		Cua.Enqueue per afegir a la cua
		 		bool success = Cua.TryDequeue(out clientActual) per extreure de la cua i posar a clientActual
		*/

        #endregion

        public static void Main(string[] args)
		{
			var clock = new Stopwatch();
			var threads = new List<Thread>();
			//Recordeu-vos que el fitxer CSV ha d'estar a la carpeta bin/debug de la solució
			const string fitxer = "CuaClients.csv";

			try
			{
				var reader = new StreamReader(File.OpenRead(@fitxer));


				//Carreguem la llista clients

				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(';');
					var newClient = new Client() { name = values[0], articles = Int32.Parse(values[1]) };
					cua.Enqueue(newClient);

				}

			}
			catch (Exception)
			{
                //En cas de que falli l'arxiu CSV entrarà al catch i mostrarà aquest missatge d'error.
				Console.WriteLine("Error accedint a l'arxiu");
				Console.ReadKey();
				Environment.Exit(0);
			}

            //Comença el temporitzador.
			clock.Start();



            // Instanciar les caixeres i afegir el thread creat a la llista de threads
            for (int i = 1; i <= maxCax; i++)
            {
                var cax = new Caixera() { idCax = i };
                var fil = new Thread(() => cax.cua());
                //operadors lambda
                fil.Start();
                threads.Add(fil);
            }


            // Procediment per esperar que acabin tots els threads abans d'acabar
            foreach (Thread thread in threads)
				thread.Join();

			// Parem el rellotge i mostrem el temps que triga
			clock.Stop();
			double temps = clock.ElapsedMilliseconds / 1000;
			Console.Clear();
			Console.WriteLine("Temps total Task: " + temps + " segons");
			Console.ReadKey();
		}
	}
	#region ClassCaixera
	public class Caixera
	{
		public int idCax
		{
			get;
			set;
		}

		public void cua()
		{
            // Llegirem la cua extreient l'element
            // cridem al mètode ProcesarCompra passant-li el client

            while (!MainClass.cua.IsEmpty)
            {
                Client newClient = new Client();

                bool funciona = MainClass.cua.TryDequeue(out newClient);

                if (funciona)
                {
                processarArticles(newClient);
                }

            }

        }


		private void processarArticles(Client client)
		{

			Console.WriteLine("La caixera " + this.idCax + " comença amb el client " + client.name + " que té " + client.articles + " productes");

			for (int i = 0; i < client.articles; i++)
			{
				this.ProcessaProducte();

			}

			Console.WriteLine(">>>>>> La caixera " + this.idCax + " ha acabat amb el client " + client.name);
		}


		private void ProcessaProducte()
		{
			Thread.Sleep(TimeSpan.FromSeconds(1));
		}
	}


	#endregion

	#region ClassClient

	public class Client
	{
		public string name
		{
			get;
			set;
		}


		public int articles
		{
			get;
			set;
		}


	}

	#endregion
}
