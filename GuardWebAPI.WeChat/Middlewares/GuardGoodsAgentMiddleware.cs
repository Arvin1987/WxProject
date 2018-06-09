using GuardWebAPI.WeChat.Data;
using GuardWebAPI.WeChat.Domain;
using GuardWebAPI.WeChat.Interfaces;
using GuardWebAPI.WeChat.Models.Goods;
using GuardWebAPI.WeChat.Services.Goods;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GuardWebAPI.WeChat.Middlewares
{
    public class GuardGoodsAgentMiddleware
    {
        private readonly RequestDelegate _next;
        private IMemoryCache _memoryCache;
        private GuardGoodsDbContext _dbContext;
        private GoodsProcessing _goodsProcessing;
        private Stopwatch _stopwatch;
        private long _latestElapsed;

        public GuardGoodsAgentMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext context, GuardGoodsDbContext dbContext)
        {
            _dbContext = dbContext;
            _goodsProcessing = new GoodsProcessing(dbContext);

            if (context.WebSockets.IsWebSocketRequest)
            {
                #region 解析参数
                if (!context.Request.Query.ContainsKey("action"))
                {
                    context.Response.StatusCode = 400;
                }
                GoodsPutRequest @params = null;
                if (context.Request.Query.TryGetValue("params", out StringValues value))
                {
                    @params = JsonConvert.DeserializeObject<GoodsPutRequest>(HttpUtility.UrlDecode(value.ToString()));
                }
                #endregion

                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                _stopwatch = new Stopwatch();
                _stopwatch.Start();
                _latestElapsed = 0;
                await Put(context, webSocket, @params);
                _stopwatch.Stop();
            }
            else
            {
                context.Response.StatusCode = 400;
                return;
            }
        }

        private async Task Put(HttpContext context, WebSocket webSocket, GoodsPutRequest @params)
        {
            #region Get Details
            object detailsCacheKey = new { @params.Channel, @params.RawId };
            IGoodsService goodsService;
            switch (@params.Channel)
            {
                case ChannelType.JD:
                    goodsService = new JDGoodsService();
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (@params.Latest || !_memoryCache.TryGetValue(detailsCacheKey, out GoodsDetails details))
            {
                details = await goodsService.GetDetailsAsync(@params.RawId);
                if (details == null)
                {
                    await SendAsync(webSocket, "details", null);
                    return;
                }

                _memoryCache.Set(detailsCacheKey, details, DateTimeOffset.Now.AddHours(1));
            }
            await SendAsync(webSocket, "details", details);
            #endregion

            object stockCacheKey = new { @params.Channel, @params.RawId, @params.Province, @params.City, @params.District };
            if (_memoryCache.TryGetValue(stockCacheKey, out List<StockInfo> results))
            {
                await SendAsync(webSocket, "stock", @params.All ? results : results.Where(s => s.State == 1));
                return;
            }

            results = new List<StockInfo>();
            if (!string.IsNullOrWhiteSpace(@params.Province))
            {
                CountryArea provinceCountryArea = _dbContext.CountryAreas.FirstOrDefault(ca => ca.Name.Contains(@params.Province) && ca.ParentId == 0);
                if (@params.City == "全部")
                {
                    List<CountryArea> cityCountryAreaList = _dbContext.CountryAreas.Where(ca => ca.ParentId == provinceCountryArea.Id).ToList();
                    foreach (CountryArea cityCountryArea in cityCountryAreaList)
                    {
                        await GetStockInfoAsync(webSocket, results, provinceCountryArea, cityCountryArea, goodsService, details, @params);
                    }
                }
                else
                {
                    CountryArea cityCountryArea = _dbContext.CountryAreas.FirstOrDefault(ca => ca.ParentId == provinceCountryArea.Id && ca.Name.Contains(@params.City));
                    await GetStockInfoAsync(webSocket, results, provinceCountryArea, cityCountryArea, goodsService, details, @params);
                }
            }

            if ((@params.All && results.Count <= 0) || (!@params.All && results.Count(s => s.State == 1) <= 0))
            {
                await SendAsync(webSocket, "stock", null);
            }

            if (results.Count > 0)
            {
                _memoryCache.Set(stockCacheKey, results);
            }

            await SendAsync(webSocket, "finshed", null);
        }

        private async Task GetStockInfoAsync(WebSocket webSocket, List<StockInfo> results, CountryArea provinceCountryArea, CountryArea cityCountryArea, IGoodsService goodsService, GoodsDetails details, GoodsPutRequest @params)
        {
            cityCountryArea.Parent = provinceCountryArea;
            IEnumerable<StockInfo> stockInfo = await _goodsProcessing.GetDistrictStock(cityCountryArea, goodsService, details, @params.BuyNum);
            results.AddRange(stockInfo);

            if (!@params.All)
            {
                stockInfo = stockInfo.Where(s => s.State == 1);
            }

            if (stockInfo?.Count() > 0)
            {
                await SendAsync(webSocket, "stock", stockInfo);
            }
        }

        private Task SendAsync(WebSocket webSocket, string type, object data)
        {
            long elapsed = _stopwatch.ElapsedMilliseconds;
            Task task = webSocket.SendAsync(GoodsResponse.GetArraySegment(type, data, elapsed - _latestElapsed), WebSocketMessageType.Text, true, CancellationToken.None);
            _latestElapsed = elapsed;
            return task;
        }
    }

    public class GoodsResponse
    {
        public GoodsResponse()
        { }

        public GoodsResponse(string type, object data, long elapsed)
        {
            Type = type;
            Data = data;
            Elapsed = elapsed;
        }

        public string Type { get; set; }
        public object Data { get; set; }
        public long Elapsed { get; set; }

        public ArraySegment<byte> GetArraySegment()
        {
            byte[] detailsBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            return new ArraySegment<byte>(detailsBuffer);
        }

        public static ArraySegment<byte> GetArraySegment(string type, object data, long elapsed)
        {
            GoodsResponse response = new GoodsResponse(type, data, elapsed);
            return response.GetArraySegment();
        }
    }
}