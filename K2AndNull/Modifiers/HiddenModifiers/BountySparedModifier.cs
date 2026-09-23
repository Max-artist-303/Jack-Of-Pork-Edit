using MiraAPI.Modifiers;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class BountySparedModifier(byte jailorId) : BaseModifier
{
    public override string ModifierName => "Bounty Immune";
    public override bool HideOnUi => true;
    public byte JailorId { get; } = jailorId;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}