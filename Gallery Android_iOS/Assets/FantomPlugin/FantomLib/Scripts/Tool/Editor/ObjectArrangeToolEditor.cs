using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ValidStatus = FantomLib.ObjectArrangeTool.ValidStatus;

namespace FantomLib
{
    /// <summary>
    /// Arrange objects at equal intervals (mainly for UI)
    /// 
    /// オブジェクトを等間隔に並べる（主にUI用）
    /// </summary>
    [CustomEditor(typeof(ObjectArrangeTool))]
    public class ObjectArrangeToolEditor : Editor
    {
        SerializedProperty axis;
        GUIContent axisLabel = new GUIContent("Axis");
        SerializedProperty step;
        GUIContent stepLabel = new GUIContent("Step");
        GUIContent dropLabel = new GUIContent("Drop here to add  (If multiple drops, lock the inspector)");

        ReorderableList reorderableObjects;
        SerializedProperty objects;

        int fromIndex;
        int toIndex;
        int resize = -1;
        Vector3 addPosition = Vector3.zero;

        //Check for errors
        ValidStatus validStatus = new ValidStatus();
        string emptyIndexError = "";
        string duplicateError = "";


        private void OnEnable()
        {
            axis = serializedObject.FindProperty("axis");
            step = serializedObject.FindProperty("step");

            objects = serializedObject.FindProperty("objects");
            reorderableObjects = new ReorderableList(serializedObject, objects);
            reorderableObjects.drawHeaderCallback = (rect) =>  
                EditorGUI.LabelField(rect, objects.displayName);
            reorderableObjects.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = objects.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, true);
            };
            reorderableObjects.onRemoveCallback = (list) => {
                var tool = target as ObjectArrangeTool;
                tool.RemoveElement(list.index);
            };
        }

        const float MoveButtonWidthMax = 30;

        public override void OnInspectorGUI()
        {
            var tool = target as ObjectArrangeTool;
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target) , typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            bool edited = false;

            GUILayout.Space(5);

            DropObjectGUI();

            EditorGUI.BeginChangeCheck();
            reorderableObjects.DoLayoutList();
            edited |= EditorGUI.EndChangeCheck();

            GUILayout.Space(5);

            //Display error status
            if (tool.objects != null && tool.objects.Length > 0) {
                if (!string.IsNullOrEmpty(emptyIndexError))
                    EditorGUILayout.HelpBox("There is empty element (index) : " + emptyIndexError, MessageType.Error);
                if (!string.IsNullOrEmpty(duplicateError))
                    EditorGUILayout.HelpBox("There is duplicate object : " + duplicateError, MessageType.Error);
            }

            if (!Application.isPlaying)
            {
                if (tool.executing)
                    return;

                GUILayout.Space(5);

                EditorGUILayout.PropertyField(axis, axisLabel, true);
                EditorGUILayout.PropertyField(step, stepLabel, true);
                if (GUILayout.Button("Arrage"))
                    tool.Arrange();

                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();
                addPosition = EditorGUILayout.Vector3Field("Add Position", addPosition);
                if (GUILayout.Button("+", GUILayout.MaxWidth(MoveButtonWidthMax)))
                {
                    if (!tool.executing)
                        tool.MoveObjects(addPosition);
                }
                if (GUILayout.Button("-", GUILayout.MaxWidth(MoveButtonWidthMax)))
                {
                    if (!tool.executing)
                        tool.MoveObjects(addPosition * -1);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(15);

                EditorGUILayout.LabelField("Copy Index");
                EditorGUI.indentLevel++;
                fromIndex = EditorGUILayout.IntField("From", fromIndex);
                toIndex = EditorGUILayout.IntField("To", toIndex);
                EditorGUI.indentLevel--;
                if (GUILayout.Button("Copy Elements"))
                {
                    Undo.RecordObject(target, "Copy elements");
                    if (tool.CopyElements(fromIndex, toIndex))
                        edited = true;
                }

                GUILayout.Space(15);

                resize = EditorGUILayout.IntField("Resize Length", resize);
                if (GUILayout.Button("Resize"))
                {
                    Undo.RecordObject(target, "Resize length");
                    if (tool.ResizeLength(resize))
                        edited = true;
                }

                GUILayout.Space(15);
                if (GUILayout.Button("Clear All"))
                {
                    Undo.RecordObject(target, "Clear all");
                    if (tool.ClearElements())
                        edited = true;
                }
            }

            serializedObject.ApplyModifiedProperties();

            //Check for errors
            if (edited)
                CheckValidity();
        }

        private void CheckValidity()
        {
            var tool = target as ObjectArrangeTool;
            tool.CheckValidity(ref validStatus);
            emptyIndexError = validStatus.GetEmptyError();
            duplicateError = validStatus.GetDuplicateError();
        }

        public void DropObjectGUI()
        {
            Event ev = Event.current;
            Rect dropRect = GUILayoutUtility.GetRect(0f, 21f, GUILayout.ExpandWidth(true));
            GUI.Box(dropRect, dropLabel);

            switch (ev.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropRect.Contains(ev.mousePosition))
                        return;
             
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
                    if (ev.type == EventType.DragPerform) {
                        Undo.RecordObject(target, "Drop objects");
                        DragAndDrop.AcceptDrag();
                        var tool = target as ObjectArrangeTool;
                        bool edited = false;
                        foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                            edited |= tool.AddElement((GameObject)obj);
                        if (edited)
                            CheckValidity();
                    }

                    ev.Use();
                    break;
            }
        }
    }
}
