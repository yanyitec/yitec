using System;
namespace Yitec.Caches
{
    public interface ICache<T>
    {
        long AddOrUpdate(string key, T value, Func<string, T, long, DateTime, bool> removing = null, Func<string, T, long, DateTime, DateTime> flushExpire = null);
        event Action<System.Collections.Generic.Dictionary<string, T>, DateTime> DropItems;
        void FlushExpire(DateTime flushTime);
        int MillisecondsCheckExpire { get; }
        int MillisecondsDelay { get; }
        int MillisecondsTimeout { get; }
        bool Remove(string key, Action<string, T, long, DateTime> onRemoved = null);
        bool RemoveDirectly(string key);
        System.Threading.ReaderWriterLockSlim Slim { get; }
        long TryGetValue(string key, out T value);
    }
}
