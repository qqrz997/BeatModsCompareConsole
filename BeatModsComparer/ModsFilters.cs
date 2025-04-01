using BeatModsComparer;

namespace BeatmodsComparer;

internal static class ModsFilters
{
    public static IEnumerable<ModResponse> FilterForVerified(this IEnumerable<ModResponse> mods) =>
        mods.Where(x => x.Latest.Status.Equals("verified", StringComparison.InvariantCultureIgnoreCase));
}