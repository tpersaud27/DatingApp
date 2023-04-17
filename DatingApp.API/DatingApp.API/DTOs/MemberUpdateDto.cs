namespace DatingApp.Domain.DTOs
{
    /// <summary>
    /// These are the fields in the form the user will use to update their profile
    /// </summary>
    public class MemberUpdateDto
    {

        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }


    }
}
