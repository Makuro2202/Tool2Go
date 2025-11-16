using Tool2Go.Models;
using Tool2Go.Utils;
using Tool2Go.Interfaces;

namespace Tool2Go.Services
{
    /// <summary>
    /// Service zur Verwaltung von Buchungen (Verleihvorgänge).
    /// </summary>
    public class BuchungService : IVerwaltbar<Buchung>
    {
        private readonly List<Buchung> buchungen;
        private readonly IVerwaltbar<Kunde> kundenService;
        private readonly IVerwaltbar<Werkzeugkategorie> werkzeugKategorienService;
        private readonly IVerwaltbar<Werkzeug> werkzeugService;

        public BuchungService(
            IVerwaltbar<Kunde> kundenService,
            IVerwaltbar<Werkzeugkategorie> werkzeugKategorienService,
            IVerwaltbar<Werkzeug> werkzeugService,
            List<Buchung>? startDaten = null)
        {
            this.kundenService = kundenService;
            this.werkzeugKategorienService = werkzeugKategorienService;
            this.werkzeugService = werkzeugService;
            buchungen = startDaten ?? new List<Buchung>();
        }

        public void Hinzufuegen() => BuchungHinzufuegen();
        public void Bearbeiten() => BuchungBearbeiten();
        public void Loeschen() => BuchungLoeschen();
        public void Anzeigen() => AlleBuchungenAnzeigen();
        public List<Buchung> GetElemente() => buchungen;

        private void AlleBuchungenAnzeigen()
        {
            if (!buchungen.Any())
            {
                Console.WriteLine("⚠️ Es sind keine Buchungen vorhanden.");
                return;
            }

            Console.WriteLine("Buchungen:");
            for (int i = 0; i < buchungen.Count; i++)
            {
                var buchung = buchungen[i];
                Console.WriteLine($"{i + 1}. {buchungen[i]}");
            }
        }

