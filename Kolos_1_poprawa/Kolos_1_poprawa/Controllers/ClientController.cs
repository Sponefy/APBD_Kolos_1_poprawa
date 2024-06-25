using Kolos_1_poprawa.Dtos;
using Kolos_2_poprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolos_1_poprawa.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientController : ControllerBase
{
    private readonly IClientService _service;

    public ClientController(IClientService service)
    {
        _service = service;
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClient(int clientId)
    {
        var result = await _service.GetClient(clientId);

        if (result == null)
        {
            return BadRequest("Client does not exist");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(AddRentalDto rentalDto)
    {
        var result = _service.AddClient(rentalDto);

        if (result == null)
        {
            return BadRequest("Auto o takim Id nie istnieje");
        }
        
        return Ok();
    }
}