using System.Xml.Serialization;

namespace Tool2Go.Utils
{
    /// <summary>
    /// Hilfsklasse zur Serialisierung und Deserialisierung von Objekten im XML-Format.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Speichert ein beliebiges Objekt als XML-Datei.
        /// </summary>
        public static void Speichern<T>(T daten, string pfad)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var stream = new FileStream(pfad, FileMode.Create);
                serializer.Serialize(stream, daten);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Datei '{pfad}': {ex.Message}");
            }
        }

        /// <summary>
        /// Lädt ein Objekt vom angegebenen Pfad, wenn die XML-Datei existiert.
        /// </summary>
        public static T? Laden<T>(string pfad) where T : class
        {
            if (!File.Exists(pfad))
            {
                return null;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using var reader = new StreamReader(pfad);
                return serializer.Deserialize(reader) as T;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Datei '{pfad}': {ex.Message}");
                return null;
            }
        }
    }
}
