using System.Security.Claims;
using System;
using Coffeecard.Models;
using System.Linq;
using coffeecard.Services;
using coffeecard.Helpers;
using System.Collections.Generic;
using coffeecard.Models.DataTransferObjects.User;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class AccountService : IAccountService
{
    private readonly CoffeecardContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IHashService _hashService;
    private readonly IPurchaseService _purchaseService;

    public AccountService(CoffeecardContext context, ITokenService tokenService, IEmailService emailService, IHashService hashService, IPurchaseService purchaseService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _hashService = hashService;
        _purchaseService = purchaseService;
    }

    public User GetAccountByEmail(string email)
    {
        var user = _context.Users
            .Include(x => x.Programme)
            //.Include(x => x.Statistics)
            .FirstOrDefault(x => x.Email == email);
        if (user == null) throw new ApiException("No user found with the given email", 401);
        return user;
    }

    public string Login(string username, string password, string version)
    {
        Log.Information($"Logging in user with username: {username} version: {version}");
        var user = _context.Users.FirstOrDefault(x => x.Email == username);
        if (user != null)
        {
            var hashedPw = _hashService.Hash(password + user.Salt);
            if (user.Password.Equals(hashedPw))
            {
                var claims = new Claim[] { new Claim(ClaimTypes.Email, username), new Claim(ClaimTypes.Name, user.Name), new Claim("UserId", user.Id.ToString()) };
                var token = _tokenService.GenerateToken(claims);

                // check for incomplete purchases
                _purchaseService.CheckIncompletePurchases(user);

                return token;
            }
        }
        throw new ApiException("The username or password does not match", 401);
    }

    public User RegisterAccount(RegisterDTO registerDto)
    {
        Log.Information($"Trying to register new user. Name: {registerDto.Name} Email: {registerDto.Email} ProgrammeId: {registerDto.ProgrammeId}");
        if (_context.Users.Any(x => x.Email == registerDto.Email)) throw new ApiException($"The email {registerDto.Email} is already being used by another user", 400);
        var salt = _hashService.GenerateSalt();
        var hashedPassword = _hashService.Hash(registerDto.Password + salt);

        var programme = _context.Programmes.FirstOrDefault(x => x.Id == registerDto.ProgrammeId);
        if (programme == null) throw new ApiException($"No programme found with the id: {registerDto.ProgrammeId}", 400);

        var user = new User { Name = EscapeName(registerDto.Name), Email = registerDto.Email, Password = hashedPassword, Salt = salt, Programme = programme };

        _context.Users.Add(user);
        if (_context.SaveChanges() == 0) throw new ApiException($"The user could not be created - try again in a minute", 500);

        var claims = new Claim[] { new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Role, "verification_token") };
        var verificationToken = _tokenService.GenerateToken(claims);

        _emailService.SendRegistrationVerificationEmail(user, verificationToken);

        return user;
    }

    public bool VerifyRegistration(string token)
    {
        Log.Information($"Trying to verify registration with token: {token}");
        var jwtToken = _tokenService.ReadToken(token);
        if (!jwtToken.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == "verification_token")) throw new ApiException($"The token is invalid!", 400);
        var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (emailClaim == null) throw new ApiException($"The token is invalid!", 400);
        var user = _context.Users.FirstOrDefault(x => x.Email == emailClaim.Value);
        if (user == null) throw new ApiException($"The token is invalid!", 400);
        user.IsVerified = true;
        return _context.SaveChanges() > 0;
    }

    public User UpdateAccount(IEnumerable<Claim> claims, UpdateUserDTO userDto)
    {
        var user = GetAccountByClaims(claims);

        if (userDto.Email != null)
        {
            if (_context.Users.Any(x => x.Email == userDto.Email)) throw new ApiException($"The email {userDto.Email} is already in use!", 400);
            Log.Information($"Changing email of user from {user.Email} to {userDto.Email}");
            user.Email = userDto.Email;
        }

        if (userDto.Name != null)
        {
            Log.Information($"Changing name of user from {user.Name} to {EscapeName(userDto.Name)}");
            user.Name = EscapeName(userDto.Name);
        }

        if (userDto.PrivacyActivated != null)
        {
            Log.Information($"Changing privacy of user from {user.PrivacyActivated} to {(bool)userDto.PrivacyActivated}");
            user.PrivacyActivated = (bool)userDto.PrivacyActivated;
        }

        if (userDto.ProgrammeId != null)
        {
            var programme = _context.Programmes.FirstOrDefault(x => x.Id == userDto.ProgrammeId);
            if (programme == null) throw new ApiException($"No programme with id {userDto.ProgrammeId} exists!", 400);
            Log.Information($"Changing programme of user from {user.Programme.Id} to {programme.Id}");
            user.Programme = programme;
        }

        if (userDto.Password != null)
        {
            var salt = _hashService.GenerateSalt();
            var hashedPassword = _hashService.Hash(userDto.Password + salt);
            user.Salt = salt;
            user.Password = hashedPassword;
            Log.Information($"User changed password");
        }

        _context.SaveChanges();
        return user;
    }

    private string EscapeName(string name)
    {
        return name.Trim(new Char[] { '<', '>', '{', '}' });
    }

    public User GetAccountByClaims(IEnumerable<Claim> claims)
    {
        var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        if (emailClaim == null) throw new ApiException($"The token is invalid!", 401);
        var user = GetAccountByEmail(emailClaim.Value);
        if (user == null) throw new ApiException($"The user could not be found", 400);


        return user;
    }

    public void UpdateExperience(int userId, int exp)
    {
        var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        if (user == null) throw new ApiException($"Could not update user", 500);
        user.Experience += exp;
        _context.SaveChanges();
    }

    public (int Total, int Semester, int Month) GetLeaderboardPlacement(User user)
    {
        var leaderBoardPlacement = (Total: 0, Semester: 0, Month: 0);

        var totalUsers = _context.Statistics.Include(x => x.User).Where(s => s.Preset == StatisticPreset.Total).OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
        var totalSwipeRank = totalUsers.FindIndex(x => x.User.Id == user.Id) + 1;

        var semesterUsers = _context.Statistics.Include(x => x.User).Where(s => s.Preset == StatisticPreset.Semester && Statistic.ValidateSemesterExpired(s.LastSwipe)).OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
        var semesterSwipeRank = semesterUsers.FindIndex(x => x.User.Id == user.Id) + 1;

        var monthUsers = _context.Statistics.Include(x => x.User).Where(s => s.Preset == StatisticPreset.Monthly && Statistic.ValidateMonthlyExpired(s.LastSwipe)).OrderByDescending(x => x.SwipeCount).ThenBy(x => x.LastSwipe).ToList();
        var monthSwipeRank = monthUsers.FindIndex(x => x.User.Id == user.Id) + 1;

        leaderBoardPlacement.Total = totalSwipeRank;
        leaderBoardPlacement.Semester = semesterSwipeRank;
        leaderBoardPlacement.Month = monthSwipeRank;

        return leaderBoardPlacement;
    }
}