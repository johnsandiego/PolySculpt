using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace sandiegoJohn.VRsculpting{

    public class SubdivisionUtility : EditorWindow
    {
        public SubdivisionUtility() {
            titleContent.text = "Torec/Subdivision";
        }

        Vector2 selectionScroll = Vector2.zero;
        int iterations = 1;
        CatmullClark.Options.BoundaryInterpolation boundaryInterpolation;

        void OnGUI() {

            EditorGUIUtility.labelWidth = 80;

            Transform[] selection = Selection.transforms;

            // Selection
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selection", GUILayout.Width(80));
            selectionScroll = EditorGUILayout.BeginScrollView(selectionScroll);
            foreach (Transform t in selection) {
                EditorGUILayout.LabelField(t.name);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            // Iterations
            iterations = (int)EditorGUILayout.Slider("Iterations", iterations, 1, 5);

            // Boundaries
            boundaryInterpolation = (CatmullClark.Options.BoundaryInterpolation)
                EditorGUILayout.EnumPopup("Boundaries", boundaryInterpolation);

            // Button
            if (GUILayout.Button("Subdivide")) {
                if (selection.Length == 0) throw new System.Exception("Nothing selected to subdivide");
                var options = new CatmullClark.Options {
                    boundaryInterpolation = boundaryInterpolation,
                };
                foreach (Transform t in selection) {
                    // Add Undo record
                    MeshFilter mf = CatmullClark.CheckMeshFilter(t.gameObject);
                    Undo.RecordObject(mf, "Subdivide " + t.name);
                    // Subdivide
                    CatmullClark.Subdivide(t.gameObject, iterations, options);
                }
                if (selection.Length > 1) {
                    Undo.SetCurrentGroupName(string.Format("Subdivide {0} objects", selection.Length));
                }
            }
        }

        void OnInspectorUpdate() {
            // Permanently update list of selected objects
            // ~ 10 times per sec
            Repaint();
        }

        [MenuItem("Torec/Subdivision")]
        static void ShowSubdivisionUtility() {
            if (window == null) {
                window = ScriptableObject.CreateInstance<SubdivisionUtility>();
            }
            window.ShowUtility();
        }
        static private SubdivisionUtility window = null;
    }
}
