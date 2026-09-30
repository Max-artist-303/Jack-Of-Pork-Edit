using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using JackOfAllMods.Roles.Crewmate;
using JackOfAllMods.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace JackOfAllMods.Options.Roles.Impostor;

public sealed class DeceiverOptions : AbstractOptionGroup<DeceiverRole>
{
    public enum DeceiverRoleDisplayed
    {
        Investigator,
        RandomCrew
    }

        public override string GroupName => "Deceiver Options";
    
        [ModdedEnumOption("Deceiver Shows As", typeof(DeceiverRoleDisplayed), ["Investigator", "Random Crew"])]
    public DeceiverRoleDisplayed DeceiverDisplayedAs { get; set; } = DeceiverRoleDisplayed.RandomCrew;

    [ModdedToggleOption("Deceive Crew Killing")]
    public bool DeceiveCrewKillers { get; set; } = false;
}
