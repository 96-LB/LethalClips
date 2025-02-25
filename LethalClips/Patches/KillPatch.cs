﻿using System;
using HarmonyLib;
using GameNetcodeStuff;
using Steamworks;

namespace LethalClips.Patches;

using P = PlayerControllerB;


enum TranslatedCauseOfDeath {
    Killed, // Unknown
    Bludgeoned, // Bludgeoning
    SPLAT, // Gravity
    Blast, // Blast
    Strangled, // Strangulation
    Suffocated, // Suffocation
    Mauled, // Mauling
    Shot, // Gunshots
    Crushed, // Crushing
    Drowned, // Drowning
    Abandoned, // Abandoned
    Electrocuted, // Electrocution
    Kicked, // Kicking
    Incinerated, // Burning
    Stabbed, // Stabbing
    Sliced, // Fan
    Crashed, // Inertia
    Snipped, // Snipped
    Devoured,
    Springed,
    Died,
    Disintegrated,
    Infected,
    Exploded,
    Embarrassing
}

internal class Death {
    internal TranslatedCauseOfDeath cause;

    internal string source;

    internal string Message {
        get {
            string message = Enum.GetName(typeof(TranslatedCauseOfDeath), cause) ?? "Killed";
            if(!string.IsNullOrEmpty(source)) {
                message += " by " + source;
            }
            return message;
        }
    }
}


[HarmonyPatch(typeof(P), nameof(P.KillPlayer))]
internal class KillPatch {
    private static void Prefix(
        P __instance,
        ref bool __state
    ) {
        // the body of the method changes these values, so cache them now
        __state = __instance.IsOwner && !__instance.isPlayerDead && __instance.AllowPlayerDeath();
    }

    private static void Postfix(
        P __instance,
        bool __state,
        CauseOfDeath causeOfDeath
    ) {
        // use stored value to determine if we actually need to do anything
        if(__state) {
            var death = State<Death>.Of(__instance);
            if(death.cause == TranslatedCauseOfDeath.Killed) {
                death.cause = (TranslatedCauseOfDeath)causeOfDeath;
            }

            Plugin.Log.LogInfo($"Player died! Cause of death: {death.Message}");
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                death.Message,
                "git gud lol",
                "steam_death",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");
        }
    }
}
