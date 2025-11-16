using Tool2Go.Interfaces;
using Tool2Go.Models;
using Tool2Go.Services;
using Tool2Go.Utils;

namespace Tool2Go
{
    public class Program
    {
        private static IVerwaltbar<Kunde>? kundenService;
        private static IVerwaltbar<Werkzeug>? werkzeugService;
        private static IVerwaltbar<Werkzeugkategorie>? werkzeugkategorieService;
        private static IVerwaltbar<Buchung>? buchungService;

        private static WerkzeugService? werkzeugServiceImpl;
        private static WerkzeugKategorieService? werkzeugkategorieServiceImpl;
        private static BuchungService? buchungServiceImpl;

        public static void Main(string[] args)
        {
           Console.OutputEncoding = System.Text.Encoding.UTF8;

            InitialisiereDaten();

            bool exit = false;

            while (!exit)
            {
                ZeigeMenue();

                string eingabe = Console.ReadLine()!;
                Console.WriteLine();

                switch (eingabe)
                {
                    case "1":
                        kundenService!.Anzeigen();
                        break;
                    case "2":
                        kundenService!.Hinzufuegen();
                        Speichern();
                        break;
                    case "3":
                        kundenService!.Loeschen();
                        Speichern();
                        break;
                    case "4":
                        kundenService!.Bearbeiten();
                        Speichern();
                        break;

                    case "5":
                        werkzeugkategorieService!.Anzeigen();
                        break;
                    case "6":
                        werkzeugkategorieService!.Hinzufuegen();
                        Speichern();
                        break;
                    case "7":
                        werkzeugkategorieService!.Loeschen();
                        Speichern();
                        break;
                    case "8":
                        werkzeugkategorieService!.Bearbeiten();
                        Speichern();
                        break;

                    case "9":
                        werkzeugService!.Anzeigen();
                        break;
                    case "10":
                        werkzeugService!.Hinzufuegen();
                        Speichern();
                        break;
                    case "11":
                        werkzeugService!.Loeschen();
                        Speichern();
                        break;
                    case "12":
                        werkzeugService!.Bearbeiten();
                        Speichern();
                        break;

                    case "13":
                        buchungService!.Anzeigen();
                        break;
                    case "14":
                        buchungService!.Hinzufuegen();
                        Speichern();
                        break;
                    case "15":
                        buchungService!.Loeschen();
                        Speichern();
                        break;
                    case "16":
                        buchungService!.Bearbeiten();
                        Speichern();
                        break;

                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("❌ Ungültige Auswahl.");
                        break;
                }
            }

            Speichern();
            Console.WriteLine("Programm beendet. Daten wurden gespeichert.");
        }

        private static void ZeigeMenue()
        {
            Console.WriteLine("\n===== Tool2Go - Hauptmenü =====");
            Console.WriteLine("1  - Kunden anzeigen");
            Console.WriteLine("2  - Kunde hinzufügen");
            Console.WriteLine("3  - Kunde löschen");
            Console.WriteLine("4  - Kunde bearbeiten\n");

            Console.WriteLine("5  - Werkzeugkategorien anzeigen");
            Console.WriteLine("6  - Werkzeugkategorie hinzufügen");
            Console.WriteLine("7  - Werkzeugkategorie löschen");
            Console.WriteLine("8  - Werkzeugkategorie bearbeiten\n");

            Console.WriteLine("9  - Werkzeuge anzeigen");
            Console.WriteLine("10 - Werkzeug hinzufügen");
            Console.WriteLine("11 - Werkzeug löschen");
            Console.WriteLine("12 - Werkzeug bearbeiten\n");

            Console.WriteLine("13 - Buchungen anzeigen");
            Console.WriteLine("14 - Buchung hinzufügen");
            Console.WriteLine("15 - Buchung löschen");
            Console.WriteLine("16 - Buchung bearbeiten\n");

            Console.WriteLine("0  - Beenden\n");
            Console.Write("Auswahl: ");
        }

        private static void RekonstruiereAlleKategorieZuordnungen(List<Werkzeugkategorie> kategorien, List<Buchung> buchungen)
        {
            // 1. IDs den Werkzeugen in den Kategorien zuweisen
            foreach (var kategorie in kategorien)
            {
                foreach (var werkzeug in kategorie.Werkzeuge)
                {
                    werkzeug.KategorieId = kategorie.Id;
                }
            }

            // 2. IDs den Werkzeugen in den Buchungspositionen zuweisen
            foreach (var buchung in buchungen)
            {
                foreach (var pos in buchung.Positionen)
                {
                    foreach (var werkzeug in pos.Werkzeuge)
                    {
                        var gefundeneKat = kategorien.FirstOrDefault(k =>
                            k.Werkzeuge.Any(w =>
                                w.Hersteller == werkzeug.Hersteller &&
                                w.Modell == werkzeug.Modell &&
                                w.TechnischeDaten == werkzeug.TechnischeDaten
                            )
                        );

                        if (gefundeneKat != null)
                        {
                            werkzeug.KategorieId = gefundeneKat.Id;
                        }
                    }
                }
            }
        }

