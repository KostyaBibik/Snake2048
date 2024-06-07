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

            // Проверяем, есть ли уже в списке элемент с текущим грейдом
            bool gradeExists = config.config.Exists(option => option.grade == grade);

            // Если элемент с текущим грейдом уже существует, пропускаем его
            if (gradeExists)
                continue;

            // Создаем новый элемент с базовыми значениями
            BoxPoolOption newOption = new BoxPoolOption
            {
                grade = grade,
                maxCount = 1500, // Установите желаемые базовые значения
                initialCount = 500 // Установите желаемые базовые значения
            };

            // Добавляем новый элемент в список
            config.config.Add(newOption);
        }

        // Сортировка по грейдам в порядке возрастания
        config.config.Sort((x, y) => x.grade.CompareTo(y.grade));
    }

}