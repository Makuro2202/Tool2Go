using Tool2Go.Models;
using Tool2Go.Utils;
using Tool2Go.Interfaces;

namespace Tool2Go.Services
{
    /// <summary>
    /// Serviceklasse zur Verwaltung aller Kunden.
    /// Ermöglicht Erstellen, Bearbeiten, Löschen und Anzeigen von Kunden.
    /// </summary>
    public class KundenService : IVerwaltbar<Kunde>
    {
        private readonly List<Kunde> kunden;

        /// <summary>
        /// Erstellt eine neue Kundenverwaltung mit optionalen Startdaten.
        /// </summary>
        public KundenService(List<Kunde>? startDaten = null)
        {
            kunden = startDaten ?? new List<Kunde>();
        }

        public void Hinzufuegen() => KundeHinzufuegen();
        public void Bearbeiten() => KundeBearbeiten();
        public void Loeschen() => KundeLoeschen();
        public void Anzeigen() => AlleKundenAnzeigen();
        public List<Kunde> GetElemente() => kunden;

        /// <summary>
        /// Gibt alle aktuell gespeicherten Kunden auf der Konsole aus.
        /// </summary>
        private void AlleKundenAnzeigen()
        {
            if (kunden.Count == 0)
            {
                Console.WriteLine("⚠️ Es sind keine Kunden vorhanden.");
                return;
            }

            Console.WriteLine("👤Verfügbare Kunden:");
            for (int i = 0; i < kunden.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {kunden[i]}");
            }
        }

        /// <summary>
        /// Legt einen neuen Kunden über Benutzereingabe an.
        /// </summary>
        private void KundeHinzufuegen()
        {
            try
            {
                // 1) Zeige bereits existierende Kunden
                if (kunden.Count == 0)
                {
                    Console.WriteLine("⚠️ Derzeit sind keine Kunden vorhanden.");
                }
                else
                {
                    Console.WriteLine("Bereits vorhandene Kunden:");
                    for (int i = 0; i < kunden.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {kunden[i].Vorname} {kunden[i].Nachname} – {kunden[i].Adresse}");
                    }
                }

                // 2) Eingabe des neuen Kunden
                Console.WriteLine($"\n➕ Neuen Kunden anlegen – ({InputHelper.AbbrechenHinweis}) ");

                string vorname = InputHelper.Eingabe("Vorname: ", EingabeParser.String);
                string nachname = InputHelper.Eingabe("Nachname: ", EingabeParser.String);
                string adresse = InputHelper.Eingabe("Adresse: ", EingabeParser.String);
                DateTime geburtsdatum = InputHelper.Eingabe("Geburtsdatum (TT.MM.JJJJ): ", EingabeParser.Geburtsdatum);

                string iban;
                while (true)
                {
                    iban = InputHelper.Eingabe("IBAN (oder 'bar'): ", EingabeParser.String);

                    if (iban.Equals("bar", StringComparison.OrdinalIgnoreCase) || IbanValidator.IstGueltig(iban))
                        break;

                    Console.WriteLine("❌ Ungültige IBAN. Bitte erneut eingeben.");
                }

                var kunde = new Kunde
                {
                    Vorname = vorname,
                    Nachname = nachname,
                    Adresse = adresse,
                    Geburtsdatum = geburtsdatum,
                    Iban = iban.Equals("bar", StringComparison.OrdinalIgnoreCase) ? null : iban
                };

                kunden.Add(kunde);
                Console.WriteLine("✅ Kunde erfolgreich hinzugefügt.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang wurde abgebrochen.");
            }
        }

        /// <summary>
        /// Entfernt einen Kunden basierend auf Auswahl durch den Benutzer.
        /// </summary>
        private void KundeLoeschen()
        {
            try
            {
                if (kunden.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Kunden vorhanden.");
                    return;
                }

                Console.WriteLine($"🗑 Kundenübersicht ({InputHelper.AbbrechenHinweis})");
                AlleKundenAnzeigen();

                int index;
                while (true)
                {
                    index = InputHelper.Eingabe(
                        $"Kundennummer zum Löschen auswählen ({InputHelper.AbbrechenHinweis}): ",
                        EingabeParser.Int
                    );

                    if (index < 1 || index > kunden.Count)
                    {
                        Console.WriteLine("❌ Ungültige Auswahl. Bitte erneut eingeben.");
                        continue;
                    }

                    break;
                }

                var kunde = kunden[index - 1];
                Console.WriteLine($"⚠️ Möchten Sie '{kunde.Vorname} {kunde.Nachname}' wirklich löschen?");
                bool bestaetigen = InputHelper.Eingabe("Bestätigen mit (j/n): ", EingabeParser.Bool);

                if (bestaetigen)
                {
                    kunden.Remove(kunde);
                    Console.WriteLine("✅ Kunde wurde gelöscht.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Kunde wurde nicht gelöscht.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Vorgang abgebrochen.");
            }
        }

        /// <summary>
        /// Ermöglicht das Bearbeiten eines bestehenden Kunden.
        /// </summary>
        private void KundeBearbeiten()
        {
            try
            {
                if (kunden.Count == 0)
                {
                    Console.WriteLine("⚠️ Es sind keine Kunden vorhanden.");
                    return;
                }

                Console.WriteLine($"✏️ Kundenübersicht ({InputHelper.AbbrechenHinweis})");
                AlleKundenAnzeigen();

                int index = InputHelper.Eingabe($"Kundennummer auswählen ({InputHelper.AbbrechenHinweis}): ", EingabeParser.Int);
                if (index < 1 || index > kunden.Count)
                {
                    Console.WriteLine("❌ Ungültige Kundennummer.");
                    return;
                }

                var alterKunde = kunden[index - 1];
                Console.WriteLine($"🔧 Bearbeite Kunde: {alterKunde.Vorname} {alterKunde.Nachname}\n({InputHelper.AbbrechenHinweis})");

                // Eingaben mit "Enter = behalten"
                string vorname = InputHelper.EingabeOptional("Neuer Vorname (Enter = behalten): ", alterKunde.Vorname);
                string nachname = InputHelper.EingabeOptional("Neuer Nachname (Enter = behalten): ", alterKunde.Nachname);
                string adresse = InputHelper.EingabeOptional("Neue Adresse (Enter = behalten): ", alterKunde.Adresse);
                DateTime geburtsdatum = InputHelper.EingabeOptional("Neues Geburtsdatum (TT.MM.JJJJ, Enter = behalten): ", alterKunde.Geburtsdatum, EingabeParser.Geburtsdatum);

                string neueIban;
                while (true)
                {
                    neueIban = InputHelper.EingabeOptional("Neue IBAN (oder 'bar', Enter = behalten): ", alterKunde.Iban ?? "bar");

                    if (neueIban.Equals("bar", StringComparison.OrdinalIgnoreCase) || IbanValidator.IstGueltig(neueIban))
                        break;

                    Console.WriteLine("❌ Ungültige IBAN. Bitte erneut eingeben.");
                }

                // Jetzt Änderungen übernehmen
                alterKunde.Vorname = vorname;
                alterKunde.Nachname = nachname;
                alterKunde.Adresse = adresse;
                alterKunde.Geburtsdatum = geburtsdatum;
                alterKunde.Iban = neueIban.Equals("bar", StringComparison.OrdinalIgnoreCase) ? null : neueIban;

                Console.WriteLine("✅ Kunde erfolgreich aktualisiert.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("📌 Bearbeitung wurde abgebrochen. Keine Änderungen übernommen.");
            }
        }
    }
}

