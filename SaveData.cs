using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace NietGrappigNetwerkDing
{
    internal class SaveData
    {
        public string Naam { get; set; }
        public string Bericht { get; set; }
        public string IPaanvanger { get; set; }

        public SaveData()
        {
            Naam = string.Empty;
            Bericht = string.Empty;
            IPaanvanger = string.Empty;
        }

        public void Save()
        {
            //Maak de path
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "woorddoorgeefsysteem");
            var filePath = Path.Combine(path, "savedata.json");

            // maak directory als die nog niet bestaat
            Directory.CreateDirectory(path);

            // maak of overschrijf een bestand
            var writer = File.CreateText(filePath);

            // voeg een optie toe om het bestand leesbaar te houden
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // schrijf content van library naar bestand
            writer.Write(JsonSerializer.Serialize(this, options));

            // sla het bestand op
            writer.Flush();
        }
    }
}
