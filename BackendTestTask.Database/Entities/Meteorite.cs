using BackendTestTask.Services.Services.Generic.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackendTestTask.Database.Entities
{
    public class Meteorite : IEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string Class { get; set; } = default!;

        public double Mass { get; set; }

        public int Year { get; set; }

        public string Fall { get; set; } = default!;

        public double? Reclat { get; set; }

        public double? Reclong { get; set; }

        public string? GeoType { get; set; }

        public double? GeoLongitude { get; set; }

        public double? GeoLatitude { get; set; }
    }
}
