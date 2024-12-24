using System;

namespace MySingletonExchangeApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

                // Appel du Singleton
                var exchanged = ExchangeService.GetInstance.Convert(baseCurrency, targetCurrency, amount);

                Console.WriteLine($"{amount} {baseCurrency} = {exchanged} {targetCurrency}");
                Console.WriteLine("----------------------------------------------\n");
            }
        }
    }
}
