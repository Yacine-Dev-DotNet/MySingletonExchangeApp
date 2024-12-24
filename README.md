Implémentation d’un convertisseur de devises avec le pattern Singleton en C#

Dans cet article, nous allons présenter un petit projet console en C# illustrant l’utilisation du design pattern Singleton. Le but est de créer un convertisseur de devises simple, avec une classe gérant les taux d’échange de manière unique dans l’application, puis un programme console qui interagit avec l’utilisateur pour effectuer les conversions.
________________________________________
1. Structure du projet :
Notre projet comporte deux fichiers principaux :
1.	ExchangeService.cs
-	Contient la classe ExchangeService, qui implémente le pattern Singleton.
-	Gère le chargement des taux de conversion et la logique de conversion (base -> cible).
2.	Program.cs
-	Contient la méthode Main (point d’entrée de l’application).
-	Gère l’interface en ligne de commande, demandant les devises et montant à l’utilisateur.
-	Appelle ExchangeService pour effectuer la conversion.
Le namespace commun MySingletonExchangeApp permet de regrouper nos classes et d’éviter les conflits de noms dans un plus grand projet.
________________________________________
2. Classe ExchangeService : mise en œuvre du Singleton :
#
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    
    namespace MySingletonExchangeApp
    {
        public class ExchangeService
        {
            // Dictionnaire pour associer (base, cible) -> Taux
            private Dictionary<(string Base, string Target), decimal> _rates;
    
            // Constructeur privé pour empêcher toute instanciation extérieure
            private ExchangeService()
            {
                LoadRates();
            }
    
            // Méthode simulant un chargement (ici, on simule un traitement "lourd")
            private void LoadRates()
            {
                // Simulation d’un délai de 2 secondes (ex. appel à une API externe)
                Thread.Sleep(2000);
    
                // Exemple de quelques taux de conversion
                // Clé : Tuple (Devise de base, Devise cible)
                _rates = new Dictionary<(string, string), decimal>
                {
                    { ("DZD", "EUR"), 0.0069m },
                    { ("DZD", "GBP"), 0.0053m },
                    { ("EUR", "USD"), 1.06m  },
                    { ("USD", "DZD"), 137.0m },
                    { ("EUR", "GBP"), 0.85m }
                    // { ("GBP", "DZD"), 188.0m }  // À ajouter si besoin
                };
            }
    
            // Méthode de conversion
            public decimal Convert(string baseCurrency, string targetCurrency, decimal amount)
            {
                // Recherche d’un taux direct
                if (_rates.TryGetValue((baseCurrency, targetCurrency), out decimal rate))
                {
                    return amount * rate;
                }
                else
                {
                    // Aucune conversion directe trouvée
                    // (Optionnel) Tentative de conversion indirecte via une devise intermédiaire
                    // ou levée d’une exception / renvoi d’une valeur par défaut.
                    
                    // Ici, on choisit de renvoyer 0 ou de lever une exception :
                    // throw new KeyNotFoundException($"Taux de conversion introuvable pour {baseCurrency} vers {targetCurrency}.");
                    return 0;
                }
            }
    
            // ---------------------------
            //   PATTERN SINGLETON
            // ---------------------------
    
            private static ExchangeService _instance;
            private static readonly object _lock = new object();
    
            // Propriété publique permettant d’accéder à l’unique instance
            public static ExchangeService Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (_lock) // Empêche plusieurs threads de créer plusieurs instances
                        {
                            if (_instance == null)
                            {
                                _instance = new ExchangeService();
                            }
                        }
                    }
                    return _instance;
                }
            }
    
            // Alternative courante : méthode GetInstance()
            public static ExchangeService GetInstance => Instance;
        }
    }

Analyse :
1.	Constructeur privé : Empêche d’instancier la classe depuis l’extérieur.
2.	Propriété statique Instance : Retourne l’unique instance de ExchangeService.
3.	Lock (thread-safety) : En contexte multi-threads, on évite qu’un second thread crée une autre instance pendant que le premier est en train de la créer.
4.	Dictionnaire _rates : Associe chaque paire de devises (base, cible) à un taux decimal.
5.	LoadRates() : Simule le chargement « lourd » des taux (2 secondes de Sleep). Dans une application réelle, on remplacerait cela par un appel asynchrone à une base de données ou une API externe.
________________________________________
3. Classe Program : interface console :
#
    using System;
    
    namespace MySingletonExchangeApp
    {
        internal class Program
        {
            static void Main(string[] args)
            {
                // Boucle infinie pour permettre plusieurs conversions
                while (true)
                {
                    Console.Write("Entrez la devise de base (ex: DZD, EUR, USD, GBP) : ");
                    var baseCurrency = Console.ReadLine();
    
                    Console.Write("Entrez la devise cible (ex: EUR, USD, GBP, DZD) : ");
                    var targetCurrency = Console.ReadLine();
    
                    Console.Write("Entrez le montant à convertir : ");
                    var input = Console.ReadLine();
                    if (!decimal.TryParse(input, out decimal amount))
                    {
                        Console.WriteLine("Valeur incorrecte, veuillez réessayer.\n");
                        continue;
                    }
    
                    // Appel du Singleton pour effectuer la conversion
                    var exchanged = ExchangeService.GetInstance.Convert(baseCurrency, targetCurrency, amount);
    
                    Console.WriteLine($"{amount} {baseCurrency} = {exchanged} {targetCurrency}");
                    Console.WriteLine("----------------------------------------------\n");
                }
            }
        }
    }

