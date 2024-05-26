namespace PatientService.RequestModels
{
    public class BasePatientRequestModel
    {
        public string Use { get; set; }
        public string Family { get; set; }
        public string[] Given { get; set; }
        
        //todo: ToString
        public int Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
    }
}
