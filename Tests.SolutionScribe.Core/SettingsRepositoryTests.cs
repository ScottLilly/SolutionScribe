using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Services;
using System;
using System.IO;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class SettingsRepositoryTests
{
    private string _testFolder = string.Empty;
    private string _settingsFilePath = string.Empty;

    [TestInitialize]
    public void CreateTestFolder()
    {
        _testFolder = Path.Combine(Path.GetTempPath(), $"SolutionScribeTests-{Guid.NewGuid():N}");
        _settingsFilePath = Path.Combine(_testFolder, "settings.json");

        Directory.CreateDirectory(_testFolder);
    }

    [TestCleanup]
    public void DeleteTestFolder()
    {
        if (Directory.Exists(_testFolder))
        {
            Directory.Delete(_testFolder, true);
        }
    }

    #region Round trip

    [TestMethod]
    public void GetSetting_AfterSaveSetting_ReturnsTheSavedValue()
    {
        var repository = new SettingsRepository(_settingsFilePath);

        repository.SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Scott Lilly");

        Assert.AreEqual("Scott Lilly",
            repository.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder));
    }

    [TestMethod]
    public void GetSetting_NewRepositoryOverTheSameFile_ReturnsTheSavedValue()
    {
        new SettingsRepository(_settingsFilePath)
            .SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Scott Lilly");

        Assert.AreEqual("Scott Lilly",
            new SettingsRepository(_settingsFilePath)
                .GetSetting(SettingsRepository.Key.DefaultCopyrightHolder));
    }

    [TestMethod]
    public void SaveSetting_CalledTwice_KeepsTheLastValue()
    {
        var repository = new SettingsRepository(_settingsFilePath);

        repository.SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, "First");
        repository.SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Second");

        Assert.AreEqual("Second",
            new SettingsRepository(_settingsFilePath)
                .GetSetting(SettingsRepository.Key.DefaultCopyrightHolder));
    }

    [TestMethod]
    public void SaveSetting_FolderDoesNotExist_CreatesIt()
    {
        string nestedPath = Path.Combine(_testFolder, "Solution Scribe", "settings.json");

        new SettingsRepository(nestedPath)
            .SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Scott Lilly");

        Assert.IsTrue(File.Exists(nestedPath));
    }

    #endregion

    #region Missing and unreadable files

    [TestMethod]
    public void GetSetting_NoSettingsFile_ReturnsTheDefaultValue()
    {
        var repository = new SettingsRepository(_settingsFilePath);

        Assert.AreEqual("Fallback",
            repository.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Fallback"));
    }

    [TestMethod]
    public void GetSetting_NoSettingsFileAndNoDefaultValue_ReturnsAnEmptyString()
    {
        var repository = new SettingsRepository(_settingsFilePath);

        Assert.AreEqual(string.Empty,
            repository.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder));
    }

    [TestMethod]
    public void GetSetting_NoSettingsFile_DoesNotCreateTheFile()
    {
        new SettingsRepository(_settingsFilePath)
            .GetSetting(SettingsRepository.Key.DefaultCopyrightHolder);

        Assert.IsFalse(File.Exists(_settingsFilePath));
    }

    [TestMethod]
    public void GetSetting_CorruptSettingsFile_ReturnsTheDefaultValueWithoutThrowing()
    {
        File.WriteAllText(_settingsFilePath, "{ this is not json");

        var repository = new SettingsRepository(_settingsFilePath);

        Assert.AreEqual("Fallback",
            repository.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Fallback"));
    }

    [TestMethod]
    public void GetSetting_CorruptSettingsFile_ReportsTheErrorToTheCaller()
    {
        File.WriteAllText(_settingsFilePath, "{ this is not json");

        Exception? reported = null;

        var repository = new SettingsRepository(_settingsFilePath, ex => reported = ex);

        repository.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder);

        Assert.IsNotNull(reported);
    }

    [TestMethod]
    public void GetSetting_SettingsFileHoldingJsonOfTheWrongShape_ReturnsTheDefaultValue()
    {
        File.WriteAllText(_settingsFilePath, "[ 1, 2, 3 ]");

        var repository = new SettingsRepository(_settingsFilePath, _ => { });

        Assert.AreEqual("Fallback",
            repository.GetSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Fallback"));
    }

    [TestMethod]
    public void SaveSetting_AfterACorruptFileWasRead_WritesReadableSettings()
    {
        File.WriteAllText(_settingsFilePath, "{ this is not json");

        new SettingsRepository(_settingsFilePath, _ => { })
            .SaveSetting(SettingsRepository.Key.DefaultCopyrightHolder, "Scott Lilly");

        Assert.AreEqual("Scott Lilly",
            new SettingsRepository(_settingsFilePath)
                .GetSetting(SettingsRepository.Key.DefaultCopyrightHolder));
    }

    #endregion

    #region Default location

    [TestMethod]
    public void DefaultSettingsFilePath_Always_IsSettingsJsonUnderApplicationData()
    {
        string path = SettingsRepository.DefaultSettingsFilePath;

        Assert.AreEqual("settings.json", Path.GetFileName(path));
        Assert.AreEqual("Solution Scribe", Path.GetFileName(Path.GetDirectoryName(path)));
        StringAssert.StartsWith(path,
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    }

    #endregion
}
