namespace MySingletonExchangeApp
{
    public class ExchangeService
    {
        // Dictionnaire pour associer (base, cible) -> Taux
        private Dictionary<(string Base, string Target), decimal> _rates;

        // Constructeur privé
        private ExchangeService()
        {
            LoadRates();
        }

        // Méthode simulant un chargement lourd ou un appel externe
        private void LoadRates()
        {
            // Exemple d'implémentation asynchrone (simule un traitement asynchrone)
            Task.Delay(2000).Wait();

            // Exemple de quelques taux (libres d’être ajustés)
            _rates = new Dictionary<(string, string), decimal>
            {
                { ("DZD", "EUR"), 0.0069m },
                { ("DZD", "GBP"), 0.0053m },
                { ("GBP", "DZD"), 188.0m },
                { ("EUR", "USD"), 1.06m  },
                { ("USD", "DZD"), 137.0m },
                { ("EUR", "GBP"), 0.85m }
            };
        }

        // Singleton : instance unique + verrou d’accès
        private static ExchangeService _instance;
        private static readonly object _lock = new object();

        public static ExchangeService GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
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

        // Méthode de conversion améliorée
        public decimal Convert(string baseCurrency, string targetCurrency, decimal amount)
        {
            try
            {
                if (_rates.TryGetValue((baseCurrency, targetCurrency), out decimal rate))
                {
                    return amount * rate;
                }
                else
                {
                    foreach (var intermediate in _rates.Keys.Select(k => k.Base).Distinct())
                    {
                        if (_rates.TryGetValue((baseCurrency, intermediate), out decimal rateToIntermediate) &&
                            _rates.TryGetValue((intermediate, targetCurrency), out decimal rateFromIntermediate))
                        {
                            return amount * rateToIntermediate * rateFromIntermediate;
                        }
                    }

                    // Lever une exception si aucune conversion n'est trouvée
                    throw new KeyNotFoundException($"Taux de conversion introuvable pour {baseCurrency} vers {targetCurrency}.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return 0; // Ou autre valeur par défaut
            }
        }
    }
}
