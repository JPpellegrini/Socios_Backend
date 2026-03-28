namespace Socios.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        private Usuario() { } // EF

        public Usuario(string email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
