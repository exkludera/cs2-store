﻿using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CS2MenuManager.API.Class;
using CS2MenuManager.API.Enum;
using CS2MenuManager.API.Interface;
using System.Text.Json;
using static Store.Config_Config;
using static Store.Store;
using static StoreApi.Store;

namespace Store;

public static class MenuBase
{
    public static void DisplayStoreMenu(CCSPlayerController? player, bool inventory)
    {
        if (player == null)
            return;

        Menu.DisplayStore(player, inventory);
    }

    public static int GetSellingPrice(Dictionary<string, string> item, Store_Item playerItem)
    {
        float sellRatio = Config.Settings.SellRatio;
        bool usePurchaseCredit = Config.Settings.SellUsePurchaseCredit;

        int purchasePrice = usePurchaseCredit && playerItem != null ? playerItem.Price : int.Parse(item["price"]);
        return (int)(purchasePrice * sellRatio);
    }

    public static bool CheckFlag(CCSPlayerController player, Dictionary<string, string> item, bool sell = false)
    {
        item.TryGetValue("flag", out string? flag);
        return CheckFlag(player, flag, !sell);
    }

    public static bool CheckFlag(CCSPlayerController player, string? flagAll, bool trueIfNull)
    {
        return string.IsNullOrEmpty(flagAll)
            ? trueIfNull
            : flagAll.Split(',')
            .Any(flag => (flag.StartsWith('@') && AdminManager.PlayerHasPermissions(player, flag)) ||
                         (flag.StartsWith('#') && AdminManager.PlayerInGroup(player, flag)) ||
                         (flag == player.SteamID.ToString()));
    }

    public static string GetCategoryName(CCSPlayerController player, JsonProperty category)
    {
        return Instance.Localizer.ForPlayer(player, category.Name);
    }

    public static void InspectAction(CCSPlayerController player, Dictionary<string, string> item, string type)
    {
        switch (type)
        {
            case "playerskin":
                item.TryGetValue("skin", out string? skn);
                Item_PlayerSkin.Inspect(player, item["model"], skn);
                break;
                /*
            case "customweapon":
                Item_CustomWeapon.Inspect(player, item["viewmodel"], item["weapon"]);
                break;
                */
        }
    }

    public static void AddMenuOption(this IMenu menu, CCSPlayerController player, DisableOption disableOption, string display, params object[] args)
    {
        menu.AddItem(Instance.Localizer.ForPlayer(player, display, args), disableOption);
    }

    public static void AddMenuOption(this IMenu menu, CCSPlayerController player, Action<CCSPlayerController, ItemOption> callback, string display, params object[] args)
    {
        menu.AddItem(Instance.Localizer.ForPlayer(player, display, args), callback);
    }

    public static void AddMenuOption(this IMenu menu, CCSPlayerController player, Action<CCSPlayerController, ItemOption> callback, DisableOption disableOption, string display, params object[] args)
    {
        menu.AddItem(Instance.Localizer.ForPlayer(player, display, args), callback, disableOption);
    }

    public static BaseMenu CreateMenuByType(string title)
    {
        return MenuManager.MenuByType(Config.Menu.MenuType, title, Instance);
    }
}
