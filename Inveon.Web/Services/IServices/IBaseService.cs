using Inveon.Web.Models;
using Inveon.Models;
using Inveon.Models.DTOs;

namespace Inveon.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
