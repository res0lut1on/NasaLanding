using BackendTestTask.Services.Models.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace BackendTestTask.Services.Models.Meteorite
{
    public class MeteoriteSearchModel : BaseSearchModel<string?>
    {
        [Range(0, 9999, ErrorMessage = "YearFrom должен быть положительным числом")]
        public int? YearFrom { get; set; }

        [Range(0, 9999, ErrorMessage = "YearTo должен быть положительным числом")]
        public int? YearTo { get; set; }

        public string? MeteoriteClass { get; set; }
        public string? SortBy { get; set; } 
        public bool? SortDescending { get; set; }
    }
}
