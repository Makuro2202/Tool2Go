using System.Xml.Serialization;
using Tool2Go.Utils;

namespace Tool2Go.Models
{
    /// <summary>
    /// Repräsentiert einen Kunden, der Werkzeuge ausleihen kann.
    /// Enthält persönliche Daten sowie optional eine IBAN.
    /// </summary>
    public class Kunde
    {
        /// <summary>
        /// Vorname des Kunden.
        /// </summary>
        public string Vorname { get; set; } = string.Empty;

        /// <summary>
        /// Nachname des Kunden.
        /// </summary>
        public string Nachname { get; set; } = string.Empty;

        /// <summary>
        /// Wohnadresse des Kunden (z. B. Straße, Hausnummer, PLZ, Ort).
        /// </summary>
        public string Adresse { get; set; } = string.Empty;

        /// <summary>
        /// Geburtsdatum des Kunden.
        /// Dient zur Altersprüfung bei Buchungen.
        /// </summary>
        public DateTime Geburtsdatum { get; set; }

        /// <summary>
        /// Optionale Bankverbindung im IBAN-Format.
        /// Barzahlung ist erlaubt, daher kann dieser Wert null sein.
        /// </summary>
        public string? Iban { get; set; }

        /// <summary>
        /// Gibt das Alter des Kunden in Jahren zurück.
        /// Wird aus dem Geburtsdatum berechnet.
        /// </summary>
        [XmlIgnore]
        public int Alter
        {
            get
            {
                var heute = DateTime.Today;
                var alter = heute.Year - Geburtsdatum.Year;

                if (Geburtsdatum.Date > heute.AddYears(-alter))
                {
                    alter--;
                }

                return alter;
            }
        }

        /// <summary>
        /// Gibt eine formatierte Zeichenkette mit den Kundendaten zurück.
        /// </summary>
        /// <returns>Vorname Nachname, Adresse, Geburtsdatum, ggf. IBAN</returns>
        public override string ToString()
        {
            return $"{Vorname} {Nachname}, {Adresse}, Geboren am {Geburtsdatum:dd.MM.yyyy}, " +
                   $"IBAN: {(string.IsNullOrEmpty(Iban) ? "Barzahlung" : Iban)}";
        }
    }
}
