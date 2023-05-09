using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Images
{
    public interface IImageService
    {
        Task<List<ImageDto>> GetImagesByProductId(int id);
    }
}
