using EnterpriseApp.Application.Dtos;
using EnterpriseApp.Application.Interfaces;
using EnterpriseApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IDgiiService _dgiiService;

        public CompanyController(ICompanyRepository companyRepository, IDgiiService dgiiService)
        {
            _companyRepository = companyRepository;
            _dgiiService = dgiiService;
        }

        [HttpGet]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _companyRepository.ListAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _companyRepository.GetByIdAsync(id, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("dgii-lookup/{rnc}")]
        public async Task<IActionResult> DgiiLookup([FromRoute] string rnc, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _dgiiService.GetByRncAsync(rnc, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _companyRepository.CreateCompanyAsync(dto, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCompanyDto dto, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _companyRepository.UpdateAsync(id, dto, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _companyRepository.DeleteAsync(id, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
