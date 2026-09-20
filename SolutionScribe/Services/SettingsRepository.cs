using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace SolutionScribe.Services;

internal static class SettingsRepository
{
    public enum Key
    {
        DefaultCopyrightHolder
    }

    private static readonly string _appDataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Solution Scribe");

    private static readonly string _settingsFilePath = Path.Combine(_appDataFolder, "settings.json");

    private static Dictionary<string, string> _settingsCache;

    private static void EnsureSettingsLoaded()
    {
        if (_settingsCache != null)
        {
            return;
        }

        if (!Directory.Exists(_appDataFolder))
        {
            Directory.CreateDirectory(_appDataFolder);
        }

        if (!File.Exists(_settingsFilePath))
        {
            _settingsCache = [];

            return;
        }

        try
        {
            string json = File.ReadAllText(_settingsFilePath);

            _settingsCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? [];
        }
        catch
        {
            _settingsCache = [];
        }
    }

    public static void SaveSetting(Key key, string value)
    {
        EnsureSettingsLoaded();

        _settingsCache[key.ToString()] = value;

        string json = JsonConvert.SerializeObject(_settingsCache, Formatting.Indented);

        File.WriteAllText(_settingsFilePath, json);
    }

    public static string GetSetting(Key key, string defaultValue = null)
    {
        EnsureSettingsLoaded();

        return _settingsCache.TryGetValue(key.ToString(), out var value)
            ? value
            : defaultValue ?? string.Empty;
    }
}
