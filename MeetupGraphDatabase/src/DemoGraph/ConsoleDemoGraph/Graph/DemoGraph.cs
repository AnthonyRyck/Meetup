using ArangoConnect;
using ConsoleDemoGraph.Models;
using Core.Arango.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemoGraph.Graph
{
	internal class DemoGraph : ArangoLoader
	{
		// Les Vertices
		private const string UTILISATEUR = "Utilisateur";
		private const string CENTRE_INTERET = "CentreInteret";
		private const string EVENEMENT = "Evenement";
		private const string LIEU = "Lieu";
		private const string SOCIETE = "Societe";

		// Edges
		private const string AMIS = "Amis";
		private const string LIKE = "Like";
		private const string WORK_FOR = "WorkFor";
		private const string ETUDIER = "Etudier";
		private const string COUPLE = "EnCouple";
		private const string RECHERCHE = "Recherche";
		private const string PARTICIPE = "Participe";

		private readonly string DatabaseName;
		private readonly string GraphName;

		private readonly Action<string> Logger;

		public DemoGraph(string url, int port, string projectName, string login, string password, string databaseName, string graphName, Action<string> log)
			: base(url, port, projectName, login, password)
		{
			DatabaseName = databaseName;
			GraphName = graphName;
			Logger = log;
		}

		/// <summary>
		/// Créer un graph sur la base de donnée fournit.
		/// </summary>
		/// <param name="nameDatabase">Nom de la base de donnée</param>
		/// <returns></returns>
		internal async Task CreateGraph(string nameDatabase, string graphName)
		{
			await Arango.Collection.CreateAsync(nameDatabase, UTILISATEUR, ArangoCollectionType.Document);
			await Arango.Collection.CreateAsync(nameDatabase, CENTRE_INTERET, ArangoCollectionType.Document);
			await Arango.Collection.CreateAsync(nameDatabase, EVENEMENT, ArangoCollectionType.Document);
			await Arango.Collection.CreateAsync(nameDatabase, LIEU, ArangoCollectionType.Document);
			await Arango.Collection.CreateAsync(nameDatabase, SOCIETE, ArangoCollectionType.Document);

			await Arango.Collection.CreateAsync(nameDatabase, AMIS, ArangoCollectionType.Edge);
			await Arango.Collection.CreateAsync(nameDatabase, LIKE, ArangoCollectionType.Edge);
			await Arango.Collection.CreateAsync(nameDatabase, WORK_FOR, ArangoCollectionType.Edge);
			await Arango.Collection.CreateAsync(nameDatabase, ETUDIER, ArangoCollectionType.Edge);
			await Arango.Collection.CreateAsync(nameDatabase, COUPLE, ArangoCollectionType.Edge);
			await Arango.Collection.CreateAsync(nameDatabase, RECHERCHE, ArangoCollectionType.Edge);
			await Arango.Collection.CreateAsync(nameDatabase, PARTICIPE, ArangoCollectionType.Edge);

			await Arango.Graph.CreateAsync(nameDatabase, new ArangoGraph
			{
				Name = graphName,
				EdgeDefinitions = new List<ArangoEdgeDefinition>
				{
					new()
					{
					  Collection = AMIS,
					  From = new List<string> {UTILISATEUR },
					  To = new List<string> { UTILISATEUR }
					},
					new()
					{
					  Collection = COUPLE,
					  From = new List<string> {UTILISATEUR },
					  To = new List<string> { UTILISATEUR }
					},
					new()
					{
						Collection = LIKE,
						From = new List<string> {UTILISATEUR },
						To = new List<string> { CENTRE_INTERET }
					},
					new()
					{
						Collection = ETUDIER,
						From = new List<string> {UTILISATEUR },
						To = new List<string> { LIEU }
					},
					new()
					{
						Collection = WORK_FOR,
						From = new List<string> {UTILISATEUR },
						To = new List<string> { SOCIETE }
					},
					new()
					{
						Collection = RECHERCHE,
						From = new List<string> {UTILISATEUR },
						To = new List<string> { UTILISATEUR }
					},
					new()
					{
						Collection = PARTICIPE,
						From = new List<string> {UTILISATEUR },
						To = new List<string> { EVENEMENT }
					}
				}
			});
		}

		/// <summary>
		/// Ajoutes les Vertices au Graph
		/// </summary>
		/// <returns></returns>
		internal async Task AddVertices()
		{
			// Ajout des utilisateurs
			Log($"---Ajout des utilisateurs");
			foreach (var user in GetUtilisateurs())
			{
				await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, UTILISATEUR,
				new
				{
					Key = user.Nom,
					Age = user.Age,
					Sexe = user.Sexe
				});

				Log($"Ajout de : {user.Nom}");
			}

			// Ajout centre d'intérets.
			Log("---Ajout des centres d'intérêts");
			foreach (var centre in GetCentreInterets())
			{
				await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, CENTRE_INTERET,
				new
				{
					Key = centre.NomDuCentreInteret
				});

				Log($"Ajout du centre d'intéret : {centre.NomDuCentreInteret}");
			}

			// Ajout de la société.
			Log("---Ajout de l'entreprise");
			await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, SOCIETE,
				new
				{
					Key = "Entreprise"
				});

			// Ajout d'un lieu.
			Log("---Ajout d'un lieu");
			await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, LIEU,
				new
				{
					Key = "Lycee"
				});

			// Ajout des événements
			Log("---Ajout d'un évènement");
			await Arango.Graph.Vertex.CreateAsync(DatabaseName, GraphName, EVENEMENT,
				new
				{
					Key = "GrosseSoiree"
				});
		}

		/// <summary>
		/// Ajout des relations
		/// </summary>
		/// <returns></returns>
		internal async Task AddEdges()
		{
			await AddEdgesAmis();
			await AddEdgesWorksFor();
			await AddEdgesLike();
			await AddEdgesEtudier();
			await AddEdgesParticipe();
			await AddEdgesRecherche();
			await AddEdgesEnCouple();
		}


		private IEnumerable<Utilisateur> GetUtilisateurs()
		{
			return new List<Utilisateur>()
			{
				new Utilisateur(){ Nom = "Marc", Age = 29, Sexe = "M"},
				new Utilisateur(){ Nom = "Alex", Age = 28, Sexe = "M"},
				new Utilisateur(){ Nom = "Julie", Age = 29, Sexe = "F"},
				new Utilisateur(){ Nom = "Louis", Age = 31, Sexe = "M"},
				new Utilisateur(){ Nom = "Bernard", Age = 23, Sexe = "M"},
				new Utilisateur(){ Nom = "Jean", Age = 23, Sexe = "M"},
				new Utilisateur(){ Nom = "Clara", Age = 22, Sexe = "F"},
				new Utilisateur(){ Nom = "Michel", Age = 42, Sexe = "M"},
			};
		}

		private IEnumerable<CentreInteret> GetCentreInterets()
		{
			return new List<CentreInteret>()
			{
				new CentreInteret() { NomDuCentreInteret= "Code"},
				new CentreInteret() { NomDuCentreInteret= "JeuxVideo"}
			};
		}



		private async Task AddEdgesAmis()
		{
			Log("Ajout des relations AMIS");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Marc-Alex",
				From = UTILISATEUR + "/" + "Marc",
				To = UTILISATEUR + "/" + "Alex",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Alex-Marc",
				From = UTILISATEUR + "/" + "Alex",
				To = UTILISATEUR + "/" + "Marc",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Alex-Louis",
				From = UTILISATEUR + "/" + "Alex",
				To = UTILISATEUR + "/" + "Louis",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Louis-Alex",
				From = UTILISATEUR + "/" + "Louis",
				To = UTILISATEUR + "/" + "Alex",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Bernard-Jean",
				From = UTILISATEUR + "/" + "Bernard",
				To = UTILISATEUR + "/" + "Jean",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Jean-Bernard",
				From = UTILISATEUR + "/" + "Jean",
				To = UTILISATEUR + "/" + "Bernard",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Jean-Clara",
				From = UTILISATEUR + "/" + "Jean",
				To = UTILISATEUR + "/" + "Clara",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Clara-Jean",
				From = UTILISATEUR + "/" + "Clara",
				To = UTILISATEUR + "/" + "Jean",
				Label = AMIS
			});

			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Bernard-Louis",
				From = UTILISATEUR + "/" + "Bernard",
				To = UTILISATEUR + "/" + "Louis",
				Label = AMIS
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, AMIS, new
			{
				Key = "Louis-Bernard",
				From = UTILISATEUR + "/" + "Louis",
				To = UTILISATEUR + "/" + "Bernard",
				Label = AMIS
			});
		}

		private async Task AddEdgesLike()
		{
			Log("Ajout des relations LIKE");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, LIKE, new
			{
				Key = "Marc-Jeux",
				From = UTILISATEUR + "/" + "Marc",
				To = CENTRE_INTERET + "/" + "JeuxVideo",
				Label = LIKE
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, LIKE, new
			{
				Key = "Marc-Code",
				From = UTILISATEUR + "/" + "Marc",
				To = CENTRE_INTERET + "/" + "Code",
				Label = LIKE
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, LIKE, new
			{
				Key = "Michel-Code",
				From = UTILISATEUR + "/" + "Michel",
				To = CENTRE_INTERET + "/" + "Code",
				Label = LIKE
			});
		}

		private async Task AddEdgesWorksFor()
		{
			Log("Ajout des relations WORKS FOR");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, WORK_FOR, new
			{
				Key = "Marc-Entreprise",
				From = UTILISATEUR + "/" + "Marc",
				To = SOCIETE + "/" + "Entreprise",
				Label = WORK_FOR
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, WORK_FOR, new
			{
				Key = "Bernard-Entreprise",
				From = UTILISATEUR + "/" + "Bernard",
				To = SOCIETE + "/" + "Entreprise",
				Label = WORK_FOR
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, WORK_FOR, new
			{
				Key = "Jean-Entreprise",
				From = UTILISATEUR + "/" + "Jean",
				To = SOCIETE + "/" + "Entreprise",
				Label = WORK_FOR
			});
		}

		private async Task AddEdgesEtudier()
		{
			Log("Ajout des relations ETUDIER");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, ETUDIER, new
			{
				Key = "Marc-Lycee",
				From = UTILISATEUR + "/" + "Marc",
				To = LIEU + "/" + "Lycee",
				Label = ETUDIER,
				Quand = "2000-2003"
			});

			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, ETUDIER, new
			{
				Key = "Louis-Lycee",
				From = UTILISATEUR + "/" + "Louis",
				To = LIEU + "/" + "Lycee",
				Label = ETUDIER,
				Quand = "2000-2003"
			});
		}

		private async Task AddEdgesParticipe()
		{
			Log("Ajout des relations PARTICIPE");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, PARTICIPE, new
			{
				Key = "Marc-Soiree",
				From = UTILISATEUR + "/" + "Marc",
				To = EVENEMENT + "/" + "GrosseSoiree",
				Label = EVENEMENT
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, PARTICIPE, new
			{
				Key = "Alex-Soiree",
				From = UTILISATEUR + "/" + "Alex",
				To = EVENEMENT + "/" + "GrosseSoiree",
				Label = EVENEMENT
			});
		}

		private async Task AddEdgesRecherche()
		{
			Log("Ajout de la relation RECHERCHE");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, RECHERCHE, new
			{
				Key = "Marc-Louis",
				From = UTILISATEUR + "/" + "Marc",
				To = UTILISATEUR + "/" + "Louis",
				Label = RECHERCHE
			});
		}

		private async Task AddEdgesEnCouple()
		{
			Log("Ajout de la relation EN COUPLE");
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, COUPLE, new
			{
				Key = "Louis-Julie",
				From = UTILISATEUR + "/" + "Louis",
				To = UTILISATEUR + "/" + "Julie",
				Label = COUPLE
			});
			await Arango.Graph.Edge.CreateAsync(DatabaseName, GraphName, COUPLE, new
			{
				Key = "Julie-Louis",
				From = UTILISATEUR + "/" + "Julie",
				To = UTILISATEUR + "/" + "Louis",
				Label = COUPLE
			});
		}

		private void Log(string message)
		{
			if (Logger != null)
				Logger.Invoke(message);
		}
	}
}
