using MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{

    public enum DecrateType
    {
        NotDefined= -1,

        NoDecrate = 17,

        Left = 2,
        Top = 6,
        Right = 9,
        Bottom = 12,

        LeftTop = 3,
        TopRight = 7,
        RightBottom = 10,
        BottomLeft = 13,
        LeftRight = 15,
        TopBottom = 16,

        LeftTopRight = 4,
        TopRightBottom = 8,
        RightBottomLeft = 11,
        BottomLeftTop = 14,

        LeftTopRightBottom = 5,
    }

    public enum LoadType
    {
        MapObject,//物件
        Weapon,//武器
        SpawPoint,//出生点
    }

    public enum TouchBehaviour
    {
        Added,//添加物件
        Deleted, // 移除物件
        Select, // 选择物件
    }
    public enum MapEditorStep
    {
        MapObject,//摆放物件
        SpawnPoint,//设置出生点
        WeaponSpawn,//设置武器 信息
    }
    public class StepBase
    {
        public MapObjectRoot obj_root;
        public string error_string = "";
        // next step  下一步
        public virtual bool OnEnter()
        {
            return false;
        }
        //last step 上一步
        public virtual bool OnExit()
        {
            return false;
        }
    }
    public class Step1_MapObject : StepBase
    {
        public override bool OnEnter()
        {
            //
            MapEditorMgr.ins.CurrentStep = MapEditorStep.MapObject;
            if (obj_root != null)
            {
                obj_root.SetSpawnPointVisible(false);
                obj_root.SetWeaponSpawnPointVisible(false);
            }
            else
            {
                MapObjectRoot.ins.SetSpawnPointVisible(false);
                MapObjectRoot.ins.SetWeaponSpawnPointVisible(false);
            }
            
            MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Added;
            MapEditorInputMgr.ins.ClearSelect();
            
            
            var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>();
            if (dlg != null)
                dlg.LoadListViewByType(LoadType.MapObject);

            return false;
        }
        public override bool OnExit()
        {
            return false;
        }
    }

    public class Step2_SpawnPoint : StepBase
    {
        public override bool OnEnter()
        {
            if (UIEditorGuide.ins != null)
            {
                UIEditorGuide.ins.ShowGuideStep2();
            }
            MapEditorMgr.ins.CurrentStep = MapEditorStep.SpawnPoint;

            if (obj_root != null)
            {
                obj_root.SetSpawnPointVisible(true);
                obj_root.SetWeaponSpawnPointVisible(false);
            }
            else
            {
                MapObjectRoot.ins.SetSpawnPointVisible(true);
                MapObjectRoot.ins.SetWeaponSpawnPointVisible(false);
            }

            MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Added;
            MapEditorInputMgr.ins.ClearSelect();



            var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>();
            if (dlg != null)
                dlg.LoadListViewByType(LoadType.SpawPoint);
            return false;
        }
        public override bool OnExit()
        {
            return false;
        }
    }
    public class Step3_WeaponSpawn : StepBase
    {
        public override bool OnEnter()
        {
            if (UIEditorGuide.ins != null)
            {
                UIEditorGuide.ins.ShowGuideStep3();
            }
            MapEditorMgr.ins.CurrentStep = MapEditorStep.WeaponSpawn;
            if (obj_root != null)
            {
                obj_root.SetSpawnPointVisible(false);
                obj_root.SetWeaponSpawnPointVisible(true);
            }
            else
            {
                MapObjectRoot.ins.SetSpawnPointVisible(false);
                MapObjectRoot.ins.SetWeaponSpawnPointVisible(true);
            }
            
            MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Added;
            MapEditorInputMgr.ins.ClearSelect();

            MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorDown>();
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>().LoadListViewByType(LoadType.Weapon);

            return false;
        }
        public override bool OnExit()
        {
            return false;
        }
    }


    public class MapEditorMgr
    {
        static MapEditorMgr _instance = null;

        public static MapEditorMgr ins
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapEditorMgr();
                    _instance.list_step.Add(new Step1_MapObject
                    {
                        obj_root = MapObjectRoot.ins
                    });

                    _instance.list_step.Add(new Step2_SpawnPoint
                    {
                        obj_root = MapObjectRoot.ins
                    });

                    _instance.list_step.Add(new Step3_WeaponSpawn
                    {
                        obj_root = MapObjectRoot.ins
                    });

                }

                return _instance;
            }
        }


        public bool IsNewMapVersion(string strVersion)
        {
            string[] t = strVersion.Split('.');
            if (t.Length == 2)
            {
                int mainVersion = int.Parse(t[0]);
                int subVersion = int.Parse(t[1]);

                // > “1.0” 就是新版本
                if (mainVersion * 10000 + subVersion > 10000)
                    return true;

            }

            return false;
        }

        /// <summary>
        /// 如果是老版地图，预制的名字需要转换成对应表的id
        /// </summary>
        /// <param name="name">之前的预制名字</param>
        /// <param name="map">当前地图数据</param>
        public string ConvertPrefabName(string name, Serializable.Map map)
        {
            //已经是新地图，不做转换
            if (this.IsNewMapVersion(map.version))
                return name;
            else
            {
                string key = map.theme.ToString() + "/" + name;
                return GameConfig.instance.GetComponentIdByName(key).ToString();
            }

        }

        public int ConvertLayerIndex(int layerIndex)
        {
            if (layerIndex == 0)
                return EditorLayerMgr.TERRAIN_LAYER_INDEX;
            else
                return layerIndex;
        }


        public int GetDecorateIndex(GameObject obj)
        {
            if (obj != null)
            {
                var dec = obj.transform.Find("change");
                if (dec != null)
                {
                    for (int i = 2; i <= 16; i++)
                    {
                        var child = dec.Find(i.ToString());
                        if (child != null)
                        {
                            if (child.gameObject.activeSelf)
                                return i;
                        }

                        if (i == (int)DecrateType.Top || (i == (int)DecrateType.TopBottom))
                        {
                            var randDec = dec.Find((i + 100).ToString());
                            if (randDec != null)
                            {
                                if (randDec.gameObject.activeSelf)
                                    return i + 100;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 表里填的字段是否包含指定主题
        /// </summary>
        /// <param name="excelTxt"></param>
        /// <param name="curThemeId"></param>
        /// <returns></returns>

        public bool IsHasTheme(string excelTxt, int curThemeId)
        {
            string[] themes = excelTxt.Split(',');
            for (int i = 0; i < themes.Length; i++)
            {
                if (themes[i] == curThemeId.ToString())
                    return true;
            }
            return false;
        }

        public bool IsHasGameMode(string excelTxt, int curGameMode)
        {
            string[] gamemodes = excelTxt.Split(',');
            for (int i = 0; i < gamemodes.Length; i++)
            {
                if (gamemodes[i] == curGameMode.ToString())
                    return true;
            }
            return false;
        }

        public void Init()
        {
            EditorUndo.Reset();
        }

        public void Clear()
        {
            this.EnableAutoGrid = true;
            this.IsShowBottomToolbar = true;
            this.HasPreview = false;
            this.CurrentStep = MapEditorStep.MapObject;
            this.touchBehaviour = TouchBehaviour.Added;
        }

        public Vector3 CorrentScale(string name, int theme, Vector3 scale)
        {

            if (theme == 26 && name == "36")
            {
                return Vector3.one;
            }

            if (theme == 25)
            {
                if (name == "50" || name == "48")
                {
                    return Vector3.one;
                }
            }

            return scale;

        }

        public Vector3 CorrentRotation(string name, int theme, Vector3 rotation)
        {

            if (theme == 26 && name == "36")
            {
                return Vector3.zero;
            }

            if (theme == 25)
            {
                if (name == "50" || name == "48")
                {
                    return Vector3.zero;
                }
            }

            return rotation;

        }

        public TouchBehaviour touchBehaviour = TouchBehaviour.Added;

        public List<StepBase> list_step = new List<StepBase>();
        public int CurrentStepIndex = 0;
        public MapEditorStep CurrentStep = MapEditorStep.MapObject;

        //是否自动吸附网格
        public bool EnableAutoGrid = true;

        //是否显示底部栏
        public bool IsShowBottomToolbar = true;
        public bool HasPreview = false;


    }

}

