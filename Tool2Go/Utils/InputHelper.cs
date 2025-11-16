using System;

namespace Tool2Go.Utils
{
    using static EingabeParser;

    /// <summary>
    /// Statische Hilfsklasse zur robusten und abbrechbaren Benutzereingabe.
    /// </summary>
    public static class InputHelper
    {
        private const string AbbruchBefehl = "abbruch";

        /// <summary>
        /// Hinweistext für Benutzerprompts.
        /// </summary>
        public static string AbbrechenHinweis => $"Zum Abbrechen '{AbbruchBefehl}' eingeben.";

        /// <summary>
        /// Prüft, ob der Benutzer den Vorgang abbrechen möchte.
        /// </summary>
        private static void AbbruchPruefen(string eingabe)
        {
            if (eingabe.Trim().ToLower() == AbbruchBefehl)
            {
                throw new OperationCanceledException("Benutzer hat den Vorgang abgebrochen.");
            }
        }

        /// <summary>
        /// Generische Eingabemethode mit Parser, Validierung und Abbruchoption.
        /// </summary>
        public static T Eingabe<T>(string prompt, EingabeDefinition<T> def)
        {
            while (true)
            {
                Console.Write(prompt);
                var eingabe = Console.ReadLine()?.Trim() ?? "";

                AbbruchPruefen(eingabe);

                var (ok, value) = def.Parser(eingabe);
                if (ok) return value;

                Console.WriteLine(def.Fehlermeldung);
            }
        }
        public static string EingabeOptional(string prompt, string bisherigerWert)
        {
            Console.Write($"{prompt}"); // Hinweis kommt vom Aufrufer
            var eingabe = Console.ReadLine()?.Trim();

            AbbruchPruefen(eingabe ?? "");

            return string.IsNullOrWhiteSpace(eingabe) ? bisherigerWert : eingabe;
        }

        public static DateTime EingabeOptional(string prompt, DateTime bisherigesDatum, EingabeDefinition<DateTime> def)
        {
            Console.Write($"{prompt}"); // z. B. "Neues Geburtsdatum (TT.MM.JJJJ, Enter = behalten): "
            var eingabe = Console.ReadLine()?.Trim() ?? "";

            AbbruchPruefen(eingabe);

            if (string.IsNullOrWhiteSpace(eingabe))
                return bisherigesDatum;

            var (ok, value) = def.Parser(eingabe);
            if (ok) return value;

            Console.WriteLine(def.Fehlermeldung);
            return EingabeOptional(prompt, bisherigesDatum, def); // rekursiver Retry
        }

        public static decimal EingabeOptional(string prompt, decimal bisher, EingabeDefinition<decimal> def)
        {
            Console.Write(prompt);
            var eingabe = Console.ReadLine()?.Trim() ?? "";

            AbbruchPruefen(eingabe);

            if (string.IsNullOrWhiteSpace(eingabe))
                return bisher;

            var (ok, value) = def.Parser(eingabe);
            if (ok) return value;

            Console.WriteLine(def.Fehlermeldung);
            return EingabeOptional(prompt, bisher, def);
        }
        public static bool EingabeOptional(string prompt, bool bisher, EingabeDefinition<bool> def)
        {
            Console.Write(prompt);
            var eingabe = Console.ReadLine()?.Trim() ?? "";

            AbbruchPruefen(eingabe);

            if (string.IsNullOrWhiteSpace(eingabe))
                return bisher;

            var (ok, value) = def.Parser(eingabe);
            if (ok) return value;

            Console.WriteLine(def.Fehlermeldung);
            return EingabeOptional(prompt, bisher, def);
        }

    }
}
