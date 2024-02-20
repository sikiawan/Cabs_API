using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Model.Models;
using Model.Models.AuthModels;
using Model.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Model.Models.RequestModels;
using Model.ViewModel;
using Microsoft.AspNetCore.Http;

namespace Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMailService _mailService;
        private readonly IUserService _useService;
        private readonly IClientPreferenceService _clientPreferenceService;

        public AuthService(IConfiguration config,
                            UserManager<ApplicationUser> userManager, 
                            IMailService mailService, 
                            IUserService userService,
                            RoleManager<IdentityRole> roleManager,
                            IClientPreferenceService clientPreferenceService)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailService = mailService;
            _useService = userService;
            _clientPreferenceService = clientPreferenceService;
        }

        //Register User
        public async Task<ResponseManager> RegisterUser(RegisterUser model)
        {
        
            var identityUser = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Username,
            };

            //user creation
            var createdUser = await _useService.CreateUser(model);

            //Mail Sending
            if (createdUser.IsSuccess != false)
            {
                string confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var emailToken = Uri.EscapeDataString(confirmEmailToken);
                var encodedEmailToken = Encoding.UTF8.GetBytes(emailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                var confirmUser = await _userManager.FindByEmailAsync(identityUser.Email);

                string url = $"{_config["AppUrl"]}/api/auth/ConfirmEmail?userid={confirmUser.Id}&token={validEmailToken}";

                var mailContent = new MailRequest
                {
                    ToEmail = identityUser.Email,
                    Subject = "Confirm your email",
                    Body = $"<h1>Welcome to Identity Test API</h1>" + $"<p>Hi {identityUser.UserName} !, Please confirm your email by <a href='{url}'>Clicking here</a></p><br><strong>Email Confirmation token for ID '"+confirmUser.Id+"' : <code>"+validEmailToken + "</code></strong>"
                };

                await _mailService.SendEmailAsync(mailContent);

                return new ResponseManager
                {
                    IsSuccess = true,
                    Response = "User created successfully! Please confirm the your Email!",
                };
            }

            return new ResponseManager
            {
                IsSuccess = false,
                Response = "User Email Already Registered, Try Login(/api/auth/Authenticate)"
            };

        }

        //Login User
        public async Task<ResponseManager> LoginUser(AuthUser model, HttpRequest Request)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                return new ResponseManager
                {
                    Response = "There is no user with that Email address!",
                    IsSuccess = false,
                };
            }
            else
            {
                var result = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!result)
                    return new ResponseManager
                    {
                        Response = "Invalid password",
                        IsSuccess = false,
                    };


                //var userRole = new List<string>(await _userManager.GetRolesAsync(user));
                //Generate Token JWT
                var Token = await GenerateToken(user);
                //Get client preference
                ClientPreferenceVM clientPreference = new ClientPreferenceVM();
                var clientPreferenceResult = await _clientPreferenceService.GetClientPreferenceByTenantId(user.TenantId, Request);
                if (clientPreferenceResult.IsSuccess)
                {
                    clientPreference = clientPreferenceResult.Response;
                }
                var response = new
                {
                    Token = Token,
                    ClientPreferences = clientPreference
                };
                return new ResponseManager
                {   
                    Response = response,
                    IsSuccess = true,
                };
            }

        }
        //Login User by Id
        public async Task<ResponseManager> LoginUserById(string id, string saId)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            if(user != null)
            {
                //Generate Token JWT
                var Token = await GenerateToken(user, saId);

                return new ResponseManager
                {   
                    Response = Token,
                    IsSuccess = true,
                };
            }
            else
            {
                return new ResponseManager
                {
                    Response = "User no found.",
                    IsSuccess = false,
                };
            }

        }

        //ConfirmEmail
        public async Task<ResponseManager> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "User not found"
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalToken = Encoding.UTF8.GetString(decodedToken);

            var result =  await _userManager.ConfirmEmailAsync(user,normalToken);

            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return new ResponseManager
                {
                    Response = "Email confirmed successfully!",
                    IsSuccess = true,
                };
            }

            return new ResponseManager
            {
                IsSuccess = false,
                Response = "Email did not confirm",
                //Errors = result.Errors.ToArray()
            };
        }

        //Forget Password
        public async Task<ResponseManager> ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "No user associated with email",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);
            string pass = "Tester@123";
            string url = $"{_config["AppUrl"]}/api/Auth/ResetPassword?Email={email}&Token={validToken}&NewPassword={pass}&ConfirmPassword={pass}";
            
            var mailContent = new MailRequest
            {
                ToEmail = email,
                Subject = "Reset Password",
                Body = "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password, <br><br> 1. Copy the Link :  <a href='{url}'>{url}</a><br><br> 2. Navigate to API Testing Tools(Postman)<br><br> 3. Set the Method to 'POST' <br><br> 4. Make a Request <br><br> or Use SWAGGER <br><br> <strong>Reset Token : {validToken}</strong></p>"
            };

            await _mailService.SendEmailAsync(mailContent);

            return new ResponseManager
            {
                IsSuccess = true,
                Response = "Reset password URL has been sent to the email successfully!"
            };
        }

        //Reset Password
        public async Task<ResponseManager> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "No user associated with email",
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new ResponseManager
                {
                    IsSuccess = false,
                    Response = "Password doesn't match its confirmation",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new ResponseManager
                {
                    Response = "Password has been reset successfully!",
                    IsSuccess = true,
                };

            return new ResponseManager
            {
                Response = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        //Token Genereator
        private async Task<string> GenerateToken(ApplicationUser user, string saId = "0")
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();

            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);

            var userRoless = await _roleManager.Roles.Where(x => roles.Contains(x.Name)).ToListAsync();

            var allRoleClaims = new List<Claim>();

            foreach (var role in userRoless)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                allRoleClaims.AddRange(roleClaims);
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("saId", saId),
                new Claim("tenantId", user.TenantId.ToString())

            }.Union(userClaims).Union(allRoleClaims).Union(userRoles);

            //claims.AddRange(userRole.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var tokenClaims = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenClaims);
            return tokenString;
        }


        //Authenticate

        //private User Authenticate(AuthUser auth)
        //{


        //    if (string.IsNullOrEmpty(auth.UserName) || string.IsNullOrEmpty(auth.Email) || string.IsNullOrEmpty(auth.Password))
        //        return null;

        //    var user = _userManger.Users.SingleOrDefault(x => x.Email == auth.Email);

        //     check if username exists
        //    if (user == null)
        //        return null;
        //     check if password is correct
        //    if (!VerifyPasswordHash(auth.Password, user.PasswordHash, user.PasswordSalt))
        //        return null;

        //     authentication successful
        //    return null;
        //}



        //private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        //{
        //    if (password == null) throw new ArgumentNullException("password");
        //    if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
        //    if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
        //    if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

        //    using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
        //    {
        //        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //        for (int i = 0; i < computedHash.Length; i++)
        //        {
        //            if (computedHash[i] != storedHash[i]) return false;
        //        }
        //    }

        //    return true;
        //}


    }
}
