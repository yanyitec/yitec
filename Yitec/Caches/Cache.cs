using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yitec.Caches
{
    public abstract class Cache
    {
        public Cache(int millisecondsDelay , int millisecondsTimeout=2000) {
            this.Slim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this.MillisecondsDelay = millisecondsDelay;
            this.MillisecondsTimeout = millisecondsTimeout;
        }
        public ReaderWriterLockSlim Slim { get; private set; }

        public int MillisecondsTimeout { get; private set; }

        public int MillisecondsDelay { get; private set; }

        public abstract void FlushExpire(DateTime flushTime);
    }
}
