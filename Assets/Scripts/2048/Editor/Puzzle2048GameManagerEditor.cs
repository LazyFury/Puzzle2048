using UnityEditor;
using UnityEngine;
using Puzzle2048;

namespace Assets.Scripts._2048.Editor
{
    [CustomEditor(typeof(Puzzle2048GameManager))]
    public class Puzzle2048GameManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Puzzle2048GameManager puzzle2048GameManager = (Puzzle2048GameManager)target;
            if (GUILayout.Button("InitBoard"))
            {
                puzzle2048GameManager.InitBoard();
            }
        }
    }
}