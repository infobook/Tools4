using System;
using System.Collections.Generic;
using System.Text;

namespace CommandAS.Tools
{
    /// <summary>
    ///  ласс математических функций, не поддерживаемых Framework 1.1
    /// </summary>
    public class Mathematics
    {
        private Mathematics() { }

        /// <summary>
        /// ќбщепривычное округление.
        /// 2.5 округл€етс€ до 3, а не до 2! ¬ отличие от "банковского" округлени€.
        /// </summary>
        public static decimal RoundUp(decimal val)
        {
            return RoundUp(val, 0);
        }

        public static decimal RoundUp(decimal val, int digit)
        {
            //			return System.Data.SqlTypes.SqlDecimal.Round(System.Data.SqlTypes.SqlDecimal val, digit);			
            //			int tCount = Math.Pow(10, digit);
            int tCount = 1;
            for (int i = 0; i < digit; i++)
                tCount = tCount * 10;

            val = val * tCount;

            if (val - decimal.Truncate(val) >= 0.5m)
                val = decimal.Truncate(val) + 1;
            else
                val = decimal.Truncate(val);

            return val / tCount;
        }

        public static int RoundUpInt(decimal val, int precision)
        {
          // ќкругление всегда к минимальному целому числу большего исходного
          // precision показывает до какого разр€да округл€ть
          // если число precision больше, чем число разр€дов до зап€той, то возвращаем 0

          int ret = 0;
          int tCount = 1;

          for (int ii=0; ii< precision; ii++)
          {
            tCount = tCount * 10;
          }

          val = val / tCount;

          if (val >= 0.1M)
          {
            ret = Convert.ToInt32(Math.Ceiling(val));
            ret = ret * tCount;
          }

          return ret;
        }
      }
}
