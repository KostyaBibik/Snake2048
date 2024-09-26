using System.Collections.Generic;
using Database;
using Enums;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxPoolConfig))]
public class BoxPoolConfigEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BoxPoolConfig config = (BoxPoolConfig)target;

        if (GUILayout.Button("Fill Config"))
        {
            FillConfig(config);
            EditorUtility.SetDirty(config);
        }
    }

    private void FillConfig(BoxPoolConfig config)
    {
        if (config.config == null)
        {
            config.config = new List<BoxPoolOption>();
        }

        foreach (EBoxGrade grade in System.Enum.GetValues(typeof(EBoxGrade)))
        {
            if (grade == EBoxGrade.None)
                continue;

            bool gradeExists = config.config.Exists(option => option.grade == grade);

            if (gradeExists)
                continue;

            BoxPoolOption newOption = new BoxPoolOption
            {
                grade = grade,
                maxCount = 1500, 
                initialCount = 500
            };

            config.config.Add(newOption);
        }

        config.config.Sort((x, y) => x.grade.CompareTo(y.grade));
    }

}
