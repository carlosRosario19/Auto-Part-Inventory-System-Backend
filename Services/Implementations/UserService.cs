using AutoMapper;
using AutoPartInventorySystem.DTOs;
using AutoPartInventorySystem.DTOs.User;
using AutoPartInventorySystem.Models;
using AutoPartInventorySystem.Repositories.Contracts;
using AutoPartInventorySystem.Services.Contracts;
using AutoPartInventorySystem.Util;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoPartInventorySystem.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration, 
            PasswordHasher passwordHasher,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(AddUserDto addUserDto)
        {
            // 1) Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(addUserDto.Email);
            if (existingUser != null)
            {
                return false; // email already taken
            }

            // 2) Map DTO → Entity using AutoMapper
            var user = _mapper.Map<User>(addUserDto);

            // 3) Hash the password manually
            user.PasswordHash = _passwordHasher.Hash(addUserDto.Password);

            // Assign staff role by default
            var staffRole = await _roleRepository.GetByNameAsync("staff");
            if (staffRole != null)
                user.Roles.Add(staffRole);

            // 4) Save user
            await _userRepository.AddAsync(user);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return false;

            // Proceed to delete
            await _userRepository.DeleteAsync(user);

            return true;
        }

        public async Task<PagedResult<UserDTO>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var (users, totalCount) = await _userRepository.GetAllUsersPagedAsync(pageNumber, pageSize);

            var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users);

            return new PagedResult<UserDTO>
            {
                Items = userDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<string?> LoginAsync(LoginDto loginDto)
        {
            // 1) Get user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
                return null;

            // 2) Verify password
            bool valid = _passwordHasher.Verify(loginDto.Password, user.PasswordHash);
            if (!valid)
                return null;

            // 3) Create JWT token
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            // Add roles (optional)
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> PromoteToAdminAsync(int id)
        {
            // Find the user
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Get admin role
            var adminRole = await _roleRepository.GetByNameAsync("admin");
            if (adminRole == null)
                throw new Exception("Admin role does not exist in the database.");

            // Update user role
            user.Roles.Add(adminRole);

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<UpdateUserResult> UpdateAsync(UpdateUserDto updateUserDto)
        {
            //Validate the user exists
            // 1) Validate that the user exists
            var user = await _userRepository.GetByIdAsync(updateUserDto.Id);
            if (user == null)
                return UpdateUserResult.NotFound;

            // 2) Check if the new email is already taken by someone else
            var existingUserWithEmail = await _userRepository.GetByEmailAsync(updateUserDto.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                return UpdateUserResult.EmailAlreadyExists;


            // 3) AutoMapper
            _mapper.Map(updateUserDto, user);

            // 4) Hash the new password
            user.PasswordHash = _passwordHasher.Hash(updateUserDto.Password);

            // 5) Update the user
            await _userRepository.UpdateAsync(user);

            return UpdateUserResult.Success;
        }
    }
}
