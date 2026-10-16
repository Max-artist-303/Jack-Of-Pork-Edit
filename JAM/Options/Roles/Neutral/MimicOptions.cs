using HarmonyLib;
using JAM.Options.OptionTypes;
using JAM.Roles.Neutral;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAM.Options.Roles.Neutral;

public sealed class MimicOptions : AbstractOptionGroup<MimicRole>
{
    public override string GroupName => "Mimic Options";

    [ModdedNumberOption("Number Of Choices", 1f, 5f, 1f)]
    public float ChoiceCount { get; set; } = 3f;

    [ModdedToggleOption("Include Random Option")]
    public bool HaveRandomOption { get; set; } = true;

    [ModdedToggleOption("Mimic Round 1")]
    public bool MimicRound1 { get; set; } = true;

    public ModdedRolesOption BlacklistedRoles { get; set; } = new("Blacklisted Mimic Roles", new List<RoleBehaviour>(), role =>
    {
        return MimicRole.IsValidMimicRole(role);
    });
}
