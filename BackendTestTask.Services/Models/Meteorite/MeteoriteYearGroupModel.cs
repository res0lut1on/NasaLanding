using BackendTestTask.Services.Models.JournalEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTestTask.Services.Models.Meteorite
{
    public class MeteoriteYearGroupModel
    {
        public int Year { get; set; }
        public int TotalCount { get; set; }
        public double TotalMass { get; set; }
        public List<ResponseMeteorite> Meteorites { get; set; } = new();
    }
}
