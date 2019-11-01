using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelMapEditorDown : MonoBehaviour
    {
        public GameObject img_selected;
        private GameObject _obj_content;
        private int _group_id = 1;

        //---Auto Generate Code Start---
        //自动声明变量
        //Text_none  没有组件的时候显示这个提示，有组件隐藏
        private Text Text_none;
        void Awake() 
        {
            //生成自动绑定代码

            Text_none = transform.Find("Text_none").GetComponent<Text>();

            this.OnAwake();
        }
//---Auto Generate Code End---

        void OnAwake()
        {
            _obj_content = transform.Find("ListUp/Viewport/ListContent").gameObject;
            img_selected = transform.Find("img_selected").gameObject;
            //this.LoadListView();
        }


        void Start()
        {
            //_obj_content = transform.Find("ListUp/Viewport/ListContent").gameObject;
            //this.LoadListView();
        }

        //public void ClearSelected()
        //{
        //    this.OnCellClick(MapEditorInputMgr.ins.currentSelect);
        //}

        List<GameObject> _child = new List<GameObject>();


        public void LoadListViewByType(LoadType type = LoadType.MapObject)
        {
            int group_id = this._group_id;
            this.LoadListView(group_id, type);
        }

        public void LoadListView(int group_id = 1, LoadType type = LoadType.MapObject)
        {
            _group_id = group_id;

            if (img_selected != null)
            {
                img_selected.transform.SetParent(transform, true);
                img_selected.transform.position = Vector3.one * 100000f;
            }

            foreach (var p in _child)
            {
                GameObject.Destroy(p.gameObject);
            }
            _child.Clear();

            if (type == LoadType.Weapon)
            {
                var list = MapEditorConfig.GetWeapons();
                foreach (var id in list)
                {
                    var obj = MapLoader.ins.LoadEditorV1("OneMapObjectWeapon");

                    if (obj == null) break;
                    obj = GameObject.Instantiate<GameObject>(obj);
                    obj.transform.SetParent(_obj_content.transform, false);
                    bool ok = obj.GetComponentFully<OneMapObjectWeapon>().InitData(id);
                    if (!ok)
                    {
                        GameObject.DestroyImmediate(obj);
                        break;
                    }
                    _child.Add(obj);
                }
            }
            else if (type == LoadType.MapObject)
            {
                var list = GameConfig.instance.GetComponentsByGroup(group_id);
                foreach (var value in list)
                {
                    var obj = MapLoader.ins.LoadEditorV1("OneMapObject");

                    if (obj == null) break;
                    obj = GameObject.Instantiate<GameObject>(obj);
                    obj.transform.SetParent(_obj_content.transform, false);
                    bool ok = obj.GetComponentFully<OneMapObjectBase>().InitData(value.id);
                    if (!ok)
                    {
                        GameObject.DestroyImmediate(obj);
                        break;
                    }

                    //切换分组重新设置之前已经选中的组件
                    if ((value.id == MapEditorInputMgr.ins.currentSelectId) &&
                        (MapEditorInputMgr.ins.currentSelectId > 0))
                    {
                        if (img_selected != null)
                        {
                            img_selected.transform.SetParent(obj.transform, true);
                            img_selected.transform.localPosition = Vector3.zero;
                        }

                    }
                    _child.Add(obj);
                }


                //重新加载正常组件，进入正常组件编辑模式
                if (list.Length > 0)
                    MapEditorUIMgr.ins.GetPanel<UIPanelMapEditor>().ChangeToEditObjectMode();
            }
            else if (type == LoadType.SpawPoint)
            {
                var list = GameConfig.instance.GetComponentsByGroup(group_id);
                foreach (var value in list)
                {
                    var obj = MapLoader.ins.LoadEditorV1("OneMapObject");

                    if (obj == null) break;
                    obj = GameObject.Instantiate<GameObject>(obj);
                    obj.transform.SetParent(_obj_content.transform, false);
                    bool ok = obj.GetComponentFully<OneMapObjectBase>().InitData(value.id);
                    if (!ok)
                    {
                        GameObject.DestroyImmediate(obj);
                        break;
                    }
                    //切换分组重新设置之前已经选中的组件
                    if ((value.id == MapEditorInputMgr.ins.currentSelectId) &&
                        (MapEditorInputMgr.ins.currentSelectId > 0))
                    {
                        if (img_selected != null)
                        {
                            img_selected.transform.SetParent(obj.transform, true);
                            img_selected.transform.localPosition = Vector3.zero;
                        }

                    }
                    _child.Add(obj);
                }
            }

            this.ResizeContent(_child.Count);
            
            if (_child.Count > 0)
                this.Text_none.gameObject.SetActive(false);
            else
            {
                this.Text_none.gameObject.SetActive(true);
            }
        }

        void ResizeContent(int num_of_one)
        {
            var trans = _obj_content.GetComponent<RectTransform>();
            var size = trans.sizeDelta;

            size.x = 95 * num_of_one + 20;//left pading is 20 
            trans.sizeDelta = size;
        }

        public void Clear()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p);
            }
            _child.Clear();
        }

        public void OnCellClick(OneMapObjectBase who)
        {
            if (who == null) return;

            MapEditorInputMgr.ins.currentSelectId = who.id;

            if (img_selected != null)
            {
                img_selected.transform.SetParent(who.transform, true);
                img_selected.transform.localPosition = Vector3.zero;
            }


            MapEditorInputMgr.ins.touchBehaviour = TouchBehaviour.Added;

            //进入正常组件编辑模式
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditor>().ChangeToEditObjectMode();
            //取消选中并进入画笔模式 ClearSelectObject内会调ChangeToPenMode??
            MapEditorInputMgr.ins.ClearSelectObject();
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditor>().ChangeToPenMode();
        }

        public void OnCellWeaponClick(OneMapObjectWeapon who)
        {
            if (currentWeapon != null && MapEditorMgr.ins.CurrentStep == MapEditorStep.WeaponSpawn)
            {
                currentWeapon.OnClick(who);
            }
            else
            {
                if (UITips.ins != null)
                {
                    UITips.ins.ShowTips("请先选择一个武器出生点!");
                }
            }
        }
        public void Sync(MapObjectWeaponSpawnPoint who)
        {
            if (MapEditorMgr.ins.CurrentStep == MapEditorStep.WeaponSpawn && currentWeapon != who && who != null)
            {
                if (currentWeapon != null)
                {
                    //last 
                    currentWeapon.FillEmpty();
                }
                // this step will sync weapon view if click
                currentWeapon = who;
                //sync
                foreach (var p in _child)
                {
                    var c = p.GetComponentFully<OneMapObjectWeapon>();
                    int idx = 0;

                    c.SetNumber(0);
                }
                foreach (var p in _child)
                {
                    var c = p.GetComponentFully<OneMapObjectWeapon>();
                    int idx = 0;
                    foreach (var id in who._weapon_ids)
                    {
                        if (id != -1 && id == c.id)
                        {
                            c.SetNumber(idx + 1);
                        }
                        idx++;
                    }
                }
            }
        }
        private MapObjectWeaponSpawnPoint currentWeapon = null;

    }
}