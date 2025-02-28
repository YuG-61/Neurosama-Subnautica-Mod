﻿using System.Collections.Generic;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

[LoadCreature]
public sealed class ErmsharkLoader : CustomCreatureLoader<CustomCreatureData, ErmsharkPrefab, ErmsharkLoader>
{
    public static GameObject Prefab; // TODO: figure this out

    public ErmsharkLoader() : base(ResourceManager.LoadAsset<CustomCreatureData>("Ermshark data"))
    {
        PDAEncyPath = IS_BELOWZERO ? "Lifeforms/Fauna/Carnivores" : "Lifeforms/Fauna/Sharks";
    }

    protected override ErmsharkPrefab CreatePrefab()
    {
        return new ErmsharkPrefab(ModItems.Ermshark, creatureData.prefab);
    }

    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        foreach (BiomeType biome in BiomeHelpers.GetOpenWaterBiomes())
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.005f };
        }
    }
}
