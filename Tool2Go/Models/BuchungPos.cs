namespace Tool2Go.Models
{
    /// <summary>
    /// Repräsentiert eine einzelne Buchungsposition innerhalb einer Buchung.
    /// 
    /// Eine Buchung kann aus mehreren Buchungspositionen bestehen,
    /// z.B. wenn der Kunde mehrere verschiedene Werkzeuge (oder denselben Typ in mehrfacher Menge) bucht.
    /// </summary>
    public class BuchungPos
    {
        /// <summary>
        /// Werkzeuge, die in dieser Position gebucht werden.
        /// </summary>
        public List<Werkzeug> Werkzeuge { get; set; } = new();

        /// <summary>
        /// Zeitraum in dem das Werkzeug ausgeliehen werden soll.
        /// </summary>
        public DateTime Startdatum { get; set; }
        public DateTime Enddatum { get; set; }

        /// <summary>
        /// Berechnet die Kosten für diese Buchungsposition.
        /// </summary>
        public decimal BerechnePosKosten(DateTime start, DateTime ende, List<Werkzeugkategorie> kategorien)
        {
            int tage = (ende - start).Days + 1;
            decimal gesamt = 0;

            foreach (var w in Werkzeuge)
            {
                var kat = kategorien.FirstOrDefault(k => k.Id == w.KategorieId);
                if (kat == null) continue;

                gesamt += kat.LeihgebuehrProTag * tage;
            }

            return gesamt;
        }
    }
}
