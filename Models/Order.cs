using System.ComponentModel.DataAnnotations;

namespace hm_4_2.Models
{

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public string State { get; set; } = "New"; // def
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; } //  orders are linked to customers




        public Customer? Customer { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();


    }
}
