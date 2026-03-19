using NanoidDotNet;
using OrchestrationService.Domain;
using OrchestrationService.Shared;
using OrchestrationService.Model.Dtos;
using OrchestrationService.Model.Tokens;
using OrchestrationService.Repositories;
using OrchestrationService.Model.Requests;
using OrchestrationService.Services.RedisStore;
using OrchestrationService.Services.TokenProvider;
using OrchestrationService.Services.PasswordHashers;

namespace OrchestrationService.Services.Authorization;

public class UserService(
        IUserRepository userRepository,
        ITokenProvider tokenProvider,
        IPasswordHasher passwordHasher,
        IRedisRefreshTokenStore  redisStore) : IUserService
{
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IRedisRefreshTokenStore _redisStore = redisStore;

        public async Task<Result<TokensDto>> Login(LoginRequest loginRequest)
        {
                var user = await _userRepository.GetUserByUsername(loginRequest.Username);

                if (user is null || !_passwordHasher.Verify(loginRequest.Password, user.PasswordHash))
                        return Result<TokensDto>.Failure("Invalid credentials", ErrorStatus.Unauthorized);

                var accessToken = _tokenProvider.CreateAccessToken(user);

                var refreshToken = _tokenProvider.CreateRefreshToken(user);
                await _redisStore.SaveAsync(refreshToken);

                return Result<TokensDto>.Success(new TokensDto
                {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                });
        }

        public async Task<Result<AccessToken>> Register(RegisterRequest request, string role = "student")
        {
                if (string.IsNullOrWhiteSpace(request.Username) ||
                    await _userRepository.UserExists(request.Username))
                        return Result<AccessToken>.Failure("User already exists", ErrorStatus.Conflict);

                var hashed = _passwordHasher.Generate(request.Password);

                var user = new User
                {
                        Username = request.Username,
                        PasswordHash = hashed,
                        Role = role.ToLower() == "admin" ? Role.Admin : Role.Student,
                        PublicId = await GenerateUniquePublicId(),
                        Class = request.ClassName
                };

                await _userRepository.AddUser(user);

                var accessToken = _tokenProvider.CreateAccessToken(user);

                return Result<AccessToken>.Success(accessToken);
        }
        
        public async Task<string> GenerateUniquePublicId()
        {
                string id;
                do
                {
                        id = await Nanoid.GenerateAsync(Nanoid.Alphabets.LettersAndDigits, size: 21);
                } while (await _userRepository.UserExistsByPublicId(id));

                return id;
        }
}