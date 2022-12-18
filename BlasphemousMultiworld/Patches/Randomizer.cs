﻿using HarmonyLib;
using System.Collections.Generic;
using BlasphemousRandomizer;
using BlasphemousRandomizer.Fillers;
using BlasphemousRandomizer.Shufflers;
using BlasphemousRandomizer.Structures;
using Framework.FrameworkCore;
using Framework.Managers;

namespace BlasphemousMultiworld.Patches
{
    // Change randomization methods
    [HarmonyPatch(typeof(Randomizer), "Randomize")]
    public class RandomizerRandomize_Patch
    {
        public static bool Prefix(Randomizer __instance, int ___seed)
        {
            if (Main.Multiworld.connection.connected)
            {
                __instance.itemShuffler.Shuffle(___seed);
                Main.Multiworld.modifyNewItems(__instance.itemShuffler.getNewItems()); // Change to not randomize items first before replacing them
                //__instance.hintShuffler.Shuffle(___seed);
                __instance.enemyShuffler.Shuffle(___seed);
            }
            return false;
        }
    }

    // Load data on new game
    [HarmonyPatch(typeof(Randomizer), "newGame")]
    public class RandomizerNew_Patch
    {
        public static void Postfix()
        {
            Main.Multiworld.newGame();
        }
    }

    // Get all items from the item filler
    [HarmonyPatch(typeof(ItemFiller), "addSpecialItems")]
    public class ItemFiller_Patch
    {
        public static void Postfix(List<Item> items)
        {
            Main.Multiworld.allItems = items;
        }
    }

    // Send location check when giving item
    [HarmonyPatch(typeof(ItemShuffle), "giveItem")]
    public class ItemShuffle_Patch
    {
        public static void Postfix(string locationId)
        {
            Main.Multiworld.sendLocation(locationId);
        }
    }

    // Set multiworld flag when starting new game
    [HarmonyPatch(typeof(Randomizer), "setUpExtras")]
    public class RandomizerSetup_Patch
    {
        public static void Postfix()
        {
            Core.Events.SetFlag("MULTIWORLD", true, false);
        }
    }

    // Show different notification when receiving an item
    [HarmonyPatch(typeof(Item), "getRewardInfo")]
    public class Item_Patch
    {
        public static void Postfix(ref RewardInfo __result)
        {
            if (Main.Multiworld.receivedPlayer != "")
            {
                __result.notification = "Receieved from " + Main.Multiworld.receivedPlayer + "!";
                Main.Multiworld.receivedPlayer = "";
            }
        }
    }
}
