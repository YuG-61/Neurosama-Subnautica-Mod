﻿using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Loading
{
    [CreateAssetMenu(menuName = "SCHIZO/Loading/Loading Background Collection")]
    public sealed class LoadingBackgroundCollection : ScriptableObject
    {
        [ReorderableList]
        public LoadingBackground[] backgrounds;
    }
}
