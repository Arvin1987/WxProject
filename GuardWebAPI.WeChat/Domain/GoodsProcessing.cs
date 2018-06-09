using GuardWebAPI.WeChat.Data;
using GuardWebAPI.WeChat.Interfaces;
using GuardWebAPI.WeChat.Models.Goods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Domain
{
    public class GoodsProcessing
    {
        private GuardGoodsDbContext _dbContext;

        public GoodsProcessing(GuardGoodsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<StockInfo>> GetDistrictStock(CountryArea cityCountryArea, IGoodsService goodsService, GoodsDetails details, int buyNum)
        {
            List<StockInfo> results = new List<StockInfo>();
            List<CountryArea> searchCountryAreaList = _dbContext.CountryAreas.Where(ca => ca.ParentId == cityCountryArea.Id).ToList();
            foreach (CountryArea item in searchCountryAreaList)
            {
                item.Parent = cityCountryArea;
                bool searchStockFlag = await goodsService.CheckStock(details, buyNum, item);
                results.Add(new StockInfo
                {
                    Address = item.ToString(),
                    State = searchStockFlag ? 1 : 0
                });
            }

            return results;
        }
    }
}