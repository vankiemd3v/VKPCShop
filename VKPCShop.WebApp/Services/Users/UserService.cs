using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VKPCShop.Data.EF;
using VKPCShop.Data.Entities;
using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Users
{
    public class UserService: IUserService
    {
        private readonly VKPCShopDbContext _context;
        private readonly IMapper _mapper;
        public UserService(VKPCShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UserDto> Create(UserDto dto)
        {
            var user = await _context.Users.Where(x=>x.Email == dto.Email).SingleOrDefaultAsync();
            if (user == null)
            {
                user = new User()
                {
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    Image = dto.Image,
                    RoleId = 2
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
            }
            if(user.Image != dto.Image)
            {
                user.Image = dto.Image;
                _context.Users.UpdateRange(user);
                await _context.SaveChangesAsync();
            }
            if (user.FullName != dto.FullName)
            {
                user.FullName = dto.FullName;
                _context.Users.UpdateRange(user);
                await _context.SaveChangesAsync();
            }
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;               
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _context.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }
        public async Task<UserDto> FindByEmail(string email)
        {
            var user = await _context.Users.Where(x => x.Email == email).SingleOrDefaultAsync();
            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<bool> Update(UserDto dto)
        {
            var user = await _context.Users.Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (user != null)
            {
                user.FullName = dto.FullName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address;
                _context.Users.UpdateRange(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<int> CountUser()
        {
            return await _context.Users.CountAsync();
        }
    }
}
