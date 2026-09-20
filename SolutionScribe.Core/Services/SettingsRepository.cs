using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SolutionScribe.Core.Services;

public class SettingsRepository
{
    public enum Key
    {
        DefaultCopyrightHolder
    }

    private readonly string _settingsFilePath;
    private readonly Action<Exception>? _onLoadError;

    private Dictionary<string, string>? _settingsCache;

    /// <param name="settingsFilePath">Where the settings live. The folder is created on first save.</param>
    /// <param name="onLoadError">
    /// Called when the settings file exists but cannot be read. Nothing is thrown either way, so
    /// this is the caller's only chance to report it before the next save overwrites the file.
    /// </param>
    public SettingsRepository(string settingsFilePath, Action<Exception>? onLoadError = null)
    {
        _settingsFilePath = settingsFilePath;
        _onLoadError = onLoadError;
    }

    public static string DefaultSettingsFilePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Solution Scribe",
            "settings.json");

    private Dictionary<string, string> Settings => _settingsCache ??= LoadSettings();

    public string GetSetting(Key key, string? defaultValue = null)
    {
        return Settings.TryGetValue(key.ToString(), out string value)
            ? value
            : defaultValue ?? string.Empty;
    }

    public void SaveSetting(Key key, string value)
    {
        Settings[key.ToString()] = value;

        string folder = Path.GetDirectoryName(_settingsFilePath);

        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
    }

    private Dictionary<string, string> LoadSettings()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            string json = File.ReadAllText(_settingsFilePath);

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                   ?? new Dictionary<string, string>();
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is JsonException)
        {
            // An unreadable or corrupt settings file must not stop the command, but the next save
            // overwrites it, so it goes to the caller rather than being lost silently.
            _onLoadError?.Invoke(ex);

            return new Dictionary<string, string>();
        }
    }
}
