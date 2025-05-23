﻿using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = RadMechAI;


[HarmonyPatch(typeof(P))]
internal class OldPatch {
    [HarmonyPatch("Stomp")]
    [HarmonyPrefix]
    private static void Stomp(
        Transform stompTransform,
        float radius
    ) {
        // check if the player is within the stomp radius
        double num = Vector3.Distance(KillPatch.Player.transform.position, stompTransform.position);
        if(num < radius) {
            if(num < radius * 0.175) {
                KillPatch.Damage(TranslatedCauseOfDeath.Crushed, "Old Bird", 70);
            } else if(num < radius * 0.5f) {
                KillPatch.Damage(TranslatedCauseOfDeath.Crushed, "Old Bird", 30);
            }
        }
    }

    [HarmonyPatch(nameof(P.SetExplosion))]
    [HarmonyPrefix]
    private static void SetExplosion(Vector3 explosionPosition, Vector3 forwardRotation) {
        LandminePatch_Detonate.SpawnExplosion(explosionPosition - forwardRotation * 0.1f, 1f, 7f, 30, "Old Bird");
    }
}