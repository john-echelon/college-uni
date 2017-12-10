namespace CollegeUni.Services.Models
{
    public class ServiceRequest : IServiceRequest
    {
        public int? Id { get; set; }
        public byte[] RowVersion { get; set; }
        public Utilities.Enumeration.ResolveStrategy ConflictStrategy { get; set; } = Utilities.Enumeration.ResolveStrategy.StoreWins;
    }
}
