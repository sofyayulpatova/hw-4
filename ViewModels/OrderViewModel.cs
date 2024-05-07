using hm_4_2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace hm_4_2.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
       

    }
}
