using IvyScans.API.Data;
using IvyScans.API.Models;
using IvyScans.API.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IvyScans.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email and password are required."
                };
            }

            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Check if user exists and password is correct
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            try
            {
                // Generate JWT token
                var token = GenerateJwtToken(user);

                // Generate refresh token
                var refreshToken = await GenerateRefreshTokenAsync(user.Id);

                // Calculate reading stats safely
                var readingStats = user.Id != null ? CalculateReadingStats(user.Id) : new ReadingStatsDto();

                // Return successful response
                return new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        JoinDate = user.JoinDate,
                        Avatar = user.Avatar ?? GetDefaultAvatar(),
                        ReadingStats = readingStats
                    }
                };
            }
            catch (Exception ex)
            {
                // Log the exception (you should implement proper logging)
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");

                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email already in use"
                };
            }

            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username already taken"
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                JoinDate = DateTime.UtcNow,
                Avatar = GetDefaultAvatar()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token after successful registration
            var token = GenerateJwtToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            // Create user DTO with reading stats
            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                JoinDate = user.JoinDate,
                Avatar = user.Avatar,
                ReadingStats = CalculateReadingStats(user.Id)
            };

            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                RefreshToken = refreshToken,
                User = userDto
            };
        }

        public async Task LogoutAsync(string userId)
        {
            // Remove all refresh tokens for this user
            var refreshTokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow);

            if (storedToken == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                };
            }

            var user = storedToken.User;
            var newJwtToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Remove old refresh token
            _context.RefreshTokens.Remove(storedToken);

            // Add new refresh token
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            });

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                User = null // No need to return user details on refresh
            };
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                JoinDate = user.JoinDate,
                Avatar = user.Avatar,
                ReadingStats = CalculateReadingStats(userId)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("uid", user.Id),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            // Generate a unique refresh token
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(), // Explicitly set the Id
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            // Save the refresh token to the database
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }

        private string HashPassword(string password)
        {
            // In a real application, use a proper password hashing library like BCrypt
            // This is a simplified example
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // In a real application, use a proper password verification method
            // This is a simplified example
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToBase64String(hashedBytes);
            return hash == storedHash;
        }

        private string GetDefaultAvatar()
        {
            // Return a default avatar URL or path
            return "/assets/images/default-avatar.png";
        }

        private ReadingStatsDto CalculateReadingStats(string userId)
        {
            // Get all reading history for the user
            var history = _context.ReadingHistories
                .Where(h => h.UserId == userId)
                .ToList();

            // Get distinct comic IDs that the user has read
            var readComicIds = history.Select(h => h.ComicId).Distinct().ToList();

            // Count total unique comics read
            var totalRead = readComicIds.Count;

            // Get completed comics
            var completedComics = 0;
            foreach (var comicId in readComicIds)
            {
                var comic = _context.Comics
                    .Include(c => c.Chapters)
                    .FirstOrDefault(c => c.Id == comicId && c.Status == "Completed");

                if (comic != null)
                {
                    // Check if user has read all chapters of this comic
                    var readChaptersForComic = history.Count(h => h.ComicId == comicId);
                    if (comic.Chapters.Count <= readChaptersForComic)
                    {
                        completedComics++;
                    }
                }
            }

            return new ReadingStatsDto
            {
                TotalRead = totalRead,
                CurrentlyReading = totalRead - completedComics,
                CompletedSeries = completedComics,
                TotalChaptersRead = history.Count
            };
        }
    }
}