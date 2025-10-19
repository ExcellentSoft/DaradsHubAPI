using DaradsHubAPI.Core.IRepository;
using DaradsHubAPI.Core.Model.Request;
using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Core.Services.Interface;
using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Customs;
using DaradsHubAPI.Shared.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DaradsHubAPI.Domain.Enums.Enum;

namespace DaradsHubAPI.Core.Repository;
public class UserRepository(AppDbContext _context, UserManager<User> _userManager, SignInManager<User> _signInManager, IServiceProvider _serviceProvider, IOptionsSnapshot<AppSettings> optionsSnapshot) : GenericRepository<userstb>(_context), IUserRepository
{
    public AppSettings _optionsSnapshot { get; } = optionsSnapshot.Value;
    private User? _user;

    public async Task<(bool status, string message, string? userId)> CreateCustomer(CreateCustomerRequest request)
    {
        try
        {
            var uCustomer = request.ToUser();
            var createUser = await _userManager.CreateAsync(uCustomer, request.Password);
            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(x => x.Description);
                return new(false, $"Unable to register at this time. {string.Join(" ", errors)}", null);
            }

            var customer = request.ToCustomer(uCustomer.Id);
            await _context.userstb.AddAsync(customer);

            var walletCode = $"Wal-{CustomizeCodes.GetCode()}";
            var customerWallet = request.ToCustomerWallet(walletCode);
            await _context.wallettb.AddAsync(customerWallet);

            await _context.SaveChangesAsync();


            var otp = CustomizeCodes.GenerateOTP(6);
            await SendOTPCodeAsync(new SendOtpRequest
            {
                Code = otp,
                FirstName = request.FullName,
                UserId = uCustomer.Id,
                SenderName = "Darads",
                SendingMode = "Email",
                UserEmail = uCustomer.Email,
                Purpose = OtpVerificationPurposeEnum.EmailVerification
            });

            var scope = _serviceProvider.GetRequiredService<IEmailService>();
            string message = $"Hello {request.FullName}, kindly utilize the code {otp} to finalize the registration process. We're excited to welcome you onboard!<br/><br/>";
            await scope.SendMail(request.Email, "Email Verification", message, "Darads", useTemplate: true);

            return new(true, message = $"Success! Kindly check your email and use the provided code to finalize your registration.", uCustomer.Id);
        }
        catch (Exception)
        {
            return new(false, "Unable to register, please try again later.", null);
        }
    }
    public async Task<(bool status, string message, CustomerLoginResponse? cresponse)> LoginUser(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return new(false, "Email is required.", null);

        if (string.IsNullOrEmpty(request.Password))
            return new(false, "Password is required.", null);

        if (!StringExtensions.IsValidEmail(request.Email.Trim()))
            return new(false, "Email address is not valid, check and try again.", null);

        var user = await _userManager.Users.FirstOrDefaultAsync(d => d.Email == request.Email);

        if (user is null)
            return new(false, "User record not found, check and try again.", null);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return new(false, "Unauthorized.", null);

        var customer = await _context.userstb.AsNoTracking().Where(us => us.userid == user.Id).FirstOrDefaultAsync();
        if (customer is null)
            return new(false, "User record not found.", null);

        if (!user.EmailConfirmed)
        {
            var otp = CustomizeCodes.GenerateOTP(6);
            await SendOTPCodeAsync(new SendOtpRequest
            {
                Code = otp,
                UserId = customer.userid,
                UserEmail = customer.email,
                Purpose = OtpVerificationPurposeEnum.EmailVerification
            });
            var scope = _serviceProvider.GetRequiredService<IEmailService>();
            string message = $"Hello {customer.username}, kindly utilize the code {otp} to finalize the registration process. We're excited to welcome you onboard!" +
                "<br/><br/>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.<br/><br/>";
            await scope.SendMail(customer.email, "Email Verification", message, $"Darads", useTemplate: true);

            return new(false, "Your email address has not been verified. Please verify it.", new CustomerLoginResponse { UserId = user.Id, IsVerifyCodeRequire = true });
        }

        if (user.Status == EntityStatusEnum.InActive || user.Status == EntityStatusEnum.Delete)
            return new(false, "Your has been deactivated. Kindly contact admin.", null);


        if (user.Is_customer.GetValueOrDefault() != 1 && user.Is_admin.GetValueOrDefault() != 1)
        {
            //is a agent
            if (!customer!.IsAgent.GetValueOrDefault())
            {
                return new(false, "Your onboarding registration is still pending. Kindly contact admin.", null);
            }
        }

        try
        {
            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    user.LockoutEnd = DateTime.MaxValue;
                    await _userManager.UpdateAsync(user);
                    return new(false, "Your account has been locked. Kindly initiate a password reset to unlock your account.", null);
                }
                else
                {
                    int maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                    int failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                    if (maxAttempts - failedAttempts == 0)
                    {
                        user.LockoutEnabled = true;
                        user.LockoutEnd = DateTime.MaxValue;
                        await _userManager.UpdateAsync(user);

                        return new(false, "Your account has been locked. Kindly initiate a password reset to unlock your account.", null);
                    }

                    return new(false, $"Invalid login credentials.You have {maxAttempts - failedAttempts} attempts left.", null);
                }
            }


            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            var lockoutResponse = await _userManager.UpdateAsync(user);
            var lifeTime = _optionsSnapshot.Lifetime;
            var expires = DateTimeOffset.Now.AddMinutes(Convert.ToDouble(lifeTime));
            var token = await CreateToken(user, customer!.id);

            var response = new CustomerLoginResponse
            {
                UserId = user.Id,
                IsCustomer = user.Is_customer,
                IsAgent = user.Is_agent,
                IsAdmin = user.Is_admin,
                UserIdInt = customer.id,
                Email = user.Email,
                Expires = expires.ToUnixTimeSeconds(),
                ExpiresTime = expires,
                Name = customer.fullname,
                Token = token,
                Photo = "",
                Is2FA = user.TwoFactorEnabled,
                IsVerify = customer.status == 1,
                PhoneNumber = customer.phone
            };
            //Send Message to Customer: 
            return new(true, "Login was successful.", response);
        }
        catch (Exception)
        {
            return new(false, $"Unable to login, please try again later.", null);
        }
    }

    public async Task<(bool status, string message, CustomerLoginResponse? cresponse)> LoginAdmin(AdminLoginRequest request)
    {
        var aUser = await _context.userstb.Where(d => d.phone == request.PhoneNumber).FirstOrDefaultAsync();

        if (aUser is null)
            return new(false, "User record not found, check and try again.", null);
        var user = await _userManager.Users.FirstOrDefaultAsync(d => d.UserName == aUser.username);
        if (user is null)
            return new(false, "User record not found, check and try again.", null);
        if (!await _userManager.CheckPasswordAsync(user!, request.PIN))
            return new(false, "Unauthorized.", null);

        try
        {
            var signInResult = await _signInManager.PasswordSignInAsync(user, request.PIN, false, true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    user.LockoutEnd = DateTime.MaxValue;
                    await _userManager.UpdateAsync(user);
                    return new(false, "Your account has been locked. Kindly initiate a password reset to unlock your account.", null);
                }
                else
                {
                    int maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                    int failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                    if (maxAttempts - failedAttempts == 0)
                    {
                        user.LockoutEnabled = true;
                        user.LockoutEnd = DateTime.MaxValue;
                        await _userManager.UpdateAsync(user);

                        return new(false, "Your account has been locked. Kindly initiate a password reset to unlock your account.", null);
                    }

                    return new(false, $"Invalid login credentials.You have {maxAttempts - failedAttempts} attempts left.", null);
                }
            }

            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            var lockoutResponse = await _userManager.UpdateAsync(user);
            var lifeTime = _optionsSnapshot.Lifetime;
            var expires = DateTimeOffset.Now.AddMinutes(Convert.ToDouble(lifeTime));
            var token = await CreateToken(user, aUser!.id);

            var response = new CustomerLoginResponse
            {
                UserId = user.Id,
                IsCustomer = user.Is_customer,
                IsAgent = user.Is_agent,
                IsAdmin = user.Is_admin,
                UserIdInt = aUser.id,
                Email = user.Email,
                Expires = expires.ToUnixTimeSeconds(),
                ExpiresTime = expires,
                Name = aUser.fullname,
                Token = token,
                Photo = aUser.Photo,
                Is2FA = user.TwoFactorEnabled,
                IsVerify = aUser.status == 1,
                PhoneNumber = aUser.phone
            };
            //Send Message to Customer: 
            return new(true, "Login was successful.", response);
        }
        catch (Exception)
        {
            return new(false, $"Unable to login, please try again later.", null);
        }
    }


    public async Task<(bool status, string message)> ResendEmailVerificationCode(string userId)
    {
        var model = await (from customer in _context.userstb.Where(vc => vc.userid == userId)
                           select new { customer }).FirstOrDefaultAsync();
        if (model == null)
            return new(false, "Customer record not found, check and try again later.");

        var otp = CustomizeCodes.GenerateOTP(6);
        await SendOTPCodeAsync(new SendOtpRequest
        {
            Code = otp,
            UserId = model.customer.userid,
            UserEmail = model.customer.email,
            Purpose = OtpVerificationPurposeEnum.EmailVerification
        });
        var scope = _serviceProvider.GetRequiredService<IEmailService>();
        string message = $"Hello {model.customer.username}, kindly utilize the code {otp} to finalize the registration process. We're excited to welcome you onboard!" +
            "<br/><br/>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.<br/><br/>";
        await scope.SendMail(model.customer.email, "Email Verification", message, $"Darads", useTemplate: true);

        return new(true, "Success! Please check your email and use the provided code.");
    }
    public async Task<(bool status, string message, CustomerLoginResponse? cresponse)> VerifyUserAccount(string code)
    {
        var optValidationLog = await _context.OtpVerificationLogs.FirstOrDefaultAsync(g => g.Code == code);

        if (optValidationLog is null)
            return new(false, "Email verification code does not exist, check and try again later.", null);

        var vResponse = await ValidateOTPCodeAsync(new ValidateOtpRequest
        {
            Code = optValidationLog.Code,
            UserId = optValidationLog.UserId,
            Purpose = optValidationLog.Purpose
        });


        if (!vResponse.status)
            return new(vResponse.status, vResponse.msg, new CustomerLoginResponse { UserId = optValidationLog.UserId });

        var user = await _userManager.FindByIdAsync(optValidationLog.UserId);
        user!.Status = EntityStatusEnum.Active;
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        var customer = await _context.userstb.FirstOrDefaultAsync(cs => cs.userid == user.Id);
        customer!.status = (int)EntityStatusEnum.Active;
        customer.ModifiedDate = GetLocalDateTime.CurrentDateTime_().GetValueOrDefault();
        await _context.SaveChangesAsync();

        await _signInManager.SignInAsync(user, false, JwtBearerDefaults.AuthenticationScheme);

        var lifeTime = _optionsSnapshot.Lifetime;
        var expires = DateTimeOffset.Now.AddMinutes(Convert.ToDouble(lifeTime));
        var token = await CreateToken(user, customer.id);
        var response = new CustomerLoginResponse
        {
            UserId = user.Id,
            IsCustomer = user.Is_customer,
            IsAgent = user.Is_agent,
            IsAdmin = user.Is_admin,
            Email = user.Email,
            Is2FA = user.TwoFactorEnabled,
            IsVerify = customer.status == 1,
            PhoneNumber = customer.phone,
            Expires = expires.ToUnixTimeSeconds(),
            ExpiresTime = expires,
            Name = customer.fullname,
            Token = token,
            Photo = string.Empty
        };

        return new(true, "Registration was successful.", response);
    }
    public async Task<(bool status, string message, string? userId)> ForgetPassword(ForgetPasswordRequest request)
    {
        if (!request.Email.IsValidEmail())
            return new(false, "Invalid email address.", null);

        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Email == request.Email);
        if (user == null)
            return new(false, $"User record not found, check and try again later.", null);

        var otp = CustomizeCodes.GenerateOTP(6);
        await SendOTPCodeAsync(new SendOtpRequest
        {
            Code = otp,
            UserId = user.Id,
            UserEmail = user.Email,
            Purpose = OtpVerificationPurposeEnum.ForgetPassword
        });

        var customer = await _context.userstb.FirstOrDefaultAsync(vd => vd.userid == user.Id);
        var scope = _serviceProvider.GetRequiredService<IEmailService>();
        string message = $"Hello {customer!.username}, kindly utilize the code {otp} to reset your password." +
            "<br/><br/>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.";

        await scope.SendMail(user.Email!, "Forget password code Verification", message, "Darads");

        return new(true, $"Success! Kindly check your email and use the provided code to finalize your reset password.", user.Id);
    }
    public async Task<(bool status, string message)> ResetPassword(ResetPasswordRequest request)
    {
        if (!request.NewPassword.Equals(request.ConfirmPassword))
            return new(false, "Passwords do not match.");

        if (string.IsNullOrEmpty(request.Code))
            return new(false, "Code is required.");

        var optValidationLog = await _context.OtpVerificationLogs.FirstOrDefaultAsync(g => g.Code == request.Code);

        if (optValidationLog is null)
            return new(false, "Reset password code does not exist, check and try again later.");

        var validateCodeResponse = await ValidateOTPCodeAsync(new ValidateOtpRequest
        {
            Code = optValidationLog.Code,
            UserId = optValidationLog.UserId,
            Purpose = optValidationLog.Purpose
        });

        if (!validateCodeResponse.status)
            return new(validateCodeResponse.status, validateCodeResponse.msg);

        var user = await _userManager.FindByIdAsync(optValidationLog.UserId);
        if (user is null)
            return new(false, "Unable to find user. Please try again later.");

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.NewPassword);
        user.SecurityStamp = Guid.NewGuid().ToString();
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return new(true, "Password updated successfully.");
        else
        {
            var errorMessage = result.Errors.Select(c => c.Description).FirstOrDefault();
            return new(false, errorMessage ?? "");
        }
    }
    public async Task<(bool status, string message)> ResendResetPasswordCode(string email)
    {
        var model = await (from customer in _context.userstb.Where(vc => vc.email == email)
                           select new { customer }).FirstOrDefaultAsync();
        if (model == null)
            return new(false, "Customer record not found, check and try again later.");

        var otp = CustomizeCodes.GenerateOTP(6);
        await SendOTPCodeAsync(new SendOtpRequest
        {
            Code = otp,
            UserId = model.customer.userid,
            UserEmail = model.customer.email,
            Purpose = OtpVerificationPurposeEnum.ForgetPassword
        });

        var scope = _serviceProvider.GetRequiredService<IEmailService>();
        string message = $"Hello {model.customer.username}, kindly utilize the code {otp} to finalize your reset password." +
            "<br/><br/>If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.<br/><br/>";
        await scope.SendMail(model.customer.email, "Email Verification", message, $"Darads", useTemplate: true);

        return new(true, "Success! Kindly check your email and use the provided code to finalize your reset password.");
    }
    public async Task<(bool status, string message, CustomerProfileResponse? res)> GetProfile(string email)
    {
        var customerUser = await _context.userstb.FirstOrDefaultAsync(us => us.email == email);
        if (customerUser is null)
            return new(false, "Customer record not found.", null);

        var boldd = await _context.CustomerVirtualAccounts.Where(c => c.UserId == customerUser.id).FirstOrDefaultAsync();

        decimal walletBalance = await _context.wallettb.Where(cw => cw.UserId == email).Select(bl => bl.Balance).FirstOrDefaultAsync() ?? 0m;

        var virtualAccts = new List<VirtualAccountDetails>();
        if (string.IsNullOrEmpty(customerUser.VpayAccountName) && boldd is null)
        {
            virtualAccts = [];
        }
        else
        {
            var vboldd = new VirtualAccountDetails();
            var vpay = new VirtualAccountDetails();
            if (boldd is not null)
            {
                vboldd = new VirtualAccountDetails
                {
                    AccountName = boldd.AcctountName,
                    AccountNumber = boldd.AcctountNumber,
                    BankName = boldd.BankName
                };
                virtualAccts.Add(vboldd);
            }
            if (!string.IsNullOrEmpty(customerUser.VpayAccountName))
            {
                vpay = new VirtualAccountDetails
                {
                    AccountName = customerUser.VpayAccountName,
                    AccountNumber = customerUser.VpayAccountNumber,
                    BankName = customerUser.VpayBankName
                };
                virtualAccts.Add(vpay);
            }
        }

        var response = new CustomerProfileResponse
        {
            Email = email,
            UserName = customerUser.username,
            PhoneNumber = customerUser.phone,
            Photo = customerUser.Photo,
            VirtualAccountDetails = virtualAccts,
            WalletBalance = walletBalance,
            UserIdInt = customerUser.id,
            UserId = customerUser.userid,
            Address = _context.ShippingAddresses.Where(d => d.CustomerId == customerUser.id).Select(d => d.Address).FirstOrDefault()
        };

        return new(true, "Customer profile fetched successfully.", response);
    }
    public async Task<(bool status, string message, CustomerProfileResponse? res)> GetAdminProfile(string email)
    {
        var customerUser = await _context.userstb.FirstOrDefaultAsync(us => us.email == email);
        if (customerUser is null)
            return new(false, "Admin record not found.", null);

        var boldd = await _context.CustomerVirtualAccounts.Where(c => c.UserId == customerUser.id).FirstOrDefaultAsync();

        decimal walletBalance = await _context.wallettb.Where(cw => cw.UserId == email).Select(bl => bl.Balance).FirstOrDefaultAsync() ?? 0m;

        var virtualAccts = new List<VirtualAccountDetails>();
        if (string.IsNullOrEmpty(customerUser.VpayAccountName) && boldd is null)
        {
            virtualAccts = [];
        }
        else
        {
            var vboldd = new VirtualAccountDetails();
            var vpay = new VirtualAccountDetails();
            if (boldd is not null)
            {
                vboldd = new VirtualAccountDetails
                {
                    AccountName = boldd.AcctountName,
                    AccountNumber = boldd.AcctountNumber,
                    BankName = boldd.BankName
                };
                virtualAccts.Add(vboldd);
            }
            if (!string.IsNullOrEmpty(customerUser.VpayAccountName))
            {
                vpay = new VirtualAccountDetails
                {
                    AccountName = customerUser.VpayAccountName,
                    AccountNumber = customerUser.VpayAccountNumber,
                    BankName = customerUser.VpayBankName
                };
                virtualAccts.Add(vpay);
            }
        }

        var response = new CustomerProfileResponse
        {
            Email = email,
            FullName = customerUser.fullname,
            PhoneNumber = customerUser.phone,
            Photo = customerUser.Photo,
            VirtualAccountDetails = virtualAccts,
            WalletBalance = walletBalance,
            UserIdInt = customerUser.id,
            UserId = customerUser.userid,
            Address = _context.ShippingAddresses.Where(d => d.CustomerId == customerUser.id).Select(d => d.Address).FirstOrDefault()
        };

        return new(true, "Admin profile fetched successfully.", response);
    }
    public async Task<(bool status, string message, AgentProfileResponse? res)> GetAgentProfile(string email)
    {
        var customerUser = await _context.userstb.FirstOrDefaultAsync(us => us.email == email);
        if (customerUser is null)
            return new(false, "Agent record not found.", null);

        var boldd = await _context.CustomerVirtualAccounts.Where(c => c.UserId == customerUser.id).FirstOrDefaultAsync();

        decimal walletBalance = await _context.wallettb.Where(cw => cw.UserId == email).Select(bl => bl.Balance).FirstOrDefaultAsync() ?? 0m;

        var virtualAccts = new List<VirtualAccountDetails>();
        if (string.IsNullOrEmpty(customerUser.VpayAccountName) && boldd is null)
        {
            virtualAccts = [];
        }
        else
        {
            var vboldd = new VirtualAccountDetails();
            var vpay = new VirtualAccountDetails();
            if (boldd is not null)
            {
                vboldd = new VirtualAccountDetails
                {
                    AccountName = boldd.AcctountName,
                    AccountNumber = boldd.AcctountNumber,
                    BankName = boldd.BankName
                };
                virtualAccts.Add(vboldd);
            }
            if (!string.IsNullOrEmpty(customerUser.VpayAccountName))
            {
                vpay = new VirtualAccountDetails
                {
                    AccountName = customerUser.VpayAccountName,
                    AccountNumber = customerUser.VpayAccountNumber,
                    BankName = customerUser.VpayBankName
                };
                virtualAccts.Add(vpay);
            }
        }

        var response = new AgentProfileResponse
        {
            Email = email,
            FullName = customerUser.fullname,
            PhoneNumber = customerUser.phone,
            Photo = customerUser.Photo,
            BusinessEmail = customerUser.BusinessEmail,
            BusinessName = customerUser.BusinessName,
            VirtualAccountDetails = virtualAccts,
            WalletBalance = walletBalance,
            UserIdInt = customerUser.id,
            UserId = customerUser.userid,
            Address = _context.ShippingAddresses.Where(d => d.CustomerId == customerUser.id).Select(d => new AgentAddress
            {
                Address = d.Address,
                Country = d.Country,
                State = d.State,
                City = d.City,
            }).FirstOrDefault()
        };

        return new(true, "Agent profile fetched successfully.", response);
    }
    public async Task<(bool status, string message)> UpdateProfile(CustomerProfileRequest request, string email, string imagePath)
    {
        var customerUser = await _context.userstb.Where(us => us.email == email).FirstOrDefaultAsync();
        customerUser!.Photo = imagePath;

        if (_context.userstb.Any(us => us.phone == request.PhoneNumber && us.email != email))
        {
            return new(false, $"Phone number  {request.PhoneNumber} already registered, check and try again later.");
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Email == email);
        user!.PhoneNumber = request.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {

            return new(false, result.Errors.Select(c => c.Description).FirstOrDefault() ?? "");
        }

        var iCustomerUser = await _context.userstb.Where(us => us.email == email).ExecuteUpdateAsync(setter => setter
                           .SetProperty(s => s.phone, request.PhoneNumber)
                           .SetProperty(s => s.fullname, request.FullName)
                           .SetProperty(s => s.Photo, string.IsNullOrEmpty(imagePath) ? customerUser.Photo : imagePath)
                           .SetProperty(s => s.ModifiedDate, DateTime.Now));
        var address = await _context.ShippingAddresses.FirstOrDefaultAsync(d => d.CustomerId == customerUser.id);

        if (address is null)
        {
            address = new ShippingAddress
            {
                Address = request.Address,
                CustomerId = customerUser.id,
                Email = customerUser.email,
                City = string.Empty,
                Country = string.Empty,
                PhoneNumber = customerUser.phone,
                State = string.Empty
            };

            _context.ShippingAddresses.Add(address);

        }
        else
        {
            address.Address = request.Address;
        }

        await _context.SaveChangesAsync();
        return new(true, "Profile updated successfully.");

    }
    public async Task<(bool status, string message)> ChangePassword(ChangePasswordRequest request, string email)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Email == email);
        if (user is null)
            return new(false, "Unable to find user. Please try again later.");

        if (string.IsNullOrEmpty(request.CurrentPassword))
            return new(false, "Please enter current password.");

        if (request.NewPassword.ToLower() != request.ConfirmPassword.ToLower())
            return new(false, "Password not the same as confirm password. Please try again later.");

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.NewPassword);
        user.SecurityStamp = Guid.NewGuid().ToString();
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return new(true, "Password updated successfully.");
        else
        {
            var errorMessage = result.Errors.Select(c => c.Description).FirstOrDefault();
            return new(false, errorMessage ?? "");
        }
    }
    public async Task<DashboardMetricsResponse> DashboardMetrics(string email)
    {
        var response = new DashboardMetricsResponse
        {
            ActiveOrderCount = await _context.HubOrders.Where(r => r.UserEmail == email).CountAsync(),
            WalletBalance = await _context.wallettb.Where(e => e.UserId == email).Select(r => r.Balance).FirstOrDefaultAsync()
        };

        return response;
    }

    public async Task<User?> GetAppUser(string email)
    {
        return await _userManager.Users.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<List<string?>> GetAppUsersEmails(bool isCustomer, bool isAgent)
    {
        int _agent = isAgent ? 1 : 0;
        int _customer = isCustomer ? 1 : 0;
        return await _userManager.Users.AsNoTracking().Where(e => e.Is_agent == _agent || e.Is_customer == _customer).Select(d => d.Email).Take(50).ToListAsync();
    }

    public async Task AddAgentReview(HubAgentReview model)
    {
        _context.HubAgentReviews.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool status, string message, string? userId)> CreateAgent(CreateAgentRequest request)
    {
        try
        {
            var uCustomer = request.ToUser();
            var createUser = await _userManager.CreateAsync(uCustomer, request.Password);
            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(x => x.Description);
                return new(false, $"Unable to register at this time. {string.Join(" ", errors)}", null);
            }

            var customer = request.ToAgent(uCustomer.Id);
            await _context.userstb.AddAsync(customer);

            var walletCode = $"Wal-{CustomizeCodes.GetCode()}";
            var customerWallet = request.ToCustomerWallet(walletCode);
            await _context.wallettb.AddAsync(customerWallet);

            await _context.SaveChangesAsync();


            var otp = CustomizeCodes.GenerateOTP(6);
            await SendOTPCodeAsync(new SendOtpRequest
            {
                Code = otp,
                FirstName = request.FullName,
                UserId = uCustomer.Id,
                SenderName = "Darads",
                SendingMode = "Email",
                UserEmail = uCustomer.Email,
                Purpose = OtpVerificationPurposeEnum.EmailVerification
            });

            var scope = _serviceProvider.GetRequiredService<IEmailService>();
            string message = $"Hello {request.FullName}, kindly utilize the code {otp} to finalize the registration process. We're excited to welcome you onboard!<br/><br/>";
            await scope.SendMail(request.Email, "Email Verification", message, "Darads", useTemplate: true);

            return new(true, message = $"Success! Kindly check your email and use the provided code to finalize your registration.", uCustomer.Id);

        }
        catch (Exception)
        {
            return new(false, "Unable to register, please try again later.", null);
        }
    }
    public async Task BlockAgent(BlockedAgent model)
    {
        _context.BlockedAgents.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task ClearSuspendBlockRecord(int agentId)
    {
        await _context.SuspendedAgents.Where(s => s.UserId == agentId).ExecuteDeleteAsync();
        await _context.BlockedAgents.Where(s => s.AgentId == agentId).ExecuteDeleteAsync();
    }

    public async Task SuspendedAgent(SuspendedAgent model)
    {
        _context.SuspendedAgents.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool status, string message)> AddAgentByAdmin(AddAgentRequest request, string photoUrl)
    {
        try
        {
            var uCustomer = request.ToUser();
            var createUser = await _userManager.CreateAsync(uCustomer, request.Password);
            if (!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(x => x.Description);
                return new(false, $"Unable to register at this time. {string.Join(" ", errors)}");
            }

            var agent = request.ToAgent(uCustomer.Id, photoUrl);
            await _context.userstb.AddAsync(agent);

            var walletCode = $"Wal-{CustomizeCodes.GetCode()}";
            var customerWallet = request.ToCustomerWallet(walletCode);
            await _context.wallettb.AddAsync(customerWallet);

            await _context.SaveChangesAsync();

            bool canSellDigitalProduct = false;
            bool canSellPhysicalProduct = false;

            if (request.CataloguesIds is not null)
            {
                await _context.CatalogueMappings.Where(e => e.AgentId == agent.id).ExecuteDeleteAsync();

                foreach (var id in request.CataloguesIds)
                {
                    var map = new CatalogueMapping
                    {
                        AgentId = agent.id,
                        CatalogueId = id
                    };
                    _context.CatalogueMappings.Add(map);
                }
                canSellDigitalProduct = true;
                await _context.SaveChangesAsync();
            }

            if (request.CategoriesIds is not null)
            {
                await _context.CategoryMappings.Where(e => e.AgentId == agent.id).ExecuteDeleteAsync();

                foreach (var id in request.CategoriesIds)
                {
                    var map = new CategoryMapping
                    {
                        AgentId = agent.id,
                        CategoryId = id
                    };
                    _context.CategoryMappings.Add(map);
                }
                canSellPhysicalProduct = true;
                await _context.SaveChangesAsync();
            }

            var settings = await _context.HubAgentProductSettings.FirstOrDefaultAsync(s => s.AgentId == agent.id);
            if (settings is null)
            {
                settings = new HubAgentProductSetting
                {
                    CanSellDigitalProduct = canSellDigitalProduct,
                    CanSellPhysicalProduct = canSellPhysicalProduct,
                    DateCreated = GetLocalDateTime.CurrentDateTime(),
                    AgentId = agent.id
                };

                _context.HubAgentProductSettings.Add(settings);
            }
            else
            {
                settings.CanSellDigitalProduct = canSellDigitalProduct;
                settings.CanSellPhysicalProduct = canSellPhysicalProduct;
            }
            await _context.SaveChangesAsync();

            var scope = _serviceProvider.GetRequiredService<IEmailService>();
            string message = $"Hello {request.FullName}, Your account has been successfully created. You can log in using the following credentials: <br/><br/>" +
                $"Email : {request.Email}" +
                $"Password : {request.Password}";

            await scope.SendMail(request.Email, "Account Creation", message, "Darads", useTemplate: true);

            return new(true, message = $"Success! Agent created successfully.");
        }
        catch (Exception)
        {
            return new(false, "Unable to register, please try again later.");
        }
    }

    public async Task<(bool status, string message)> UpdateAgentProfile(AgentProfileRequest request, string email, string imagePath)
    {
        var agentUser = await _context.userstb.Where(us => us.email == email).FirstOrDefaultAsync();
        agentUser!.Photo = imagePath;

        if (_context.userstb.Any(us => us.phone == request.PhoneNumber && us.email != email))
        {
            return new(false, $"Phone number  {request.PhoneNumber} already registered, check and try again later.");
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Email == email);
        user!.PhoneNumber = request.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {

            return new(false, result.Errors.Select(c => c.Description).FirstOrDefault() ?? "");
        }

        var iAgentUser = await _context.userstb.Where(us => us.email == email).ExecuteUpdateAsync(setter => setter
                           .SetProperty(s => s.phone, request.PhoneNumber)
                           .SetProperty(s => s.fullname, request.FullName)
                           .SetProperty(s => s.BusinessName, request.BusinessName)
                           .SetProperty(s => s.BusinessEmail, request.BusinessEmail)
                           .SetProperty(s => s.AgentExperience, request.Experience)
                           .SetProperty(s => s.Photo, string.IsNullOrEmpty(imagePath) ? agentUser.Photo : imagePath)
                           .SetProperty(s => s.ModifiedDate, DateTime.Now));
        var address = await _context.ShippingAddresses.FirstOrDefaultAsync(d => d.CustomerId == agentUser.id);

        if (address is null)
        {
            address = new ShippingAddress
            {
                Address = request.Address,
                CustomerId = agentUser.id,
                Email = agentUser.email,
                City = request.City,
                Country = string.Empty,
                PhoneNumber = agentUser.phone,
                State = request.State
            };

            _context.ShippingAddresses.Add(address);

        }
        else
        {
            address.Address = request.Address;
        }

        await _context.SaveChangesAsync();
        return new(true, "Profile updated successfully.");

    }

    public async Task<MessageAudiences> GetAudiences(string audienceType)
    {
        return await _context.MessageAudiences.FirstOrDefaultAsync(d => d.Code == audienceType) ?? new MessageAudiences();
    }

    private async Task SendOTPCodeAsync(SendOtpRequest request)
    {
        var date = DateTime.Now;
        var otp = await _context.OtpVerificationLogs.Where(x => x.UserId.Equals(request.UserId) && x.Purpose == request.Purpose && x.Status == OtpCodeStatusEnum.Sent).FirstOrDefaultAsync();

        if (otp is not null)
        {
            otp.Code = request.Code;
            otp.TimeUpdated = date;
            otp.Status = OtpCodeStatusEnum.Sent;
            _context.OtpVerificationLogs.Update(otp);
            await _context.SaveChangesAsync();
        }
        else
        {
            OtpVerificationLog verificationLog = new()
            {
                UserId = request.UserId,
                Recipient = request.UserEmail,
                Purpose = request.Purpose,
                Code = request.Code,
                Status = OtpCodeStatusEnum.Sent,
                TimeCreated = date,
                TimeUpdated = date,
            };
            _context.OtpVerificationLogs.Add(verificationLog);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<(bool status, string msg)> ValidateOTPCodeAsync(ValidateOtpRequest otpRequest)
    {
        var date = DateTime.UtcNow;
        int settingExpirationMinute = _optionsSnapshot.OtpExpiry;
        var otpVerification = await _context.OtpVerificationLogs.FirstOrDefaultAsync(v => v.UserId == otpRequest.UserId && v.Status == OtpCodeStatusEnum.Sent && v.Purpose == otpRequest.Purpose && v.Code == otpRequest.Code);

        if (otpVerification == null)
        {
            return new(false, "Verification code provided is invalid.");
        }

        if (date > otpVerification.TimeCreated.AddMinutes(settingExpirationMinute))
        {
            otpVerification.Status = OtpCodeStatusEnum.Expired;
            otpVerification.TimeUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new(false, "Code provided has expired.");
        }

        otpVerification.ConfirmedOn = DateTime.UtcNow;
        otpVerification.Status = OtpCodeStatusEnum.Verified;
        otpVerification.TimeUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new(true, "Code verify successfully.");
    }

    public async Task<string> CreateToken(User user, int customerId)
    {
        _user = user;
        var claim = await GetClaimsAsync(customerId);
        var signingCredential = GetSigning();
        var token = GetTokenOption(signingCredential, claim);
        var writeToken = new JwtSecurityTokenHandler().WriteToken(token);

        return writeToken;
    }

    private JwtSecurityToken GetTokenOption(SigningCredentials signing, List<Claim> claim)
    {
        var lifeTime = _optionsSnapshot.Lifetime;
        var issuer = _optionsSnapshot.Issuer;
        var expiration = DateTime.Now.AddMinutes(lifeTime);
        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claim,
            expires: expiration,
            signingCredentials: signing);

        return token;
    }

    private async Task<List<Claim>> GetClaimsAsync(int customerId)
    {
        var customerWalletId = await _context.wallettb.Where(cw => cw.UserId == _user!.Email).Select(cd => cd.id).FirstOrDefaultAsync();
        var claim = new List<Claim>
        {
            new Claim("userEmail", _user?.Email!),
            new Claim("id",customerId.ToString()),
            new Claim("userId",_user?.Id??""),
            new Claim("walletId",customerWalletId.ToString())
        };

        var role = await _userManager.GetRolesAsync(_user!);
        claim.AddRange(role.Select(roles => new Claim("role", roles)));
        return claim;
    }

    private SigningCredentials GetSigning()
    {
        var key = _optionsSnapshot.JwtKey;
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }


    public async Task SaveMessageSentLog(MessagesSentLogs entity)
    {
        await _context.MessagesSentLogs.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SaveMessagesLogs(List<MessagesSentLogs> entities)
    {
        await _context.MessagesSentLogs.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task SaveSentMessageDetails(MessagesSent entity)
    {
        await _context.MessagesSent.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MessagesSent>> GetAllSentMessages()
    {
        return await _context.MessagesSent.OrderByDescending(c => c.SentDate).ToListAsync();
    }

    public async Task<MessageEmailTemplates> GetEmailTemplates(int Id)
    {
        return await _context.MessageEmailTemplates.FirstOrDefaultAsync(c => c.Id == Id) ?? new MessageEmailTemplates();
    }

    public async Task<MessagesSent> GetSentMessage(int Id)
    {
        return await _context.MessagesSent.FirstOrDefaultAsync(x => x.Id == Id) ?? new MessagesSent();
    }

    public async Task<List<userstb>> GetCustomers(string audienceType, int pageSize, int pageIndex)
    {
        pageIndex = pageSize * pageIndex;

        if (!string.IsNullOrWhiteSpace(audienceType))
        {
            //template for all, allb, logc,gft
            if (audienceType == "all")
            {
                var result = await (from ct in _context.userstb
                                    join wl in _context.wallettb
                                    on ct.email equals wl.UserId
                                    select ct).OrderBy(c => c.email).Skip(pageIndex).Take(pageSize).ToListAsync();
                return result;
            }
            else if (audienceType == "allb")
            {
                var result = await (from ct in _context.userstb
                                    join wl in _context.wallettb on ct.email equals wl.UserId
                                    where wl.Balance > 500
                                    select ct).OrderBy(c => c.email).Skip(pageIndex).Take(pageSize).ToListAsync();
                return result;
            }
            else if (audienceType == "logc")
            {
                var result = await (from u in _context.userstb
                                    join log in _context.AllLogs on u.email equals log.purchasedBy
                                    group new { u, log } by new { u.email, u.phone, u.username, u.FirstName, u.fullname } into g
                                    orderby g.Key.email ascending
                                    select new userstb
                                    {
                                        email = g.Key.email,
                                        phone = g.Key.phone,
                                        username = g.Key.username,
                                        FirstName = g.Key.FirstName,
                                        fullname = g.Key.fullname,
                                    }).OrderBy(d => d.email).Skip(pageIndex).Take(pageSize).ToListAsync();
                return result;
            }
            else if (audienceType == "gft")
            {

                var result = await (from ct in _context.userstb
                                    join gf in _context.GiftCardOrders
                                    on ct.email equals gf.UserId
                                    group new { ct, gf } by new { ct.email, ct.FirstName, ct.username, ct.fullname, ct.phone, gf.UserId } into g
                                    orderby g.Key.email ascending
                                    select new userstb
                                    {
                                        email = g.Key.email,
                                        phone = g.Key.phone,
                                        username = g.Key.username,
                                        FirstName = g.Key.FirstName,
                                        fullname = g.Key.fullname,
                                    }
                                    ).OrderBy(c => c.email).Skip(pageIndex).Take(pageSize).ToListAsync();
                return result;
            }
            else if (audienceType == "ven")
            {
                //Select * From userstb where mgr_code ='Vendor'
                var result = await (from ct in _context.userstb
                                    where ct.mgr_code == "Vendor"
                                    select ct).OrderBy(c => c.email).Skip(pageIndex).Take(pageSize).ToListAsync();
                return result;
            }
            else throw new AppException("Invalid request");
        }
        else throw new AppException("Invalid request");

    }

    public async Task<int> CountCustomers(string audienceType)
    {
        if (!string.IsNullOrWhiteSpace(audienceType))
        {
            if (audienceType == "all")
            {
                var counts = await (from ct in _context.userstb join wl in _context.wallettb on ct.email equals wl.UserId select ct).CountAsync();
                return counts;
            }
            else if (audienceType == "allb")
            {
                var counts = await (from ct in _context.userstb
                                    join wl in _context.wallettb on
                                    ct.email equals wl.UserId
                                    where wl.Balance >= 500
                                    select ct).CountAsync();
                return counts;
            }
            else if (audienceType == "logc")
            {
                var counts = await (from u in _context.userstb
                                    join log in _context.AllLogs on u.email equals log.purchasedBy
                                    group u by new { u.email, u.username } into g
                                    orderby g.Key.email descending
                                    select new
                                    {
                                        g.Key.email,
                                        g.Key.username
                                    }).CountAsync();
                //Write Query for others
                // return await  _context.userstbs.CountAsync(x=>x.email !="");
                // = await (from ct in _context.userstbs join wl in _context.wallettb on ct.email equals wl.UserId select ct).CountAsync();
                return counts;
            }
            else if (audienceType == "ven")
            {
                var counts = await (from ct in _context.userstb

                                    where ct.mgr_code == "Vendor"
                                    select ct).CountAsync();
                return counts;
            }
            else if (audienceType == "gft")
            {

                var result = await (from ct in _context.userstb
                                    join gf in _context.GiftCardOrders
                                    on ct.email equals gf.UserId
                                    group new { ct, gf } by new { ct.email, ct.FirstName, ct.username, ct.fullname, ct.phone, gf.UserId } into g
                                    orderby g.Key.email ascending
                                    select new userstb
                                    {
                                        email = g.Key.email,
                                        phone = g.Key.phone,
                                        username = g.Key.username,
                                        FirstName = g.Key.FirstName,
                                        fullname = g.Key.fullname,
                                    }
                                    ).OrderBy(c => c.email).CountAsync();
                return result;
            }
        }
        else throw new AppException("Invalid request");
        throw new AppException("Invalid request");
    }

    public async Task SubmitCashPayment(CashPayment entity)
    {
        //To continue 
        await _context.CashPayment.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CashPayment>> SubmittedCashPayment(string userEmail)
    {
        return await _context.CashPayment.Where(d => d.WalletUserId == userEmail).ToListAsync();
    }

    public async Task<IEnumerable<CashPayment>> AllSubmittedCashPayment()
    {
        return await _context.CashPayment.ToListAsync();
    }
    public async Task SaveNewTransaction(GwalletTran entity)
    {
        await _context.GwalletTrans.AddAsync(entity);
    }
    public async Task<wallettb> GetWallet(string UserId)
    {

        return await _context.wallettb.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
    }
}
