using Tool2Go.Models;
using Tool2Go.Utils;
using Tool2Go.Interfaces;

namespace Tool2Go.Services
{
    /// <summary>
    /// Service zur Verwaltung von Werkzeugen innerhalb von Kategorien.
    /// </summary>
    public class WerkzeugService : IVerwaltbar<Werkzeug>
    {
        private readonly List<Werkzeugkategorie> kategorien;

        /// <summary>
        /// Initialisiert den Service mit optionalen Startdaten.
        /// </summary>
        public WerkzeugService(List<Werkzeugkategorie>? startDaten = null)
        {
            kategorien = startDaten ?? new List<Werkzeugkategorie>();
        }

        public void Hinzufuegen() => WerkzeugHinzufuegen();
        public void Bearbeiten() => WerkzeugBearbeiten();
        public void Loeschen() => WerkzeugLoeschen();
        public void Anzeigen() => AlleWerkzeugeAnzeigen();
        public List<Werkzeug> GetElemente()
        {
            return kategorien.SelectMany(k => k.Werkzeuge).ToList();
        }

        private void WerkzeugHinzufuegen()
        {
            try
            {
                // 1) Alle Kategorien vorher anzeigen
                Console.WriteLine("🗂 Verfügbare Kategorien:\n");
                if (!kategorien.Any())
                {
                    Console.WriteLine("⚠️ Es sind noch keine Kategorien vorhanden.");
                    return;
                }
                else
                {
                    foreach (var kat in kategorien)
                    {
                        Console.WriteLine($"- {kat.Name}");
                        if (kat.Werkzeuge.Count == 0)
                        {
                            Console.WriteLine("   → (Noch keine Werkzeuge vorhanden)");
                        }
                        else
                        {
                            foreach (var w in kat.Werkzeuge)
                            {
                                Console.WriteLine($"   → {w.Hersteller} | {w.Modell} | {w.TechnischeDaten}");
                            }
                        }
                        Console.WriteLine(); 
                    }
                }

                // 2) Neues Werkzeug anlegen
                Console.WriteLine($"\n➕ Neues Werkzeug anlegen – ({InputHelper.AbbrechenHinweis})");

                string kategorieName = InputHelper.Eingabe("Name der Werkzeugkategorie: ", EingabeParser.String);

                var kategorie = kategorien.FirstOrDefault(k =>
                    k.Name.Equals(kategorieName, StringComparison.OrdinalIgnoreCase));

                if (kategorie == null)
                {
                    Console.WriteLine("❌ Kategorie nicht gefunden.");
                    return;
                }

                string hersteller = InputHelper.Eingabe("Hersteller: ", EingabeParser.String);
                string modell = InputHelper.Eingabe("Modell: ", EingabeParser.String);

                // Prüfen, ob dieser Typ (Hersteller + Modell) bereits in einer anderen Kategorie existiert
                var vorhandeneKategorie = kategorien
                    .Where(k => k.Id != kategorie.Id)
                    .FirstOrDefault(k => k.Werkzeuge.Any(w =>
                        w.Hersteller.Equals(hersteller, StringComparison.OrdinalIgnoreCase) &&
                        w.Modell.Equals(modell, StringComparison.OrdinalIgnoreCase)));

                if (vorhandeneKategorie != null)
                {
                    Console.WriteLine($"❌ Ein Werkzeug mit diesem Hersteller und Modell existiert bereits in der Kategorie '{vorhandeneKategorie.Name}'.");
                    Console.WriteLine("ℹ️ Bitte fügen Sie dieses Werkzeug ausschließlich in **dieser** Kategorie hinzu.");
                    return;
                }

                string technischeDaten = InputHelper.Eingabe("Technische Daten: ", EingabeParser.String);
                int anzahl = InputHelper.Eingabe("Anzahl identischer Exemplare: ", EingabeParser.Int);

                for (int i = 0; i < anzahl; i++)
                {
                    var werkzeug = new Werkzeug
                    {
                        Id = Guid.NewGuid(),
                        Hersteller = hersteller,
                        Modell = modell,
                        TechnischeDaten = technischeDaten,
                        Anzahl = 1,
                        KategorieId = kategorie.Id
                    };

                    kategorie.Werkzeuge.Add(werkzeug);
                }

                Console.WriteLine($"✅ {anzahl} Werkzeug(e) erfolgreich hinzugefügt.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang wurde abgebrochen.");
            }
        }

