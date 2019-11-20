/* DevelopmentBuildSetter.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

[InitializeOnLoad]
public static class DevelopmentBuildSetter {
    static DevelopmentBuildSetter() {
#if !DEVELOPMENT_BUILD
        var currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        var currentBuildTargetGroup = BuildPipeline.GetBuildTargetGroup( currentBuildTarget );
        string preprocessors = PlayerSettings.GetScriptingDefineSymbolsForGroup( currentBuildTargetGroup );
        preprocessors += ";DEVELOPMENT_BUILD";
        PlayerSettings.SetScriptingDefineSymbolsForGroup( currentBuildTargetGroup, preprocessors );
#endif
    }
}
