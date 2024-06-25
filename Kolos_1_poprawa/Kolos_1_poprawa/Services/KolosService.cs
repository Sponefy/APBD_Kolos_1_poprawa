namespace Kolos_2_poprawa.Services;

public class KolosService : IKolosService
{
    private readonly IConfiguration _configuration;

    public KolosService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
}