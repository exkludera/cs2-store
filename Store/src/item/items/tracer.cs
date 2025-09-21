using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Store.Extension;
using System.Drawing;
using System.Globalization;
using static Store.Store;
using static StoreApi.Store;

namespace Store;

[StoreItemType("tracer")]
public class Item_Tracer : IItemModule
{
    public bool Equipable => true;
    public bool? RequiresAlive => null;

    public static MemoryFunctionWithReturn<CParticleSystem, int, Vector, bool> SetControlPointValue { get; }
= new("55 48 89 E5 41 57 41 56 41 55 49 89 D5 41 54 41 89 F4 53 48 89 FB 48 83 EC ? E8");

    public void OnPluginStart()
    {
        if (Item.IsAnyItemExistInType("tracer"))
            Instance.RegisterEventHandler<EventBulletImpact>(OnBulletImpact);
    }

    public void OnMapStart() { }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        Item.GetItemsByType("tracer").ForEach(item => manifest.AddResource(item.Value["model"]));
    }

    public bool OnEquip(CCSPlayerController player, Dictionary<string, string> item)
    {
        return true;
    }

    public bool OnUnequip(CCSPlayerController player, Dictionary<string, string> item, bool update)
    {
        return true;
    }

    private static HookResult OnBulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        Store_Equipment? playertracer = Instance.GlobalStorePlayerEquipments.FirstOrDefault(p => p.SteamID == player.SteamID && p.Type == "tracer");
        if (playertracer == null)
            return HookResult.Continue;

        Dictionary<string, string>? itemdata = Item.GetItem(playertracer.UniqueId);
        if (itemdata == null)
            return HookResult.Continue;

        Vector position = VectorExtensions.GetEyePosition(player);

        CEnvParticleGlow? entity = Utilities.CreateEntityByName<CEnvParticleGlow>("env_particle_glow");
        if (entity == null || !entity.IsValid)
            return HookResult.Continue;

        string acceptinputvalue = itemdata.GetValueOrDefault("acceptInputValue", "Start");
        entity.StartActive = true;
        entity.EffectName = itemdata["model"];

        Color color = Color.White;
        if (itemdata.TryGetValue("color", out string? cvalue))
        {
            if (!string.IsNullOrEmpty(cvalue))
            {
                string[] colorValues = cvalue.Split(' ');
                color = Color.FromArgb(int.Parse(colorValues[0]), int.Parse(colorValues[1]), int.Parse(colorValues[2]));
            }
        }
        entity.ColorTint = color;
        entity.TintCP = 1;

        SetControlPointValue.Invoke(entity, 4, new Vector(0, 5, 0));
        SetControlPointValue.Invoke(entity, 5, position);
        SetControlPointValue.Invoke(entity, 6, new Vector(@event.X, @event.Y, @event.Z));

        entity.Teleport(position);
        entity.DispatchSpawn();
        entity.AcceptInput(acceptinputvalue);

        float lifetime = itemdata.TryGetValue("lifetime", out string? value) && float.TryParse(value, CultureInfo.InvariantCulture, out float lt) ? lt : 0.3f;

        Instance.AddTimer(lifetime, () =>
        {
            if (entity.IsValid)
                entity.Remove();
        });

        return HookResult.Continue;
    }
}