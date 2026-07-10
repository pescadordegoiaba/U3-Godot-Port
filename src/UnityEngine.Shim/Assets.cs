using System.Text;

namespace UnityEngine;

public class ScriptableObject : Object
{
    public static T CreateInstance<T>()
        where T : ScriptableObject, new()
    {
        return new T();
    }

    public static ScriptableObject CreateInstance(Type type)
    {
        if (!typeof(ScriptableObject).IsAssignableFrom(type))
        {
            throw new ArgumentException("Type must derive from ScriptableObject.", nameof(type));
        }

        return (ScriptableObject)Activator.CreateInstance(type)!;
    }
}

public class TextAsset : Object
{
    private readonly byte[] _bytes;

    public TextAsset()
        : this(string.Empty)
    {
    }

    public TextAsset(string text)
    {
        this.text = text;
        _bytes = Encoding.UTF8.GetBytes(text);
    }

    public string text { get; }

    public byte[] bytes => _bytes.ToArray();
}

public static class Resources
{
    private static readonly Dictionary<string, List<Object>> Registered = new(StringComparer.Ordinal);

    public static T? Load<T>(string path)
        where T : Object
    {
        return Registered.TryGetValue(path, out var objects)
            ? objects.OfType<T>().FirstOrDefault()
            : null;
    }

    public static Object? Load(string path)
    {
        return Registered.TryGetValue(path, out var objects) ? objects.FirstOrDefault() : null;
    }

    public static T[] LoadAll<T>(string path)
        where T : Object
    {
        return Registered.TryGetValue(path, out var objects)
            ? objects.OfType<T>().ToArray()
            : Array.Empty<T>();
    }

    public static void UnloadAsset(Object asset)
    {
    }

    public static void Register(string path, Object obj)
    {
        if (!Registered.TryGetValue(path, out var objects))
        {
            objects = new List<Object>();
            Registered.Add(path, objects);
        }

        objects.Add(obj);
    }

    public static void ClearRegistered()
    {
        Registered.Clear();
    }
}

public enum RuntimePlatform
{
    WindowsPlayer,
    OSXPlayer,
    LinuxPlayer,
    WindowsEditor,
    OSXEditor,
    LinuxEditor
}

public static class Application
{
    public static string dataPath { get; set; } = AppContext.BaseDirectory;

    public static string persistentDataPath { get; set; } = Path.Combine(Path.GetTempPath(), "U3GodotPort", "Persistent");

    public static string streamingAssetsPath { get; set; } = Path.Combine(dataPath, "StreamingAssets");

    public static string temporaryCachePath { get; set; } = Path.Combine(Path.GetTempPath(), "U3GodotPort", "Cache");

    public static bool isPlaying { get; set; } = true;

    public static RuntimePlatform platform { get; set; } = RuntimePlatform.LinuxPlayer;

    public static void Quit()
    {
    }
}

public static class PlayerPrefs
{
    private static readonly Dictionary<string, string> Strings = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, int> Ints = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, float> Floats = new(StringComparer.Ordinal);

    public static string GetString(string key, string defaultValue = "")
    {
        return Strings.GetValueOrDefault(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        Strings[key] = value;
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return Ints.GetValueOrDefault(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        Ints[key] = value;
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return Floats.GetValueOrDefault(key, defaultValue);
    }

    public static void SetFloat(string key, float value)
    {
        Floats[key] = value;
    }

    public static bool HasKey(string key)
    {
        return Strings.ContainsKey(key) || Ints.ContainsKey(key) || Floats.ContainsKey(key);
    }

    public static void DeleteKey(string key)
    {
        Strings.Remove(key);
        Ints.Remove(key);
        Floats.Remove(key);
    }

    public static void DeleteAll()
    {
        Strings.Clear();
        Ints.Clear();
        Floats.Clear();
    }

    public static void Save()
    {
    }
}

public class AssetBundle : Object
{
    private readonly List<Object> _assets = new();

    public T? LoadAsset<T>(string name)
        where T : Object
    {
        return _assets.OfType<T>().FirstOrDefault(asset => asset.name == name);
    }

    public T[] LoadAllAssets<T>()
        where T : Object
    {
        return _assets.OfType<T>().ToArray();
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        if (unloadAllLoadedObjects)
        {
            _assets.Clear();
        }
    }

    public void RegisterAsset(Object asset)
    {
        _assets.Add(asset);
    }
}
