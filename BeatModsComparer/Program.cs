using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BeatmodsComparer;
using BeatModsComparer;

var httpClient = new HttpClient()
{
    BaseAddress = new("https://beatmods.com/api/mods"),
    DefaultRequestHeaders =
    {
        { "user-agent", "github.qqrz997.BeatmodsComparer" }
    }
};

var jsonSerializerOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

while (true)
{
    Console.WriteLine("Input a version to compare mods from:");
    var inFirst = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(inFirst))
    {
        Console.WriteLine("Input is invalid. Please try again.");
        continue;
    }

    if (!Version.TryParse(inFirst, out var verFirst))
    {
        Console.WriteLine("Input isn't a valid version. Please try again.");
        continue;
    }
    
    Console.WriteLine($"Input a second version to compare to {verFirst}:");
    var inSecond = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(inSecond))
    {
        Console.WriteLine("Input is invalid. Please try again.");
        continue;
    }

    if (!Version.TryParse(inSecond, out var verSecond))
    {
        Console.WriteLine("Input isn't a valid version. Please try again.");
        continue;
    }

    if (inFirst == inSecond)
    {
        Console.WriteLine("Versions cannot be the same. Please try again.");
        continue;
    }

    string jsonStringFirst;
    string jsonStringSecond;
    try
    {
        var responseFirst = await httpClient.GetAsync($"?gameVersion={verFirst}");
        var responseSecond = await httpClient.GetAsync($"?gameVersion={verSecond}");

        jsonStringFirst = await responseFirst.Content.ReadAsStringAsync();
        jsonStringSecond = await responseSecond.Content.ReadAsStringAsync();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Encountered an error when getting BeatMods response ({e.GetType()})");
        continue;
    }
    
    if (!TryDeserializeModsResponse(jsonStringFirst, out var modsFirst) || 
        !TryDeserializeModsResponse(jsonStringSecond, out var modsSecond))
    {
        Console.WriteLine("Error: Invalid response from server. Please try again.");
        continue;
    }

    Console.WriteLine();
    
    var modsMapFirst = modsFirst.Mods.FilterForVerified().ToDictionary(x => x.Mod.Id, x => x);
    var modsMapSecond = modsSecond.Mods.FilterForVerified().ToDictionary(x => x.Mod.Id, x => x);
    
    Console.WriteLine($"Mods available on {verSecond} that are not available on {verFirst}:");
    ConsoleListMods(modsSecond.Mods.Where(x => !modsMapFirst.ContainsKey(x.Mod.Id)).ToList());
    
    Console.WriteLine($"Mods available on {verFirst} that are not available on {verSecond}:");
    ConsoleListMods(modsFirst.Mods.Where(x => !modsMapSecond.ContainsKey(x.Mod.Id)).ToList());
    
    break;
}

Console.WriteLine("Press any key to continue...");
Console.ReadKey(true);

return;

bool TryDeserializeModsResponse(string jsonString, [NotNullWhen(true)] out ModsResponse? result)
{
    try
    {
        result = JsonSerializer.Deserialize<ModsResponse>(jsonString, jsonSerializerOptions);
    }
    catch
    {
        result = null;
    }
    return result != null;
}

static void ConsoleListMods(List<ModResponse> modResponses)
{
    if (modResponses is [])
    {
        Console.WriteLine("None");
    }
    else
    {
        foreach (var x in modResponses) Console.WriteLine($"{x.Mod.Name} ({x.Latest.ModVersion})");
    }
    Console.WriteLine();
}