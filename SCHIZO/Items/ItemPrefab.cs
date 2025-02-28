﻿using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.Items;

public abstract class ItemPrefab : CustomPrefab
{
    private static readonly Transform _keepAliveParent;

    static ItemPrefab()
    {
        _keepAliveParent = new GameObject("KeepAlive").transform;
        _keepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_keepAliveParent);
    }

    public ItemData ItemData { get; init; }
    public TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public Vector2int SizeInInventory { get; init; } = new(1, 1);
    public TechType RequiredForUnlock { get; init; }
    public EquipmentType EquipmentType { get; init; }
    public QuickSlotType QuickSlotType { get; init; }
    public LargeWorldEntity.CellLevel CellLevel { get; init; } = LargeWorldEntity.CellLevel.Near;
    public TechType CloneTechType { get; init; }

    protected readonly ModItem modItem;

    [SetsRequiredMembers]
    public ItemPrefab(ModItem modItem) : base(modItem)
    {
        this.modItem = modItem;
    }

    [SetsRequiredMembers]
    protected ItemPrefab(string classId, string displayName, string tooltip) : base(classId, displayName, tooltip)
    {
    }

    protected virtual void AddGadgets()
    {

    }

    protected virtual void ModifyPrefab(GameObject prefab)
    {

    }

    protected virtual void PostRegister()
    {

    }

    private void AddBasicGadgets()
    {
        if (ItemData!?.icon) Info.WithIcon(ItemData.icon);
        Info.WithSizeInInventory(SizeInInventory);
        this.SetRecipe(Recipe);
        if (TechGroup != TechGroup.Uncategorized) this.SetPdaGroupCategory(TechGroup, TechCategory);
        if (RequiredForUnlock != TechType.None) this.SetUnlock(RequiredForUnlock);

        if (EquipmentType != EquipmentType.None)
            this.SetEquipment(EquipmentType).WithQuickSlotType(QuickSlotType);
    }

    public new virtual void Register()
    {
        AddBasicGadgets();
        AddGadgets();

        if (CloneTechType == TechType.None)
            SetGameObject(GetPrefab);
        else
        {
            SetGameObject(new CloneTemplate(Info, CloneTechType)
            {
                ModifyPrefab = ModifyPrefab,
            });
        }

        base.Register();
        PostRegister();
    }

    protected virtual GameObject GetPrefab()
    {
        GameObject instance = Object.Instantiate(ItemData.prefab, _keepAliveParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, CellLevel);

        ModifyPrefab(instance);

        CoroutineHelpers.RunWhen(() => MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1), () => MaterialHelpers.IsReady);

        return instance;
    }
}
