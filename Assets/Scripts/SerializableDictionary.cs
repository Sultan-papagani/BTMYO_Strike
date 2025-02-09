using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary : ISerializationCallbackReceiver
{
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<Sprite> values = new List<Sprite>();

    private Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();

    public Dictionary<string, Sprite> ToDictionary() => dictionary;

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary = new Dictionary<string, Sprite>();
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    public void Add(string key, Sprite value)
    {
        dictionary[key] = value;
    }

    public bool TryGetValue(string key, out Sprite value)
    {
        return dictionary.TryGetValue(key, out value);
    }
}
