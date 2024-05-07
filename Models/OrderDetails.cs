using System.ComponentModel.DataAnnotations;

namespace hm_4_2.Models
{ 
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } // mpn
        public int OrderID { get; set; }
        public int Amount { get; set; }
    }
}
