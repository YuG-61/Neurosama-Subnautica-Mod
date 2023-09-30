﻿using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Resources;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.Items.Gymbag;

[LoadMethod]
public class Gymbag : ItemPrefab
{
    private static readonly TechType BagTechType = Retargeting.TechType.Bag;

    [LoadMethod]
    private static void Load()
    {
        new Gymbag(ModItems.Gymbag).Register();
    }

    [SetsRequiredMembers]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public Gymbag(ModItem modItem) : base(modItem)
    {
        ItemData = ResourceManager.LoadAsset<ItemData>("Gymbag data");
        Recipe = new RecipeData(new Ingredient(BagTechType, 1), new Ingredient(ModItems.Ermfish, 1), new Ingredient(TechType.PosterKitty, 1));
        SizeInInventory = new Vector2int(2, 2);
        TechGroup = TechGroup.Personal;
        TechCategory = TechCategory.Equipment;
        RequiredForUnlock = ModItems.Ermfish;
        CloneTechType = BagTechType;
        base.AddGadgets = AddGadgets;
        base.ModifyPrefab = ModifyPrefab;
        base.PostRegister = PostRegister;
    }

    private new void AddGadgets()
    {
        // make this an editor component?
        CraftingGadget crafting = GetGadget<CraftingGadget>();
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);
        crafting.WithCraftingTime(10);
    }

    private new void ModifyPrefab(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 4;
        container.height = 4;

        Type rendererType = IS_BELOWZERO ? typeof(SkinnedMeshRenderer) : typeof(MeshRenderer);
        GameObject baseModel = prefab.GetComponentInChildren(rendererType).gameObject;
        baseModel.SetActive(false);

        GameObject instance = Object.Instantiate(ItemData.prefab, baseModel.transform.parent);

        PrefabUtils.AddVFXFabricating(instance, null, 0, 0.93f, new Vector3(0, -0.05f), 0.75f, Vector3.zero);
    }

    private new void PostRegister()
    {
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagHandler.Instance.OnOpen(item), "open storage", "English");
    }
}
