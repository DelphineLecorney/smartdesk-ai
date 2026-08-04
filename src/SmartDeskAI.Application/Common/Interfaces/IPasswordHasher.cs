namespace SmartDeskAI.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string passwordHash);
}