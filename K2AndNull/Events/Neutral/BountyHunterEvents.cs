using UnityEngine;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using TownOfUs.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using Reactor;

namespace NullsMod.Events.Crewmate;
public static class BountyHunterEvents
{
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent _)
    {
        var sparedPlayers = ModifierUtils.GetPlayersWithModifier<BountySparedModifier>().ToList();
        sparedPlayers.Do(x => x.RemoveModifier<BountySparedModifier>());

        var players = ModifierUtils.GetPlayersWithModifier<BountyTargetModifier>().ToList();
        if (!OptionGroupSingleton<BountyHunterOptions>.Instance.BountyInARow)
        {
            players.Do(x => x.AddModifier<BountySparedModifier>(x.GetModifier<BountyTargetModifier>()!.BountyHunter.Id));
        }

        players.Do(x => x.RemoveModifier<BountyTargetModifier>());
    }

}