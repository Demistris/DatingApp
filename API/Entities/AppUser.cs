namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }
    // TODO: Add those properties
    // public required DateOnly DateOfBirth { get; set; }
    // public Gender Gender { get; set; }
    // PhoneNumber?
}

// public enum Gender
// {
//     Man,
//     Woman,
//     NonBinary,
//     TransMan,
//     TransWoman,
//     Genderfluid,
//     Other,
//     PreferNotToSay
// }