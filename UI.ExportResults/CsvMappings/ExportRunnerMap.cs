﻿using CsvHelper.Configuration;
using UI.ExportResults.Models;

namespace UI.ExportResults.CsvMappings
{
    public class ExportRunnerMap : ClassMap<ExportRunner>
    {
        public ExportRunnerMap()
        {
            Map(r => r.Rang).Name("Rang");
            Map(r => r.Startnummer).Name("Startnummer");
            Map(r => r.Vorname).Name("Vorname");
            Map(r => r.Nachname).Name("Nachname");
            Map(r => r.Zeit).Name("Zeit").ConvertUsing(r => r.Zeit.ToString(@"hh\:mm\:ss"));
            Map(r => r.Verein).Name("Verein");

            Map(r => r.Geburtsjahr).Ignore();
            Map(r => r.Geschlecht).Ignore();
        }
    }
}
