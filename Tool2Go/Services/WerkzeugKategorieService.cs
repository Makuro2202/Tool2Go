using System.Globalization;
using Tool2Go.Models;
using Tool2Go.Utils;
using Tool2Go.Interfaces;

namespace Tool2Go.Services
{
    public class WerkzeugKategorieService : IVerwaltbar<Werkzeugkategorie>
    {
        private readonly List<Werkzeugkategorie> kategorien;

        /// <summary>
        /// Initialisiert den Service mit optionalen Startdaten.
        /// </summary>
        public WerkzeugKategorieService(List<Werkzeugkategorie>? startDaten = null)
        {
            kategorien = startDaten ?? new List<Werkzeugkategorie>();
        }

        public void Hinzufuegen() => KategorieHinzufuegen();
        public void Bearbeiten() => KategorieBearbeiten();
        public void Loeschen() => KategorieLoeschen();
        public void Anzeigen() => AlleKategorienAnzeigen();
        public List<Werkzeugkategorie> GetElemente() => kategorien;


        /// <summary>
        /// Gibt alle Kategorien und deren Werkzeuge auf der Konsole aus.
        /// </summary>
        private void AlleKategorienAnzeigen()
        {
            if (!kategorien.Any())
            {
                Console.WriteLine("Keine Werkzeugkategorien vorhanden.");
                return;
            }

            Console.WriteLine("Werkzeugkategorien:");
            foreach (var kategorie in kategorien)
            {
                Console.WriteLine($"- {kategorie}");

                if (kategorie.Werkzeuge.Count == 0)
                {
                    Console.WriteLine("   (Keine Werkzeuge vorhanden)");
                    continue;
                }

                foreach (var werkzeug in kategorie.Werkzeuge)
                {
                    Console.WriteLine($"   → {werkzeug}");
                }
            }
        }

