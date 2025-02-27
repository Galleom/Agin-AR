﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditorInternal;

namespace UnityEditor.Animations.Rigging
{
    [CustomEditor(typeof(MultiReferentialConstraint))]
    public class MultiReferentialConstraintEditor : Editor
    {
        static readonly GUIContent k_DrivingLabel = new GUIContent("Driving");
        static readonly GUIContent k_ReferenceObjectsLabel = new GUIContent("Reference Objects");

        SerializedProperty m_Weight;
        SerializedProperty m_Driver;
        SerializedProperty m_SourceObjects;

        SerializedProperty m_SourceObjectsToggle;
        List<string> m_DrivingLabels;
        ReorderableList m_ReorderableList;

        void OnEnable()
        {
            m_Weight = serializedObject.FindProperty("m_Weight");
            m_SourceObjectsToggle = serializedObject.FindProperty("m_SourceObjectsGUIToggle");

            var data = serializedObject.FindProperty("m_Data");
            m_Driver = data.FindPropertyRelative("m_Driver");
            m_SourceObjects = data.FindPropertyRelative("m_SourceObjects");

            m_ReorderableList = ReorderableListHelper.Create(serializedObject, m_SourceObjects, false);
        
            m_DrivingLabels = new List<string>();
            UpdateDrivingList(m_ReorderableList.count);
            if (m_ReorderableList.count == 0)
                ((MultiReferentialConstraint)serializedObject.targetObject).data.sourceObjects.Add(JobTransform.defaultNoSync);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Weight);

            UpdateDrivingList(m_ReorderableList.count);
            UpdateDrivingLables();
            m_Driver.intValue = EditorGUILayout.Popup(k_DrivingLabel, m_Driver.intValue, m_DrivingLabels.ToArray());

            m_SourceObjectsToggle.boolValue = EditorGUILayout.Foldout(m_SourceObjectsToggle.boolValue, k_ReferenceObjectsLabel);
            if (m_SourceObjectsToggle.boolValue)
            {
                EditorGUI.indentLevel++;
                m_ReorderableList.DoLayoutList();
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        void UpdateDrivingList(int size)
        {
            int count = m_DrivingLabels.Count;
            if (count == size)
                return;

            if (size < count)
                m_DrivingLabels.RemoveRange(size, count - size);
            else if (size > count)
            {
                if (size > m_DrivingLabels.Capacity)
                    m_DrivingLabels.Capacity = size;
                m_DrivingLabels.AddRange(Enumerable.Repeat("", size - count));
            }
        }

        void UpdateDrivingLables()
        {
            int count = Mathf.Min(m_DrivingLabels.Count, m_SourceObjects.arraySize);
            for (int i = 0; i < count; ++i)
            {
                var element = m_SourceObjects.GetArrayElementAtIndex(i);
                var transform = element.FindPropertyRelative("transform");
                var name = transform.objectReferenceValue ? transform.objectReferenceValue.name : "None";
                m_DrivingLabels[i] = i + " : " + name; 
            }
        }
    }
}
