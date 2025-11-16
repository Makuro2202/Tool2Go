using System.Globalization;

/// <summary>
/// Statische Hilfsklasse zentralisiert den Umgang mit den Eingaben verschiedener Typen.
/// </summary>
public static class EingabeParser
{
    public record EingabeDefinition<T>(Func<string, (bool Erfolg, T Wert)> Parser, string Fehlermeldung);

    public static EingabeDefinition<int> Int => new(
        input => int.TryParse(input, out var val) ? (true, val) : (false, 0),
        "❌ Bitte geben Sie eine gültige ganze Zahl ein."
    );

    public static EingabeDefinition<decimal> Decimal => new(
    input =>
    {
        var normalisiert = input.Replace('.', ','); // Punkt zu Komma ersetzen, sonst wird es als 1000 erkannt (also aus 2.50 wird dann 25.000)
        return decimal.TryParse(normalisiert, NumberStyles.Number, CultureInfo.GetCultureInfo("de-DE"), out var val)
            ? (true, val)
            : (false, 0m);
    },
        "❌ Bitte geben Sie einen gültigen Betrag ein (z. B. 12,50 oder 12.50)."
    );

    public static EingabeDefinition<string> String => new(
        input => string.IsNullOrWhiteSpace(input) ? (false, "") : (true, input.Trim()),
        "❌ Der Text darf nicht leer sein."
    );

    public static EingabeDefinition<DateTime> Geburtsdatum => new(
    input =>
    {
        if (DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out var datum))
        {
            var alter = DateTime.Today.Year - datum.Year;
            if (datum > DateTime.Today.AddYears(-alter)) alter--; // Geburtstag noch nicht gehabt

            return (datum < DateTime.Today && alter >= 18) ? (true, datum) : (false, default);
        }
        return (false, default);
    },
    "❌ Bitte geben Sie ein gültiges Geburtsdatum ein (TT.MM.JJJJ) – mindestens 18 Jahre alt."
);

    public static EingabeDefinition<DateTime> Zukunftsdatum => new(
        input =>
        {
            if (DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out var datum))
            {
                return datum >= DateTime.Today ? (true, datum) : (false, default);
            }
            return (false, default);
        },
        "❌ Bitte geben Sie ein gültiges Datum ab heute ein (TT.MM.JJJJ)."
    );

    public static EingabeDefinition<bool> Bool => new(
        input => input.Trim().ToLower() switch
        {
            "j" => (true, true),
            "n" => (true, false),
            _ => (false, false)
        },
        "❌ Bitte geben Sie 'j' für Ja oder 'n' für Nein ein."
    );

    public static EingabeDefinition<string> StringMitAbbrechen => new(
    input =>
    {
        if (input.Trim().ToLower() is "zurück" or "abbrechen")
            throw new OperationCanceledException();
        return string.IsNullOrWhiteSpace(input) ? (false, "") : (true, input.Trim());
    },
    "❌ Der Text darf nicht leer sein."
);

    public static EingabeDefinition<int> IntMitAbbrechen => new(
        input =>
        {
            if (input.Trim().ToLower() is "zurück" or "abbrechen")
                throw new OperationCanceledException();
            return int.TryParse(input, out var val) ? (true, val) : (false, 0);
        },
        "❌ Bitte geben Sie eine gültige ganze Zahl ein."
    );

    public static EingabeDefinition<DateTime> ZukunftsdatumMitAbbrechen => new(
        input =>
        {
            if (input.Trim().ToLower() is "zurück" or "abbrechen")
                throw new OperationCanceledException();
            if (DateTime.TryParseExact(input, "dd.MM.yyyy", CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out var datum))
            {
                return datum >= DateTime.Today ? (true, datum) : (false, default);
            }
            return (false, default);
        },
        "❌ Bitte geben Sie ein gültiges Datum ab heute ein (TT.MM.JJJJ)."
    );
}