        private void BuchungHinzufuegen()
        {
            try
            {
                var kunden = kundenService.GetElemente();
                var kategorien = werkzeugKategorienService.GetElemente();

                if (kunden.Count == 0 || kategorien.Count == 0)
                {
                    Console.WriteLine("❌ Es müssen mindestens ein Kunde und eine Werkzeugkategorie vorhanden sein.");
                    return;
                }

                // Anzeigen der verfügbaren Kunden
                kundenService.Anzeigen();
                int kundenIndex = InputHelper.Eingabe($"Kundennummer auswählen: ({InputHelper.AbbrechenHinweis}) ", EingabeParser.Int);
                if (kundenIndex < 1 || kundenIndex > kunden.Count)
                {
                    Console.WriteLine("❌ Ungültige Kundennummer.");
                    return;
                }

                var kunde = kunden[kundenIndex - 1];

                // Erfragen von Zeitraum
                DateTime start, ende;
                while (true)
                {
                    start = InputHelper.Eingabe("Startdatum (TT.MM.JJJJ): ", EingabeParser.ZukunftsdatumMitAbbrechen);
                    ende = InputHelper.Eingabe("Enddatum (TT.MM.JJJJ): ", EingabeParser.ZukunftsdatumMitAbbrechen);

                    if (ende < start)
                    {
                        Console.WriteLine("❌ Enddatum darf nicht vor dem Startdatum liegen.");
                        continue;
                    }

                    bool verfuegbar = werkzeugService
                        .GetElemente()
                        .Any(w => AnzahlBelegt(w, start, ende) < w.Anzahl);

                    if (!verfuegbar)
                    {
                        Console.WriteLine("❌ Keine Werkzeuge im gewünschten Zeitraum verfügbar.");
                        continue;
                    }

                    break;
                }

                bool zeitraumGlobal = InputHelper.Eingabe("🕒 Soll der Zeitraum für alle Werkzeuge gelten? (j/n): ", EingabeParser.Bool);
                var positionen = new List<BuchungPos>();
                Werkzeugkategorie? aktuelleKategorie = null;
                bool weitereWerkzeuge = true;
                bool erstesWerkzeug = true;

                while (weitereWerkzeuge)
                {
                    try
                    {
                        if (aktuelleKategorie == null || !InputHelper.Eingabe("➡️ Gleiche Kategorie verwenden wie vorher? (j/n): ", EingabeParser.Bool))
                        {
                            Console.WriteLine($"📁 Verfügbare Kategorien: ({InputHelper.AbbrechenHinweis} oder 'zurück')");
                            werkzeugKategorienService.Anzeigen();

                            string kategorieName = InputHelper.Eingabe("Kategorie auswählen (oder 'zurück'): ", EingabeParser.StringMitAbbrechen);
                            aktuelleKategorie = kategorien.FirstOrDefault(k => k.Name.Equals(kategorieName, StringComparison.OrdinalIgnoreCase));

                            if (aktuelleKategorie == null || aktuelleKategorie.Werkzeuge.Count == 0)
                            {
                                Console.WriteLine("❌ Kategorie nicht gefunden oder leer.");
                                continue;
                            }
                        }

                        // Übersichtshalber die Werkzeuge nach Hersteller + Modell gruppiert anzeigen
                        var gruppierteWerkzeuge = aktuelleKategorie.Werkzeuge
                            .GroupBy(w => new { w.Hersteller, w.Modell })
                            .ToList();

                        Console.WriteLine("🛠 Verfügbare Werkzeugtypen:");
                        for (int i = 0; i < gruppierteWerkzeuge.Count; i++)
                        {
                            var instanz = gruppierteWerkzeuge[i].First();
                            Console.WriteLine($"{i + 1}. {instanz.Hersteller} {instanz.Modell}");
                        }

                        int auswahl = InputHelper.Eingabe("Werkzeugnummer auswählen (oder 'zurück'): ", EingabeParser.IntMitAbbrechen);
                        if (auswahl < 1 || auswahl > gruppierteWerkzeuge.Count)
                        {
                            Console.WriteLine("❌ Ungültige Auswahl.");
                            continue;
                        }

                        var gruppe = gruppierteWerkzeuge[auswahl - 1];
                        var exemplar = gruppe.First();

                        DateTime localStart;
                        DateTime localEnde;

                        if (zeitraumGlobal) // wenn alle Werkzeuge für denselben Zeitraum gebucht werden sollen.
                        {
                            localStart = start;
                            localEnde = ende;
                        }
                        else if (erstesWerkzeug)
                        {
                            localStart = start;
                            localEnde = ende;
                            erstesWerkzeug = false; 
                        }
                        else // wenn für jedes Werkzeug ein anderer Zeitraum erwünscht ist.
                        {
                            localStart = InputHelper.Eingabe("Startdatum für dieses Werkzeug: ", EingabeParser.ZukunftsdatumMitAbbrechen);
                            localEnde = InputHelper.Eingabe("Enddatum für dieses Werkzeug: ", EingabeParser.ZukunftsdatumMitAbbrechen);
                        }

                        if (localEnde < localStart)
                        {
                            Console.WriteLine("❌ Enddatum darf nicht vor dem Startdatum liegen.");
                            continue;
                        }

                        if (aktuelleKategorie.Versicherungspflicht) // darf nur an Kunden > 21 Jahre verliehen werden
                        {
                            int alter = DateTime.Today.Year - kunde.Geburtsdatum.Year;
                            if (kunde.Geburtsdatum > DateTime.Today.AddYears(-alter)) alter--;
                            if (alter < 21)
                            {
                                Console.WriteLine("🚫 Kunden unter 21 Jahren dürfen keine versicherungspflichtigen Werkzeuge buchen.");
                                continue;
                            }
                        }

                        // überprüfen WIEVIELE Instanzen verfügbar sind.
                        var instanzenVomTyp = gruppe.ToList();
                        int frei = AnzahlFreierInstanzenVomTyp(instanzenVomTyp, localStart, localEnde, positionen);

                        if (frei <= 0)
                        {
                            Console.WriteLine("❌ Keine freien Instanzen verfügbar.");
                            continue;
                        }

                        int menge = InputHelper.Eingabe($"Wieviele Instanzen von diesem Typ? (max. {frei}): ", EingabeParser.IntMitAbbrechen);
                        while (menge > frei)
                        {
                            Console.WriteLine($"❌ Nur {frei} verfügbar. Bitte kleinere Anzahl eingeben.");
                            menge = InputHelper.Eingabe($"Wieviele Instanzen von diesem Typ? (max. {frei}): ", EingabeParser.IntMitAbbrechen);
                        }

                        // überprüfen WELCHE Instanzen verfügbar sind.
                        var freieWerkzeuge = instanzenVomTyp
                            .SelectMany(w => Enumerable.Repeat(w, w.Anzahl))
                            .Where(w =>
                                    buchungen.All(b =>
                                        b.Positionen.All(pos =>
                                            !pos.Werkzeuge.Contains(w) ||
                                            pos.Enddatum <= localStart || pos.Startdatum >= localEnde
                                        )
                                    ) &&
                                    positionen.All(pos =>
                                        !pos.Werkzeuge.Contains(w) ||
                                        pos.Enddatum <= localStart || pos.Startdatum >= localEnde
                                    )
                                )
                            .Take(menge)
                            .ToList();

                        if (freieWerkzeuge.Count < menge)
                        {
                            Console.WriteLine($"❌ Nur {freieWerkzeuge.Count} Instanzen verfügbar. Bitte neuen Zeitraum wählen.");
                            continue;
                        }

                        // Position abspeichern
                        positionen.Add(new BuchungPos
                        {
                            Werkzeuge = freieWerkzeuge,
                            Startdatum = localStart,
                            Enddatum = localEnde
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("↩️ Hinzufügen abgebrochen.");
                    }

                    weitereWerkzeuge = InputHelper.Eingabe("Noch ein weiteres Werkzeug buchen? (j/n): ", EingabeParser.Bool);
                }

                var buchungStart = positionen.Min(p => p.Startdatum);
                var buchungEnde = positionen.Max(p => p.Enddatum);
                var buchung = new Buchung(kunde, buchungStart, buchungEnde, positionen, kategorien);

                // Übersicht der Buchung anzeigen.
                Console.WriteLine("\n📄 Buchungsvorschau:");
                Console.WriteLine(buchung.ToString());

                // nach Bestätigung Buchung abspeichern
                bool speichern = InputHelper.Eingabe("Möchten Sie diese Buchung speichern? (j/n): ", EingabeParser.Bool);
                if (speichern)
                {
                    buchungen.Add(buchung);
                    Console.WriteLine("✅ Buchung wurde gespeichert.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Buchung wurde verworfen.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang wurde abgebrochen.");
            }
        }

        private void BuchungLoeschen()
        {
            try
            {
                if (buchungen.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Buchungen vorhanden.");
                    return;
                }

                Console.WriteLine("📄 Aktuelle Buchungen:");
                AlleBuchungenAnzeigen();

                int index;
                while (true)
                {
                    index = InputHelper.Eingabe(
                        $"Buchungsnummer zum Löschen auswählen ({InputHelper.AbbrechenHinweis}): ",
                        EingabeParser.Int
                    );

                    if (index < 1 || index > buchungen.Count)
                    {
                        Console.WriteLine("❌ Ungültige Auswahl. Bitte erneut eingeben.");
                        continue;
                    }

                    break;
                }

                var buchung = buchungen[index - 1];
                Console.WriteLine("\n📄 Ausgewählte Buchung zur Löschung:");
                Console.WriteLine(buchung.ToString());
                bool bestaetigen = InputHelper.Eingabe("Bestätigen mit (j/n): ", EingabeParser.Bool);

                if (bestaetigen)
                {
                    buchungen.Remove(buchung);
                    Console.WriteLine("✅ Buchung wurde gelöscht.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Buchung wurde nicht gelöscht.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang abgebrochen.");
            }
        }

        private void BuchungBearbeiten()
        {
            try
            {
                if (buchungen.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Buchungen vorhanden.");
                    return;
                }

                Console.WriteLine("📄 Aktuelle Buchungen: ");
                AlleBuchungenAnzeigen();

                int index = InputHelper.Eingabe($"Buchungsnummer auswählen ({InputHelper.AbbrechenHinweis}) ", EingabeParser.Int);
                if (index < 1 || index > buchungen.Count)
                {
                    Console.WriteLine("❌ Ungültige Buchungsnummer.");
                    return;
                }

                var buchung = buchungen[index - 1];
                Console.WriteLine($"🔧 Bearbeite Buchung: {buchung}");

                DateTime start, ende;
                List<Buchung> konflikte = new();

                do
                {
                    // Eingabe von Leertaste bedeutet "Behalte alte Eingabe bei" -> weniger Aufwand
                    start = InputHelper.EingabeOptional("Neues Startdatum (TT.MM.JJJJ, Enter = behalten): ", buchung.Startdatum, EingabeParser.Zukunftsdatum);
                    ende = InputHelper.EingabeOptional("Neues Enddatum (TT.MM.JJJJ, Enter = behalten): ", buchung.Enddatum, EingabeParser.Zukunftsdatum);

                    if (ende < start)
                    {
                        Console.WriteLine("❌ Das Enddatum darf nicht vor dem Startdatum liegen.");
                        continue;
                    }

                    konflikte = buchungen
                        .Where(b => b != buchung &&
                                    b.Werkzeug.Id == buchung.Werkzeug.Id &&
                                    start <= b.Enddatum &&
                                    ende >= b.Startdatum)
                        .ToList();

                    if (konflikte.Any())
                    {
                        Console.WriteLine("❌ Das gewählte Werkzeug ist im neuen Zeitraum bereits verliehen.");
                        foreach (var k in konflikte)
                        {
                            Console.WriteLine($"→ Belegt von {k.Startdatum:dd.MM.yyyy} bis {k.Enddatum:dd.MM.yyyy}");
                        }
                        Console.WriteLine("🔁 Bitte neuen Zeitraum wählen.\n");
                    }

                } while (ende < start || konflikte.Any());

                buchung.Startdatum = start;
                buchung.Enddatum = ende;
                buchung.Gesamtkosten = buchung.BerechneGesamtkosten(werkzeugKategorienService.GetElemente());

                Console.WriteLine("✅ Buchung erfolgreich aktualisiert.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Bearbeitung wurde abgebrochen. Keine Änderungen vorgenommen.");
            }
        }

        private int AnzahlBelegt(Werkzeug werkzeug, DateTime start, DateTime ende, List<BuchungPos>? temporaerePositionen = null)
        {
            // Alle Buchungspositionen (inkl. temporärer, wenn vorhanden)
            var allePositionen = buchungen.SelectMany(b => b.Positionen).ToList();
            if (temporaerePositionen != null)
                allePositionen.AddRange(temporaerePositionen);

            // Nur Werkzeuge mit exakt gleicher Referenz (nicht nur Typ!) zählen
            return allePositionen
                .Where(pos => pos.Startdatum < ende && pos.Enddatum > start)
                .SelectMany(pos => pos.Werkzeuge)
                .Count(w => w == werkzeug); 
        }

        private int AnzahlFreierInstanzenVomTyp(List<Werkzeug> werkzeugeVomTyp, DateTime start, DateTime ende, List<BuchungPos>? temporaerePositionen = null)
        {
            return werkzeugeVomTyp
                .SelectMany(w => Enumerable.Repeat(w, w.Anzahl))
                .Count(w => AnzahlBelegt(w, start, ende, temporaerePositionen) == 0);
        }

    }
}