﻿global using static SCHIZO.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Resources;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "Neuro-sama Mod", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
#if BELOWZERO
    public static bool IS_BELOWZERO => true;
#else
    public static bool IS_BELOWZERO => false;
#endif
    public static bool IS_SUBNAUTICA => !IS_BELOWZERO;

    public static ManualLogSource LOGGER { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        LOGGER = Logger;
        DependencyResolver.InjectResources();

        LoadMethodAttribute.LoadAll();
        LoadComponentAttribute.AddAll(gameObject);
        LoadConsoleCommandsAttribute.RegisterAll();
        LoadCreatureAttribute.RegisterAll();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}
