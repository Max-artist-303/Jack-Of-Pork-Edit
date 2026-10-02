using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using JAM.Roles.Crewmate;
using JAM.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace JAM.Options.Roles.Crewmate;

public sealed class JackOfAllOptions : AbstractOptionGroup<JackOfAllRole>
{
    public override string GroupName => "Jack-Of-All-Trades Options";

    [ModdedNumberOption("Starting Modifiers", 0f, 10f, 1f)]
    public float NumModifiers { get; set; } = 5f;

    [ModdedToggleOption("Can Get More Modifiers From Tasks")]
    public bool ModsFromTasks { get; set; } = true;

    [ModdedNumberOption("Number of tasks per modifier", 1f, 5f, 1f)]
    public float TasksPerMod { get; set; } = 1f;
}
