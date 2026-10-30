using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Study3D.FpsGame
{
    public class ObjectPoolManager : MonoBehaviour
    {
        [Serializable]
        public class ObjectPoolData
        {
            [FormerlySerializedAs("Key")]
            [SerializeField] private string key;
            [FormerlySerializedAs("Prefab")]
            [SerializeField] private GameObject prefab;
            [FormerlySerializedAs("ExpandSize")]
            [SerializeField] private byte expandSize;
            [FormerlySerializedAs("Parent")]
            [SerializeField] private Transform parent;

            public string Key => key;
            public GameObject Prefab => prefab;
            public byte ExpandSize => expandSize;
            public Transform Parent => parent;
        }

        private class ObjectPool
        {
            private readonly Stack<IObjectPoolItem> pool = new Stack<IObjectPoolItem>();
            private readonly GameObject prefab;
            private readonly Transform parent;
            private readonly int expandSize;
            private readonly string key;

            public ObjectPool(GameObject prefab, Transform parent, int expandSize, string key)
            {
                this.prefab = prefab;
                this.parent = parent;
                this.expandSize = Mathf.Max(1, expandSize);
                this.key = key;
                Expand();
            }

            private void Expand()
            {
                for (int i = 0; i < expandSize; i++)
                {
                    var instance = Instantiate(prefab, parent).GetComponent<IObjectPoolItem>();
                    instance.Key = key;
                    Return(instance);
                }
            }

            public IObjectPoolItem Get()
            {
                if (pool.Count == 0) Expand();
                return pool.Pop();
            }

            public void Return(IObjectPoolItem item)
            {
                item.GameObject.SetActive(false);
                pool.Push(item);
            }
        }

        public static ObjectPoolManager Instance { get; private set; }

        [Header("만들 풀 목록")]
        [FormerlySerializedAs("ObjectPoolDatas")]
        [SerializeField] private ObjectPoolData[] objectPoolDatas;

        private readonly Dictionary<string, ObjectPool> objectPoolDic = new Dictionary<string, ObjectPool>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"{name}: ObjectPoolManager가 이미 있습니다. 이 컴포넌트는 쓰지 않습니다");
                enabled = false;
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            foreach (var data in objectPoolDatas)
            {
                CreatePool(data);
            }
        }

        public void CreatePool(ObjectPoolData data)
        {
            if (objectPoolDic.ContainsKey(data.Key))
            {
                Debug.LogWarning($"{data.Key}값이 Pool에 이미 존재합니다.");
                return;
            }

            if (data.Prefab.GetComponent<IObjectPoolItem>() == null)
            {
                Debug.LogWarning($"{data.Prefab.name}의 프리펩에 IObjectPoolItem을" +
                                 $"상속받은 컴포넌트가 없습니다.");
                return;
            }

            Transform poolParent = data.Parent == null ? transform : data.Parent;
            objectPoolDic.Add(data.Key, new ObjectPool(data.Prefab, poolParent, data.ExpandSize, data.Key));
        }

        public IObjectPoolItem GetObjectOrNull(string key)
        {
            if (objectPoolDic.TryGetValue(key, out var pool)) return pool.Get();

            Debug.LogWarning($"{key}값의 Pool이 존재하지 않습니다");
            return null;
        }

        public void ReturnToPool(IObjectPoolItem item)
        {
            if (objectPoolDic.TryGetValue(item.Key, out var pool) == false)
            {
                Debug.LogWarning($"{item.Key}값의 Pool이 존재하지 않습니다");
                return;
            }

            pool.Return(item);
        }
    }
}
