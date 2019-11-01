using MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMergeMgr
{
    static TerrainMergeMgr _instance = null;

    public bool needMerge = false;

    public const int ROW_COUNT = 640;
    public const int COLUMN_COUNT = 640;

    //误差范围 (合并时判断用)
    private float delta = 0.1f;

    //组件位置相对网格的 偏移
    //public int ROW_OFFSET = 0;
    //public int COLUMN_OFFSET = 0;

    //组件分布
    private float maxLeft = 0f;
    private float maxRight = 0f;
    private float maxDown = 0f;
    private float maxUp = 0f;

    private int rowOffset = 0;
    private int columnOfsset = 0;

    private int maxRow = 0;
    private int maxColumn = 0;

    //网格数组
    private Serializable.MapObject[,] array = new Serializable.MapObject[ROW_COUNT, COLUMN_COUNT];

    //新合出来的(只有物理)
    private List<Serializable.MapObject> collideMapObjects = new List<Serializable.MapObject>();

    //被合并的(只有图形)
    private List<Serializable.MapObject> terrainMapObjects = new List<Serializable.MapObject>();

    //未参与合并的
    private List<Serializable.MapObject> remainMapObjects = new List<Serializable.MapObject>();

    public static TerrainMergeMgr ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TerrainMergeMgr();
            }

            return _instance;
        }
    }


    public void Init()
    {
        for (int i = 0; i < ROW_COUNT; i++)
        {
            for (int j = 0; j < COLUMN_COUNT; j++)
            {
                array[i, j] = null;
            }
        }

        this.needMerge = false;
        this.collideMapObjects.Clear();
        this.terrainMapObjects.Clear();
        this.remainMapObjects.Clear();
    }

    private void AddGridObject(Serializable.MapObject obj)
    {
        int rowIndex = Mathf.FloorToInt(obj.transform.position.y) + rowOffset;
        if (rowIndex < 0)
        {
            Debug.LogError("Error rowIndex: " + obj.prefab + "  pos: " + obj.transform.position);
            rowIndex = 0;
        }
        //
        int columnIndex = Mathf.FloorToInt(obj.transform.position.z) + columnOfsset;
        if (columnIndex < 0)
        {
            Debug.LogError("Error rowIndex: " + obj.prefab + "  pos: " + obj.transform.position);
            columnIndex = 0;
        }

        array[rowIndex, columnIndex] = obj;

    }

    /// <summary>
    /// 合并
    /// </summary>
    public void MergeGrid()
    {
        ///////////////////////////////////////////////////////
        //先横向合
        for (int i = 0; i <= maxRow; i++)
        {
            int start = -1;
            int end = -1;
            for (int j = 0; j <= maxColumn; j++)
            {
                if (array[i, j] != null)
                {
                    if (start == -1)
                        start = j;
                    else
                        end = j;
                }
                else //横向中断了
                {
                    //之前已经有连续的了
                    if ((end > start) && (start >= 0))
                    {

                        Serializable.MapObject obj = new Serializable.MapObject();
                        obj.Clone(array[i, start]);
                        //调整位置要合并组件的中间
                        obj.transform.position = (array[i, start].transform.position + array[i, end].transform.position) /2;
                        //调整缩放系数
                        obj.transform.scale.z = obj.transform.scale.z * (end - start + 1);

                        for (int k = start; k <= end; k++)
                        {
                            //被合了
                            this.terrainMapObjects.Add(array[i, k]);

                            array[i, k] = null;
                        }

                        this.AddGridObject(obj);
                    }
                    //重置变量
                    start = -1;
                    end = -1;
                }
                    
            }
        }


        ///////////////////////////////////////////////////////
        //后竖向合
        for (int j = 0; j <= maxColumn; j++)
        {
            int start = -1;
            int end = -1;
            for (int i = 0; i <= maxRow; i++)
            {
                if (array[i, j] != null)
                {
                    if (start == -1)
                    {
                        start = i;
                        end = i;
                    }
                    else
                        end = i;
                }
                else //竖向中断了
                {
                    //之前已经有连续的了
                    if ((end >= start) && (start >= 0))
                    {

                        //新生成的合并组件
                        Serializable.MapObject obj = new Serializable.MapObject();
                        obj.Clone(array[start, j]);
 
                        //获取横向最小的格子(Object)
                        float minZScale = array[start, j].transform.scale.z;
                        for (int a = start; a <= end; a++)
                        {
                            if (array[a, j].transform.scale.z < minZScale)
                                minZScale = array[a, j].transform.scale.z;
                        }

                        //如果两端还有更宽的组件，延伸一格或两格
                        bool hasMoreStartGrid = this.HasMoreStartGrid(start, j, minZScale);
                        bool hasMoreEndGrid = this.HasMoreEndGrid(end, j, minZScale);
                        if (hasMoreStartGrid && hasMoreEndGrid)
                        {
                            //调整位置要合并组件的中间
                            obj.transform.position = (array[start, j].transform.position + array[end, j].transform.position) / 2;
                            //多二格
                            obj.transform.scale.y = obj.transform.scale.y * (end - start + 3);
                            obj.transform.scale.z = minZScale;
                        }
                        else if (hasMoreStartGrid)
                        {
                            //调整位置要合并组件的中间
                            obj.transform.position = (array[start, j].transform.position + array[end, j].transform.position) / 2;
                            //start更小
                            if (array[end, j].transform.position.y >= array[start, j].transform.position.y)
                                obj.transform.position.y -= 0.5f;
                            else
                            {
                                obj.transform.position.y += 0.5f;
                            }
                            //多一格
                            obj.transform.scale.y = obj.transform.scale.y * (end - start + 2);
                            obj.transform.scale.z = minZScale;
                        }
                        else if (hasMoreEndGrid)
                        {
                            //调整位置要合并组件的中间
                            obj.transform.position = (array[start, j].transform.position + array[end, j].transform.position) / 2;
                            //end更大
                            if (array[end, j].transform.position.y >= array[start, j].transform.position.y)
                                obj.transform.position.y += 0.5f;
                            else
                            {
                                obj.transform.position.y -= 0.5f;
                            }
                            //多一格
                            obj.transform.scale.y = obj.transform.scale.y * (end - start + 2);
                            obj.transform.scale.z = minZScale;
                        }
                        else
                        {
                            //调整位置要合并组件的中间
                            obj.transform.position = (array[start, j].transform.position + array[end, j].transform.position) / 2;
                            //调整缩放系数
                            obj.transform.scale.y = obj.transform.scale.y * (end - start + 1);
                            obj.transform.scale.z = minZScale;
                        }


                        //新的不再被合并了，输出
                        if (end > start || obj.transform.scale.z > 1.0f)
                            this.collideMapObjects.Add(obj);

                        //处理要被合并的格子(Object)
                        for (int k = start; k <= end; k++)
                        {
                            //宽度相等的格子(Object)
                            if (this.IsFloatEqual(array[k, j].transform.scale.z, minZScale))
                            {
                                if (end > start) //2个及以上格子要合并
                                {
                                    //之前没有被合并过的格子(Object)
                                    if (this.IsFloatEqual(array[k, j].transform.scale.z, 1.0f))
                                    {
                                        this.terrainMapObjects.Add(array[k, j]);

                                    }

                                }
                                array[k, j] = null;

                            }
                            else// !=minZScale
                            {
                                //之前被合过的，又大于最小宽度， 并放入collide中
                                if (array[k, j].transform.scale.z > 1.0f)
                                {
                                    this.collideMapObjects.Add(array[k,j]);
                                    array[k, j] = null;
                                }
                            }
                            
                        }

                        //合并后的继续放在array里因为后续的合并还需要之前的信息（看看合并能不能扩展一格）
                        this.AddGridObject(obj);

                    }
                    //重置变量
                    start = -1;
                    end = -1;
                }

            }
        }

    }

    //array[k, j] 下一行是不是有更宽的组件（水平方向是覆盖array[k,j]的）

    public bool HasMoreStartGrid(int k, int j, float minZScale)
    {
        
        if (k-1 >= 0)
        {
            //水平方向范围 scale代表几个格子, delta:适当缩小范围
            float left = array[k, j].transform.position.z - minZScale / 2 + this.delta;
            float right = array[k, j].transform.position.z + minZScale / 2 - this.delta;
            for (int jj = 0; jj <= maxColumn; jj++)
            {
                if (array[k - 1, jj] != null)
                {
                    //水平方向范围 scale代表几个格子
                    float curLeft = array[k-1, jj].transform.position.z - array[k - 1, jj].transform.scale.z / 2;
                    float curRight = array[k-1, jj].transform.position.z + array[k - 1, jj].transform.scale.z / 2;
                    if (curLeft <= left && curRight >= right)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        else
        {
            return false;
        }


    }

    public bool HasMoreEndGrid(int k, int j, float minZScale)
    {

        if (k + 1 <= maxRow)
        {
            //水平方向范围 scale代表几个格子, delta:适当缩小范围
            float left = array[k, j].transform.position.z - minZScale / 2 + this.delta;
            float right = array[k, j].transform.position.z + minZScale / 2 - this.delta;
            for (int jj = 0; jj <= maxColumn; jj++)
            {
                if (array[k + 1, jj] != null)
                {
                    //水平方向范围 scale代表几个格子
                    float curLeft = array[k + 1, jj].transform.position.z - array[k + 1, jj].transform.scale.z / 2;
                    float curRight = array[k + 1, jj].transform.position.z + array[k + 1, jj].transform.scale.z / 2;
                    if (curLeft <= left && curRight >= right)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        else
        {
            return false;
        }


    }

    /// <summary>
    /// 组件是否可以合并 (根据id范围快速确定)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool CanBeMerge(Serializable.MapObject obj)
    {
        var strId = obj.prefab;
        int id = 0;
        int.TryParse(strId, out id);
        if (id >= 14000 && id < 15000)
            return true;
        else
            return false;
    }

    private void InitGrid(Serializable.Map map)
    {
        List<Serializable.MapObject> gridMapObjects = new List<Serializable.MapObject>();
        bool init = false;
        var objects = map.objects;
        for (int i = 0; i < objects.Count; i++)
        {

            if (this.CanBeMerge(objects[i]))
            {
                gridMapObjects.Add(objects[i]);
                if (!init)
                {
                    this.maxLeft = objects[i].transform.position.z;
                    this.maxRight = objects[i].transform.position.z;
                    this.maxDown = objects[i].transform.position.y;
                    this.maxUp = objects[i].transform.position.y;

                    init = true;
                }
                else
                {
                    // z<------- maxLeft代表z的正方向
                    if (objects[i].transform.position.z > this.maxLeft)
                        this.maxLeft = objects[i].transform.position.z;

                    if (objects[i].transform.position.z < this.maxRight)
                        this.maxRight = objects[i].transform.position.z;

                    if (objects[i].transform.position.y < this.maxDown)
                        this.maxDown = objects[i].transform.position.y;

                    if (objects[i].transform.position.y > this.maxUp)
                        this.maxUp = objects[i].transform.position.y;
                }
            }

            else
            {
                this.remainMapObjects.Add(objects[i]);
            }

        }


        int tmpMaxLeft = Mathf.FloorToInt(this.maxLeft);
        int tmpMaxRight = Mathf.FloorToInt(this.maxRight);
        int tmpMaxDown = Mathf.FloorToInt(this.maxDown);
        int tmpMaxUp = Mathf.FloorToInt(this.maxUp);

        this.maxRow = tmpMaxUp - tmpMaxDown + 1;
        this.maxColumn = tmpMaxLeft - tmpMaxRight +1;
        this.rowOffset = -tmpMaxDown;
        this.columnOfsset = - tmpMaxRight;

        for (int j = 0; j < gridMapObjects.Count; j++)
        {
            this.AddGridObject(gridMapObjects[j]);
        }

    }

    private void UpdateObjectsToMap(Serializable.Map map)
    {
        map.objects.Clear();
        if (this.remainMapObjects.Count > 0) 
            map.objects.AddRange(this.remainMapObjects);

        for (int i = 0; i < ROW_COUNT; i++)
        {
            for (int j = 0; j < COLUMN_COUNT; j++)
            {
                if (array[i, j] != null)
                {
                    //合并生成的放在collide
                    if ((array[i, j].transform.scale.z > 1.0f) || (array[i, j].transform.scale.y > 1.0f))
                    {
                        //注释掉是因为合并的时候已经加入collideMapObjects中了
                        //this.collideMapObjects.Add(array[i, j]);
                    }
                    else
                    {
                        //没有被合并的单元格
                        map.objects.Add(array[i, j]);
                    }
                };
            }
        }


        if (this.collideMapObjects.Count > 0)
        {
            map.colliders.AddRange(this.collideMapObjects);
        }

        if (this.terrainMapObjects.Count > 0)
        {
            map.terrain.AddRange(this.terrainMapObjects);
        }
    }

    public void MergeAndUpdate(Serializable.Map map)
    {
        this.Init();
        //网格化
        this.InitGrid(map);

        //合并
        this.MergeGrid();

        //输出
        this.UpdateObjectsToMap(map);

    }

    public bool IsFloatEqual(float left, float right)
    {
        if (Mathf.Abs(left - right) < 0.001)
            return true;
        else
            return false;
    }




}

