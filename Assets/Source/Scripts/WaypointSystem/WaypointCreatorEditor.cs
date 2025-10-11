using UnityEngine;
using UnityEditor;

namespace Assets.Source.Game.Scripts.WaypointSystem
{
    [CustomEditor(typeof(WaypointCreator))]
    public class WaypointCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WaypointCreator creator = (WaypointCreator)target;

            if (GUILayout.Button("Create Waypoint"))
            {
                creator.CreateWaypoint();

                if (!Application.isPlaying)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(creator.gameObject.scene);
            }
        }
    }
}