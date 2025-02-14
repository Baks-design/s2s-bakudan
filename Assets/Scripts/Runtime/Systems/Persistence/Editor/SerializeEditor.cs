using UnityEditor;
using UnityEngine;

namespace Game.Runtime.Systems.Persistence.Editor
{
    [CustomEditor(typeof(SerializeManager))]
    public class SerializeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var serializeManager = (SerializeManager)target;
            var gameName = serializeManager.GameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game"))
                serializeManager.NewGame();

            if (GUILayout.Button("Save Game"))
                serializeManager.SaveGame();

            if (GUILayout.Button("Load Game"))
                serializeManager.LoadGame(gameName);

            if (GUILayout.Button("Delete Game"))
                serializeManager.DeleteGame(gameName);
        }
    }
}