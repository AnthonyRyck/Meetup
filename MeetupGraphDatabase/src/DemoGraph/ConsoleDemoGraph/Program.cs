using ConsoleDemoGraph.Graph;
using System;
using System.Threading.Tasks;

namespace ConsoleDemoGraph
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			"######## Début de l'application Démo ########".ToConsoleInfo();
			"#:> Appuyer sur une touche pour commencer.".ToConsoleInfo();
			Console.ReadKey();
			
			string urlArango = "localhost";
			int port = 8529;
			string projectName = "MeetupGraph";
			string databaseName = "MeetupGraphDb";
			string graphName = "DemoGraph";
			string login = "root";
			string password = "PassMeetupGraph";

			Console.WriteLine();
			"######################################################".ToConsoleInfo();
			"####### 1ere étape : Connexion à ArangoDb. #######".ToConsoleInfo();
			"Appuyer sur une touche pour commencer".ToConsoleInfo();
			Console.ReadKey();
			$"----> Connexion sur : {urlArango}:{port}".ToConsoleInfo();
			DemoGraph demo = new DemoGraph(urlArango, port, projectName, login, password, databaseName, graphName, InfoLog);
			$"----> Connexion sur : {urlArango}:{port}".ToConsoleInfo();

			await demo.DeleteDatabase(databaseName);
			await demo.CreateDatabase(databaseName);
			$"Création de la base {databaseName} - OK".ToConsoleResult();

			await demo.CreateGraph(databaseName, graphName);
			"Graph créé - OK".ToConsoleResult();

			"Ajout des Vertices au Graph".ToConsoleInfo();
			Console.WriteLine();
			Console.ReadKey();
			await demo.AddVertices();

			"Ajout des relations au Graph".ToConsoleInfo();
			Console.WriteLine();
			Console.ReadKey();
			await demo.AddEdges();

			Console.WriteLine();
			"######## Fin de l'application Démo ########".ToConsoleInfo();
			Console.ReadKey();
		}

		private static void InfoLog(string message)
		{
			message.ToConsoleInfo();
		}
	}

	public static class ConsoleExtension
	{
		public static void ToConsoleInfo(this string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(message);
		}

		public static void ToConsoleResult(this string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(message);
		}
	}
}
