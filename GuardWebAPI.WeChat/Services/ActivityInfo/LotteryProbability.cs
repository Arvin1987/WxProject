using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuardWebAPI.WeChat.Services.ActivityInfo
{
    public class LotteryProbability
    {
        private static Random rnd = new Random();

        /// <summary>
        /// 获取抽奖结果
        /// </summary>
        /// <param name="prob">各物品的抽中概率</param>
        /// <returns>返回抽中的物品所在数组的位置</returns>
        public static int Get(IEnumerable<int> prob)
        {
            int result = 0;
            int n = (int)(prob.Sum() * 10000);           //计算概率总和，放大1000倍
            Random r = rnd;
            float x = (float)r.Next(0, n) / 10000;       //随机生成0~概率总和的数字

            for (int i = 0; i < prob.Count(); i++)
            {
                float pre = prob.Take(i).Sum();         //区间下界
                float next = prob.Take(i + 1).Sum();    //区间上界
                if (x >= pre && x < next)               //如果在该区间范围内，就返回结果退出循环
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public static int GetRondom()
        {
            int[] a = new int[] { 2, 3, 4 };
            Random r = rnd;
            int index = rnd.Next(a.Length);
            return a[index];
        }
    }
}