        private void WerkzeugBearbeiten()
        {
            try
            {
                if (kategorien.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Kategorien vorhanden.");
                    return;
                }

                Console.WriteLine("🔧 Kategorienübersicht:");
                foreach (var k in kategorien)
                {
                    Console.WriteLine("- " + k.Name);
                }

                Werkzeugkategorie? kategorie = null;

                while (true)
                {
                    string eingabe = InputHelper.Eingabe($"Kategorie auswählen ({InputHelper.AbbrechenHinweis}): ", EingabeParser.String);

                    kategorie = kategorien.FirstOrDefault(k =>
                        k.Name.Equals(eingabe, StringComparison.OrdinalIgnoreCase));

                    if (kategorie == null)
                    {
                        Console.WriteLine("❌ Kategorie nicht gefunden. Bitte erneut eingeben.");
                        continue;
                    }

                    if (kategorie.Werkzeuge.Count == 0)
                    {
                        Console.WriteLine("❌ Diese Kategorie enthält keine Werkzeuge. Bitte andere wählen.");
                        continue;
                    }

                    break;
                }

                Console.WriteLine("🛠 Werkzeuge in dieser Kategorie:");
                for (int i = 0; i < kategorie.Werkzeuge.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {kategorie.Werkzeuge[i]}");
                }

                int werkzeugIndex = InputHelper.Eingabe("Werkzeugnummer auswählen: ", EingabeParser.Int);
                if (werkzeugIndex < 1 || werkzeugIndex > kategorie.Werkzeuge.Count)
                {
                    Console.WriteLine("❌ Ungültige Auswahl.");
                    return;
                }

                var werkzeug = kategorie.Werkzeuge[werkzeugIndex - 1];
                Console.WriteLine($"🔧 Bearbeite Werkzeug: {werkzeug}");

                string hersteller = InputHelper.EingabeOptional("Hersteller (Enter = behalten): ", werkzeug.Hersteller);
                string modell = InputHelper.EingabeOptional("Modell (Enter = behalten): ", werkzeug.Modell);
                string daten = InputHelper.EingabeOptional("Technische Daten (Enter = behalten): ", werkzeug.TechnischeDaten);

                werkzeug.Hersteller = hersteller;
                werkzeug.Modell = modell;
                werkzeug.TechnischeDaten = daten;

                Console.WriteLine("✅ Werkzeug erfolgreich aktualisiert.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Bearbeitung wurde abgebrochen. Keine Änderungen übernommen.");
            }
        }

        private void WerkzeugLoeschen()
        {
            try
            {
                var alleWerkzeuge = kategorien
                    .SelectMany(k => k.Werkzeuge.Select(w => new { Werkzeug = w, Kategorie = k }))
                    .ToList();

                if (alleWerkzeuge.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Werkzeuge vorhanden.");
                    return;
                }

                Console.WriteLine("🛠 Verfügbare Werkzeuge:");
                for (int i = 0; i < alleWerkzeuge.Count; i++)
                {
                    var wk = alleWerkzeuge[i];
                    Console.WriteLine($"{i + 1}. {wk.Werkzeug} (Kategorie: {wk.Kategorie.Name})");
                }

                int index;
                while (true)
                {
                    index = InputHelper.Eingabe(
                        $"Werkzeugnummer zum Löschen auswählen ({InputHelper.AbbrechenHinweis}): ",
                        EingabeParser.Int
                    );

                    if (index < 1 || index > alleWerkzeuge.Count)
                    {
                        Console.WriteLine("❌ Ungültige Auswahl. Bitte erneut eingeben.");
                        continue;
                    }

                    break;
                }

                var ziel = alleWerkzeuge[index - 1];
                Console.WriteLine($"⚠️ Möchten Sie '{ziel.Werkzeug}' aus Kategorie '{ziel.Kategorie.Name}' wirklich löschen?");
                bool bestaetigen = InputHelper.Eingabe("Bestätigen mit (j/n): ", EingabeParser.Bool);

                if (bestaetigen)
                {
                    ziel.Kategorie.Werkzeuge.Remove(ziel.Werkzeug);
                    Console.WriteLine("✅ Werkzeug erfolgreich gelöscht.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Werkzeug wurde nicht gelöscht.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang abgebrochen.");
            }
        }

        private void AlleWerkzeugeAnzeigen()
        {
            if (kategorien.All(k => k.Werkzeuge.Count == 0))
            {
                Console.WriteLine("⚠️ Es sind keine Werkzeuge vorhanden.");
                return;
            }

            var werkzeugeMitKategorie = kategorien
                .SelectMany(k => k.Werkzeuge.Select(w => new { Werkzeug = w, Kategorie = k }))
                .ToList();

            var gruppiertNachHersteller = werkzeugeMitKategorie
                .GroupBy(wk => wk.Werkzeug.Hersteller)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

            Console.WriteLine("📦 Alle Werkzeuge (sortiert nach Hersteller und Modell):\n");

            foreach (var gruppe in gruppiertNachHersteller)
            {
                Console.WriteLine($"🔧 Hersteller: {gruppe.Key}");

                foreach (var wk in gruppe.OrderBy(w => w.Werkzeug.Modell, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"   → {wk.Werkzeug} (Kategorie: {wk.Kategorie.Name})");
                }

                Console.WriteLine();
            }
        }
    }
}
