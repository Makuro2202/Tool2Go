using System.Text.RegularExpressions;

namespace Tool2Go.Utils
{
    /// <summary>
    /// Statische Hilfsklasse zur Validierung von deutschen IBANs.
    /// </summary>
    public static class IbanValidator
    {
        /// <summary>
        /// Prüft, ob die IBAN formal und logisch korrekt ist (DE-Format + Mod-97).
        /// </summary>
        public static bool IstGueltig(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
            {
                return true; // Leere IBAN ist erlaubt → Barzahlung
            }

            var cleanIban = iban.Replace(" ", "").ToUpper();

            if (!Regex.IsMatch(cleanIban, @"^DE\d{20}$"))
            {
                return false;
            }

            return Mod97Pruefung(cleanIban) == 1;
        }

        /// <summary>
        /// Führt die Modulo-97-Prüfziffernprüfung durch.
        /// </summary>
        private static int Mod97Pruefung(string iban)
        {
            var rearranged = iban.Substring(4) + iban.Substring(0, 4);

            var numericIban = string.Empty;
            foreach (char c in rearranged)
            {
                numericIban += char.IsLetter(c)
                    ? (c - 'A' + 10).ToString()
                    : c.ToString();
            }

            // Rechne mod 97 auf langen String
            int rest = 0;
            foreach (char digit in numericIban)
            {
                rest = (rest * 10 + int.Parse(digit.ToString())) % 97;
            }

            return rest;
        }
    }
}
