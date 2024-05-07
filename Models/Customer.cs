using System.ComponentModel.DataAnnotations;

namespace hm_4_2.Models;
public class Customer
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public override string ToString()
    {
        return $"name is: {Name}, surname is: {Surname}, email is: {Email}";
    }

}