Analyse :
1.	Lecture des entrées utilisateur :
-	BaseCurrency : devise de départ (ex. DZD, EUR, USD, GBP).
-	TargetCurrency : devise cible (ex. EUR, USD, GBP, DZD).
-	Montant : on utilise decimal.TryParse pour éviter toute exception si l’utilisateur saisit autre chose qu’un nombre.
2.	Boucle infinie : L’utilisateur peut enchaîner les conversions tant qu’il le souhaite (un Ctrl + C forcera la sortie ou un arrêt du programme console).
3.	Appel à ExchangeService.GetInstance : On récupère l’instance Singleton et on appelle la méthode Convert(...).
4.	Affichage du résultat : On affiche la conversion sous la forme xxx baseCurrency = yyy targetCurrency.
________________________________________
4. Améliorations recommandées

  4.1.	Validation des devises :
  -	Si l’utilisateur saisit une devise qui n’existe pas dans le dictionnaire (ex. « ABC »), le code retournera 0 par défaut. On peut lever une exception ou afficher un message d’erreur plus clair.
  -	Par exemple, vérifier si la devise saisie fait partie d’un ensemble connu, sinon avertir l’utilisateur.

  4.2.	Sortie de la boucle :
  -	Proposer à l’utilisateur de taper un mot-clé (ex. « exit » ou « q ») pour sortir de la boucle while (true).
  -	Éviter que le programme soit entièrement bloqué et nécessite un arrêt forcé.

  4.3.	Gestion des erreurs :
  -	Aujourd’hui, si la conversion n’est pas trouvée, on renvoie 0 ou on peut lever une KeyNotFoundException. Pour un meilleur confort utilisateur, on pourrait plutôt afficher un message clair.
  -	En production, le code pourrait capturer l’exception et rediriger l’utilisateur ou effectuer une conversion indirecte (ex. passer par l’USD comme devise de référence si le taux   direct n’existe pas).
  
  4.4.	Chargement asynchrone des taux :
  -	Au lieu d’utiliser Thread.Sleep(2000);, remplacer par un vrai appel asynchrone (await Task.Delay(2000);) pour ne pas bloquer le thread principal.
  -	Cela permettrait de ne pas geler l’application si l’opération de chargement est lourde.
  
  4.5.	Gestion dynamique des taux :
  -	Les taux sont codés en dur dans LoadRates(). On peut envisager de les charger depuis une API (fournisseur de taux de change) ou depuis une base de données si l’application doit être plus évolutive.
________________________________________
5. Exécution et test :
   
  •	Démarrez l’application (F5 ou via la ligne de commande dotnet run).
  •	Saisissez la devise de base (p. ex. DZD).
  •	Saisissez la devise cible (p. ex. EUR).
  •	Entrez un montant à convertir (p. ex. 1000).
  •	Le programme affiche alors la valeur convertie, par exemple 1000 DZD = 6.9 EUR, selon les taux définis.
  En lançant la première conversion, vous constaterez un léger délai de 2 secondes lors de la création de l’instance ExchangeService (à cause du LoadRates()), ce qui simule un chargement ou un appel réseau. Ensuite, les conversions ultérieures sont quasi instantanées car le service est déjà initialisé (grâce au Singleton).
________________________________________
6. Conclusion
   
Ce projet console illustre de façon pédagogique comment implémenter un Singleton en C# pour gérer des ressources partagées (ici, des taux de change). Les points-clés à retenir sont :
  •	Constructeur privé et propriété statique : assurent l’unicité de l’instance.
  •	Gestion multithread (lock) : évite les créations concurrentes.
  •	Logique métier (charge des taux, conversion) : centralisée dans la classe Singleton pour simplifier l’utilisation et éviter les duplications.
  •	Interface console : permet de tester facilement la fonctionnalité.
Pour aller plus loin, vous pouvez personnaliser ce service avec des appels réels à une API de taux de change, ajouter un cache côté serveur, ou encore intégrer ce pattern dans un framework web (ex. ASP.NET Core) où il servirait de service central d’échange de devises.
________________________________________
Félicitations ! Vous disposez maintenant d’un exemple concret qui applique le pattern Singleton en C# pour un service de conversion de devises. N’hésitez pas à l’adapter à vos propres besoins ou à l’enrichir avec des fonctionnalités plus poussées. Bon coding !