        private static void InitialisiereDaten()
        {
            var kunden = XmlHelper.Laden<List<Kunde>>("kunden.xml") ?? new List<Kunde>();
            var kategorien = XmlHelper.Laden<List<Werkzeugkategorie>>("kategorien.xml") ?? new List<Werkzeugkategorie>();
            var buchungen = XmlHelper.Laden<List<Buchung>>("buchungen.xml") ?? new List<Buchung>();
            RekonstruiereAlleKategorieZuordnungen(kategorien, buchungen);

            // Kunden erstellen, falls noch keine vorhanden
            if (kunden.Count == 0)
            {
                kunden.Add(new Kunde
                {
                    Vorname = "Erwachsener",
                    Nachname = "Kunde",
                    Adresse = "Altstraße 10, 12345 Berlin",
                    Geburtsdatum = new DateTime(1990, 1, 1),
                    Iban = "DE44500105175407324931"
                });

                kunden.Add(new Kunde
                {
                    Vorname = "Junger",
                    Nachname = "Kunde",
                    Adresse = "Jungstraße 20, 54321 Köln",
                    Geburtsdatum = DateTime.Today.AddYears(-17), // unter 21
                    Iban = null
                });
            }

            // Kategorien + Werkzeuge erstellen, wenn leer
            if (kategorien.Count == 0)
            {
                // Kategorie 1: Bagger
                var kat1 = new Werkzeugkategorie("Bagger", 90, 500, true);
                kat1.Werkzeuge.Add(new Werkzeug("Caterpillar", "301.7D", "1,7t, Diesel, Tieflöffel")); 
                kat1.Werkzeuge.Add(new Werkzeug("Caterpillar", "301.7D", "1,7t, Diesel, Tieflöffel")); 
                kat1.Werkzeuge.Add(new Werkzeug("Caterpillar", "301.7D", "1,8t, Diesel, Tieflöffel")); 
                kat1.Werkzeuge.Add(new Werkzeug("Hitachi", "ZX10U", "1,1t, Diesel, Tieflöffel"));       

                // Kategorie 2: Bohrhammer
                var kat2 = new Werkzeugkategorie("Bohrhammer", 30, 150, false);
                kat2.Werkzeuge.Add(new Werkzeug("Bosch", "X3", "500W, SDS-plus"));
                kat2.Werkzeuge.Add(new Werkzeug("Bosch", "X3", "600W, SDS-plus")); 
                kat2.Werkzeuge.Add(new Werkzeug("Bosch", "X3", "500W, SDS-plus"));
                kat2.Werkzeuge.Add(new Werkzeug("Makita", "BHR202", "18V, Akku"));

                // Kategorie 3: Rüttelplatte
                var kat3 = new Werkzeugkategorie("Rüttelplatte", 40, 200, true);
                kat3.Werkzeuge.Add(new Werkzeug("Wacker Neuson", "VP1550", "90kg, Benzin"));
                kat3.Werkzeuge.Add(new Werkzeug("Wacker Neuson", "VP1550", "90kg, Benzin"));
                kat3.Werkzeuge.Add(new Werkzeug("Bomag", "BVP 18/45", "108kg, Benzin"));
                kat3.Werkzeuge.Add(new Werkzeug("Bomag", "BVP 18/45", "110kg, Benzin")); 

                // 🔴 Kategorie 4: Winkelschleifer
                var kat4 = new Werkzeugkategorie("Winkelschleifer", 25, 120, false);
                kat4.Werkzeuge.Add(new Werkzeug("Bosch", "GWS 7-125", "720W, 125mm"));
                kat4.Werkzeuge.Add(new Werkzeug("Bosch", "GWS 7-125", "720W, 125mm"));
                kat4.Werkzeuge.Add(new Werkzeug("Einhell", "TE-AG 125", "850W, Softstart"));
                kat4.Werkzeuge.Add(new Werkzeug("Makita", "GA5030", "720W, 125mm"));

                kategorien.AddRange(new[] { kat1, kat2, kat3, kat4 });
            }

            kundenService = new KundenService(kunden);
            var werkzeugKategorieService = new WerkzeugKategorieService(kategorien);
            werkzeugServiceImpl = new WerkzeugService(kategorien);
            IVerwaltbar<Werkzeugkategorie> werkzeugKategorieVerwaltbar = werkzeugKategorieService;
            werkzeugService = werkzeugServiceImpl;
            werkzeugkategorieServiceImpl = new WerkzeugKategorieService(kategorien);
            werkzeugkategorieService = werkzeugkategorieServiceImpl;
            buchungServiceImpl = new BuchungService(kundenService, werkzeugKategorieVerwaltbar, werkzeugService!, buchungen);
            buchungService = buchungServiceImpl;
        }

        private static void Speichern()
        {
            XmlHelper.Speichern(((KundenService)kundenService!).GetElemente(), "kunden.xml");
            foreach (var kategorie in werkzeugkategorieServiceImpl!.GetElemente())
            {
                foreach (var werkzeug in kategorie.Werkzeuge)
                {
                    werkzeug.KategorieId = kategorie.Id;
                }
            }
            XmlHelper.Speichern(werkzeugkategorieServiceImpl!.GetElemente(), "kategorien.xml");
            XmlHelper.Speichern(buchungServiceImpl!.GetElemente(), "buchungen.xml");
        }
    }
}
