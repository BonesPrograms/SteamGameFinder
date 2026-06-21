using Microsoft.Win32;

namespace Bones;


/// <summary>
/// Finds the EXE of a Steam Game that is stored in the Common folder. Only for Windows 64 bit.
/// </summary>
public class SteamGameFinder
{
    public static readonly string SteamCommonPath = Path.Combine(GetSteamPath(), @"steamapps\common");
    public string SearchFor => _searchPattern!.Remove(_searchPattern.IndexOf('*'), 1);
    public EnumerationOptions EnumerationOptions //Should always IgnoreInaccessible if not running in Admin, or will throw
    {
        get => _enumerationOptions;
        set => _enumerationOptions = value ?? throw new NullReferenceException("EnumerationOptions cannot be null!");
    }
    EnumerationOptions _enumerationOptions = new()
    {
        IgnoreInaccessible = true,
        RecurseSubdirectories = true,

    };
    string? _searchPattern;
    public SteamGameFinder(string gameName)
    {
        ChangeSearch(this, gameName);
    }
    public void ChangeSearch(string gameName)
    {
        ChangeSearch(this, gameName);
    }
    public string? Find() => Directory.EnumerateFiles(SteamCommonPath, _searchPattern!, EnumerationOptions).FirstOrDefault();
    public string FindOrThrow() => Find() ?? throw new FileNotFoundException($"{SearchFor} not found!");
    static void ChangeSearch(SteamGameFinder instance, string fileName)
    {
        if (fileName == null)
            throw new NullReferenceException("FileName cannot be null!");
        instance._searchPattern = $"{fileName}*.exe";
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
