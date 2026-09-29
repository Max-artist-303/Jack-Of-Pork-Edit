using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace JackOfAllMods.Assets;

public static class K2ModifierIcons
{
    private const string ShortPath = "JackOfAllMods.Resources.ModifierIcons";

    public static LoadableAsset<Sprite> Blind { get; } = new LoadableResourceAsset($"{ShortPath}.Blind.png", 200);
    public static LoadableAsset<Sprite> Rivalry { get; } = new LoadableResourceAsset($"{ShortPath}.Rivalry.png", 200);
    public static LoadableAsset<Sprite> Unstable { get; } = new LoadableResourceAsset($"{ShortPath}.Unstable.png", 200);
    public static LoadableAsset<Sprite> Ventable { get; } = new LoadableResourceAsset($"{ShortPath}.Ventable.png", 200);
}
