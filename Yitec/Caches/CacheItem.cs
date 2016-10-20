using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Caches
{
    public struct CacheItem<T>
    {
        public long Version { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime ExpireTime { get; set; }

        internal Func<string, T,long, DateTime, DateTime> FlushExpire { get; set; }

        internal Func<string, T,long, DateTime, bool> DropCheck { get; set; }
        
        
        public T Value { get; set; }

        

    }
}
