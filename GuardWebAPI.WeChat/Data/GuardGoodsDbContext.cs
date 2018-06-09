using GuardWebAPI.WeChat.Models.Goods;
using Microsoft.EntityFrameworkCore;

namespace GuardWebAPI.WeChat.Data
{
    public class GuardGoodsDbContext : DbContext
    {
        public GuardGoodsDbContext(DbContextOptions<GuardGoodsDbContext> options) : base(options)
        { }

        public virtual DbSet<CountryArea> CountryAreas { get; set; }
    }
}