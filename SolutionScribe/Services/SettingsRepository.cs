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

    private static readonly string s_appDataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Solution Scribe");

    private static readonly string s_settingsFilePath = Path.Combine(s_appDataFolder, "settings.json");

    private static Dictionary<string, string>? s_settingsCache;

    private static Dictionary<string, string> Settings => s_settingsCache ??= LoadSettings();

    private static Dictionary<string, string> LoadSettings()
    {
        if (!Directory.Exists(s_appDataFolder))
        {
            Directory.CreateDirectory(s_appDataFolder);
        }

        if (!File.Exists(s_settingsFilePath))
        {
            return [];
        }

        try
        {
            string json = File.ReadAllText(s_settingsFilePath);

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? [];
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is JsonException)
        {
            // An unreadable or corrupt settings file must not stop the command, but the next save
            // overwrites it, so say so in the Extensions output pane rather than losing it silently.
            ex.Log();

            return [];
        }
    }

    public static void SaveSetting(Key key, string value)
    {
        Settings[key.ToString()] = value;

        string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);

        File.WriteAllText(s_settingsFilePath, json);
    }

    public static string GetSetting(Key key, string? defaultValue = null)
    {
        return Settings.TryGetValue(key.ToString(), out var value)
            ? value
            : defaultValue ?? string.Empty;
    }
}
