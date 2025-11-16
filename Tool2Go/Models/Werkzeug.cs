namespace Tool2Go.Models
{
    /// <summary>
    /// Repräsentiert ein konkretes Werkzeug innerhalb einer Werkzeugkategorie.
    /// </summary>
    public class Werkzeug
    {
        /// <summary>
        /// Eindeutige Identifier.
        /// Dient zur internen Verwaltung, insbesondere bei Buchungen oder Bearbeitung.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Hersteller des Werkzeugs (z. B. "Bosch").
        /// </summary>
        public string Hersteller { get; set; } = string.Empty;

        /// <summary>
        /// Modellbezeichnung des Werkzeugs (z. B. "GBM 16-2 RE").
        /// </summary>
        public string Modell { get; set; } = string.Empty;

        /// <summary>
        /// Technische Spezifikationen und Besonderheiten des Werkzeugs (Freitext).
        /// </summary>
        public string TechnischeDaten { get; set; } = string.Empty;

        /// <summary>
        /// Gesamtanzahl identischer Exemplare dieses Typs (z.B. 5x Bosch X3).
        /// </summary>
        public int Anzahl { get; set; } = 1;

        /// <summary>
        /// Gibt an in welcher Kategorie sich das Werkzeug befindet.
        /// </summary>
        public Guid KategorieId { get; set; }

        /// <summary>
        /// Parameterloser Konstruktor für XML-Serialisierung.
        /// </summary>
        public Werkzeug()
        {
        }

        /// <summary>
        /// Erstellt ein neues Werkzeug mit Hersteller, Modell und technischen Daten.
        /// </summary>
        public Werkzeug(string hersteller, string modell, string technischeDaten)
        {
            Id = Guid.NewGuid();
            Hersteller = hersteller;
            Modell = modell;
            TechnischeDaten = technischeDaten;
        }

        /// <summary>
        /// Gibt eine formatierte Zeichenkette mit den wichtigsten Werkzeugdaten zurück.
        /// </summary>
           public override string ToString()
        {
            return $"{Hersteller} {Modell} | {TechnischeDaten}";
        }
    }
}
