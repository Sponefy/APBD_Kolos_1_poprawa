namespace Kolos_1_poprawa.Dtos;

public class ClientDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    
    public List<RentalDto> Rentals { get; set; }
}