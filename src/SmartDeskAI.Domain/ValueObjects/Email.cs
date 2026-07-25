using System.Text.RegularExpressions;

namespace SmartDeskAI.Domain.ValueObjects
{
    /// <summary>
    /// Représente une adresse email validée et normalisée au sein du domaine (ValueObject).
    /// Immuable, elle garantit qu'aucune adresse email invalide ne peut circuler dans le système.
    /// </summary>
    public sealed class Email
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get;  }

        private Email (string value) => Value = value;

        public static Email Create(string rawEmail)
        {
            if (string.IsNullOrWhiteSpace(rawEmail))
                throw new ArgumentException("L'email ne peut pas être vide.", nameof(rawEmail));

            var normalized = rawEmail.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(normalized))
                throw new ArgumentException($"'{rawEmail}' n'est pas un email valide.", nameof(rawEmail));

            return new Email (normalized);
        }

        public bool Equals(Email? other) => other is not null && Value == other.Value;
        public override bool Equals(object? obj) => Equals(obj as Email);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
    }
}
