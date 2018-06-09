using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Models.ActivityInfo
{
    public class WxCodeInfo
    {
        /// <summary>
        /// 最大32个可见字符，只支持数字，大小写英文以及部分特殊字符：!#$&'()*+,/:;=?@-._~，其它字符请自行编码为合法字符（因不支持%，中文无法使用 urlencode 处理，请使用其他编码方式）
        /// </summary>
        public string scene { get; set; }// String   
        /// <summary>
        /// 必须是已经发布的小程序存在的页面（否则报错），例如 "pages/index/index" ,根路径前不要填加'/',不能携带参数（参数请放在scene字段里），如果不填写这个字段，默认跳主页面
        /// </summary>
        public string page { get; set; }// String    
        /// <summary>
        /// 二维码的宽度
        /// </summary>
        public int width { get; set; } = 430;// Int	430	
        public bool auto_color { get; set; } = false;//  Bool	false	自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调
        public object line_color { get; set; } = new { r = "0", g = "0", b = "0" };//  Object	{"r":"0","g":"0","b":"0"}为 false 时生效，使用 rgb 设置颜色 例如 {"r":"xxx","g":"xxx","b":"xxx"} 十进制表示
        public bool is_hyaline { get; set; } = false;// Bool	false	是否需要透明底色， is_hyaline 为true时，生成透明底色的小程序码
    }
}
