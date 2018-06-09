using GSBase.ConfigCenter;
using GuardWebAPI.WeChat.Interfaces;
using GuardWebAPI.WeChat.Models.Goods;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Services.Goods
{
    public class JDGoodsService : IGoodsService
    {
        public async Task<GoodsDetails> GetDetailsAsync(long rawId)
        {
            HttpWebRequest request = WebRequest.CreateHttp($"https://item.m.jd.com/product/{ rawId }.html");
            request.UserAgent = ConfigCenterClient.GetSection("UserAgent").GetValue<string>("Chrome");

            using (WebResponse response = await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string html = reader.ReadToEnd();

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                // 商品名称
                HtmlNode goodNameHtmlNode = document.DocumentNode.SelectSingleNode("//*[@id=\"goodName\"]");
                string goodsName = goodNameHtmlNode.GetAttributeValue("value", string.Empty);
                if (string.IsNullOrWhiteSpace(goodsName))
                {
                    return null;
                }

                // 京东售卖价格
                HtmlNode priceHtmlNode = document.DocumentNode.SelectSingleNode("//*[@id=\"jdPrice\"]");
                string priceText = priceHtmlNode.GetAttributeValue("value", string.Empty);
                bool stopSelling = false;
                if (priceText == "暂无报价")
                {
                    stopSelling = true;
                }
                if (!decimal.TryParse(priceText, out decimal price))
                {
                    // TODO: add logs.
                }

                // 商品分类
                HtmlNode categoryIdHtmlNode = document.DocumentNode.SelectSingleNode("//*[@id=\"categoryId\"]");
                string categoryId = categoryIdHtmlNode.GetAttributeValue("value", string.Empty);

                // 图片
                HtmlNode specImageHtmlNode = document.DocumentNode.SelectSingleNode("//*[@id=\"spec_image\"]");
                string specImage = specImageHtmlNode.GetAttributeValue("src", string.Empty);

                return new GoodsDetails
                {
                    Id = 1,
                    RawId = rawId,
                    Channel = ChannelType.JD,
                    Name = goodsName,
                    SalePrice = price,
                    StopSelling = stopSelling,
                    Image = specImage,
                    JDCategoryIds = categoryId?.Split('_').Select(ci =>
                    {
                        if (long.TryParse(ci, out long result))
                        {
                            return result;
                        }

                        return 0;
                    }).ToArray()
                };
            }
        }

        public async Task<bool> CheckStock(GoodsDetails details, int buyNum, CountryArea countryArea)
        {
            if (details == null || countryArea == null || details.StopSelling)
            {
                return false;
            }

            string area = string.Empty;
            GetArea(countryArea, ref area);

            WebClient webClient = new WebClient();
            string json = await webClient.DownloadStringTaskAsync($"https://c0.3.cn/stock?skuId={ details.RawId }&area={ area }&cat={ string.Join(',', details.JDCategoryIds) }&buyNum={ buyNum }&extraParam={{%22originid%22:%221%22}}");
            JDStockResponse response = JsonConvert.DeserializeObject<JDStockResponse>(json);
            return response.Stock.StockState == 33;
        }

        private void GetArea(CountryArea countryArea, ref string area)
        {
            if (countryArea == null)
            {
                return;
            }

            area = countryArea.JDCode.ToString() + "_" + area;
            if (countryArea.Parent != null)
            {
                GetArea(countryArea.Parent, ref area);
            }

            area = area.TrimEnd('_');
        }
    }

    public class JDStockResponse
    {
        public JDStockData Stock { get; set; }
    }

    public class JDStockData
    {
        public int StockState { get; set; }
        public string StockStateName { get; set; }
    }
}