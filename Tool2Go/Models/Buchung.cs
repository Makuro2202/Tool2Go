using System.Text;

namespace Tool2Go.Models
{
    /// <summary>
    /// Repräsentiert eine Buchung eines Werkzeugs durch einen Kunden.
    /// Beinhaltet alle relevanten Daten inkl. Zeit, Preis und Versicherungsprüfung.
    /// </summary>
    public class Buchung
    {
        /// <summary>
        /// Kunde, der die Buchung durchführt.
        /// </summary>
        public Kunde Kunde { get; set; }

        /// <summary>
        /// Werkzeugkategorien dienen zur Preisberechnung und Versicherungsprüfung.
        /// </summary>
        public List<Werkzeugkategorie> Kategorien { get; set; }
   
        /// <summary>
        /// Konkretes Werkzeug, das verliehen wird.
        /// </summary>
        public Werkzeug Werkzeug { get; set; }

        /// <summary>
        /// Beginn der Buchung im Format "dd.MM.yyyy".
        /// </summary>
        public DateTime Startdatum { get; set; }

        /// <summary>
        /// Ende der Buchung im Format "dd.MM.yyyy".
        /// </summary>
        public DateTime Enddatum { get; set; }

        /// <summary>
        /// Buchungspositionen mit Werkzeug und Menge
        /// <summary>
        public List<BuchungPos> Positionen { get; set; } = new();

        /// <summary>
        /// Speichert die Kosten der Gesamten Buchung (alle Buchungspositionen ab)
        /// </summary>
        public decimal Gesamtkosten { get; set; }

        /// <summary>
        /// Parameterloser Konstruktor für XML-Serialisierung
        /// </summary>
        public Buchung() { }

        /// <summary>
        /// Parametrisierter Konstruktor für XML-Serialisierung
        /// </summary>
        public Buchung(Kunde kunde, DateTime start, DateTime ende, List<BuchungPos> positionen, List<Werkzeugkategorie> kategorien)
        {
            Kunde = kunde;
            Startdatum = start;
            Enddatum = ende;
            Positionen = positionen ?? new List<BuchungPos>();
            Kategorien = kategorien;
            Gesamtkosten = BerechneGesamtkosten(kategorien);
        }

        /// <summary>
        /// Berechnet die Gesamtkosten der Buchung basierend auf Buchungspositionen.
        /// </summary>
        public decimal BerechneGesamtkosten(List<Werkzeugkategorie> kategorien)
        {
            var tage = (Enddatum - Startdatum).Days + 1;

            decimal sum = 0;
            foreach (var pos in Positionen)
                sum += pos.BerechnePosKosten(Startdatum, Enddatum, kategorien);
            return sum;
        }

        /// <summary>
        /// Gibt eine kompakte textuelle Darstellung der Buchung zurück.
        /// </summary>
        public override string ToString()
        {
            return FormatMitKategorien(Kategorien ?? new List<Werkzeugkategorie>());
        }

        private string FormatMitKategorien(List<Werkzeugkategorie> kategorien)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"📄 {Kunde.Vorname} {Kunde.Nachname} buchte:");
            builder.AppendLine($"🗓️  Zeitraum: {Startdatum:dd.MM.yyyy} bis {Enddatum:dd.MM.yyyy}");
            builder.AppendLine("🔧 Werkzeuge:");

            // Sortiere die Buchungspositionen nach Startdatum
            foreach (var pos in Positionen.OrderBy(p => p.Startdatum))
            {
                var werkzeug = pos.Werkzeuge.First();
                var kat = kategorien.FirstOrDefault(k => k.Id == werkzeug.KategorieId);
                string katName = kat?.Name ?? "Unbekannt";

                builder.AppendLine(
                    $"→ {pos.Werkzeuge.Count}x {werkzeug.Hersteller} {werkzeug.Modell} ({werkzeug.TechnischeDaten}) – " +
                    $"📆 Zeitraum: {pos.Startdatum:dd.MM.yyyy} bis {pos.Enddatum:dd.MM.yyyy} – " +
                    $"Kategorie: {katName} – " +
                    $"Kosten: {pos.BerechnePosKosten(pos.Startdatum, pos.Enddatum, kategorien):N2} €"
                );
            }

            builder.AppendLine($"💰 Gesamtkosten: {BerechneGesamtkosten(kategorien):N2} €");
            return builder.ToString();
        }
    }
}
