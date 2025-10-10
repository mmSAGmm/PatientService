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

        //todo: add swagger description
        //todo: vliadte pattern
        [HttpGet("search")]
        public async Task<IActionResult> Query(string[] pattern, CancellationToken token)
        {
            var result = await _patientBossService.Search(pattern, token);

            return Ok(result.Select(x => _mapper.Map<GetPatientResponseModel>(x)));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetPatientResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPatient(Guid id, CancellationToken token)
        {
            var model = await _patientBossService.Get(id, token);
            return model != null ? Ok(_mapper.Map<GetPatientResponseModel>(model)) : NoContent();
        }

        [HttpPost]
        [ProducesResponseType(typeof(GetPatientResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequestModel requestModel, CancellationToken token)
        {
            var patient = _mapper.Map<Patient.DomainModels.Patient>(requestModel);
            await _patientBossService.Add(patient, token);

            var model = _mapper.Map<GetPatientResponseModel>(patient);
            return Created(patient.Id.ToString(), model);
        }

        //TODO: api description
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdatePatient([FromBody] UpdatePatientRequestModel requestModel, CancellationToken token)
        {
            var patient = _mapper.Map<Patient.DomainModels.Patient>(requestModel);
            await _patientBossService.Update(patient, token);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePatient(Guid id, CancellationToken token)
        {
            await _patientBossService.Delete(id, token);
            return Ok();
        }
    }
}
