namespace CollegeUni.Services.Models
{
    public interface IServiceRequest
    {
        int? Id { get; set; }
        Utilities.Enumeration.ResolveStrategy ConflictStrategy { get; set; }
        byte[] RowVersion { get; set; }
    }
}
