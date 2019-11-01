using MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeLayerMgr
{
    static RuntimeLayerMgr _instance = null;

    private Transform _transform = null;
    public static RuntimeLayerMgr ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RuntimeLayerMgr();
            }

            return _instance;
        }
    }

    public Transform transform
    {
        get
        {
            return _transform;
        }
        set
        {
            _transform = value;
        }
    }
    public int curEditLayer = -1;
    //当前有的总层数
    public int curLayerCount = 0;
    public const int MAX_LAYER_COUNT = 60;
    public Transform[] layers = new Transform[MAX_LAYER_COUNT];

    public MapEditorStroageData.MapLayerData[] layerdatas = new MapEditorStroageData.MapLayerData[MAX_LAYER_COUNT];
    //public static RuntimeLayerMgr ins = null;

    public void Clear()
    {
        for (int i=0; i< MAX_LAYER_COUNT; ++i)
        {
            layers[i] = null;
            layerdatas[i] = null;
        }
    }

    public Transform GetCurLayerTransform()
    {
        if ((curEditLayer > 0) && (curEditLayer <= MAX_LAYER_COUNT))
        {
            if (layers[curEditLayer - 1] != null)
                return layers[curEditLayer - 1];
        }

        return transform;
    }

    public GameObject GetLayerObjectByIndex(int layerIndex)
    {
        if (layerIndex > 0)
        {
            return layers[layerIndex - 1].gameObject;
        }

        return null;
    }

    public Transform GetLayerByIndex(int layerIndex)
    {
        if (layerIndex > 0)
        {
            return layers[layerIndex - 1];
        }

        //--没有层节点，就用层节点上一层的节点（MapObjectRoot所在的节点）
        return transform;
    }

    public MapEditorStroageData.MapLayerData GetLayerDataByIndex(int layerIndex)
    {
        if (layerIndex > 0 && layerIndex < MAX_LAYER_COUNT)
        {
            return layerdatas[layerIndex - 1];
        }

        return null;
    }

    /// <summary>
    /// 获取未用的层索引
    /// </summary>
    /// <returns></returns>
    public int GetFreeLayerIndex(int start, int end)
    {
        int layerIndex = start;
        int count = 0;
        if (curEditLayer >= start && curEditLayer < end)
        {
            layerIndex = curEditLayer + 1;
        }

        while ((layers[layerIndex - 1] != null) && (count <= 20))
        {
            layerIndex = layerIndex % end + 1;
            count++;
        }

        if (layers[layerIndex - 1] == null)
            return layerIndex;
        else
            return -1;
    }

    /// <summary>
    ///  取得空闲的背景层索引,
    /// </summary>
    /// <returns>-1表示没找到</returns>
    public int GetFreeBackLayerIndex()
    {
        int start = 1;
        int end = 20;

        return GetFreeLayerIndex(start, end);
    }

    /// <summary>
    ///  取得空闲的地形层索引,
    /// </summary>
    /// <returns>-1表示没找到</returns>
    public int GetFreeTerrainLayerIndex()
    {
        int start = 21;
        int end = 40;

        return GetFreeLayerIndex(start, end);
    }

    /// <summary>
    /// 去掉空闲的前景层索引
    /// </summary>
    /// <returns>-1 表示没找到</returns>
    public int GetFreeForeLayerIndex()
    {
        int start = 41;
        int end = 60;

        return GetFreeLayerIndex(start, end);
    }

    /// <summary>
    /// 创建前景装饰层   41-60层属于前景
    /// </summary>
    public void CreateForeDecLayer()
    {
        int layerIndex = GetFreeForeLayerIndex();
        if (layerIndex != -1)
        {
            CreateLayer(layerIndex);
            RefreshData();
        }
    }

    /// <summary>
    ///  创建地形层 21-40层属于地形层
    /// </summary>
    public void CreateTerrainLayer()
    {
        int layerIndex = GetFreeTerrainLayerIndex();
        if (layerIndex != -1)
        {
            CreateLayer(layerIndex);
            RefreshData();
        }
    }

    /// <summary>
    /// 创建背景装饰层  1-20属于背景
    /// </summary>

    public void CreateBackDecLayer()
    {
        int layerIndex = GetFreeBackLayerIndex();
        if (layerIndex != -1)
        {
            CreateLayer(layerIndex);
            RefreshData();
        }
    }
   
    /// <summary>
    /// 创建层
    /// </summary>
    /// <param name="layerIndex">1-->MAX_LAYER_COUNT</param>
    //public void CreateLayer()
    //{
    //    int layerIndex = GetFreeLayerIndex();
    //    float moveFactor = 1.0f;
    //    if (layerIndex == 2)
    //    {
    //        moveFactor = 0.8f;
    //    }
    //    else if (layerIndex == 3)
    //    {
    //        moveFactor = 0.5f;
    //    }
    //    else  if (layerIndex == 4)
    //    {
    //        moveFactor = 0.2f;
    //    }

    //    Debug.Log("@@@CreateLayer layerIndex: " + layerIndex + " moveFactor: " + moveFactor);

    //    MapEditorStroageData.MapLayerData data = new MapEditorStroageData.MapLayerData { layerIndex = layerIndex, moveFactor = moveFactor };

    //    CreateLayer(data);
    //}

    public void CreateLayer(int layerIndex, float moveFactorX = 1.0f, float moveFactorY = 1.0f)
    {
        MapEditorStroageData.MapLayerData data = new MapEditorStroageData.MapLayerData { layerIndex = layerIndex, moveFactorX = moveFactorX, moveFactorY = moveFactorY };

        CreateLayer(data);
    }

    public void CreateLayer(MapEditorStroageData.MapLayerData data)
    {
        if (data.layerIndex > 0)
        {
            if (layers[data.layerIndex - 1] == null)
            {
                GameObject layerObj = new GameObject("layer" + data.layerIndex.ToString());
                layerObj.transform.parent = transform;
                layerObj.transform.localRotation = Quaternion.identity;
                layers[data.layerIndex - 1] = layerObj.transform;
                layerdatas[data.layerIndex - 1] = data;
                curLayerCount += 1;

                //新加的层属于当前正在编辑的层
                curEditLayer = data.layerIndex;
            }
            else
            {
                layerdatas[data.layerIndex - 1] = data;
            }
        }
    }



    /// <summary>
    /// 删除层
    /// </summary>
    /// <param name="layerIndex">1-->MAX_LAYER_COUNT</param>
    public void DeleteLayer(int layerIndex)
    {
        if (layerIndex > 0)
        {
            GameObject.DestroyImmediate(layers[layerIndex - 1].gameObject);
            layers[layerIndex - 1] = null;

            layerdatas[layerIndex - 1] = null;


            RefreshData();
        }
    }

    /// <summary>
    /// 选择层
    /// </summary>
    /// <param name="layerIndex">1-->MAX_LAYER_COUNT</param>
    public void SelectLayer(int layerIndex)
    {
        curEditLayer = layerIndex;
        RefreshData();
    }


    /// <summary>
    /// 相机移动的时候相应的层也对应移动
    /// </summary>
    /// <param name="offsetZ">因为相机设置的关系这是水平方向</param>
    /// <param name="offsetY">垂直方向</param>
    public void OnCameraMove(Vector3 offsetPos)
    {
        for (int i=0; i < MAX_LAYER_COUNT; i++)
        {
            //moveFactor接近1不处理，减少运算
            if (layers[i] != null && layerdatas[i].moveFactorX < 0.999f)
            {
                Vector3 pos = layers[i].position;
                //if (offsetPos.x > 0.05)
                //{
                //    Debug.Log("offsetPos.x: " + offsetPos.x);
                //}

                //if (offsetPos.x > 0.05)
                //{
                //    Debug.Log("offsetPos.y: " + offsetPos.y);
                //}

                //if (offsetPos.z > 0.05)
                //{
                //    Debug.Log("offsetPos.z: " + offsetPos.z);
                //}
                pos.y += offsetPos.y * (1.0f - layerdatas[i].moveFactorY)*0.3f;
                pos.z += offsetPos.z * (1.0f - layerdatas[i].moveFactorX)*0.3f;
                layers[i].position = pos;
            }
        }
    }


    public void RefreshData()
    {
#if UNITY_EDITOR
        Debug.Log("@@@@@@@@@@@@RuntimeLayerMgr->RefreshData");
#endif
        for (int i = 0; i < MAX_LAYER_COUNT; i++)
        {
            if (layers[i] != null)
            {
                Vector3 worldPos = layers[i].position;

                if (i <= 19)
                {
                    layers[i].position = new Vector3(2000 + (20 - i) * 100, worldPos.y, worldPos.z);
                }
                else if (i >= 21 && i <= 39) //i=20 不缩放
                {
                    //20= 200 /10 每层多200空间，前后都多200   10是组件本身的默认深度
                    layers[i].localScale = new Vector3(1.0f, 1.0f, (i - 20) * 20);
                }
                else
                {
                    layers[i].position = new Vector3(-2000 - (i - 40) * 100, worldPos.y, worldPos.z);
                }
            }
        }
    }


    /// <summary>
    /// 取得当前选中层的上面一层的索引
    /// </summary>
    /// <param name="curLayerIndex"></param>
    /// <returns></returns>
    public int GetUpLayerIndex(int curLayerIndex)
    {
        int tmpIndex = curLayerIndex - 1;

        while ((tmpIndex >= 1) && (layers[tmpIndex - 1] == null) )
        {
            tmpIndex--;
        }

        if ((tmpIndex >= 1) && (layers[tmpIndex - 1] != null))
            return tmpIndex;
        else
            return -1;
    }
    /// <summary>
    /// 取得当前选中层的下面一层的索引
    /// </summary>
    /// <param name="curLayerIndex"></param>
    /// <returns></returns>

    public int GetDownLayerIndex(int curLayerIndex)
    {
        int tmpIndex = curLayerIndex + 1;

        while ((tmpIndex <= MAX_LAYER_COUNT) && (layers[tmpIndex - 1] == null))
        {
            tmpIndex++;
        }

        if ((tmpIndex <= MAX_LAYER_COUNT) &&  (layers[tmpIndex - 1] != null))
            return tmpIndex;
        else
            return -1;
    }

    public void ShowLayer(int layerIndex)
    {
        if (layers[layerIndex - 1] != null)
            layers[layerIndex - 1].gameObject.SetActive(true);
    }

    public void HideLayer(int layerIndex)
    {
        if (layers[layerIndex - 1] != null)
            layers[layerIndex - 1].gameObject.SetActive(false);
    }

    //void Awake()
    //{
    //    Debug.LogError("2222222222222222222222 RuntimeLayerMgr.Awake");

    //    ins = this;
    //}
    //void OnDestroy()
    //{
    //    ins = null;
    //}

}
