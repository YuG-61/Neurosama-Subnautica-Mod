﻿using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using Nautilus.Utility;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

public class TutelPrefab : PickupableCreaturePrefab
{
    public TutelPrefab(ModItem regular, ModItem cooked, ModItem cured, GameObject prefab) : base(regular, cooked, cured, prefab)
    {
    }

    private const float swimVelocity = 2f;
    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = new(creaturePrefab, BehaviourType.Crab, EcoTargetType.Coral, float.MaxValue)
        {
            // Cell level needs to be set to Near to avoid our spawns falling through the unrendered map which is too far
            // fish are not affected by this, but walking creatures are, as they have gravity on
            CellLevel = LargeWorldEntity.CellLevel.Near,
            Mass = 20,
            BioReactorCharge = 0,
            EyeFOV = 0,
            ScareableData = new ScareableData(),
            PickupableFishData = new PickupableFishData(TechType.Floater, "WM", "VM"),
            EdibleData = new EdibleData(9, -7, false, 1f),
            ScannerRoomScannable = true,
            SizeDistribution = new AnimationCurve(new Keyframe(0, 0.5f), new Keyframe(1, 1f)),
            AnimateByVelocityData = new AnimateByVelocityData(swimVelocity),
            // Walk on surface and swim above water true
            LocomotionData = new LocomotionData(10f, 2f, 1f, 0.1f, true, true, true),
        };
        template.SetCreatureComponentType<CaveCrawler>();
        template.WithoutInfection();
        template.SetWaterParkCreatureData(new WaterParkCreatureDataStruct(0.1f, 0.5f, 1f, 1.5f, true, true, ClassID));

        return template;
    }

    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        prefab.GetComponent<HeldFish>().ikAimLeftArm = true;

        // ECC can only add SwimBehaviour but we want WalkBehaviour
        // these depend on SwimBehaviour so we have to destroy them first
        Object.DestroyImmediate(prefab.GetComponent<FleeOnDamage>());
        Object.DestroyImmediate(prefab.GetComponent<SwimRandom>());
        Object.DestroyImmediate(prefab.GetComponent<SwimBehaviour>());

        // for hurt sounds (otherwise GetComponent in patches picks the looping emitter)
        FMOD_CustomEmitter emitter = prefab.AddComponent<FMOD_CustomEmitter>();
        emitter.followParent = true;

        CaveCrawler crawler = (CaveCrawler)components.Creature;
        crawler.walkingSound = prefab.EnsureComponent<FMOD_CustomLoopingEmitter>(); // empty
        crawler.jumpSound = AudioUtils.GetFmodAsset("event:/sub/common/fishsplat"); // placeholder

        WalkBehaviour walk = prefab.AddComponent<WalkBehaviour>();

#if BELOWZERO
        LandCreatureGravity gravity = prefab.EnsureComponent<LandCreatureGravity>();
        gravity.applyDownforceUnderwater = true;
#else
        CaveCrawlerGravity gravity = prefab.EnsureComponent<CaveCrawlerGravity>();
#endif

        MoveOnSurface moveSurface = prefab.AddComponent<MoveOnSurface>();
        moveSurface.moveVelocity = swimVelocity;

        FleeOnDamage fleeDamage = prefab.EnsureComponent<FleeOnDamage>();
        fleeDamage.damageThreshold = 0.01f; // very easily scared tutel
        fleeDamage.swimVelocity = swimVelocity * 1.5f;
        FleeWhenScared fleeScared = prefab.EnsureComponent<FleeWhenScared>();
        fleeScared.swimVelocity = swimVelocity * 1.25f;

        GetCarried getCarried = prefab.EnsureComponent<GetCarried>();
        getCarried.emitter = emitter;

        WorldSounds.Add(prefab, CreatureSoundsHandler.GetCreatureSounds(ModItems.Tutel).AmbientWorldSounds);

        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Acid, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Cold, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Fire, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Poison, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Radiation, 0f);
        CreaturePrefabUtils.AddDamageModifier(prefab, DamageType.Starve, 0f);

        prefab.EnsureComponentFields();

        yield break;
    }
}
