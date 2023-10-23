using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(IngameScene))]
public class IngameSceneInspector : Editor
{
    private IngameScene _targetScene;

    private void OnEnable()
    {
        _targetScene = target as IngameScene;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(_targetScene.ingameScenes == null)
        {
            _targetScene.ingameScenes = new IngameSceneInfo[1];
        }

        int count = EditorGUILayout.IntField("Scene Count", _targetScene.ingameScenes.Length);
        if(_targetScene.ingameScenes.Length != count)
        {
            IngameSceneInfo[] scenes = new IngameSceneInfo[count];

            for(int i = 0; i < count; ++i)
            {
                if(i < _targetScene.ingameScenes.Length)
                {
                    scenes[i] = _targetScene.ingameScenes[i];
                }
                else
                {
                    scenes[i] = new IngameSceneInfo();
                }
            }
            _targetScene.ingameScenes = scenes;
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
