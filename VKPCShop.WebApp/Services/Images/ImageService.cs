using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VKPCShop.Data.EF;
using VKPCShop.WebApp.Dtos;

namespace VKPCShop.WebApp.Services.Images
{
    public class ImageService: IImageService
    {
        private readonly VKPCShopDbContext _context;
        private readonly IMapper _mapper;
        public ImageService( VKPCShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ImageDto>> GetImagesByProductId(int id)
        {
            var images = await _context.Images.Where(x => x.ProductId == id).ToListAsync();
            if(images != null)
            {
                var imagesDto = _mapper.Map<List<ImageDto>>(images);
                return imagesDto;
            }
            return null;
        }
    }
}
