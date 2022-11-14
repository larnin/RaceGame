using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SavedData
{
    static SavedData m_instance = null;
    public static SavedData instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new SavedData();
            return m_instance;
        }
    }

    Dictionary<string, int> m_ints = new Dictionary<string, int>();
    Dictionary<string, float> m_floats = new Dictionary<string, float>();
    Dictionary<string, string> m_strings = new Dictionary<string, string>();

    public void Clear()
    {
        m_ints.Clear();
        m_floats.Clear();
        m_strings.Clear();
    }

    public void Set(string key, int value)
    {
        m_ints[key] = value;
    }

    public void Set(string key, float value)
    {
        m_floats[key] = value;
    }

    public void Set(string key, string value)
    {
        m_strings[key] = value;
    }

    const string _x = ".x";
    const string _y = ".y";
    const string _z = ".z";

    public void Set(string key, Vector2 value)
    {
        Set(key + _x, value.x);
        Set(key + _y, value.y);
    }

    public void Set(string key, Vector2Int value)
    {
        Set(key + _x, value.x);
        Set(key + _y, value.y);
    }

    public void Set(string key, Vector3 value)
    {
        Set(key + _x, value.x);
        Set(key + _y, value.y);
        Set(key + _z, value.z);
    }

    public void Set(string key, Vector3Int value)
    {
        Set(key + _x, value.x);
        Set(key + _y, value.y);
        Set(key + _z, value.z);
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        int value = defaultValue;
        if (m_ints.TryGetValue(key, out value))
            return value;
        return defaultValue;
    }

    public float GetFloat(string key, float defaultValue = 0)
    {
        float value = defaultValue;
        if (m_floats.TryGetValue(key, out value))
            return value;
        return defaultValue;
    }

    public string GetString(string key, string defaultValue = "")
    {
        string value = defaultValue;
        if (m_strings.TryGetValue(key, out value))
            return value;
        return defaultValue;
    }

    public Vector2 GetVector2(string key) { return GetVector2(key, Vector2.zero); }
    public Vector2 GetVector2(string key, Vector2 defaultValue)
    {
        return new Vector2(GetFloat(key + _x, defaultValue.x), GetFloat(key + _y, defaultValue.y));
    }

    public Vector2Int GetVector2Int(string key) { return GetVector2Int(key, Vector2Int.zero); }
    public Vector2Int GetVector2Int(string key, Vector2Int defaultValue)
    {
        return new Vector2Int(GetInt(key + _x, defaultValue.x), GetInt(key + _y, defaultValue.y));
    }

    public Vector3 GetVector3(string key) { return GetVector3(key, Vector3.zero); }
    public Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        return new Vector3(GetFloat(key + _x, defaultValue.x), GetFloat(key + _y, defaultValue.y), GetFloat(key + _z, defaultValue.z));
    }

    public Vector3Int GetVector3Int(string key) { return GetVector3Int(key, Vector3Int.zero); }
    public Vector3Int GetVector3Int(string key, Vector3Int defaultValue)
    {
        return new Vector3Int(GetInt(key + _x, defaultValue.x), GetInt(key + _y, defaultValue.y), GetInt(key + _z, defaultValue.z));
    }
}
