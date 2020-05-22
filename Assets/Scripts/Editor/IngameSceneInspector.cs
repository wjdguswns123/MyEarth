using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(IngameScene))]
public class IngameSceneInspector : Editor
{
    IngameScene targetScene;

    private void OnEnable()
    {
        targetScene = target as IngameScene;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(targetScene.ingameScenes == null)
        {
            targetScene.ingameScenes = new IngameSceneInfo[1];
        }

        int count = EditorGUILayout.IntField("Scene Count", targetScene.ingameScenes.Length);
        if(targetScene.ingameScenes.Length != count)
        {
            IngameSceneInfo[] scenes = new IngameSceneInfo[count];

            for(int i = 0; i < count; ++i)
            {
                if(i < targetScene.ingameScenes.Length)
                {
                    scenes[i] = targetScene.ingameScenes[i];
                }
                else
                {
                    scenes[i] = new IngameSceneInfo();
                }
            }
            targetScene.ingameScenes = scenes;
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
