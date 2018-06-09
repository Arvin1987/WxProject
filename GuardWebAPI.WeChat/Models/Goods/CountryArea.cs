using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuardWebAPI.WeChat.Models.Goods
{
    [Table("countryarea")]
    public class CountryArea
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ParentId { get; set; }
        public AreaLevel Level { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public long BaiduCode { get; set; }
        public long JDCode { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual CountryArea Parent { get; set; }

        [NotMapped]
        public string Address { get; set; }

        public override string ToString()
        {
            string name = string.Empty;
            GetName(this, ref name);
            return name;
        }

        private void GetName(CountryArea countryArea, ref string name)
        {
            if (countryArea == null)
            {
                return;
            }

            name = countryArea.Name + name;
            if (countryArea.Parent != null)
            {
                GetName(countryArea.Parent, ref name);
            }
        }
    }

    /// <summary>
    /// 区域级别
    /// </summary>
    public enum AreaLevel
    {
        /// <summary>
        /// 国家
        /// </summary>
        Country = 1,
        /// <summary>
        /// 省份
        /// </summary>
        Province = 2,
        /// <summary>
        /// 城市
        /// </summary>
        City = 3,
        /// <summary>
        /// 区县
        /// </summary>
        District = 4
    }
}