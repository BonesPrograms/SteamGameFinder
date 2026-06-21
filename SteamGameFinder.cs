using Microsoft.Win32;

namespace Bones;


/// <summary>
/// Finds the EXE of a Steam Game that is stored in the Common folder. Only for Windows 64 bit.
/// </summary>
public class SteamGameFinder
{
    public static readonly string SteamCommonPath = Path.Combine(GetSteamPath(), @"steamapps\common");
    public string SearchFor => _searchPattern.Remove(_searchPattern.IndexOf('*'), 1);
    public EnumerationOptions EnumerationOptions 
    {
        get => _enumerationOptions;
        set => _enumerationOptions = value ?? throw new ArgumentNullException(nameof(EnumerationOptions));
    }
    EnumerationOptions _enumerationOptions = new()
    {
        IgnoreInaccessible = true,
        RecurseSubdirectories = true,

    };
    string _searchPattern;
    public SteamGameFinder(string exeName)
    {
        _searchPattern = ChangeSearchInternal(exeName);
    }

    /// <summary>
    /// exeName should be the file name without the .exe extension.
    /// </summary>
    public void ChangeSearch(string exeName)
    {
        _searchPattern = ChangeSearchInternal(exeName);
    }
    public string? Find() => Directory.EnumerateFiles(SteamCommonPath, _searchPattern!, EnumerationOptions).FirstOrDefault();
    public string FindOrThrow() => Find() ?? throw new FileNotFoundException($"{SearchFor} not found!");
    static string ChangeSearchInternal(string exeName)
    {
        if (string.IsNullOrWhiteSpace(exeName))
            throw new ArgumentException("cannot be null or empty!", nameof(exeName));
        return $"{exeName}*.exe";
    }
    static string GetSteamPath()
    {
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam")) //if 32 bit, must change this string
        {
            if (key?.GetValue("InstallPath") is string path) return path;
        }
        throw new DirectoryNotFoundException("Steam directory not found!");
    }
}
