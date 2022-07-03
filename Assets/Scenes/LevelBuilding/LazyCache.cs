using System;

public class LazyCache<T> : Cache
{
    public T Value { get 
        {
            if (!hasValue)
            {
                cached = supplier.Invoke();
                hasValue = true;
            }
            return cached;
        }
    }
    private T cached;

    private Func<T> supplier;

    private bool hasValue = false;

    public LazyCache(Func<T> supplier, params CacheDiscardList[] discardLists)
    {
        this.supplier = supplier;
        foreach (CacheDiscardList cacheList in discardLists) cacheList.Add(this);
    }

    public void Discard()
    {
        hasValue = false;
    }
}
