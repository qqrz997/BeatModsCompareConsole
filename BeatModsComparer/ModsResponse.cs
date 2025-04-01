namespace BeatModsComparer;

[Serializable]
internal sealed record ModsResponse(ModResponse[] Mods);

[Serializable]
internal sealed record ModResponse(ModUploadResponse Mod, LatestModResponse Latest);

[Serializable]
internal sealed record ModUploadResponse(int Id, string Name, string Category);

[Serializable] 
internal record LatestModResponse(int Id, int ModId, string ModVersion, string Status);