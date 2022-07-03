using System;
using System.Collections;
using System.Collections.Generic;

public class CacheDiscardList
{
    private List<WeakReference> cacheList = new();

    public void Add(Cache cache)
    {
        cacheList.Add(new WeakReference(cache));
    }

    public void DiscardAll()
    {
        for (int i = cacheList.Count - 1; i >= 0; i--)
        {
            WeakReference cacheRef = cacheList[i];
            Cache cache = cacheRef.Target as Cache;
            if (cache == null)
            {
                cacheList.RemoveAt(i);
            }
            else
            {
                cache.Discard();
            }
        }
    }
}
