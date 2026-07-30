namespace SmartDeskAI.Application.Users.Queries.Common;

/// <summary>
/// Objet de transfert de données (DTO) représentant un utilisateur destiné à l'exposition externe (API, UI).
/// </summary>
/// <param name="Id">L'identifiant unique (<see cref="Guid"/>) de l'utilisateur.</param>
/// <param name="Email">L'adresse email formatée sous forme de chaîne de caractères.</param>
/// <param name="Role">Le rôle métier sous forme de texte (ex: "Admin", "Agent", "Customer").</param>
/// <param name="Status">Le statut du compte sous forme de texte (ex: "Invited", "Active", "Deactivated").</param>
/// <param name="CreatedAt">La date et l'heure UTC de création du compte.</param>
public sealed record UserDto(Guid Id, string Email, string Role, string Status, DateTime CreatedAt);