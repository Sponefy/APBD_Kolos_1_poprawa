using Kolos_2_poprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolos_1_poprawa.Controllers;

[ApiController]
[Route("api/kolos")]
public class KolosController : ControllerBase
{
    private readonly IKolosService _service;

    public KolosController(IKolosService service)
    {
        _service = service;
    }
}