namespace Tool2Go.Interfaces
{
    /// <summary>
    /// Definiert generische Verwaltungsfunktionen für ein Objekt.
    /// </summary>
    public interface IVerwaltbar<T>
    {
        void Hinzufuegen();
        void Bearbeiten();
        void Loeschen();
        void Anzeigen();
        List<T> GetElemente();
    }
}
