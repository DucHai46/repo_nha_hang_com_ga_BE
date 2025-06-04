using Microsoft.AspNetCore.Mvc;
using repo_nha_hang_com_ga_BE.Repository;
using Microsoft.AspNetCore.Authorization;
using repo_nha_hang_com_ga_BE.Models.Requests;

namespace repo_nha_hang_com_ga_BE.Controllers;



[ApiController]
[Route("api/phu-phi")]


public class PhuPhiController : ControllerBase
{
    private readonly IPhuPhiRepository _PhuPhiRepository;

    public PhuPhiController(IPhuPhiRepository PhuPhiRepository)
    {
        _PhuPhiRepository = PhuPhiRepository;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllPhuPhis([FromQuery] RequestSearchPhuPhi request)
    {
        return Ok(await _PhuPhiRepository.GetAllPhuPhis(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhuPhiById(string id)
    {
        return Ok(await _PhuPhiRepository.GetPhuPhiById(id));
    }
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreatePhuPhi(RequestAddPhuPhi request)
    {
        return Ok(await _PhuPhiRepository.CreatePhuPhi(request));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePhuPhi(string id, RequestUpdatePhuPhi request)
    {
        return Ok(await _PhuPhiRepository.UpdatePhuPhi(id, request));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhuPhi(string id)
    {
        return Ok(await _PhuPhiRepository.DeletePhuPhi(id));
    }
}
