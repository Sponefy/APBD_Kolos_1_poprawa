namespace Kolos_1_poprawa.Dtos;

public class AddRentalDto
{
    public AddClientDto Client { get; set; }
    
    public int CarID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}