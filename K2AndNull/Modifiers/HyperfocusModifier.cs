using JackOfAllMods.Assets;
using JackOfAllMods.Options.Modifiers.UniversalModifierOptions;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfUs.Assets;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Wiki;
using TownOfUs.Utilities;
using UnityEngine;

namespace JackOfAllMods.Modifiers.Game.Universal;

 
public sealed class HyperfocusModifier : TouGameModifier, IWikiDiscoverable
{
     
    public override string ModifierName => "Hyperfocus";
     
    public override string IntroInfo => "You can see nothing but tasks in tasks";
     
    public override string GetDescription()
    {
        return IntroInfo;
    }
    
     
    public string GetAdvancedDescription() { return IntroInfo + MiscUtils.AppendOptionsText(base.GetType()); }

     
    public override ModifierFaction FactionType => ModifierFaction.Crewmate;

    public override ModifierUiConfiguration Configuration
    {
        get
        {
            return new ModifierUiConfiguration(JackOfAllMods.Colors.Hyperfocus, TmpSpriteUtils.CreateSpriteAsset(K2ModifierIcons.Blind.LoadAsset(), "TouMira.Modifier.Game.Universal.Blind", 1.45f));
        }
    }
     
    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<HyperfocusOptions>.Instance.HyperfocusChance;
    }
     
    public override float IntroSize => 3f;
     
    public override bool HideOnUi => false;
     
    public override LoadableAsset<Sprite> ModifierIcon => TouAssets.TerminologySprite;
     
    public override int GetAmountPerGame()
    {
        return CustomAmount;
    }
     
    public override int CustomAmount => (int)OptionGroupSingleton<HyperfocusOptions>.Instance.HyperfocusCount;
     
    public override int CustomChance => (int)OptionGroupSingleton<HyperfocusOptions>.Instance.HyperfocusChance;

     
    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role.IsCrewmate();
    }
}
