# ADR-007 : Authentification maison (mot de passe + JWT) plutôt qu'ASP.NET Core Identity

- **Statut** : accepté
- **Date** : Juillet 2026

## Contexte
Besoin d'authentifier les utilisateurs. ASP.NET Core Identity a été envisagé pour sa simplicité mais sa classe `IdentityUser` ne peut pas être intégrée au Domain sans violer la Clean Architecture (le Domain doit rester indépendant de toute bibliothèque tierce).

## Options considérées
- **Option 1** : faire hériter l'entité `User` du Domain d'`IdentityUser`, simple à mettre en place mais pollue le Domain avec une dépendance technique tierce.
- **Option 2** : garder `IdentityUser` et l'entité `User` du Domain séparés, synchronisés par un `Id` commun qui préserve la pureté du DOmain mais ajoute de la complexité de synchronisation sans réduire le travail total.
- **Option 3** : authentification maison (mot de pass hashé + JWT généré par le projet) sans dépendance à ASP.NET Core Identity.

## Décision
Option 3. Si la préservation de l'architecture impose de toute façon une gestion manuelle du lien entre identité technique et entité métier (Option 2), autant assumer une solution entièrement maîtrisée plutôt que de garder la dépendance à Identity pour un gain de simplicité qui disparaît en pratique.

## Conséquences
- `Domain.Entities.User` reste inchangé, aucune notion d'authentification n'y est introduite.
- Une entité technique `UserCredential` (uniquement en Infrastructure) porte `UserId` (FK) et `PasswordHash`.
- Deux interfaces côté Application : `IPasswordHasher` (hash/vérification, implémenté via BCrypt) et `IJwtTokenGenerator` (génération du JWT avec `UserId`, `TenantId`, `Role` en claims).
- Plus de code à écrire et maintenir soi-même (hashage, génération/ validation de JWT) en échange d'une maîtrise complète du mécanisme et d'aucune dépendance technique dans le Domain.