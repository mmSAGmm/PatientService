namespace PatientService.ResponseModels
{
    public class GetPatientResponseModel
    {
        public Guid Id { get; set; }
        public string Use { get; set; }
        public string Family { get; set; }
        public string[] Given { get; set; }
        public int Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }
}
