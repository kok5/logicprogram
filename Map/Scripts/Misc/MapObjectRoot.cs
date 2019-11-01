/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapObjectRoot : MonoBehaviour
    {
        public static MapObjectRoot ins = null;

        //------------地图额外配置信息



        //---------------------------
        public void Reload(MapEditorStroageData data)
        {
            data.ReloadJson();

            {//reload  spawn point   
                var list = this.GetComponentsInChildren<MapObjectSpawnPoint>(true);
                if (list.Length != data._spawn_points.Count)
                {
                    Debug.LogError("0check length is right  when reload MapEditor.Reload  " + list.Length + "      " + data._spawn_points.Count);
                }
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].transform.position = data._spawn_points[i].position;
                }
            }
            {//reload weapon spawn point   
                var list = this.GetComponentsInChildren<MapObjectWeaponSpawnPoint>(true);
                if (list.Length != data._weapon_spawn_points.Count)
                {
                    Debug.LogError("1check length is right  when reload MapEditor.Reload  " + list.Length + "      " + data._weapon_spawn_points.Count);
                }
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].transform.position = data._weapon_spawn_points[i].position;
                    list[i]._weapon_ids = new List<int>(data._weapon_spawn_points[i].ids);
                }
            }

            {//reload layer and map obect and map object decroate
                EditorLayerMgr.ins.Clear();
                foreach (var l in data._layer_objs)
                {
                    EditorLayerMgr.ins.CreateLayer(l);
                    //GameObject layer = new GameObject("layer" + l.layerIndex.ToString());
                    //layer.transform.parent = LayerMgr.ins.transform;
                    //layers[l.layerIndex-1] = layer.transform;
                }
                EditorLayerMgr.ins.RefreshData();

                GameObject obj = null;
                foreach (var p in data._map_objs)
                {
                    if (p.layerIndex > 0)
                    {
                        //obj = CreateObject(p.prefab, LayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        //*****tmp 可以完全用CreateObjectById替代不？
                        int id = 0;
                        id = int.Parse(p.prefab);
                        obj = CreateObjectById(id, EditorLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        if (obj != null)
                        {
                            obj.transform.position = p.position;
                            obj.transform.localScale = p.localScale;
                            obj.transform.rotation = Quaternion.Euler(p.rotation);

                            //CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                            ////自定义属性
                            //if ((com != null) && (p.extPropJson != ""))
                            //{
                            //    com.OnDeseriazlie(p.extPropJson);
                            //}

                            if (MapObjectRoot.ins != null)
                                MapObjectRoot.ins.AddCheckDecrate(obj);

                        }
                    }
                    else
                    {
                        //obj = CreateObject(p.prefab, LayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        //*****tmp 可以完全用CreateObjectById替代不？
                        int id = 0;
                        id = int.Parse(p.prefab);
                        obj = CreateObjectById(id, EditorLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        if (obj != null)
                        {
                            obj.transform.position = p.position;
                            obj.transform.localScale = p.localScale;
                            obj.transform.rotation = Quaternion.Euler(p.rotation);

                            //CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                            ////自定义属性
                            //if ((com != null) && (p.extPropJson != ""))
                            //{
                            //    com.OnDeseriazlie(p.extPropJson);
                            //}

                            if (MapObjectRoot.ins != null)
                                MapObjectRoot.ins.AddCheckDecrate(obj);

                        }
                    }

                }

                foreach (var p in data._terrain_objs)
                {
                    if (p.layerIndex > 0)
                    {
                        //obj = CreateObject(p.prefab, LayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        //*****tmp 可以完全用CreateObjectById替代不？
                        int id = 0;
                        id = int.Parse(p.prefab);
                        obj = CreateObjectById(id, EditorLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        if (obj != null)
                        {
                            obj.transform.position = p.position;
                            obj.transform.localScale = p.localScale;
                            obj.transform.rotation = Quaternion.Euler(p.rotation);

                            //CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                            ////自定义属性
                            //if ((com != null) && (p.extPropJson != ""))
                            //{
                            //    com.OnDeseriazlie(p.extPropJson);
                            //}

                            if (MapObjectRoot.ins != null)
                                MapObjectRoot.ins.AddCheckDecrate(obj);

                        }
                    }
                    else
                    {
                        //obj = CreateObject(p.prefab, LayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        //*****tmp 可以完全用CreateObjectById替代不？
                        int id = 0;
                        id = int.Parse(p.prefab);
                        obj = CreateObjectById(id, EditorLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        if (obj != null)
                        {
                            obj.transform.position = p.position;
                            obj.transform.localScale = p.localScale;
                            obj.transform.rotation = Quaternion.Euler(p.rotation);

                            //CustomerPropertyBase com = obj.GetComponent<CustomerPropertyBase>();
                            ////自定义属性
                            //if ((com != null) && (p.extPropJson != ""))
                            //{
                            //    com.OnDeseriazlie(p.extPropJson);
                            //}

                            if (MapObjectRoot.ins != null)
                                MapObjectRoot.ins.AddCheckDecrate(obj);

                        }
                    }

                }
            }
            //  this.map_brief = data.map_brief;
            // this.map_name = data.map_name;
            //加载了目标地图 需要恢复状态
            MapEditorStroageData.current_map_brief = data.map_brief;
            MapEditorStroageData.current_map_name = data.map_name;

        }

        void Awake()
        {
            ins = this;
        }
        public static MapEditorStroageData data = null;

        //  //UI修改时  写入
        // public string map_name = "";//地图名字  跟随 json 和data
        //  public string map_brief = "";//地图简介 跟随json 和data

        void OnDestroy()
        {
            ins = null;
        }

        void Start()
        {
            //init spawn points
            for (int i = 0; i < 4; i++)
            {

                var obj = MapLoader.ins.LoadMapObjectV1("Common/OneSpawnPoint");

                obj = GameObject.Instantiate<GameObject>(obj);
                //PrefabsMgr.Load("Map/Prefabs/MapObject/Common/OneSpawnPoint");
                var pos = Vector3.zero;
                pos.z = -10f + 6f * i;
                pos.x = 0f;//just for show in the front of everything
                pos.y = 6f;
                obj.transform.position = pos;
                obj.transform.parent = transform;
                //    obj.GetComponentFully<OneMapObjectBase>().Init(i);
                this.CheckAddObject(obj);
                var scale = obj.transform.localScale;
                scale.x = 50f;
                obj.transform.localScale = scale;
                obj.GetComponentInChildren<TextMesh>().text = (i + 1).ToString() + "P";
            }
            //init weapon spawn point
            int WEAPON_SPAWN_POINT = 4;
            if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour)
            {
                WEAPON_SPAWN_POINT = 20;
            }
            else
            {
#if UNITY_EDITOR
                WEAPON_SPAWN_POINT = 10;
#endif
            }
            EditorWeaponSpawnPointsRoot weapon_spawn_root = this.transform.root.GetComponentInChildren<EditorWeaponSpawnPointsRoot>();
            for (int i = 0; i < WEAPON_SPAWN_POINT; i++)
            {
                var obj =
                     MapLoader.ins.LoadMapObjectV1("Common/OneWeaponSpawnPoint");
                obj = GameObject.Instantiate<GameObject>(obj);
                // PrefabsMgr.Load("Map/Prefabs/MapObject/Common/OneWeaponSpawnPoint");
                var pos = Vector3.zero;
#if UNITY_EDITOR
                pos.z = -15f + 3f * i;
#else
                pos.z = -10f + 6f * i;
#endif
                pos.x = 0f;//just for show in the front of everything
                pos.y = 3f;

                obj.transform.position = pos;
                obj.transform.parent = transform;
                //    obj.GetComponentFully<OneMapObjectBase>().Init(i);

                if (i >= weapon_spawn_root.transform.childCount)
                {
                    //实际武器点大于阈值 需要扩容  runtime 也要处理这个
                    GameObject x = GameObject.Instantiate<GameObject>(weapon_spawn_root.transform.GetChild(0).gameObject, weapon_spawn_root.transform, false);
                    x.transform.position = pos;
                    x.name = "p" + (i + 1);
                }

                this.CheckAddObject(obj);
                var scale = obj.transform.localScale;
                scale.x = 50f;
                obj.transform.localScale = scale;
                obj.GetComponentInChildren<TextMesh>().text = "武器" + (i + 1).ToString();

            }


            ///////////////////////////////////////////////////////////
            /// 先隐藏以免 武器点和出生点影响碰撞检测
            this.SetSpawnPointVisible(false);
            this.SetWeaponSpawnPointVisible(false);

            MapEditorMgr.ins.Init();

            //表示是自动恢复的 或者 编辑的地图
            if (data != null)
            {
                this.Reload(data);
                data = null;
            }
            else
            {
                //新建的地图  需要初始化一下数据  方便 预览图预览什么的
                this.SerializeToJson();

                EditorLayerMgr.ins.Init();

            }

        }
        public void DestroyObject(GameObject obj)
        {
            this.CheckDestroyObject(obj);
            GameObject.Destroy(obj);
        }
        public void DestroyObjectImmediate(GameObject obj)
        {
            this.CheckDestroyObject(obj);
            GameObject.DestroyImmediate(obj);
        }

        void CheckDestroyObject(GameObject obj)
        {
            //TYPE CHECK just for fast get Component
            var cmp = obj.GetComponent<MapObjectBase>();
            if (cmp == null)
            {
                Debug.LogError("  create object error  CreateObject");
            }
            if (cmp is MapObject)
            {
                _list_map_object.Remove(cmp as MapObject);
            }
            else if (cmp is MapObjectDecorate)
            {
                _list_map_object_decroate.Remove(cmp as MapObjectDecorate);
            }
            else if (cmp is MapObjectSpawnPoint)
            {
                _list_spawn_points.Remove(cmp as MapObjectSpawnPoint);
            }
            else if (cmp is MapObjectWeaponSpawnPoint)
            {
                _list_weapon_spawn_points.Remove(cmp as MapObjectWeaponSpawnPoint);
            }
            else
            {
                Debug.LogError("unknow type of " + cmp.GetType().ToString());
            }

        }
        public GameObject CreateObject(string name, Transform parent = null)
        {
            Transform p = transform;
            if (parent != null)
                p = parent;
            GameObject obj1 = //PrefabsMgr.LoadMapObjectEditor(name);

                MapLoader.ins.LoadEditorV1(MapEditor.MapEditorConfig.CurrentSelectTheme, name);
            //this object is not exist
            if (obj1 == null)
            {
#if UNITY_EDITOR
                Debug.LogError("can not find this object = " + name);
#endif
                return null;
            }

            var obj = GameObject.Instantiate<GameObject>(obj1, p, true);
#if UNITY_EDITOR
            Debug.Log(" create new mapobject name=" + name);
#endif
            obj.name = name;
            //this.CheckAllConflict();
            this.CheckAddObject(obj);
            return obj;
        }


        /// <summary>
        /// 创建组件前的预判断，上下左右 0offset范围内有物体，则不让创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool OnPreCreateObject(Vector3 worldPos, int objectId, float widthOffset = 0.1f, float heightOffset = 0.1f)
        {
            if (objectId <= 0)
                return false;

            Vector3[] positions = { worldPos + new Vector3(0, heightOffset, 0), worldPos + new Vector3(0, -heightOffset, 0) , worldPos + new Vector3(0, 0, widthOffset) , worldPos + new Vector3(0, 0, -widthOffset) };
            for (int i = 0; i<positions.Length; i++)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(positions[i]);
                var ray = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("MapEditorObject");
                if (Physics.Raycast(ray, out hit, 30000f, mask))
                {
                    return false;
                }
            }

            //判断校正后的位置是否冲突 (原位置有相同的组件就算冲突)
            if (MapEditorMgr.ins.EnableAutoGrid)
            {
                GameObject obj = MapLoader.ins.LoadEditorById(objectId);
                if (obj != null)
                {
                    Bounds boudingBox;
                    //计算包围盒, 有"main"子节点就按main计算，没有就按整个计算
                    var objMain = obj.transform.Find("main");
                    if (objMain != null)
                        boudingBox = objMain.gameObject.CalculateBounds();
                    else
                        boudingBox = obj.CalculateBounds();

                    Vector3 newPos = this.ResetPositon(boudingBox, worldPos);
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(newPos);
                    var ray = Camera.main.ScreenPointToRay(screenPos);
                    RaycastHit hit;
                    LayerMask mask = LayerMask.GetMask("MapEditorObject");
                    RaycastHit[] hits = Physics.RaycastAll(ray, 30000f, mask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                        //相同组件                                           
                        if ((tmpObj != null) && (tmpObj.name == objectId.ToString()))
                        {
                            return false;
                        }
                    }


                }
            }


            return true;
        }

        /// <summary>
        /// 创建完组件后的后处理
        /// </summary>
        public void OnPostCreateObject(GameObject obj)
        {
            ResetPosition(obj);

            AddCheckDecrate(obj);
        }

        public void OnEndMoveObject(GameObject obj)
        {
            if (obj != null)
            {
                Vector3 pos = obj.transform.position;

                //0.5格对齐  0 0.5 1 1.5...
                pos.z = Mathf.RoundToInt(pos.z * 2);
                pos.z /= 2;

                pos.y = Mathf.RoundToInt(pos.y * 2);
                pos.y /= 2;

                obj.transform.position = pos;
            }

            AddCheckDecrate(obj);
        }

        //
        public void ResetPosition(GameObject obj)
        {
            if (!MapEditorMgr.ins.EnableAutoGrid && !this.IsGridObject(obj))
                return;

            if (obj != null)
            {
                Vector3 pos = obj.transform.position;
                Bounds boudingBox;
                //计算包围盒, 有"main"子节点就按main计算，没有就按整个计算
                var objMain = obj.transform.Find("main");
                if (objMain != null)
                    boudingBox = objMain.gameObject.CalculateBounds();
                else
                    boudingBox = obj.CalculateBounds();

                Vector3 newPos = this.ResetPositon(boudingBox, pos);

                obj.transform.position = newPos;
            }
        }

        public Vector3 ResetPositon(Bounds boudingBox, Vector3 pos)
        {
            if (boudingBox != null)
            {
                Vector3 retPos = pos;

                int widh = Mathf.RoundToInt(boudingBox.size.z);
                int height = Mathf.RoundToInt(boudingBox.size.y);

                //包围盒长度偶数个格子, 定在 1 (网格点)
                if (widh % 2 == 0)
                {
                    retPos.z = Mathf.RoundToInt(pos.z);
                }
                else //奇数个格子，定位在0.5 （网格的中心点）
                {
                    retPos.z = 0.5f + Mathf.RoundToInt(pos.z - 0.5f);
                }

                //包围盒长度偶数个格子, 定在 1 (网格点)
                if (height % 2 == 0)
                {
                    retPos.y = Mathf.RoundToInt(pos.y);
                }
                else //奇数个格子，定位在0.5 （网格的中心点）
                {
                    retPos.y = 0.5f + Mathf.RoundToInt(pos.y - 0.5f);
                }

                return retPos;
            }
            else
                return pos;
        }



        public void AddCheckDecrate(GameObject obj)
        {
            //九宫格组件才刷新装饰
            if (!this.IsGridObject(obj))
                return;

            this.RefreshDecrate(obj);

            var adjacentObjs = this.GetAdjacentObjects(obj);

            for (int i = 0; i < adjacentObjs.Length; i++)
            {
                this.RefreshDecrate(adjacentObjs[i]);
            }

            //var dec = obj.transform.Find("change");
            //if (dec != null)
            //{
            //    DecrateType curDecrateType = GetDecrateType(obj);
            //    if (curDecrateType != DecrateType.NotDefined)
            //    {
            //        for (int i = 2; i < 16; i++)
            //        {
            //            var child = dec.Find(i.ToString());
            //            if (child != null)
            //            {
            //                if ((int)curDecrateType == i)
            //                    child.gameObject.SetActive(true);
            //                else
            //                    child.gameObject.SetActive(false);
            //            }
            //        }
            //    }
            //}



            //if (obj != null)
            //{
            //    //上面有组件，隐藏装饰
            //    Bounds boundingBox = obj.CalculateBounds();
            //    var dec = obj.transform.Find("decrate");
            //    if (dec != null)
            //    {

            //        Vector3 pos = boundingBox.center + new Vector3(0, boundingBox.size.y, 0);
            //        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            //        var ray = Camera.main.ScreenPointToRay(screenPos);
            //        RaycastHit hit;
            //        LayerMask mask = LayerMask.GetMask("MapEditorObject"); 
            //        if (Physics.Raycast(ray, out hit, 1000f, mask))
            //        {
            //            dec.gameObject.SetActive(false);
            //        }
            //        else
            //        {
            //            dec.gameObject.SetActive(true);
            //        }


            //        //下面有组件，下面的组件隐藏装饰
            //        pos = boundingBox.center - new Vector3(0, boundingBox.size.y, 0);
            //        screenPos = Camera.main.WorldToScreenPoint(pos);

            //        ray = Camera.main.ScreenPointToRay(screenPos);
            //        if (Physics.Raycast(ray, out hit, 1000f, mask))
            //        {
            //            if (hit.collider.gameObject.GetComponentFully<MapObjectBase>() != null)
            //            {
            //                var downObj = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
            //                var downDec = downObj.transform.Find("decrate");
            //                if (downDec != null)
            //                {
            //                    downDec.gameObject.SetActive(false);
            //                }
            //            }

            //        }

            //    }
            //}


        }

        public void DeleteCheckDecrate(GameObject obj)
        {
            //九宫格组件才刷新装饰
            if (!this.IsGridObject(obj))
                return;

            var adjacentObjs = this.GetAdjacentObjects(obj);

            for (int i = 0; i < adjacentObjs.Length; i++)
            {
                this.RefreshDecrate(adjacentObjs[i]);
            }

            //if (obj != null)
            //{
            //    //上面有组件，隐藏装饰
            //    Bounds boundingBox = obj.CalculateBounds();

            //        //下面有组件，下面的组件显示装饰
            //        var pos = boundingBox.center - new Vector3(0, boundingBox.size.y, 0);
            //        var screenPos = Camera.main.WorldToScreenPoint(pos);
            //        RaycastHit hit;
            //        LayerMask mask = LayerMask.GetMask("MapEditorObject");
            //        var ray = Camera.main.ScreenPointToRay(screenPos);
            //        if (Physics.Raycast(ray, out hit, 1000f, mask))
            //        {
            //            if (hit.collider.gameObject.GetComponentFully<MapObjectBase>() != null)
            //            {
            //                var downObj = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
            //                var downDec = downObj.transform.Find("decrate");
            //                if (downDec != null)
            //                {
            //                    downDec.gameObject.SetActive(true);
            //                }
            //            }

            //        }

            //    }
            //}

        }



        void CheckAddObject(GameObject obj)
        {
            //TYPE CHECK just for fast get Component
            var cmp = obj.GetComponent<MapObjectBase>();
            if (cmp == null)
            {
                Debug.LogError("  create object error  CreateObject");
            }
            if (cmp is MapObject)
            {
                _list_map_object.Add(cmp as MapObject);
            }
            else if (cmp is MapObjectDecorate)
            {
                _list_map_object_decroate.Add(cmp as MapObjectDecorate);
            }
            else if (cmp is MapObjectSpawnPoint)
            {
                _list_spawn_points.Add(cmp as MapObjectSpawnPoint);
            }
            else if (cmp is MapObjectWeaponSpawnPoint)
            {
                _list_weapon_spawn_points.Add(cmp as MapObjectWeaponSpawnPoint);
            }
            else
            {
                Debug.LogError("unknow type of " + cmp.GetType().ToString());
            }
        }
        //-----------just for fast check
        //-----------just for fast check
        //-----------just for fast check
        //-----------just for fast check
        public List<MapObject> _list_map_object = new List<MapObject>();
        public List<MapObjectDecorate> _list_map_object_decroate = new List<MapObjectDecorate>();
        public List<MapObjectSpawnPoint> _list_spawn_points = new List<MapObjectSpawnPoint>();
        public List<MapObjectWeaponSpawnPoint> _list_weapon_spawn_points = new List<MapObjectWeaponSpawnPoint>();


        /*   void Update()
           {
               //  this.CheckAllConflict();
               if (Input.GetKeyDown(KeyCode.Space))
               {
                   this.SerializeToJson();

               }
           }*/
        public bool CheckConflict(GameObject who)
        {
            //改为 不处理任何碰撞
            return false;
            bool ok = false;
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].DisableColliders();
            }
            ok = who.GetComponentFully<MapObjectBase>().CheckConflict();
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].EnabledCollider();
            }
            return ok;
        }

        public bool CheckAllConflict()
        {
            //改为 不处理任何碰撞
            if (DevConfig.MapEditorDisableAllConflictCheck)
            {
                return false;
            }
            /*   //#1000170  改为不限制碰撞和增加物件上限为50
               return false;
   #if UNITY_EDITOR
               //编辑器下  不检查碰撞
               return false;
   #endif*/
            bool ok = false;

            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].DisableColliders();
            }
            for (int i = 0; i < _list_map_object.Count; i++)
            {
                ok = _list_map_object[i].CheckConflict();
                if (ok)
                {
                    break;
                }
            }
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].EnabledCollider();
            }
            return ok;
        }
        public void SetSpawnPointVisible(bool visible)
        {
            int count = _list_spawn_points.Count;
            for (int i = 0; i < count; i++)
            {
                _list_spawn_points[i].gameObject.SetActive(visible);
            }
        }
        public void SetWeaponSpawnPointVisible(bool visible)
        {
            int count = _list_weapon_spawn_points.Count;
            for (int i = 0; i < count; i++)
            {
                _list_weapon_spawn_points[i].gameObject.SetActive(visible);
            }
        }

        public bool CanGoStartPreview(out string error)
        {
            //error = "";
            //#1000170  改为不限制碰撞和增加物件上限为50
            // return true;
            error = "您当前物体有重叠,请检查关卡!";
            //check map object number
#if UNITY_EDITOR
            //编辑器下  不检查碰撞
            return true;
#endif
            if (_list_map_object.Count <= 0)
            {
                error = "场景物件不能为空";
                return false;
            }

            //check threr is any conflict
            bool ok = false;
            for (int i = 0; i < _list_map_object_decroate.Count; i++)
            {
                _list_map_object_decroate[i].DisableColliders();
            }
            if (!ok)
            {//check 
                for (int i = 0; i < _list_map_object.Count; i++)
                {
                    ok = _list_map_object[i].CheckConflict();
                    if (ok)
                    {
                        _list_map_object[i].SetBright(true);
                        for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                        {
                            _list_map_object_decroate[ii].EnabledCollider();
                        }
                        return false;
                    }
                }
            }
            if (!ok)
            {//check
                for (int i = 0; i < _list_weapon_spawn_points.Count; i++)
                {
                    ok = _list_weapon_spawn_points[i].CheckConflict();
                    if (ok)
                    {
                        _list_weapon_spawn_points[i].SetBright(true);
                        for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                        {
                            _list_map_object_decroate[ii].EnabledCollider();
                        }
                        return false;
                    }
                }
            }
            if (!ok)
            {//check
                for (int i = 0; i < _list_weapon_spawn_points.Count; i++)
                {
                    ok = _list_weapon_spawn_points[i].CheckConflict();
                    if (ok)
                    {
                        _list_weapon_spawn_points[i].SetBright(true);
                        for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                        {
                            _list_map_object_decroate[ii].EnabledCollider();
                        }
                        return false;
                    }
                }
            }
            if (ok) return false;

            //check map object done
            /*//check spawn point is valid
            foreach (var p in _spawn_points)
            {
                var pos = p.transform.position;
                //  RaycastHit hit;
                var ray = new Ray(pos, Vector3.down);
                RaycastHit[] hits = null;
                var num = Physics.RaycastNonAlloc(ray, hits, 10000f);
                int hit_obj_num = 0;
                for (int i = 0; i < num; i++)
                {
                    if (hits[i].collider.gameObject.GetComponentFully<MapObjectBase>() != null)
                    {
                        hit_obj_num++;
                        break;
                    }
                }
                if (hit_obj_num <= 0)
                {
                    return false;
                }
                Debug.Log("check hit obj num=" + hit_obj_num);
            }
            */

            /* //check weapon spawn point  property
              int num_of_valid_weapon_spawn_point = 0;
              foreach (var p in _weapon_spawn_points)
              {
                  var c = p.GetComponent<MapObjectWeaponSpawnPoint>();
                  if (c != null)
                  {
                      if (c.IsValidForStartPreview())
                      {
                          ++num_of_valid_weapon_spawn_point;
                      }
                  }
              }
              if (num_of_valid_weapon_spawn_point <= 0)
              {
                  //make sure has at least one valid point 
                  return false;
              }*/


            return true;
        }
        public void SetDecorateColliderEnable(bool enable)
        {
            if (enable)
            {
                for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                {
                    _list_map_object_decroate[ii].EnabledCollider();
                }
            }
            else
            {
                for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                {
                    _list_map_object_decroate[ii].DisableColliders();
                }
            }
        }
        public void SetAllGameObjectActive(bool active)
        {
            // foreach (var p in this.GetComponentsInChildren<MapObjectBase>(true))
            {
                //   p.gameObject.SetActive(active);
            }

            for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
            {
                _list_map_object_decroate[ii].gameObject.SetActive(active);
            }
            for (int ii = 0; ii < _list_map_object.Count; ii++)
            {
                _list_map_object[ii].gameObject.SetActive(active);
            }
            for (int ii = 0; ii < _list_spawn_points.Count; ii++)
            {
                _list_spawn_points[ii].gameObject.SetActive(active);
            }
            for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
            {
                _list_weapon_spawn_points[ii].gameObject.SetActive(active);
            }
        }

        public void SetBrightAll(bool bright)
        {
            for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
            {
                if (_list_map_object_decroate[ii] != null)
                    _list_map_object_decroate[ii].SetBright(bright);
            }
            for (int ii = 0; ii < _list_map_object.Count; ii++)
            {
                if (_list_map_object[ii] != null)
                    _list_map_object[ii].SetBright(bright);
            }
            for (int ii = 0; ii < _list_spawn_points.Count; ii++)
            {
                if (_list_spawn_points[ii] != null)
                    _list_spawn_points[ii].SetBright(bright);
            }
            for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
            {
                if (_list_weapon_spawn_points[ii] != null)
                    _list_weapon_spawn_points[ii].SetBright(bright);
            }
        }
        public int TotalMapObjectCount
        {
            get
            {
                return _list_map_object.Count + _list_map_object_decroate.Count + _list_spawn_points.Count + _list_weapon_spawn_points.Count - DisabledMapObjectCount;
            }
        }

        public int DisabledMapObjectCount
        {
            get
            {
                int i = 0;
                for (int ii = 0; ii < _list_map_object_decroate.Count; ii++)
                {
                    if (!_list_map_object_decroate[ii].gameObject.activeSelf)
                        i++;
                }
                for (int ii = 0; ii < _list_map_object.Count; ii++)
                {
                    if ((_list_map_object[ii] != null) && !_list_map_object[ii].gameObject.activeSelf)
                        i++;
                }

                return i;
            }
        }

        public void SetPreviewColliderEnable(bool enable)
        {
            {//decorate
                MapObjectRoot.ins.SetDecorateColliderEnable(enable);
            }

            {//spawn
                if (enable)
                {
                    for (int ii = 0; ii < _list_spawn_points.Count; ii++)
                    {
                        _list_spawn_points[ii].EnabledCollider();
                    }
                }
                else
                {
                    for (int ii = 0; ii < _list_spawn_points.Count; ii++)
                    {
                        _list_spawn_points[ii].DisableColliders();
                    }
                }
            }
            {//weapon
                if (enable)
                {
                    for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
                    {
                        _list_weapon_spawn_points[ii].EnabledCollider();
                    }
                }
                else
                {
                    for (int ii = 0; ii < _list_weapon_spawn_points.Count; ii++)
                    {
                        _list_weapon_spawn_points[ii].DisableColliders();
                    }
                }
            }
        }
        //execute serialize to json string  share json data
        public static string json = "";
        public static string record_json = "";
        public string SerializeToJson()
        {
            var serizlizer = this.GetComponentInParent<EditorSerialize>();
            {//emplace weapon spawn info to 
                var tags = serizlizer.GetComponentsInChildren<EditorWeaponSpawnPointTag>();
                if (tags.Length != _list_weapon_spawn_points.Count || tags.Length <= 0)
                {
                    Debug.LogError("EditorWeaponSpawnPointTag length != _weapon_spawn_points     are you sure " + tags.Length + "  " + _list_weapon_spawn_points.Count);
                }
                for (int i = 0; i < tags.Length; i++)
                {
                    tags[i].weapon_ids = _list_weapon_spawn_points[i].GetComponentFully<MapObjectWeaponSpawnPoint>()._weapon_ids;
                    tags[i].transform.position = _list_weapon_spawn_points[i].transform.position;
                }
            }
            // emplace spawn position to 
            {
                var spawn = serizlizer.GetComponentInChildren<EditorSpawnPointsRoot>();
                for (int i = 0; i < _list_spawn_points.Count; i++)
                {
                    spawn.transform.GetChild(i).position = _list_spawn_points[i].transform.position;
                }
            }


            var json = serizlizer.ToJson(this);
            MapObjectRoot.json = json;
            MapObjectRoot.record_json = json;
#if UNITY_EDITOR
            Debug.Log(json);
#endif
            return json;

            return string.Empty;
        }

        public GameObject CreateObjectById(int id, Transform parent = null)
        {
            Transform p = transform;
            if (parent != null)
                p = parent;
            GameObject obj1 = MapLoader.ins.LoadEditorById(id);

            var obj = GameObject.Instantiate<GameObject>(obj1, p, true);


            Debug.Log(" create new mapobject id=" + id.ToString());
            obj.name = id.ToString();
            //this.CheckAllConflict();
            this.CheckAddObject(obj);
            return obj;
        }


        public DecrateType GetDecrateType(GameObject obj)
        {
            //       <--Z  (Z往正方向是往左)
            if (obj != null)
            {

                var dec = obj.transform.Find("change");
                if (dec != null)
                {
                    //上面是否有组件
                    bool hasNoTopObj = true;
                    //一格偏移检测
                    float offset = 1;
                    //往上一格
                    Vector3 pos = obj.transform.position + new Vector3(0, offset, 0);
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
                    var ray = Camera.main.ScreenPointToRay(screenPos);
                    RaycastHit hit;
                    LayerMask mask = LayerMask.GetMask("MapEditorObject");
                    var hits = Physics.RaycastAll(ray, 30000f, mask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                        //相同组件才判断装饰判断                                                 
                        if ((tmpObj != null) && (obj.name == tmpObj.name))
                        {
                            hasNoTopObj = false;
                            break;
                        }
                    }


                    //右面是否有组件
                    bool hasNoRightObj = true;
                    pos = obj.transform.position + new Vector3(0, 0, -offset);
                    screenPos = Camera.main.WorldToScreenPoint(pos);
                    ray = Camera.main.ScreenPointToRay(screenPos);
                    hits = Physics.RaycastAll(ray, 30000f, mask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                        //相同组件才判断装饰判断                                                 
                        if ((tmpObj != null) && (obj.name == tmpObj.name))
                        {
                            hasNoRightObj = false;
                            break;
                        }
                    }

                    //下面是否有组件
                    bool hasNoBottomObj = true;
                    pos = obj.transform.position + new Vector3(0, -offset, 0);
                    screenPos = Camera.main.WorldToScreenPoint(pos);
                    ray = Camera.main.ScreenPointToRay(screenPos);
                    hits = Physics.RaycastAll(ray, 30000f, mask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                        //相同组件才判断装饰判断                                                 
                        if ((tmpObj != null) && (obj.name == tmpObj.name))
                        {
                            hasNoBottomObj = false;
                            break;
                        }
                    }

                    //左面是否有组件
                    bool hasNoLeftObj = true;
                    pos = obj.transform.position + new Vector3(0, 0, offset);
                    screenPos = Camera.main.WorldToScreenPoint(pos);
                    ray = Camera.main.ScreenPointToRay(screenPos);
                    hits = Physics.RaycastAll(ray, 30000f, mask);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                        //相同组件才判断装饰判断                                                 
                        if ((tmpObj != null) && (obj.name == tmpObj.name))
                        {
                            hasNoLeftObj = false;
                            break;
                        }
                    }

                    if (hasNoLeftObj && hasNoTopObj && hasNoRightObj && hasNoBottomObj)
                        return DecrateType.LeftTopRightBottom;

                    else if (!hasNoLeftObj && hasNoTopObj && hasNoRightObj && hasNoBottomObj)
                        return DecrateType.TopRightBottom;
                    else if (hasNoLeftObj && !hasNoTopObj && hasNoRightObj && hasNoBottomObj)
                        return DecrateType.RightBottomLeft;
                    else if (hasNoLeftObj && hasNoTopObj && !hasNoRightObj && hasNoBottomObj)
                        return DecrateType.BottomLeftTop;
                    else if (hasNoLeftObj && hasNoTopObj && hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.LeftTopRight;

                    else if (!hasNoLeftObj && !hasNoTopObj && hasNoRightObj && hasNoBottomObj)
                        return DecrateType.RightBottom;
                    else if (hasNoLeftObj && !hasNoTopObj && !hasNoRightObj && hasNoBottomObj)
                        return DecrateType.BottomLeft;
                    else if (hasNoLeftObj && hasNoTopObj && !hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.LeftTop;
                    else if (!hasNoLeftObj && hasNoTopObj && hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.TopRight;
                    else if (!hasNoLeftObj && hasNoTopObj && !hasNoRightObj && hasNoBottomObj)
                        return DecrateType.TopBottom;
                    else if (hasNoLeftObj && !hasNoTopObj && hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.LeftRight;

                    else if (!hasNoLeftObj && !hasNoTopObj && !hasNoRightObj && hasNoBottomObj)
                        return DecrateType.Bottom;
                    else if (hasNoLeftObj && !hasNoTopObj && !hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.Left;
                    else if (!hasNoLeftObj && hasNoTopObj && !hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.Top;
                    else if (!hasNoLeftObj && !hasNoTopObj && hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.Right;

                    else if (!hasNoLeftObj && !hasNoTopObj && !hasNoRightObj && !hasNoBottomObj)
                        return DecrateType.NoDecrate;
                    else
                        return DecrateType.NotDefined;


                }
            }

            return DecrateType.NotDefined;
        }


        /// <summary>
        /// 刷新单个组件的装饰
        /// </summary>
        /// <param name="obj"></param>
        public void RefreshDecrate(GameObject obj)
        {
            var dec = obj.transform.Find("change");
            if (dec != null)
            {
                DecrateType curDecrateType = GetDecrateType(obj);
                if (curDecrateType != DecrateType.NotDefined)
                {
                    for (int i = 2; i <= 16; i++)
                    {
                        var child = dec.Find(i.ToString());
                        if (child != null)
                        {
                            if ((int)curDecrateType == i)
                                child.gameObject.SetActive(true);
                            else
                                child.gameObject.SetActive(false);
                        }

                        var randTopDec = dec.Find("106");
                        if (randTopDec != null)
                            randTopDec.gameObject.SetActive(false);
                        var randBottomTopDec = dec.Find("116");
                        if (randBottomTopDec != null)
                            randBottomTopDec.gameObject.SetActive(false);
                    }

                    if (curDecrateType == DecrateType.Top || curDecrateType == DecrateType.TopBottom)
                    {
                        int rand = Random.Range(0, 100);
                        if (rand <= 20)
                        {
                            var randDec = dec.Find(((int)curDecrateType + 100).ToString());
                            if (randDec != null)
                            {
                                randDec.gameObject.SetActive(true);

                                var child = dec.Find(((int)curDecrateType).ToString());
                                if (child != null)
                                    child.gameObject.SetActive(false);
                            }
                        }

                    }

                }


            }
        }

        public GameObject[] GetAdjacentObjects(GameObject obj)
        {
            //var downObj = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
            //上面是否有组件
            List<GameObject> objList = new List<GameObject>();

            if (obj != null)
            {
                //一格偏移检测
                float offset = 1;
                //往上一格
                Vector3 pos = obj.transform.position + new Vector3(0, offset, 0);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
                var ray = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("MapEditorObject");
                var hits = Physics.RaycastAll(ray, 30000f, mask);
                for (int i = 0; i < hits.Length; i++)
                {
                    var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                    if (this.IsGridObject(tmpObj))
                        objList.Add(tmpObj);
                }

                //右面是否有组件
                bool hasNoRightObj = true;
                pos = obj.transform.position + new Vector3(0, 0, -offset);
                screenPos = Camera.main.WorldToScreenPoint(pos);
                ray = Camera.main.ScreenPointToRay(screenPos);
                hits = Physics.RaycastAll(ray, 30000f, mask);
                for (int i = 0; i < hits.Length; i++)
                {
                    var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                    if (this.IsGridObject(tmpObj))
                        objList.Add(tmpObj);
                }

                //下面是否有组件
                bool hasNoBottomObj = true;
                pos = obj.transform.position + new Vector3(0, -offset, 0);
                screenPos = Camera.main.WorldToScreenPoint(pos);
                ray = Camera.main.ScreenPointToRay(screenPos);
                hits = Physics.RaycastAll(ray, 30000f, mask);
                for (int i = 0; i < hits.Length; i++)
                {
                    var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                    if (this.IsGridObject(tmpObj))
                        objList.Add(tmpObj);
                }

                //左面是否有组件
                bool hasNoLeftObj = true;
                pos = obj.transform.position + new Vector3(0, 0, offset);
                screenPos = Camera.main.WorldToScreenPoint(pos);
                ray = Camera.main.ScreenPointToRay(screenPos);
                hits = Physics.RaycastAll(ray, 30000f, mask);
                for (int i = 0; i < hits.Length; i++)
                {
                    var tmpObj = hits[i].collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                    if (this.IsGridObject(tmpObj))
                        objList.Add(tmpObj);
                }
            }


            return objList.ToArray();

        }


        public void PlayClickEffect(Vector3 pos)
        {
            var objClickEffect = this.GetClickEffect();
            if (objClickEffect != null)
            {
                var particle = objClickEffect.GetComponentInChildren<ParticleSystem>();
                if (particle != null)
                {
                    particle.Stop();
                    particle.Play();
                }
                pos.x -= 100;
                objClickEffect.transform.position = pos;
                objClickEffect.SetActive(true);
            }
        }

        public GameObject GetClickEffect()
        {
            if (_cachedEffects.Count > 0)
            {
                GameObject objClickEffect = _cachedEffects[0];
                _cachedEffects.Remove(objClickEffect);
                return objClickEffect;
            }
            else
            {
                var objClickEffect = EffectsLoader.ins.LoadBattle("Effect_add_remove_obj", true, this.transform.parent);
                return objClickEffect;
            }
        }

        public void RecyleClickEffect(GameObject obj)
        {
            _cachedEffects.Add(obj);

        }

        public bool IsGridObject(GameObject obj)
        {
            var strId = obj.name;
            int id = 0;
            int.TryParse(strId, out id);

            return this.IsGridObjectById(id);
        }

        public bool IsGridObjectById(int id)
        {
            if (id >= 14000 && id < 15000)
                return true;
            else
                return false;
        }

        private List<GameObject> _cachedEffects = new List<GameObject>();

        //普通组件的数量
        public int GetNormalMapObjectCount()
        {
            int count = 0;
            for (int i = 0; i < _list_map_object.Count; i++)
            {
                if (!this.IsGridObject(_list_map_object[i].gameObject)  && _list_map_object[i].gameObject.activeSelf)
                {
                    count++;
                }
            }

            return count;
        }

        //九宫格组件的数量
        public int GetGridMapObjectCount()
        {
            int count = 0;
            for (int i = 0; i < _list_map_object.Count; i++)
            {
                if (this.IsGridObject(_list_map_object[i].gameObject) && _list_map_object[i].gameObject.activeSelf)
                {
                    count++;
                }
            }

            return count;
        }

    }
}