        /// <summary>
        /// Fügt eine neue Werkzeugkategorie hinzu.
        /// </summary>
        private void KategorieHinzufuegen()
        {
            try
            {
                // 1) Zeige bereits existierende Kategorien
                if (!kategorien.Any())
                {
                    Console.WriteLine("⚠️ Derzeit sind keine Kategorien vorhanden.");
                }
                else
                {
                    Console.WriteLine("Bereits existierende Kategorien:");
                    foreach (var kat in kategorien)
                    {
                        Console.WriteLine($"- {kat.Name}");
                    }
                }

                Console.WriteLine($"\n➕ Neue Kategorie anlegen ({InputHelper.AbbrechenHinweis})");

                // 2) Eingabe der neuen Kategorie
                string name = InputHelper.Eingabe("Name der Kategorie: ", EingabeParser.String);

                if (kategorien.Any(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("❌ Kategorie mit diesem Namen existiert bereits.");
                    return;
                }

                decimal proTag = InputHelper.Eingabe("Leihgebühr pro Tag (€): ", EingabeParser.Decimal);
                decimal proWoche = InputHelper.Eingabe("Leihgebühr pro Woche (€): ", EingabeParser.Decimal);
                bool versicherung = InputHelper.Eingabe("Versicherungspflichtig (j/n): ", EingabeParser.Bool);

                var neueKategorie = new Werkzeugkategorie(name, proTag, proWoche, versicherung);
                kategorien.Add(neueKategorie);

                Console.WriteLine("✅ Kategorie erfolgreich hinzugefügt.");

                // 3) Frage nach Werkzeug-Hinzufügen
                Console.WriteLine("\nMöchten Sie jetzt direkt ein Werkzeug zur neuen Kategorie hinzufügen?");
                bool direktWerkzeug = InputHelper.Eingabe("Antwort (j/n): ", EingabeParser.Bool);

                if (direktWerkzeug)
                {
                    var werkzeugService = new WerkzeugService(kategorien);
                    werkzeugService.Hinzufuegen(); 
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang wurde abgebrochen.");
            }
        }

        /// <summary>
        /// Entfernt eine Kategorie anhand des Namens.
        /// </summary>
        private void KategorieLoeschen()
        {
            try
            {
                if (kategorien.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Kategorien vorhanden.");
                    return;
                }

                Console.WriteLine("📦 Verfügbare Kategorien:");
                AlleKategorienAnzeigen();

                string name;
                Werkzeugkategorie? kategorie;

                while (true)
                {
                    name = InputHelper.Eingabe(
                        $"Name der Kategorie zum Löschen ({InputHelper.AbbrechenHinweis}): ",
                        EingabeParser.String
                    );

                    kategorie = kategorien.FirstOrDefault(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                    if (kategorie == null)
                    {
                        Console.WriteLine("❌ Kategorie nicht gefunden. Bitte erneut versuchen.");
                        continue;
                    }

                    break;
                }

                if (kategorie.Werkzeuge.Any())
                {
                    Console.WriteLine($"⚠️ Die Kategorie '{kategorie.Name}' enthält {kategorie.Werkzeuge.Count} Werkzeug(e).");
                    Console.WriteLine("Wenn Sie fortfahren, werden alle zugehörigen Werkzeuge dauerhaft gelöscht.");
                }

                Console.WriteLine($"⚠️ Möchten Sie die Kategorie '{kategorie.Name}' wirklich löschen?");
                bool bestaetigen = InputHelper.Eingabe("Bestätigen mit (j/n): ", EingabeParser.Bool);

                if (bestaetigen)
                {
                    kategorien.Remove(kategorie);
                    Console.WriteLine("✅ Kategorie wurde gelöscht.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Kategorie wurde nicht gelöscht.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang abgebrochen.");
            }
        }

        /// <summary>
        /// Ermöglicht das Bearbeiten einer bestehenden Werkzeugkategorie.
        /// </summary>
        private void KategorieBearbeiten()
        {
            if (!kategorien.Any())
            {
                Console.WriteLine("⚠️ Keine Kategorien vorhanden.");
                return;
            }

            AlleKategorienAnzeigen();

            Console.Write("Name der zu bearbeitenden Kategorie: ");
            var name = Console.ReadLine();

            var kategorie = kategorien.FirstOrDefault(k =>
                k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (kategorie == null)
            {
                Console.WriteLine("⚠️ Kategorie nicht gefunden.");
                return;
            }

            Console.WriteLine($"\nBearbeite Kategorie: {kategorie.Name}");
            Console.Write($"Neuer Name ({kategorie.Name}): ");
            var eingabe = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(eingabe))
            {
                if (kategorien.Any(k => k != kategorie &&
                    k.Name.Equals(eingabe, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("⚠️ Es existiert bereits eine Kategorie mit diesem Namen.");
                }
                else
                {
                    kategorie.Name = eingabe;
                }
            }

            Console.Write($"Neue Leihgebühr pro Tag ({kategorie.LeihgebuehrProTag:C}): ");
            eingabe = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(eingabe) &&
                decimal.TryParse(eingabe, NumberStyles.Number,
                CultureInfo.GetCultureInfo("de-DE"), out var tagessatz) &&
                tagessatz >= 0)
            {
                kategorie.LeihgebuehrProTag = tagessatz;
            }

            Console.Write($"Neue Leihgebühr pro Woche ({kategorie.LeihgebuehrProWoche:C}): ");
            eingabe = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(eingabe) &&
                decimal.TryParse(eingabe, NumberStyles.Number,
                CultureInfo.GetCultureInfo("de-DE"), out var wochensatz) &&
                wochensatz >= 0)
            {
                kategorie.LeihgebuehrProWoche = wochensatz;
            }

            Console.Write($"Versicherungspflicht ({(kategorie.Versicherungspflicht ? "j" : "n")}): ");
            eingabe = Console.ReadLine()?.Trim().ToLower();

            if (eingabe == "j") kategorie.Versicherungspflicht = true;
            else if (eingabe == "n") kategorie.Versicherungspflicht = false;

            Console.WriteLine("Kategorie erfolgreich bearbeitet.");
        }
    }
}
