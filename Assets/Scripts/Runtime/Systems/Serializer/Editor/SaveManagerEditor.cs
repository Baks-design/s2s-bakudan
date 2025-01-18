using UnityEditor;
using UnityEngine;

namespace Game.Runtime.Systems.Serializer
{
    [CustomEditor(typeof(SerializerManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var saveLoadSystem = (SerializerManager)target;
            var gameName = saveLoadSystem.GameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("New Game"))
                saveLoadSystem.NewGame();

            if (GUILayout.Button("Save Game"))
                saveLoadSystem.SaveGame();

            if (GUILayout.Button("Load Game"))
                saveLoadSystem.LoadGame(gameName);

            if (GUILayout.Button("Delete Game"))
                saveLoadSystem.DeleteGame(gameName);
        }
    }
}