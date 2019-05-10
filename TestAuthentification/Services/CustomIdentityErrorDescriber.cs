
using Microsoft.AspNetCore.Identity;

namespace TestAuthentification.Services
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = "Error", Description = $"An unknown failure has occurred." }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = "Error", Description = "Optimistic concurrency failure, object has been modified." }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = "Error", Description = "Mot de passe incorrect." }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = "Error", Description = "Jeton d'authentification non valide." }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = "Error", Description = "Un utilisateur avec ce login existe déja." }; }
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = "Error", Description = $"L'utilisateur '{userName}' est invalide, il ne peut contenir que des lettres ou des chiffres." }; }
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = "Error", Description = $"L'email '{email}' est invalide" }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = "Error", Description = $"Le nom '{userName}' est déja pris." }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = "Error", Description = $"L'email '{email}' est déja pris."}; }
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = "Error", Description = $"Le role '{role}' est invalide." }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = "Error", Description = $"Le role '{role}' is already taken." }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = "Error", Description = "L'utilisateur à déja un password." }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = "Error", Description = "Lockout is not enabled for this user." }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = "Error", Description = $"L'utilsateur est déja dans le rôle'{role}'." }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = "Error", Description = $"L'utilisateur n'est pas dans le role '{role}'." }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = "Error", Description = $"Les mots de passe doievent comporter au moins {length} caractères." }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = "Error", Description = "Les mots de passe doivent comporter au moins un caractère non alphanumérique." }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = "Error", Description = "Les mots de passe doivent comporter au moins un chiffre('0'-'9')." }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = "Error", Description = "Les mots de passe doivent comporter au moins une minuscule ('a'-'z')." }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = "Error", Description = "Les mots de passe doivent comporter au moins une majuscule ('A'-'Z')." }; }
        public IdentityError InvalidPhoneNumber(string userPhone) { return new IdentityError { Code = "Error", Description = $"Le numéro de téléphone '{userPhone}' est déja pris." }; }
    }
}
