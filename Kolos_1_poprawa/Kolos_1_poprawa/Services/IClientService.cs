using Kolos_1_poprawa.Dtos;

namespace Kolos_2_poprawa.Services;

public interface IClientService
{
    Task<ClientDto?> GetClient(int clientId);
    Task<int> AddClient(AddRentalDto clientDto);
}