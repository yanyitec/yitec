using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yitec.Caches
{
    public class Cache<T> : Cache,ICache<T>
    {
        public Cache(int millisecondsDelay, int millisecondsTimeout = 2000):base(millisecondsDelay,millisecondsTimeout)
        {
            
            this._Data = new SortedDictionary<string, CacheItem<T>>();
        }

        Action<Dictionary<string, T>, DateTime> _DropItems;

        public event Action<Dictionary<string, T>, DateTime> DropItems {
            add {
                lock (this.Slim) {
                    _DropItems += value;
                }
            }
            remove {
                lock (this.Slim) {
                    _DropItems -= value;
                }
            }
        }

        

        

        SortedDictionary<string, CacheItem<T>> _Data;

        Thread _Thread;
        /// <summary>
        /// 试图获取缓存里的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public long TryGetValue(string key,out T value) {
            if (this.Slim.TryEnterReadLock(this.MillisecondsTimeout))
            {
                try {
                    CacheItem<T> item;
                    if (this._Data.TryGetValue(key, out item))
                    {
                        item.ExpireTime = DateTime.Now;
                        value = item.Value;
                        return item.Version;
                    }
                    else {
                        value = default(T);
                        return 0;
                    }
                }
                finally {
                    this.Slim.ExitReadLock();
                }
                
            }
            else {
                throw new InvalidOperationException("Cannot get read lock.");
            }
        }
        
        /// <summary>
        /// 添加或更新缓存项
        /// onFlushExpireTime最后一个Datetime参数是当前缓存项的过期时间，如果是新添的缓存项，DateTime.Min会被传递进去
        /// </summary>
        /// <param name="key">查找关键key</param>
        /// <param name="value">缓存的值</param>
        /// <param name="removing">在被移除缓存前调用该代理。如果返回false则不会从缓存中移除。可以为空</param>
        /// <param name="flushExpire">更新过期时间时调用,nullable。最后一个Datetime参数是当前缓存项的过期时间，如果是新添的缓存项，DateTime.Min会被传递进去</param>
        /// <returns></returns>
        public long AddOrUpdate(string key, T value, Func<string,T,long, DateTime, bool> removing = null, Func<string, T,long, DateTime, DateTime> flushExpire = null)
        {
            if (this.Slim.TryEnterWriteLock(this.MillisecondsTimeout)) {
                var now = DateTime.Now;
                try
                {
                    CacheItem<T> item;
                    if (this._Data.TryGetValue(key, out item))
                    {
                        if (item.FlushExpire != null) item.ExpireTime = item.FlushExpire(key, item.Value, item.Version, item.ExpireTime);
                        else item.ExpireTime = now.AddMilliseconds(this.MillisecondsDelay);
                        item.Value = value;
                        return item.Version;
                    }
                    else
                    {
                        
                        var version = now.Ticks;
                        item = new CacheItem<T>() {
                            Version = version,
                            //ExpireTime = now.AddMilliseconds(this.MillisecondsDelay),
                            FlushExpire = flushExpire,
                            DropCheck = removing,
                            Value = value
                        };
                        if (flushExpire != null) item.ExpireTime = flushExpire(key, value,version, DateTime.MinValue); 
                        else item.ExpireTime = now.AddMilliseconds(this.MillisecondsDelay);
                        this._Data.Add(key,item);
                        return version;
                    }
                }
                finally
                {
                    this.Slim.ExitWriteLock();
                }
            } else {
                throw new InvalidOperationException("Cannot get write lock.");
            }
        }

        /// <summary>
        /// 从缓存中移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onRemoved">移除完成后回调该函数</param>
        /// <returns></returns>
        public bool Remove(string key, Action<string,T,long, DateTime> onRemoved=null) {
            bool isRemoved = false;
            CacheItem<T> item;
            if (this.Slim.TryEnterReadLock(this.MillisecondsTimeout))
            {
                try
                {
                    
                    if (this._Data.TryGetValue(key, out item))
                    {
                        if (item.DropCheck != null && !item.DropCheck(key,item.Value,item.Version,item.ExpireTime))
                        {
                            return false;
                        }
                        this._Data.Remove(key);
                        isRemoved = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    this.Slim.ExitReadLock();
                }

            }
            else
            {
                throw new InvalidOperationException("Cannot get read lock.");
            }

            if (isRemoved && onRemoved != null) {
                onRemoved(key, item.Value, item.Version, item.ExpireTime);
            }

            lock (this.Slim) {
                if (this._DropItems != null) {
                    var items = new Dictionary<string, T>();
                    items.Add(key,item.Value);
                    this._DropItems(items,DateTime.MinValue);
                }
            }
            return true;
        }

        public bool RemoveDirectly(string key) {
            if (this.Slim.TryEnterReadLock(this.MillisecondsTimeout))
            {
                try
                {

                    return this._Data.Remove(key);
                }
                finally
                {
                    this.Slim.ExitReadLock();
                }

            }
            else
            {
                throw new InvalidOperationException("Cannot get read lock.");
            }
        }


        public int MillisecondsCheckExpire { get; private set; }

        

        public override void FlushExpire(DateTime flushTime) {
            var result = new Dictionary<string, T>();
            if (this.Slim.TryEnterReadLock(this.MillisecondsTimeout))
            {
                try
                {
                    var newData = new SortedDictionary<string, CacheItem<T>>();
                    foreach (var pair in this._Data) {
                        var key = pair.Key;
                        var item = pair.Value;
                        if (item.ExpireTime < flushTime)
                        {
                            if (item.DropCheck != null && !item.DropCheck(key, item.Value, item.Version, item.ExpireTime))
                            {
                                newData.Add(key, item);
                                continue;
                            }
                            result.Add(key, item.Value);
                        }
                        else newData.Add(key,item);
                    }

                    this._Data = newData;
                    
                }
                finally
                {
                    this.Slim.ExitReadLock();
                }

            }
            else
            {
                throw new InvalidOperationException("Cannot get read lock.");
            }

            lock (this.Slim)
            {
                if (this._DropItems != null)
                {
                    this._DropItems(result, flushTime);
                }
            }
        }
    }
}
