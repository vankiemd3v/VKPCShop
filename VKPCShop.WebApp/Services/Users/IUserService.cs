using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Users
{
    public interface IUserService
    {
        Task<UserDto> Create(UserDto dto); 
        Task<UserDto> GetById(int id);
        Task<UserDto> FindByEmail(string email);
        Task<bool> Update(UserDto dto);
        Task<int> CountUser();
    }
}
