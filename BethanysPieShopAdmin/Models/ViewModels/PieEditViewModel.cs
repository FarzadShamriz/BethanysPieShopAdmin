using BethanysPieShopAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BethanysPieShopAdmin.Models.ViewModels
{
    public class PieEditViewModel
    {
        public IEnumerable<SelectListItem>? Categories { get; set; } = default!;
        public Pie Pie { get; set; } 
    }
}