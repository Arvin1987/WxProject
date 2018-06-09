using GuardWebAPI.WeChat.Models.Goods;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Interfaces
{
    public interface IGoodsService
    {
        Task<GoodsDetails> GetDetailsAsync(long rawId);
        Task<bool> CheckStock(GoodsDetails details, int buyNum, CountryArea countryArea);
    }
}