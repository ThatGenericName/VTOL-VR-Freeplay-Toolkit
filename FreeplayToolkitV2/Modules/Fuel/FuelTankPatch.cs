using System;
using HarmonyLib;

namespace FreeplayToolkitV2.Modules.Fuel;

[HarmonyPatch(typeof(FuelTank), nameof(FuelTank.RequestFuel), new Type[] { typeof(double) })]
public static class FuelTankPatch
{   
    [HarmonyPrefix]
    public static bool RequestFuelPrefix(FuelTank __instance, ref double deltaFuel)
    {
        // Note, this is a bit annoying, FuelTanks don't store any information about
        // what it's connected to, and so I'll have to do some trickery to do so.
        // I'll use the OnSceneLoaded to cache the player's fuel tank and then
        // compare a fuel tank to that.

        if (FuelTankSearch.playerFuelTank == null)
        {
            return true;
        }
        if (__instance == FuelTankSearch.playerFuelTank)
        {
            deltaFuel = deltaFuel * Main.FuelModifier.FuelDrainModifier;
        }

        return true;
    }
}