using UnityEngine;
using System.Data.Common;




// #if UNITY_EDITOR
// using UnityEditor;
// [CustomEditor(typeof(DrawPileManager))]
// public class DrawPileManagerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         DrawPileManager drawPileManager= (DrawPileManager)target;
//         if (GUILayout.Button("Robar Carta")){
//             //HandManager handManager= FindObjectOfType<HandManager>();
//             HandManager handManager = FindFirstObjectByType<HandManager>();
//             if (handManager!=null){
//                 drawPileManager.DrawCard(handManager);
//             }
//         }

//     }
// }
// #endif