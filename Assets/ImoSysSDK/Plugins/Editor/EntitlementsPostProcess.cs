﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.Collections;

public class EntitlementsPostProcess : ScriptableObject
{
    public DefaultAsset m_entitlementsFile;
#if UNITY_IOS
    [PostProcessBuild]
    public static void OnPostProcess(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        var dummy = ScriptableObject.CreateInstance<EntitlementsPostProcess>();
        var file = dummy.m_entitlementsFile;
        ScriptableObject.DestroyImmediate(dummy);
        if (file == null)
        {
            return;
        }

        var proj_path = PBXProject.GetPBXProjectPath(buildPath);
        var proj = new PBXProject();
        proj.ReadFromFile(proj_path);

        // target_name = "Unity-iPhone"
        var target_name = PBXProject.GetUnityTargetName();
        var target_guid = proj.TargetGuidByName(target_name);
        var src = AssetDatabase.GetAssetPath(file);
        var file_name = System.IO.Path.GetFileName(src);
        var dst = buildPath + "/" + target_name + "/" + file_name;
        FileUtil.CopyFileOrDirectory(src, dst);
        proj.AddFile(target_name + "/" + file_name, file_name);
        proj.AddBuildProperty(target_guid, "CODE_SIGN_ENTITLEMENTS", target_name + "/" + file_name);

        proj.WriteToFile(proj_path);
    }
#endif
}
