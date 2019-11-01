using MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLayerMgr //: MonoBehaviour
{
    static EditorLayerMgr _instance = null;

    private Transform _transform = null;
    public static EditorLayerMgr ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EditorLayerMgr();
            }

            return _instance;
        }
    }

    public Transform transform
    {
        get
        {
                return MapObjectRoot.ins.transform;
        }
        set { _transform = transform; }
    }
    public int curEditLayer = -1;
    //当前有的总层数
    public int curLayerCount = 0;
    public const int MAX_LAYER_COUNT = 60;
    public const int TERRAIN_LAYER_INDEX = 21;

    public Transform[] layers = new Transform[MAX_LAYER_COUNT];

    public MapEditorStroageData.MapLayerData[] layerdatas = new MapEditorStroageData.MapLayerData[MAX_LAYER_COUNT];
    //public static LayerMgr ins = null;

    public void Init()
    {
        this.Clear();
        this.CreateLayer(TERRAIN_LAYER_INDEX);
    }
    
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
        

        //没找到，返回默认创建的地形层
        if (layers[TERRAIN_LAYER_INDEX - 1] == null)
        {
            this.CreateLayer(TERRAIN_LAYER_INDEX);
        }
        return layers[TERRAIN_LAYER_INDEX-1];
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
        if (layerIndex >= 1 && layerIndex <= MAX_LAYER_COUNT)
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
            if (layerIndex == end)
                layerIndex = start;
            else
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
        int start = TERRAIN_LAYER_INDEX;
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
    /// 交换层，调整层的前后顺序
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void SwitchLayer(int leftIndex , int rightIndex)
    {
        Transform leftTs = layers[leftIndex - 1];
        Transform rightTs = layers[rightIndex - 1];

        //rightTs的位置移动leftTs
        if ((leftTs == null) && (rightTs != null))
        {
            layers[leftIndex - 1] = rightTs;
            layers[rightIndex - 1] = null;
            layers[leftIndex - 1].gameObject.name = "layer" + leftIndex.ToString();
            layerdatas[leftIndex - 1]= layerdatas[rightIndex - 1];
            layerdatas[rightIndex - 1]= null;
            layerdatas[leftIndex - 1].layerIndex = leftIndex;

        }
        //leftTs的位置移动rightTs
        else if ((leftTs != null) && (rightTs == null))
        {
            layers[rightIndex - 1] = leftTs;
            layers[leftIndex - 1] = null;
            layers[rightIndex - 1].gameObject.name = "layer" + rightIndex.ToString();
            layerdatas[rightIndex - 1] = layerdatas[leftIndex - 1];
            layerdatas[leftIndex - 1] = null;
            layerdatas[rightIndex - 1].layerIndex = rightIndex;
        }
        else//两者交换
        {
            //交换深度坐标
            float z = leftTs.position.z;
            Vector3 lpos = leftTs.position;
            lpos.z = rightTs.position.z;
            leftTs.position = lpos;

            Vector3 rpos = rightTs.position;
            rpos.z = z;
            rightTs.position = rpos;

            //交换层索引
            layers[leftIndex - 1] = rightTs;
            layers[rightIndex - 1] = leftTs;

            //修改名字
            layers[leftIndex - 1].gameObject.name = "layer" + leftIndex.ToString();
            layers[rightIndex - 1].gameObject.name = "layer" + rightIndex.ToString();

            /////////////////////////////////////////////////////////
            //交换数据（层索引不用交换，设计规则是数组下标决定层）
            float tmpMoveFactorX = layerdatas[leftIndex - 1].moveFactorX;
            float tmpMoveFactorY = layerdatas[leftIndex - 1].moveFactorY;
            layerdatas[leftIndex - 1].moveFactorX = layerdatas[rightIndex - 1].moveFactorX;
            layerdatas[leftIndex - 1].moveFactorY = layerdatas[rightIndex - 1].moveFactorY;
            layerdatas[rightIndex - 1].moveFactorX = tmpMoveFactorX;
            layerdatas[rightIndex - 1].moveFactorY = tmpMoveFactorY;
        }



        RefreshData();


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
                //pos.z += offsetPos.z * (1 - layerdatas[i].moveFactor);
                //pos.y += offsetPos.y * (1 - layerdatas[i].moveFactor);
                if (offsetPos.x > 0.05)
                {
                    Debug.Log("offsetPos.x: " + offsetPos.x);
                }

                if (offsetPos.x > 0.05)
                {
                    Debug.Log("offsetPos.y: " + offsetPos.y);
                }

                if (offsetPos.z > 0.05)
                {
                    Debug.Log("offsetPos.z: " + offsetPos.z);
                }
                pos.y += offsetPos.y * (1.0f - layerdatas[i].moveFactorY)*0.3f;
                pos.z += offsetPos.z * (1.0f - layerdatas[i].moveFactorX)*0.3f;
                layers[i].position = pos;
            }
        }
    }


    public void RefreshData()
    {
        Debug.Log("@@@@@@@@@@@@EditorLayerMgr->RefreshData");

        var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorMapLayer>();
        if (dlg != null)
            dlg.Refresh();

        for (int i = 0; i < MAX_LAYER_COUNT; i++)
        {
            if (layers[i] != null)
            {
                Vector3 worldPos = layers[i].position;

                if (i <= 19)
                {
                    layers[i].position = new Vector3(2000+(20-i)*100, worldPos.y, worldPos.z);
                }
                else if (i>=21 && i <= 39) //i=20 不缩放
                {
                    //20= 200 /10 每层多200空间，前后都多200   10是组件本身的默认深度
                    layers[i].localScale = new Vector3(1.0f, 1.0f, (i - 20) * 20);
                }
                else
                {
                    layers[i].position = new Vector3(-2000 -  (i -40) * 100, worldPos.y, worldPos.z);
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
        //地形层只能跟地形层交换
        if (this.IsTerrainLayer(curLayerIndex))
        {
            int tmpIndex = curLayerIndex - 1;

            //不跟地形层交换
            while ((tmpIndex > TERRAIN_LAYER_INDEX) && (layers[tmpIndex - 1] == null) )
            {
                tmpIndex--;
            }

            if ((tmpIndex >= TERRAIN_LAYER_INDEX) && (layers[tmpIndex - 1] != null))
                return tmpIndex;
            else //没找到，找一个空的层
                return -1;
        }
        else
        {
            int tmpIndex = curLayerIndex - 1;

            //不跟地形层交换
            while ((tmpIndex >= 1) && ((layers[tmpIndex - 1] == null) || (this.IsTerrainLayer(tmpIndex))))
            {
                tmpIndex--;
            }

            if ((tmpIndex >= 1) && (layers[tmpIndex - 1] != null))
                return tmpIndex;
            else //没找到，找一个空的层
            {
                int emptyIndex = curLayerIndex - 1;
                //原来在地形层下面（前面）的一道地形层上面（后面）
                if (emptyIndex >= TERRAIN_LAYER_INDEX+19)
                    return TERRAIN_LAYER_INDEX - 1;

                return -1;
            }
        }

    }
    /// <summary>
    /// 取得当前选中层的下面一层的索引
    /// </summary>
    /// <param name="curLayerIndex"></param>
    /// <returns></returns>

    public int GetDownLayerIndex(int curLayerIndex)
    {
        //地形层只能跟地形层交换
        if (this.IsTerrainLayer(curLayerIndex))
        {
            int tmpIndex = curLayerIndex + 1;

            while ((tmpIndex < TERRAIN_LAYER_INDEX + 19) && (layers[tmpIndex - 1] == null) )
            {
                tmpIndex++;
            }

            if ((tmpIndex <= TERRAIN_LAYER_INDEX + 19) && (layers[tmpIndex - 1] != null))
                return tmpIndex;
            else
                return -1;
        }
        else //非地形层只能跟非地形层交换
        {
            int tmpIndex = curLayerIndex + 1;

            while ((tmpIndex <= MAX_LAYER_COUNT) && ((layers[tmpIndex - 1] == null) || (this.IsTerrainLayer(tmpIndex))))
            {
                tmpIndex++;
            }

            if ((tmpIndex <= MAX_LAYER_COUNT) && (layers[tmpIndex - 1] != null))
                return tmpIndex;
            else //没找到，找一个空的层
            {
                int emptyIndex = curLayerIndex + 1;
                //原来在地形层上面（后面）的移动地形层下面（前面）
                if (emptyIndex <= TERRAIN_LAYER_INDEX)
                    return TERRAIN_LAYER_INDEX + 20;

                return -1;
            }
        }

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

    public bool IsTerrainLayer(int layerIndex)
    {
        if ((layerIndex >= TERRAIN_LAYER_INDEX) && (layerIndex < TERRAIN_LAYER_INDEX + 20))
            return true;

        return false;
    }

    public void SetMoveFactor(float x , float y)
    {
        if (layerdatas[this.curEditLayer-1] != null)
        {
            layerdatas[this.curEditLayer-1].moveFactorX = x;
            layerdatas[this.curEditLayer-1].moveFactorY = y;
        }
    }

    //void Awake()
    //{
    //    Debug.LogError("2222222222222222222222 LayerMgr.Awake");

    //    ins = this;
    //}
    //void OnDestroy()
    //{
    //    ins = null;
    //}

}
