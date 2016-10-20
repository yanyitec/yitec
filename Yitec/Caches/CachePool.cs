using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yitec.Caches
{
    public class CachePool
    {

        private Thread _Thread;
        private ConcurrentDictionary<string, Cache> _Containers;

        public CachePool(int millisecondsFlushExpire)
        {
            this.MillisecondsFlushExpire = millisecondsFlushExpire;
            this._Containers = new ConcurrentDictionary<string, Cache>();
            this._Thread = new Thread( new ThreadStart(this.InternalCheckExpire));
        }

        public int MillisecondsFlushExpire { get; private set; }
        private void InternalCheckExpire()
        {
            while (true)
            {
                var now = DateTime.Now;
                this.FlushContainers(now);
                Thread.Sleep(this.MillisecondsFlushExpire);
            }
        }

        public void FlushContainers(DateTime checkTime)
        {
            foreach (var pair in this._Containers)
            {
                pair.Value.FlushExpire(checkTime);
            }
            
        }

        public ICache<T> Add<T>(string name, int millisecondsDelay, int millisecondsTimeout = 2000) {
            var container = new Cache<T>(millisecondsDelay, millisecondsTimeout);
            lock (_Thread)
            {
                if (this._Containers.TryAdd(name, container))
                {
                    return container;
                }
                else {
                    throw new Exception("Cannot add new Cache to pool,because the concurrentDic is rejected.");
                }
            }
        }

        public bool Remove(string name) {
            lock (this._Thread) {
                Cache result = null;
                return this._Containers.TryRemove(name,out result);
            }
        }

        public ICache<T> Get<T>(string name) {
            lock (this._Thread)
            {
                Cache result = null;
                if (this._Containers.TryGetValue(name, out result))
                {
                    return result as ICache<T>;
                }
                else 
                {
                    return null;
                }
            }
        }

        public readonly static CachePool Default = new CachePool(363213);
    }
}
