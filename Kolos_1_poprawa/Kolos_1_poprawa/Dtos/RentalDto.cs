namespace Kolos_1_poprawa.Dtos;

public class RentalDto
{
    public string Vin { get; set; }
    public string Color { get; set; }
    public string Model { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalPrice { get; set; }
}