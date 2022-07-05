namespace Practice.Models
{
    public interface IUserInfo
    {
        string? FirstName { get; set; }
        string? LastName { get; set; }
        string? Email { get; set; }
        int? Age { get; set; }
    }
}
