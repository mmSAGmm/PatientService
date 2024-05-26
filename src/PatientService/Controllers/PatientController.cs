using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Patient.Domain.Abstractions;
using PatientService.RequestModels;
using PatientService.ResponseModels;

namespace PatientService.Controllers
{
    [Route("api/v1/patient")]
    public class PatientController : Controller
    {
        private readonly IMapper _mapper;

        private readonly IPatientBossService _patientBossService;

        public PatientController(IMapper mapper, IPatientBossService patientBossService)
        {
            _mapper = mapper;
            _patientBossService = patientBossService;
        }

        [HttpGet("search/{pattern:string}")]
        public async Task<IActionResult> Query(string pattern)
        {

        }

        [HttpGet("/{id:guid}")]
        [ProducesResponseType(typeof(GetPatientResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPatient(Guid id)
        {
            var model = await _patientBossService.Get(id);
            return model != null ? Ok(_mapper.Map<GetPatientResponseModel>(model)) : NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(GetPatientResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequestModel requestModel)
        {
            var patient = _mapper.Map<Patient.DomainModels.Patient>(requestModel);
            await _patientBossService.Add(patient);

            var model = _mapper.Map<GetPatientResponseModel>(patient);
            return Created(patient.Id.ToString(), model);
        }

        //TODO: validation
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientRequestModel requestModel)
        {
            var patient = _mapper.Map<Patient.DomainModels.Patient>(requestModel);
            await _patientBossService.Update(patient);
            return Ok();
        }

        [HttpDelete("/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePatient(Guid id)
        {
            await _patientBossService.Delete(id);
            return Ok();
        }
    }
}
