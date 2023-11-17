using Inveon.Models.DTOs;
using Inveon.Web.Models;

namespace Inveon.Web.Services.IServices
{
    public interface ICartService : IBaseService
    {
        Task<T> GetCartByUserIdAsnyc<T>(string userId, string token);
        Task<T> AddToCartAsync2<T>(CartDetailedDto cartDto, string token);
        Task<T> UpdateCartAsync<T>(CartDetailedDto cartDto, string token);
        Task<T> RemoveFromCartAsync<T>(int cartId, string token);
        Task<T> ApplyCoupon<T>(CartDetailedDto cartDto, string token);
        Task<T> RemoveCoupon<T>(string userId, string token);

        Task<T> Checkout<T>(CartHeaderDetailedDto cartHeader, string token);
    }

}
