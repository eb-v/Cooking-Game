using System.Collections.Generic;
using UnityEngine;

public static class GenericEvent<T> where T : class, new()
{
    private static Dictionary<int, T> _eventMap = new Dictionary<int, T>();

    public static T GetEvent(int id)
    {
        _eventMap.TryAdd(id, new T());
        return _eventMap[id];
    }
}
