using System.Collections.Generic;
using UnityEngine;

public static class GenericEvent<T> where T : class, new()
{
    private static Dictionary<string, T> _eventMap = new Dictionary<string, T>();

    public static T GetEvent(string channel)
    {
        _eventMap.TryAdd(channel, new T());
        return _eventMap[channel];
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetEvents()
    {
        _eventMap.Clear();
    }
}
