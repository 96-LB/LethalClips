﻿using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ForestGiantAI;


[HarmonyPatch(typeof(P), "EatPlayerAnimation")]
internal class GiantPatch_EatPlayerAnimation {
    private static void Prefix(
        PlayerControllerB playerBeingEaten
    ) {
        var death = State<Death>.Of(playerBeingEaten);
        death.cause = TranslatedCauseOfDeath.Devoured;
        death.source = "Forest Keeper";
    }
}


[HarmonyPatch(typeof(P), nameof(P.AnimationEventA))]
internal class GiantPatch_AnimationEventA {
    private static void Prefix(
        P __instance
    ) {
        // crush players upon whom the giant falls
        RaycastHit[] array = Physics.SphereCastAll(
            __instance.deathFallPosition.position,
            2.7f,
            __instance.deathFallPosition.forward,
            3.9f,
            StartOfRound.Instance.playersMask,
            QueryTriggerInteraction.Ignore
        );
        for(int i = 0; i < array.Length; i++) {
            PlayerControllerB player = array[i].transform.GetComponent<PlayerControllerB>();
            var death = State<Death>.Of(player);
            death.cause = TranslatedCauseOfDeath.Crushed;
            death.source = "Forest Keeper";
        }
    }
}
