using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RL.CardEditor;
using TMPro;
using RL.Game;

namespace RL.CardEditor
{
    public class PathsList
    {
        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (0 > value || value >= Count)
                    throw new System.ArgumentOutOfRangeException(nameof(value));

                if (Current != null) Current.gameObject.SetActive(false);

                _index = value;
                _current = Paths[_index];

                Current.gameObject.SetActive(true);

                Dropdown.SetValueWithoutNotify(value);
                CurrentChanged.Invoke(Current);
            }
        }

        public int Count => Paths.Count;

        private CardEditorPath _current;
        public CardEditorPath Current => _current;

        public readonly UnityEngine.Events.UnityEvent<CardEditorPath> CurrentChanged = new();

        public CardEditorPath this[System.Index index] => Paths[index];

        public CardEditorPoint PointPrefab;
        public TMP_Dropdown Dropdown;

        private int lastCreatedIndex = 0;

        public readonly List<CardEditorPath> Paths = new();

        public CardEditorPath Create()
            => Create($"Path {lastCreatedIndex++}");

        public CardEditorPath Create(string name)
        {
            CardEditorPath path = SpawnPath(name);
            path.Add(0, new(0, 0));

            Index = Count - 1;

            return path;
        }

        public CardEditorPath Load(string data)
        {
            var path = SpawnPath("loading");
            path.Load(data);

            Index = Count - 1;

            return path;
        }

        protected virtual CardEditorPath SpawnPath(string name)
        {
            var path = new GameObject(name).AddComponent<CardEditorPath>();
            path.PointPrefab = PointPrefab;
            path.gameObject.SetActive(false);

            Paths.Add(path);
            Dropdown.options.Add(new TMP_Dropdown.OptionData(name));

            Debug.Log($"Создан путь {name}");
            return path;
        }

        public void DeleteLast()
            => Delete(Paths.Count - 1);

        public void DeleteCurrent()
            => Delete(_index);

        public void Delete(CardEditorPath path)
            => Delete(Paths.IndexOf(path));

        /// <summary>
        /// Удаляет путь по индексу.
        /// <para>Если путей больше не осталось будет создан новый.</para>
        /// </summary>
        /// <param name="index">Индекс удаляемого пути.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public void Delete(int index)
        {
            if (0 > index || index >= Count)
                throw new System.ArgumentOutOfRangeException(nameof(index));

            CardEditorPath path = Paths[index];

            Debug.Log($"Удалён путь {path.name}");

            Paths.RemoveAt(index);
            Object.Destroy(path.gameObject);

            if (Count <= 0) Create();

            Index = Paths.Count - 1;

            RefreshDropdownOptions();
            Dropdown.SetValueWithoutNotify(Paths.Count - 1);
        }

        public void RefreshDropdownOptions()
        {
            Dropdown.ClearOptions();

            TMP_Dropdown.OptionDataList list = new();
            for (int i = 0; i < Paths.Count; i++)
                list.options.Add(new(Paths[i].name));

            Dropdown.AddOptions(list.options);
            if (Dropdown.IsExpanded) Dropdown.RefreshShownValue();
        }

        public PathsList(CardEditorPoint pointPrefab, TMP_Dropdown dropdown)
        {
            PointPrefab = pointPrefab;
            Dropdown = dropdown;

            Dropdown.ClearOptions();
            Create();

            Index = 0;
        }
    }
}