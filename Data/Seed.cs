using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    // This class stands to populate users from a JSON File
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            
            if (await context.Users.AnyAsync()) return;

            //On this piece code, we are serializing our user seed to migrate to 
            // our database

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);   
            if(users == null) return;   

            //On this loop, we are creating a password for each user from our seed data
            // from the json, because the password hasn't been implemented to our JSON File

            foreach( var user in users ) 
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }

            //Once the data has been added, it will be saved on our database.

            await context.SaveChangesAsync();

        }
    }
}
