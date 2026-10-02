using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace JAM.Assets;

public static class K2Assets
{
    private const string ShortPath = "JAM.Resources.Other";
    public static LoadableAsset<Sprite> BountyTarget { get; } = new LoadableResourceAsset($"{ShortPath}.Bounty Target.png", 200);
}
