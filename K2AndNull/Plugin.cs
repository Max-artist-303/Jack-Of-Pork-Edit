using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using K2AmongUs.Assets;
using K2sAmongUsMod.CommsPatches;
using MiraAPI;
using MiraAPI.PluginLoading;
using PerfectComms.Api;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Globalization;
using TownOfUs;
using UnityEngine;

namespace K2AndNull;

/// <inheritdoc/>
[BepInAutoPlugin("com.K2AndNull.mod", "K2AndNull", "0.1")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[BepInDependency(TownOfUsPlugin.Id)]
[BepInDependency("com.edgetel.perfectcomms", BepInDependency.DependencyFlags.SoftDependency)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class Plugin : BasePlugin, IMiraPlugin
{
    /// <inheritdoc/>
    public static CultureInfo Culture => TownOfUs.TownOfUsPlugin.Culture;

    /// <inheritdoc/>
    public string OptionsTitleText => "K2 And Null";

    /// <inheritdoc/>
    public static bool IsDevBuild => false;

    /// <inheritdoc/>
    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    /// <inheritdoc/>
    public Harmony Harmony { get; } = new(Id);

    /// <inheritdoc/>
    public override void Load()
    {
        ReactorCredits.Register("K2 And Null", Version, IsDevBuild, ReactorCredits.AlwaysShow);

        try
        {
            Harmony.PatchAll();
        }
        catch(System.Exception e)
        {
            _ = ConstantlyError(e.ToString());
        }

        PerfectCommsSetup();
    }
    private static async Task ConstantlyError(string e)
    {
        while(true)
        {
            await Task.Delay(100);
            Fatal(e);
            
            if(Time.deltaTime > 1) break;
        }
    }

    void PerfectCommsSetup()
    {
        if (!IL2CPPChainloader.Instance.Plugins.ContainsKey(
                "com.edgetel.perfectcomms"))
            return;

        PerfectCommsVoiceIntegration.Register();
    }
}

public enum OurRpcCalls : uint
{
    ScrubModifiers = 0
}

// Add Lower Cooldown To Bounty Hunter Stuff

// Disable Person With Bounty From Calling Meeting

// ================ OTHER ================

// Make README pretty

// Make mimic more like neutral ambassador
// Add Light Blade's Semi-Transparent Modifier

// Add Extroverted And Introverted Cooldown Modifiers

// Buttons Can Be Clicked During Sabotage But Have The Blocking Symbol

// =============== FIXES ===============
/*
 * Combined K2's Mod and Null's Mod
 * Deceiver Has Option For No Longer Deceiving Crew Killing
 * Deceiver No Longer Wins With Crew For Deceived People
 * Added Option For Bounty Target To Be Unknowing That They're The Target
 * Added Option For Bounty Hunter Players To Not Include Crew Killing
 * Fixed Scrubber Bug Where Scrubber Wouldn't Scrub If You Left The Target
 * Fixed Scrubber Bug Where Scrubber Would Scrub The Wrong Person
 * Added Option For Scrubber Delay Configs
 * Modified Zombie Abilities
 * Made Snoop Button Cancelable
 * Made Bounty Hunter Unable To Target The Same Person Multiple Times In A Row
 * Reworked Bounty Hunter (Now Anyone Can Hunt The Bounty! Along with other misc. changes)
 * Added Battery Modifier
 * General Bug Fixes
*/