using System.Xml.Serialization;

namespace Tool2Go.Models
{
    /// <summary>
    /// Repräsentiert eine Werkzeugkategorie, z. B. "Schlagbohrmaschine" oder "Minibagger".
    /// Jede Kategorie enthält eine Liste konkreter Werkzeuge.
    /// </summary>
    public class Werkzeugkategorie
    {
        /// <summary>
        /// Eindeutiger identifier für die Kategorie.
        /// <summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Eindeutiger Name der Kategorie (z. B. "Minibagger").
        /// Dient auch zur Identifikation bei der Auswahl.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Leihgebühr pro Tag in Euro. 
        /// Wird zur Berechnung der Mietkosten bei kürzeren Buchungen verwendet.
        /// </summary>
        public decimal LeihgebuehrProTag { get; set; } = 0;

        /// <summary>
        /// Leihgebühr pro Woche in Euro.
        /// Wird verwendet, wenn Buchungen mindestens 7 Tage umfassen.
        /// </summary>
        public decimal LeihgebuehrProWoche { get; set; } = 0;

        /// <summary>
        /// Gibt an, ob das Werkzeug aus dieser Kategorie nur mit Versicherung gebucht werden darf (Kunde muss über 21 Jahre alt sein).
        /// </summary>
        public bool Versicherungspflicht { get; set; } = false;

        /// <summary>
        /// Liste aller konkreten Werkzeuge, die zu dieser Kategorie gehören.
        /// Wird bei XML-Serialisierung als verschachteltes Array exportiert.
        /// </summary>
        [XmlArray("Werkzeuge")]
        [XmlArrayItem("Werkzeug")]
        public List<Werkzeug> Werkzeuge { get; set; } = new List<Werkzeug>();

        /// <summary>
        /// Parameterloser Konstruktor für XML-Serialisierung und manuelle Initialisierung.
        /// </summary>
        public Werkzeugkategorie()
        {
        }

        /// <summary>
        /// Erstellt eine neue Werkzeugkategorie mit festgelegten Preisen und Versicherungspflicht.
        /// </summary>
        public Werkzeugkategorie(string name, decimal tagessatz, decimal wochensatz, bool versicherungspflicht)
        {
            Name = name;
            LeihgebuehrProTag = tagessatz;
            LeihgebuehrProWoche = wochensatz;
            Versicherungspflicht = versicherungspflicht;
        }

        /// <summary>
        /// Gibt eine formattierte Zeichenkette mit den wichtigsten Informationen zur Kategorie zurück.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} | {LeihgebuehrProTag:C} / Tag | {LeihgebuehrProWoche:C} / Woche | Versicherungspflicht: {(Versicherungspflicht ? "Ja" : "Nein")}";
        }
    }
}
