using Kieaa.Data;
using Kieaa.Dtos;
using Kieaa.IRepos;
using Kieaa.JwtClass;
using Kieaa.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Kieaa.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly Context _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;

        public UserRepo(Context context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            try
            {
                const int ExpiresDuration = 30;
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                    roleClaims.Add(new Claim("roles", role));

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
                .Union(userClaims)
                .Union(roleClaims);
                //if (string.IsNullOrEmpty(_jwt.Key))
                //{
                //    throw new ArgumentNullException(nameof(_jwt.Key), "JWT key cannot be null.");
                //}


                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Jwt:Issuer").Value!,
                    audience: _configuration.GetSection("Jwt:Audience").Value!,
                    claims: claims,
                    expires: DateTime.Now.AddDays(ExpiresDuration), // tset in minutes
                    signingCredentials: signingCredentials);

                return jwtSecurityToken;
            }
            catch
            {
                return null;
            }

        }
        // I create this because i cant convert from user to identity user
        private async Task<JwtSecurityToken> CreateJwtTokenIdentity(IdentityUser user)
        {
            try
            {
                const int ExpiresDuration = 30;
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = new List<Claim>();

                foreach (var role in roles)
                    roleClaims.Add(new Claim("roles", role));

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
                .Union(userClaims)
                .Union(roleClaims);
                //if (string.IsNullOrEmpty(_jwt.Key))
                //{
                //    throw new ArgumentNullException(nameof(_jwt.Key), "JWT key cannot be null.");
                //}


                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _configuration.GetSection("Jwt:Issuer").Value!,
                    audience: _configuration.GetSection("Jwt:Audience").Value!,
                    claims: claims,
                    expires: DateTime.Now.AddDays(ExpiresDuration), // tset in days
                    signingCredentials: signingCredentials);

                return jwtSecurityToken;
            }
            catch
            {
                return null;
            }
        }





        public async Task<IEnumerable<ViewUserDto>> GetAdminsAsync()
        {
            try
            {
                // Get the list of admin users
                var adminUsers = await _userManager.GetUsersInRoleAsync("ADMIN");

                // Extract the IDs of the admin users
                var adminUserIds = adminUsers.Select(u => u.Id).ToList();

                // Now query additional information from your _context.Users for those admin user IDs
                var users = await (from e in _context.Users
                                   where adminUserIds.Contains(e.Id)
                                   select new ViewUserDto
                                   {
                                       Id = e.Id,
                                       UserName = e.UserName,
                                       Email = e.Email,
                                       PhoneNumber = e.PhoneNumber,
                                       CreatedAt = e.CreatedAt,
                                       UpdatedAt = e.UpdatedAt,
                                       EmailConfirmed = e.EmailConfirmed,
                                       Role = new List<string> { "ADMIN" },
                                   }).ToListAsync();


                if (!users.Any())
                {
                    return Enumerable.Empty<ViewUserDto>();
                }

                return users;
            }
            catch
            {
                return Enumerable.Empty<ViewUserDto>();
            }
        }


        // maybe i have to change the return type from User to one have User and UserManager
        public async Task<ViewUserDto> GetUserByIdAsync(string userId)
        {
            try
            {
                //var uniqueFileName = await GetNniqueFileName(UserId);
                //var image = await GetImage(uniqueFileName);

                var obj = await (from e in _context.Users
                                 join p in _userManager.Users
                                 on e.Id equals p.Id
                                 where p.Id == userId
                                 select new ViewUserDto
                                 {
                                     Id = p.Id,
                                     UserName = p.UserName,
                                     Email = p.Email,
                                     PhoneNumber = p.PhoneNumber,
                                     CreatedAt = e.CreatedAt,
                                     UpdatedAt = e.UpdatedAt,
                                     EmailConfirmed = p.EmailConfirmed
                                 }).FirstOrDefaultAsync();
                if (obj != null)
                {
                    // Get the user roles using UserManager
                    var role = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));

                    // Include roles in the result
                    obj.Role = role.ToList(); // Assuming you have a Roles property in your UserDto
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string UserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserId);
                if (user == null)
                {
                    return Enumerable.Empty<string>();
                }

                var roleNames = await _userManager.GetRolesAsync(user);
                return roleNames;
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<ViewUserDto>> GetUsersAsync()
        {
            try
            {
                var obj = await (from e in _context.Users
                                 join p in _userManager.Users
                                 on e.Id equals p.Id
                                 select new ViewUserDto
                                 {
                                     Id = p.Id,
                                     UserName = p.UserName,
                                     Email = p.Email,
                                     PhoneNumber = p.PhoneNumber,
                                     CreatedAt = e.CreatedAt,
                                     EmailConfirmed = e.EmailConfirmed,
                                     UpdatedAt = e.UpdatedAt,
                                     //ProfileImage = e.ProfileImage
                                 }).ToListAsync();

                foreach (var user in obj)
                {
                    var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
                    user.Role = roles.ToList();
                }


                if (obj == null)
                {
                    return Enumerable.Empty<ViewUserDto>();
                }
                return (IEnumerable<ViewUserDto>)obj;
            }
            catch
            {
                return Enumerable.Empty<ViewUserDto>();
            }
        }

        public async Task<ViewUserDto> GetUserByEmailAsync(string Email)
        {
            try
            {
                var obj = await (from e in _context.Users
                                 join p in _userManager.Users
                                 on e.Id equals p.Id
                                 where p.Email == Email
                                 select new ViewUserDto
                                 {
                                     Id = p.Id,
                                     UserName = p.UserName,
                                     Email = p.Email,
                                     PhoneNumber = p.PhoneNumber,
                                     CreatedAt = e.CreatedAt,
                                     UpdatedAt = e.UpdatedAt,
                                     EmailConfirmed = p.EmailConfirmed
                                     //ProfileImage = image,
                                     //Bio = e.Bio,
                                     //Orders = e.Orders,
                                     //Reviews = e.Reviews,
                                     //Posts = e.Posts
                                 }).FirstOrDefaultAsync();
                if (obj != null)
                {
                    // Get the user roles using UserManager
                    var role = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(Email));

                    // Include roles in the result
                    obj.Role = role.ToList(); // Assuming you have a Roles property in your UserDto
                }
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<UserRefreshTokens> CreateUserAsync(CreateUserDto userDto)
        {
            if (await _userManager.FindByEmailAsync(userDto.Email) is not null)
            {
                return new UserRefreshTokens { Message = "Email is already Register!", IsAuthenticated = false };
                // isAuthenticated in Auth Model is false by default i handle it to understand code
            }
            //if (await _userManager.FindByNameAsync(userDto.UserName) is not null)
            //{
            //    return new UserRefreshTokens { Message = "UserName is already Register!", IsAuthenticated = false };
            //    // isAuthenticated in Auth Model is false by default i handle it to understand code
            //}

            //var uniqueFileName = await SaveUploadedFile(userDto.profileImage);
            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                //ProfileImage = uniqueFileName,
                //Bio = userDto.Bio,
                // this is for email validation
                CreatedAt = DateTime.Now.ToShortTimeString(),
                UpdatedAt= DateTime.Now.ToShortTimeString(),
                ValidationEmailToken = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(newUser, userDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                //DeleteFile(uniqueFileName);// delete image from system
                return new UserRefreshTokens { Message = errors, IsAuthenticated = false };
            }
            /*
            var uniqueFileName = await SaveUploadedFile(userDto.profileImage);
            _context.Attach(newUser);
            newUser.ProfileImage = uniqueFileName;
            await _context.SaveChangesAsync();
            */


            EmailDto email = new EmailDto()
            {
                To = newUser.Email,
                Subject = "Email Verification",
                Body = $"Please click the following link to verify your email: <a href=https://localhost:7110/api/user/verify?verificationToken={Uri.EscapeDataString(newUser.ValidationEmailToken)}>Verify Email</a>"
            };
            await _emailService.SendValidationEmailAsync(email);


            
            await _userManager.AddToRoleAsync(newUser, "USER");
            
            var jwtSecurityToken = await CreateJwtToken(newUser);
            var Uroles = new List<string>(await _userManager.GetRolesAsync(newUser));
            return new UserRefreshTokens
            {
                IsAuthenticated = true,
            };
        }
        // set user to admin
        public async Task<CheckFunc> SetAdminAsync(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return new CheckFunc { Message = $"User with ID {UserId} was not found." };
                }
                if (await _userManager.IsInRoleAsync(user, "ADMIN"))
                {
                    return new CheckFunc { Message = $"User is already admin." };
                }
                await _userManager.RemoveFromRoleAsync(user, "USER");
                await _userManager.AddToRoleAsync(user, "ADMIN");
                return new CheckFunc { IsSucceeded = true, Message = "Updated successfully" }; ;
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"Error while updating user : {ex.Message}" };
            }
        }

        public async Task<CheckFunc> RemoveRoleAdminAsync(string UserId)
        {
            try
            {
                var user = await _context.Users.FindAsync(UserId);
                if (user == null)
                {
                    return new CheckFunc { Message = $"User with ID {UserId} was not found." };
                }
                if (!await _userManager.IsInRoleAsync(user, "ADMIN"))
                {
                    return new CheckFunc { Message = $"User is not admin is viewer." };
                }
                await _userManager.RemoveFromRoleAsync(user, "ADMIN");
                await _userManager.AddToRoleAsync(user, "USER");
                return new CheckFunc { IsSucceeded = true, Message = "Updated successfully" }; ;
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"Error while updating user : {ex.Message}" };
            }
        }

        public async Task<CheckFunc> DeleteUserAsync(string UserId)
        {
            try
            {
                var obj = await _context.Users.FindAsync(UserId);
                if (obj == null)
                {
                    return new CheckFunc { Message = $"User with ID {UserId} was not found." };
                }
                if (await _userManager.IsInRoleAsync(obj, "SUPERADMIN"))
                {
                    return new CheckFunc { Message = "Fucking bitch" };
                }
                _context.Users.Remove(obj);
                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "Deleted successfully" }; ;
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"Error while updating user : {ex.Message}" };
            }
        }

        public async Task<UserLogin> LoginAsync(UserLogin users)
        {
            var user = await _userManager.FindByEmailAsync(users.email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, users.password))
            {
                return new UserLogin { IsAuthenticated = false, Message = "Email or Password is incorrect" };
            }
            if (!user.EmailConfirmed)
            {
                return new UserLogin { IsAuthenticated = false, Message = "You need to verifiy your email" };
            }
            return new UserLogin { IsAuthenticated = true };
        }

        public async Task<UserRefreshTokens> AddUserRefreshTokensAsync(UserRefreshTokens user)
        {
            try
            {
                await _context.UserRefreshTokens.AddAsync(user);
                _context.SaveChanges();
                return user;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserRefreshTokens> GetSavedRefreshTokensAsync(string username, string refreshToken)
        {
            var token = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive == true);
            return token;
        }

        public void DeleteUserRefreshTokens(string username, string refreshToken)
        {
            var item = _context.UserRefreshTokens.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _context.UserRefreshTokens.Remove(item);
            }
        }

        public async Task<CheckFunc> UpdateUserAsync(string UserId, UpdateUserDto userDto)
        {
            try
            {
                var obj = await _context.Users.FindAsync(UserId);
                if (obj == null)
                {
                    return new CheckFunc { Message = $"User with ID {UserId} was not found." };
                }
                if (!string.IsNullOrEmpty(userDto.UserName))
                {
                    userDto.UserName = userDto.UserName;
                }
                if (!string.IsNullOrEmpty(userDto.Email))
                {
                    if (await _userManager.FindByEmailAsync(userDto.Email) is not null)
                    {
                        return new CheckFunc { Message = "Email is already Register!" };
                        // isAuthenticated in Auth Model is false by default i handle it to understand code
                    }
                    userDto.Email = userDto.Email;
                }
                if (!string.IsNullOrEmpty(userDto.PhoneNumber))
                {
                    userDto.PhoneNumber = userDto.PhoneNumber;
                }
                /*
                if (userDto.profileImage != null)
                {
                    var uniqueFileName = await UpdateUserImage(UserId, userDto.profileImage);
                    if (uniqueFileName == "error")
                    {
                        return false;
                    }
                    obj.ProfileImage = uniqueFileName;
                }
                */


                await _context.SaveChangesAsync();
                return new CheckFunc { IsSucceeded = true, Message = "Updated successfully" };
            }
            catch (Exception ex)
            {
                return new CheckFunc { Message = $"Error while updating user : {ex.Message}" };
            }
        }

        public async Task<CheckFunc> ForgotPasswordConfirmationAsync(ForgotPasswordConfirmation model, string requestScheme, string requestHost)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new CheckFunc { Message = $"User with email {model.Email} was not found." };
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token); // Ensure token is URL safe
                var callbackUrl = $"{requestScheme}://{requestHost}/api/user/forgotPassword?token={encodedToken}&email={user.Email}";

                EmailDto email = new EmailDto()
                {
                    To = user.Email,
                    Subject = "Reset Password",
                    Body = $"Please reset your password by clicking here: <a href={callbackUrl}>link</a>"
                };

                await _emailService.SendValidationEmailAsync(email);

                return new CheckFunc { IsSucceeded = true, Message = "Email was sent." };
            }
            catch (Exception ex)
            {
                // You could log the exception message here for troubleshooting
                return new CheckFunc { Message = $"Error while sending the email: {ex.Message}" };
            }
        }


        public async Task<CheckFunc> ForgotPasswordAsync(ForgotPasswordModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new CheckFunc { Message = $"User with email {request.Email} was not found." };
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                return new CheckFunc { IsSucceeded = true, Message = "Password has been reset successfully." };
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new CheckFunc { Message = $"Password reset failed: {errors}" };
        }


        public async Task<User> VerifiyEmailAsync(string verificationToken)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email && x.ValidationEmailToken == req.Token);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.ValidationEmailToken == verificationToken);
            if (user is null)
            {
                return null;
            }
            _context.Attach(user);
            user.EmailConfirmed = true;
            user.ValidationEmailToken = null;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<CheckFunc> ResetPassword(string userId, ResetPasswordModel request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return new CheckFunc { Message = $"User with ID {userId} was not found." };
            }
            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (result.Succeeded)
            {
                return new CheckFunc { IsSucceeded = true, Message = "Password has been reset successfully." };
            }
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new CheckFunc { Message = $"Password reset failed: {errors}" };
        }
    }
}
