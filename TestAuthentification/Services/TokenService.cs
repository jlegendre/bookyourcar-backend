using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using TestAuthentification.Models;

namespace TestAuthentification.Services
{
    public class TokenService
    {

        public static bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Permet de générer un token
        /// Par défault (si rien n'est renseigné) il expire au bout de 10 minutes
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ExpireFromInMinutes"></param>
        /// <param name="ExpireFromInHours"></param>
        /// <returns></returns>
        public static string GenerateToken(User user, int? ExpireFromInMinutes = null, int? ExpireFromInHours = null)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A5DeveloppeurSecureKey"));
            SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            BookYourCarContext _context = new BookYourCarContext();
            user.UserRight = _context.Right.FirstOrDefault(x => x.RightId == user.UserRightId);

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.UserRight.RightLabel),
            };

        JwtSecurityToken tokeOptions = new JwtSecurityToken(
            issuer: "http://localhost:5000",
            audience: "http://localhost:5000",
            claims: claims,
            // Si ExpireFromInMinutes est renseigné 
            //      on prends ça valeur pour l'expiration du token
            // Sinon si ExpireFromInHours est renseigné 
            //      on prend ça valeur pour l'expiration du token
            // Sinon 
            //      On met 10 minutes par default
            expires: ExpireFromInMinutes.HasValue ? DateTime.Now.AddMinutes(ExpireFromInMinutes.Value).ToLocalTime() : ExpireFromInHours.HasValue ? DateTime.Now.AddHours(ExpireFromInHours.Value).ToLocalTime() : DateTime.Now.AddMinutes(10).ToLocalTime(),
            //expires: DateTime.Now.AddMinutes(ExpireFrom).ToLocalTime(),
            signingCredentials: signinCredentials
        );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
    }


    public static bool VerifDateExpiration(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var simplePrinciple = handler.ReadJwtToken(token);

        // si la date d'expiration est toujours valide

        return DateTime.Now < simplePrinciple.ValidTo.ToLocalTime();

    }

    public static bool ValidateTokenWhereIsAdmin(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        SecurityToken validatedToken;
        try
        {
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            var handler = new JwtSecurityTokenHandler();
            var simplePrinciple = handler.ReadJwtToken(authToken);
            var role = simplePrinciple.Claims.First(x => x.Type == ClaimTypes.Role).Value;

            if (role.Equals("Admin"))
            {
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidateLifetime = true, // Because there is no expiration in the generated token
            ValidateAudience = false, // Because there is no audiance in the generated token
            ValidateIssuer = false,   // Because there is no issuer in the generated token
            ValidIssuer = "Sample",
            RequireExpirationTime = true,
            ValidAudience = "Sample",
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("A5DeveloppeurSecureKey")) // The same key as the one that generate the token
        };
    }
}
}
