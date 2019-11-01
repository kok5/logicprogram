using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace MapSerializeData
{
    [System.Serializable]
    public class Prefab40SerializeData : Serializable.SerializeBase
    {
        [Description("可见性")]
        public bool visilbe;
        [Description("半径")]
        public float radius;
        [Description("宽度")]
        public int width;
        [Description("长度")]
        public int height;

        [Description("长度xx")]
        public int heightxx;

        [Description("长度yy")] [SerializeField]
        public int heightyy;
    }

    [System.Serializable]
    public class Prefab37SerializeData : Serializable.SerializeBase
    {
        public Serializable.Vector3 left;
        public Serializable.Vector3 right;
    }
}


