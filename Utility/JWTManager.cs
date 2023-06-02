using System.Text;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using Microsoft.IdentityModel.Tokens;
using MVCProject.Database.Models;
using Newtonsoft.Json;

namespace MVCProject.Utility;

public static class JwtManager {
    // TODO: Move to config file
    private const string SECRET = "JWTSecret!";

    public static string GenerateJwtToken(User user) {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm()) // TODO: Change to RSA?
            .WithSecret(Encoding.ASCII.GetBytes(SECRET))
            .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
            .AddClaim("user", user.user_id)
            .AddClaim("api_key", user.api_token)
            .Encode();
    }

    public static JwtPayload ExtractPayload(string token) {
        var payload = Base64UrlEncoder.Decode(token.Split(".")[1]);
        return JsonConvert.DeserializeObject<JwtPayload>(payload); 
    }

    public static bool IsValid(string token) {
        try {
            new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // TODO: Change to RSA?
                .WithSecret(Encoding.ASCII.GetBytes(SECRET))
                .MustVerifySignature()
                .Decode(token);
            return true;
        } catch (SignatureVerificationException e) {
            return false;
        }
    }
    
    public class JwtPayload {
        public string user { get; set; }
        public string api_key { get; set; }
        public string exp { get; set; }
    }
}