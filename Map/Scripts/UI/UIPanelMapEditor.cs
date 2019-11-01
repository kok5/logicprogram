using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public enum MapEditorComponentType
    {
        Default = -1,
        Script = 1,                  //脚本类
        DecorationLayer = 2,  //装饰层
        Ground = 3,               //设置武器 信息
        Platform = 4,             //跳台类
        Decoration = 5,         //饰品（专门放在地形层的？)
        Item = 6,                  //道具
        Motive = 7,               //运动器
        Organ = 8,                //普通机关
        OrganAttack = 9,      //伤害机关
        Monster = 10,          //怪物
        Boss = 11,               //Boss
        Npc = 12,                //Npc
        Other = 13,              //其它
        Grid = 14,                //九宫格
        Fragile = 15,            //易碎物

        PlayerPoint = 10001,           //出生点(放在这以便统一处理)
        WeaponPoint = 10002,        //武器点(放在这以便统一处理)
    }

    public class UIPanelMapEditor : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(maskRecTran, eventData.position,
                eventData.pressEventCamera))
            {
                //移动出生点武器点优先
                if (!MapEditorInputMgr.ins.OnSpawnOrWeaponPointMove(eventData.position))
                {
                    if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Added)
                        MapEditorInputMgr.ins.OnDragMoveOrAdd(eventData.position);
                    else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Deleted)
                        MapEditorInputMgr.ins.UpdateWithDelete(eventData.position);
                    else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Select)
                        MapEditorInputMgr.ins.UpdateWithSelected(eventData.position);
                }

            }

            MapEditorInputMgr.ins.EndMapObjectPressed();
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MapEditorInputMgr.ins.EndMapObjectPressed();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            MapEditorInputMgr.ins.uiCamera = eventData.pressEventCamera;

            if (RectTransformUtility.RectangleContainsScreenPoint(maskRecTran, eventData.position, eventData.pressEventCamera))
            {
                if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Added)
                    MapEditorInputMgr.ins.UpdateWithAdded(eventData.position);
                else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Deleted)
                    MapEditorInputMgr.ins.UpdateWithDelete(eventData.position);
                else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Select)
                    MapEditorInputMgr.ins.UpdateWithSelected(eventData.position);
            }
            else
                Debug.Log("OnPointerDown out");
        }


        

        public const float MAP_MINI_MOVE_WIDTH = 2.0f;
        public const float MAP_MINI_MOVE_HEIGHT = 2.0f;

        private bool isPenBtnVisible = true;

        Camera main_camera;
        GameObject obj_MapEdge;
        SceneLevelBackwardsRenderer background = null;

        private RectTransform maskRecTran;
        private Vector3 originGridPos;

        //底部栏
        private GameObject objBottomToolbar;
        //网格
        private GameObject objGrid;

        private Dictionary<MapEditorComponentType, Image> dicTabImages = new Dictionary<MapEditorComponentType, Image>();

        //---Auto Generate Code Start---
        //自动声明变量
        //描述
        private Button btn_width_left_plus;
        //描述
        private Button btn_width_left_reduce;
        //描述
        private Button btn_width_right_plus;
        //描述
        private Button btn_width_right_reduce;
        //描述
        private Button btn_height_up_plus;
        //描述
        private Button btn_height_up_reduce;
        //描述
        private Image panel_Net;
        //描述
        private Button btn_save_out;
        //描述
        private Button btn_disaster;
        //描述
        private Button btn_rule;
        //描述
        private Button btn_data;
        //描述
        private Button btn_layer_select;
        //描述
        private Button btn_layer_property;
        //描述
        private Button btn_hide;
        //img_hide选中后显示，再次点击消失
        private Image img_hide;
        //描述
        private Button btn_delete;
        //img_delete
        private Image img_delete;
        //描述
        private Button btn_pen;
        //img_pen
        private Image img_pen;
        //描述
        private Button btn_finger;
        //img_finger
        private Image img_finger;
        //描述
        private Button btn_net;
        //img_net
        private Image img_net;
        //描述
        private Button btn_cancel;
        //img_cancel 有可删除的才显示亮
        private Image img_cancel;
        //描述
        private Button btn_run;
        //描述
        private Button btn_preview;
        //描述
        private Image img_description;
        //道具简介按钮
        private Button btn_help;
        //脚本类按钮
        private Button btn_script;
        //img_script 点击后显示
        private Image img_script;
        //装饰品类按钮
        private Button btn_decoration;
        //img_decoration点击后显示
        private Image img_decoration;
        //地形类按钮
        private Button btn_ground;
        //img_ground点击后显示
        private Image img_ground;
        //运动器类按钮
        private Button btn_motive;
        //img_motive点击后显示
        private Image img_motive;
        //破碎物类按钮
        private Button btn_fragile;
        //img_fragile点击后显示
        private Image img_fragile;
        //普通机关类按钮
        private Button btn_organ;
        //img_organ点击后显示
        private Image img_organ;
        //伤害机关类按钮
        private Button btn_organ_attack;
        //img_organ_attack点击后显示
        private Image img_organ_attack;
        //九宫格类按钮
        private Button btn_grid;
        //img_grid点击后显示
        private Image img_grid;
        //编辑出生点按钮
        private Button btn_player_point;
        //img_player_point点击后显示
        private Image img_player_point;
        //编辑武器点按钮
        private Button btn_weapon_point;
        //img_weapon_point点击后显示
        private Image img_weapon_point;
        //装饰层类，标签ID2
        private Button btn_decoration_layer;
        //img_decoration_layer点击后显示
        private Image img_decoration_layer;
        //跳台类按钮
        private Button btn_platform;
        //img_platform点击后显示
        private Image img_platform;
        //道具类按钮
        private Button btn_item;
        //img_item点击后显示
        private Image img_item;
        //btn_change 点击切换组件类型与组件显示作用
        private Button btn_change;
        //img_type  显示右边选中类型的图片
        private Image img_type;
        //Text_type_name显示右边选中类型的名称
        private Text Text_type_name;
        void Awake() 
        {
            //生成自动绑定代码

            btn_width_left_plus = transform.Find("Panel_MapSize_Adjust/btn_width_left_plus").GetComponent<Button>();
            if (btn_width_left_plus != null)
                btn_width_left_plus.onClick.AddListener(OnClickWidthLeftPlus);

            btn_width_left_reduce = transform.Find("Panel_MapSize_Adjust/btn_width_left_reduce").GetComponent<Button>();
            if (btn_width_left_reduce != null)
                btn_width_left_reduce.onClick.AddListener(OnClickWidthLeftReduce);

            btn_width_right_plus = transform.Find("Panel_MapSize_Adjust/btn_width_right_plus").GetComponent<Button>();
            if (btn_width_right_plus != null)
                btn_width_right_plus.onClick.AddListener(OnClickWidthRightPlus);

            btn_width_right_reduce = transform.Find("Panel_MapSize_Adjust/btn_width_right_reduce").GetComponent<Button>();
            if (btn_width_right_reduce != null)
                btn_width_right_reduce.onClick.AddListener(OnClickWidthRightReduce);

            btn_height_up_plus = transform.Find("Panel_MapSize_Adjust/btn_height_up_plus").GetComponent<Button>();
            if (btn_height_up_plus != null)
                btn_height_up_plus.onClick.AddListener(OnClickHeightUpPlus);

            btn_height_up_reduce = transform.Find("Panel_MapSize_Adjust/btn_height_up_reduce").GetComponent<Button>();
            if (btn_height_up_reduce != null)
                btn_height_up_reduce.onClick.AddListener(OnClickHeightUpReduce);

            panel_Net = transform.Find("panel_Net").GetComponent<Image>();

            btn_save_out = transform.Find("Panel_Top/btn_save_out").GetComponent<Button>();
            if (btn_save_out != null)
                btn_save_out.onClick.AddListener(OnClickSaveOut);

            btn_disaster = transform.Find("Panel_Top/btn_disaster").GetComponent<Button>();
            if (btn_disaster != null)
                btn_disaster.onClick.AddListener(OnClickDisaster);

            btn_rule = transform.Find("Panel_Top/btn_rule").GetComponent<Button>();
            if (btn_rule != null)
                btn_rule.onClick.AddListener(OnClickRule);

            btn_data = transform.Find("Panel_Top/btn_data").GetComponent<Button>();
            if (btn_data != null)
                btn_data.onClick.AddListener(OnClickData);

            btn_layer_select = transform.Find("Panel_Top/btn_layer_select").GetComponent<Button>();
            if (btn_layer_select != null)
                btn_layer_select.onClick.AddListener(OnClickLayerSelect);

            btn_layer_property = transform.Find("Panel_Top/btn_layer_property").GetComponent<Button>();
            if (btn_layer_property != null)
                btn_layer_property.onClick.AddListener(OnClickLayerProperty);

            btn_hide = transform.Find("Panel_Top/btn_hide").GetComponent<Button>();
            if (btn_hide != null)
                btn_hide.onClick.AddListener(OnClickHide);

            img_hide = transform.Find("Panel_Top/btn_hide/img_hide").GetComponent<Image>();

            btn_delete = transform.Find("Panel_Top/btn_delete").GetComponent<Button>();
            if (btn_delete != null)
                btn_delete.onClick.AddListener(OnClickDelete);

            img_delete = transform.Find("Panel_Top/btn_delete/img_delete").GetComponent<Image>();

            btn_pen = transform.Find("Panel_Top/btn_pen").GetComponent<Button>();
            if (btn_pen != null)
                btn_pen.onClick.AddListener(OnClickPen);

            img_pen = transform.Find("Panel_Top/btn_pen/img_pen").GetComponent<Image>();

            btn_finger = transform.Find("Panel_Top/btn_finger").GetComponent<Button>();
            if (btn_finger != null)
                btn_finger.onClick.AddListener(OnClickFinger);

            img_finger = transform.Find("Panel_Top/btn_finger/img_finger").GetComponent<Image>();

            btn_net = transform.Find("Panel_Top/btn_net").GetComponent<Button>();
            if (btn_net != null)
                btn_net.onClick.AddListener(OnClickNet);

            img_net = transform.Find("Panel_Top/btn_net/img_net").GetComponent<Image>();

            btn_cancel = transform.Find("Panel_Top/btn_cancel").GetComponent<Button>();
            if (btn_cancel != null)
                btn_cancel.onClick.AddListener(OnClickCancel);

            img_cancel = transform.Find("Panel_Top/btn_cancel/img_cancel").GetComponent<Image>();

            btn_run = transform.Find("Panel_Top/btn_run").GetComponent<Button>();
            if (btn_run != null)
                btn_run.onClick.AddListener(OnClickRun);

            btn_preview = transform.Find("Panel_Top/btn_preview").GetComponent<Button>();
            if (btn_preview != null)
                btn_preview.onClick.AddListener(OnClickPreview);

            img_description = transform.Find("Panel_Bottom/img_description").GetComponent<Image>();

            btn_help = transform.Find("Panel_Bottom/btn_help").GetComponent<Button>();
            if (btn_help != null)
                btn_help.onClick.AddListener(OnClickHelp);

            btn_script = transform.Find("Panel_Bottom/Panel_bottom_types/btn_script").GetComponent<Button>();
            if (btn_script != null)
                btn_script.onClick.AddListener(OnClickScript);

            img_script = transform.Find("Panel_Bottom/Panel_bottom_types/btn_script/img_script").GetComponent<Image>();

            btn_decoration = transform.Find("Panel_Bottom/Panel_bottom_types/btn_decoration").GetComponent<Button>();
            if (btn_decoration != null)
                btn_decoration.onClick.AddListener(OnClickDecoration);

            img_decoration = transform.Find("Panel_Bottom/Panel_bottom_types/btn_decoration/img_decoration").GetComponent<Image>();

            btn_ground = transform.Find("Panel_Bottom/Panel_bottom_types/btn_ground").GetComponent<Button>();
            if (btn_ground != null)
                btn_ground.onClick.AddListener(OnClickGround);

            img_ground = transform.Find("Panel_Bottom/Panel_bottom_types/btn_ground/img_ground").GetComponent<Image>();

            btn_motive = transform.Find("Panel_Bottom/Panel_bottom_types/btn_motive").GetComponent<Button>();
            if (btn_motive != null)
                btn_motive.onClick.AddListener(OnClickMotive);

            img_motive = transform.Find("Panel_Bottom/Panel_bottom_types/btn_motive/img_motive").GetComponent<Image>();

            btn_fragile = transform.Find("Panel_Bottom/Panel_bottom_types/btn_fragile").GetComponent<Button>();
            if (btn_fragile != null)
                btn_fragile.onClick.AddListener(OnClickFragile);

            img_fragile = transform.Find("Panel_Bottom/Panel_bottom_types/btn_fragile/img_fragile").GetComponent<Image>();

            btn_organ = transform.Find("Panel_Bottom/Panel_bottom_types/btn_organ").GetComponent<Button>();
            if (btn_organ != null)
                btn_organ.onClick.AddListener(OnClickOrgan);

            img_organ = transform.Find("Panel_Bottom/Panel_bottom_types/btn_organ/img_organ").GetComponent<Image>();

            btn_organ_attack = transform.Find("Panel_Bottom/Panel_bottom_types/btn_organ_attack").GetComponent<Button>();
            if (btn_organ_attack != null)
                btn_organ_attack.onClick.AddListener(OnClickOrganAttack);

            img_organ_attack = transform.Find("Panel_Bottom/Panel_bottom_types/btn_organ_attack/img_organ_attack").GetComponent<Image>();

            btn_grid = transform.Find("Panel_Bottom/Panel_bottom_types/btn_grid").GetComponent<Button>();
            if (btn_grid != null)
                btn_grid.onClick.AddListener(OnClickGrid);

            img_grid = transform.Find("Panel_Bottom/Panel_bottom_types/btn_grid/img_grid").GetComponent<Image>();

            btn_player_point = transform.Find("Panel_Bottom/Panel_bottom_types/btn_player_point").GetComponent<Button>();
            if (btn_player_point != null)
                btn_player_point.onClick.AddListener(OnClickPlayerPoint);

            img_player_point = transform.Find("Panel_Bottom/Panel_bottom_types/btn_player_point/img_player_point").GetComponent<Image>();

            btn_weapon_point = transform.Find("Panel_Bottom/Panel_bottom_types/btn_weapon_point").GetComponent<Button>();
            if (btn_weapon_point != null)
                btn_weapon_point.onClick.AddListener(OnClickWeaponPoint);

            img_weapon_point = transform.Find("Panel_Bottom/Panel_bottom_types/btn_weapon_point/img_weapon_point").GetComponent<Image>();

            btn_decoration_layer = transform.Find("Panel_Bottom/Panel_bottom_types/btn_decoration_layer").GetComponent<Button>();
            if (btn_decoration_layer != null)
                btn_decoration_layer.onClick.AddListener(OnClickDecorationLayer);

            img_decoration_layer = transform.Find("Panel_Bottom/Panel_bottom_types/btn_decoration_layer/img_decoration_layer").GetComponent<Image>();

            btn_platform = transform.Find("Panel_Bottom/Panel_bottom_types/btn_platform").GetComponent<Button>();
            if (btn_platform != null)
                btn_platform.onClick.AddListener(OnClickPlatform);

            img_platform = transform.Find("Panel_Bottom/Panel_bottom_types/btn_platform/img_platform").GetComponent<Image>();

            btn_item = transform.Find("Panel_Bottom/Panel_bottom_types/btn_item").GetComponent<Button>();
            if (btn_item != null)
                btn_item.onClick.AddListener(OnClickItem);

            img_item = transform.Find("Panel_Bottom/Panel_bottom_types/btn_item/img_item").GetComponent<Image>();

            btn_change = transform.Find("Panel_Bottom/btn_change").GetComponent<Button>();
            if (btn_change != null)
                btn_change.onClick.AddListener(OnClickChange);

            img_type = transform.Find("Panel_Bottom/btn_change/img_type").GetComponent<Image>();

            Text_type_name = transform.Find("Panel_Bottom/btn_change/Text_type_name").GetComponent<Text>();

            this.OnAwake();
        }
//---Auto Generate Code End---
        void OnClickChange()
        {
        
        }

        void OnClickDecorationLayer()
        {
            MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorDown>();
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>().LoadListView((int)MapEditorComponentType.DecorationLayer);
        }

        void OnClickLayerSelect()
        {
            MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorMapLayer>();
        }
        void OnClickLayerProperty()
        {
            //只能编辑装饰层的属性
            if (!EditorLayerMgr.ins.IsTerrainLayer(EditorLayerMgr.ins.curEditLayer))
            {
                MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorMapLayerProperty>();
                MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorMapLayerProperty>().Init();
            }
        }

        //隐藏或显示底部栏
        void OnClickHide()
        {
            MapEditorMgr.ins.IsShowBottomToolbar = !MapEditorMgr.ins.IsShowBottomToolbar;
            if (MapEditorMgr.ins.IsShowBottomToolbar)
            {
                if (objBottomToolbar != null)
                    objBottomToolbar.SetActive(true);

                var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>();
                if (dlg != null)
                    dlg.gameObject.SetActive(true);

                btn_save_out.gameObject.SetActive(true);
#if UNITY_EDITOR
                btn_layer_select.gameObject.SetActive(true);
                btn_layer_property.gameObject.SetActive(true);
#endif
                if (this.isPenBtnVisible)
                    btn_pen.gameObject.SetActive(true);
                else
                    btn_delete.gameObject.SetActive(true);
                btn_finger.gameObject.SetActive(true);
                btn_net.gameObject.SetActive(true);
                btn_cancel.gameObject.SetActive(true);
                btn_run.gameObject.SetActive(true);

                img_hide.gameObject.SetActive(false);
                btn_hide.gameObject.GetComponent<Image>().enabled = true;
            }
            else
            {
                if (objBottomToolbar != null)
                    objBottomToolbar.SetActive(false);

                var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>();
                if (dlg != null)
                    dlg.gameObject.SetActive(false);

                btn_save_out.gameObject.SetActive(false);
                btn_layer_select.gameObject.SetActive(false);
                btn_layer_property.gameObject.SetActive(false);

                this.isPenBtnVisible = btn_pen.gameObject.activeSelf;

                btn_pen.gameObject.SetActive(false);
                btn_delete.gameObject.SetActive(false);
                btn_finger.gameObject.SetActive(false);
                btn_net.gameObject.SetActive(false);
                btn_cancel.gameObject.SetActive(false);
                btn_run.gameObject.SetActive(false);

                img_hide.gameObject.SetActive(true);
                btn_hide.gameObject.GetComponent<Image>().enabled = false;
            }
        }


        //易碎物
        void OnClickFragile()
        {
            this.OnClickTab(MapEditorComponentType.Fragile);
        }

        //9宫格
        void OnClickGrid()
        {
            this.OnClickTab(MapEditorComponentType.Grid);
        }


        void OnAwake()
        {
            maskRecTran = transform.Find("ClickMask").GetComponent<RectTransform>();
            objGrid = GameObject.Find("Maps/Grid");
            if (objGrid != null)
            {
                originGridPos = objGrid.transform.position;
            }

            objBottomToolbar = transform.Find("Panel_Bottom").gameObject;

            this.RefreshBrushBtnState();

            this.RefreshNetBtnState();


            //处理编辑器中开放不开放的
#if UNITY_EDITOR
            this.btn_layer_select.gameObject.SetActive(true);
            this.btn_layer_property.gameObject.SetActive(true);
#else
            this.btn_layer_select.gameObject.SetActive(false);
            this.btn_layer_property.gameObject.SetActive(false);
#endif

            this.dicTabImages.Add(MapEditorComponentType.Script, this.img_script);
            this.dicTabImages.Add(MapEditorComponentType.Decoration, this.img_decoration);
            this.dicTabImages.Add(MapEditorComponentType.Ground, this.img_ground);
            this.dicTabImages.Add(MapEditorComponentType.Motive, this.img_motive);
            this.dicTabImages.Add(MapEditorComponentType.Fragile, this.img_fragile);
            this.dicTabImages.Add(MapEditorComponentType.Organ, this.img_organ);
            this.dicTabImages.Add(MapEditorComponentType.OrganAttack, this.img_organ_attack);
            this.dicTabImages.Add(MapEditorComponentType.Grid, this.img_grid);

            this.dicTabImages.Add(MapEditorComponentType.PlayerPoint, this.img_player_point);
            this.dicTabImages.Add(MapEditorComponentType.WeaponPoint, this.img_weapon_point);
        }

    //玩家出生点
    void OnClickPlayerPoint()
        {
            if (MapEditorMgr.ins.CurrentStep == MapEditorStep.MapObject)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.MapObject].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.SpawnPoint].OnEnter();
                MapEditorMgr.ins.CurrentStep = MapEditorStep.SpawnPoint;

                this.RefreshTabImages(MapEditorComponentType.PlayerPoint);
            }
            else if (MapEditorMgr.ins.CurrentStep == MapEditorStep.SpawnPoint)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.SpawnPoint].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.MapObject].OnEnter();

                MapEditorMgr.ins.CurrentStep = MapEditorStep.MapObject;

                this.RefreshTabImages(MapEditorComponentType.Default);
            }
            else if (MapEditorMgr.ins.CurrentStep == MapEditorStep.WeaponSpawn)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.WeaponSpawn].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.SpawnPoint].OnEnter();
                MapEditorMgr.ins.CurrentStep = MapEditorStep.SpawnPoint;

                this.RefreshTabImages(MapEditorComponentType.PlayerPoint);
            }
        }

        //武器出生点
        void OnClickWeaponPoint()
        {
            if (MapEditorMgr.ins.CurrentStep == MapEditorStep.MapObject)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.MapObject].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.WeaponSpawn].OnEnter();
                MapEditorMgr.ins.CurrentStep = MapEditorStep.WeaponSpawn;

                this.RefreshTabImages(MapEditorComponentType.WeaponPoint);
            }
            else if (MapEditorMgr.ins.CurrentStep == MapEditorStep.SpawnPoint)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.SpawnPoint].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.WeaponSpawn].OnEnter();

                MapEditorMgr.ins.CurrentStep = MapEditorStep.WeaponSpawn;

                this.RefreshTabImages(MapEditorComponentType.WeaponPoint);
            }
            else if (MapEditorMgr.ins.CurrentStep == MapEditorStep.WeaponSpawn)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.WeaponSpawn].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.MapObject].OnEnter();
                MapEditorMgr.ins.CurrentStep = MapEditorStep.MapObject;

                this.RefreshTabImages(MapEditorComponentType.Default);
            }
        }


        //网格
        void OnClickNet()
        {
            MapEditorMgr.ins.EnableAutoGrid = !MapEditorMgr.ins.EnableAutoGrid;
            this.RefreshNetBtnState();
        }

        void RefreshNetBtnState()
        {
            if (MapEditorMgr.ins.EnableAutoGrid)
            {
                if (objGrid != null)
                    objGrid.SetActive(true);
                if (img_net != null)
                    img_net.gameObject.SetActive(true);
            }
            else
            {
                if (objGrid != null)
                    objGrid.SetActive(false);
                if (img_net != null)
                    img_net.gameObject.SetActive(false);
            }
        }

        //撤销
        void OnClickCancel()
        {
            EditorUndo.Undo();
            this.RefreshUndoBtnState();
        }

        void OnClickRedo()
        {
            EditorUndo.Redo();
        }

        //运行
        void OnClickRun()
        {
            //check current map is valid;
            string error = "";
            bool ok = MapObjectRoot.ins.CanGoStartPreview(out error);
            if (!ok)
            {
                Debug.Log("check for preview faild ：" + error);
                //show error msg
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowOK(error);
                }

                return;
            }

            // check has any weapon
            ok = false;
            {
                var list = MapObjectRoot.ins._list_weapon_spawn_points;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].HasAnyWeapon())
                    {
                        ok = true;
                        break;
                    }
                }
            }
            if (!ok)
            {
                if (UICommonDialog.ins != null)
                {
                    UICommonDialog.ins.ShowYesNo("您的武器出生点未配置任何枪支掉落，确定本地图为肉搏战？", () =>
                    {
                        MapObjectRoot.data = new MapEditorStroageData();
                        MapObjectRoot.data.Save(MapObjectRoot.ins);
                        //点击"Run"也合并
                        TerrainMergeMgr.ins.needMerge = true;
                        MapObjectRoot.ins.SerializeToJson();
                        HasPreview = true;
                        StartCoroutine(AsyncLoad());
                    }, () =>
                    {


                    });
                    return;
                }
            }
            MapObjectRoot.data = new MapEditorStroageData();
            MapObjectRoot.data.Save(MapObjectRoot.ins);
            //点击"Run"也合并
            TerrainMergeMgr.ins.needMerge = true;
            MapObjectRoot.ins.SerializeToJson();
            HasPreview = true;
            StartCoroutine(AsyncLoad());
        }

        public static bool HasPreview = false;

        IEnumerator AsyncLoad()
        {
            MapObjectRoot.ins.SetPreviewColliderEnable(false);
            MapObjectRoot.ins.SetAllGameObjectActive(false);

            SceneMgr.LoadLevel("MapEditorPreview");
            SceneMgr.LoadLevelAdditive("MapEditorPreviewRuntime");

            this.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();

        }

        void OnClickPreview()
        {
        
        }

        //左加宽
        void OnClickWidthLeftPlus()
        {
            var pos = obj_MapEdge.transform.Find("left").transform.position;
            pos.z += (2 * MAP_MINI_MOVE_WIDTH);
            MapEditor.MapEditorStroageData.edgebox_left = pos.z;
            obj_MapEdge.transform.Find("left").transform.position = pos;
        }
        //左减宽
        void OnClickWidthLeftReduce()
        {
            var pos = obj_MapEdge.transform.Find("left").transform.position;
            pos.z -= (2 * MAP_MINI_MOVE_WIDTH);
            MapEditor.MapEditorStroageData.edgebox_left = pos.z;
            obj_MapEdge.transform.Find("left").transform.position = pos;
        }
        //右加宽
        void OnClickWidthRightPlus()
        {
            var pos = obj_MapEdge.transform.Find("right").transform.position;
            pos.z -= (2 * MAP_MINI_MOVE_WIDTH);
            MapEditor.MapEditorStroageData.edgebox_right = pos.z;
            obj_MapEdge.transform.Find("right").transform.position = pos;
        }
        //右减宽
        void OnClickWidthRightReduce()
        {
            var pos = obj_MapEdge.transform.Find("right").transform.position;
            pos.z += (2 * MAP_MINI_MOVE_WIDTH);
            MapEditor.MapEditorStroageData.edgebox_right = pos.z;
            obj_MapEdge.transform.Find("right").transform.position = pos;
        }
        //上加高
        void OnClickHeightUpPlus()
        {
            var pos = obj_MapEdge.transform.Find("up").transform.position;
            pos.y += (2 * MAP_MINI_MOVE_HEIGHT);
            MapEditor.MapEditorStroageData.edgebox_up = pos.y;
            obj_MapEdge.transform.Find("up").transform.position = pos;
        }
        //上减高
        void OnClickHeightUpReduce()
        {
            var pos = obj_MapEdge.transform.Find("up").transform.position;
            pos.y -= (2 * MAP_MINI_MOVE_HEIGHT);
            MapEditor.MapEditorStroageData.edgebox_up = pos.y;
            obj_MapEdge.transform.Find("up").transform.position = pos;
        }

        
        //保存退出
        void OnClickSaveOut()
        {
            if (Base.Events.ins != null)
            {
                Base.Events.ins.FireLua("map_editor", "open_settings");
            }
            return;
            //
            if (UICommonDialog.ins != null)
            {
                UICommonDialog.ins.ShowYesNo("是否放弃本次编辑直接返回到主界面？", () =>
                {

                }, () =>
                {
                    SceneMgr.LoadLevel("GameLogin");
                }, "继续编辑", "放弃编辑");
            }
        }
        void OnClickDisaster()
        {
        
        }
        void OnClickRule()
        {
        
        }
        void OnClickData()
        {
        
        }

        //画笔模式-->橡皮模式

        void OnClickPen()
        {
            if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Added)
            {
                MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Deleted;
            }
            else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Select)
            {
                MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Deleted;
            }

            this.RefreshBrushBtnState();

            MapEditorInputMgr.ins.ClearSelectObject();
        }

        //橡皮模式-->画笔模式
        void OnClickDelete()
        {
            if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Deleted)
            {
                MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Added;
            }
            else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Select)
            {
                MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Added;
            }

            this.RefreshBrushBtnState();

            MapEditorInputMgr.ins.ClearSelectObject();
        }

        //-->选择模式(手指模式)
        void OnClickFinger()
        {
            //
            if (MapEditorInputMgr.ins.CurrentSelectObject != null)
            {
                if (!MapEditorInputMgr.ins.CanChangeToFingerMode(MapEditorInputMgr.ins.CurrentSelectObject))
                    return;
            }

            if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Added)
            {
                MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Select;
            }
            else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Deleted)
            {
                MapEditorMgr.ins.touchBehaviour = TouchBehaviour.Select;
            }

            this.RefreshBrushBtnState();
        }

        void RefreshBrushBtnState()
        {
            if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Deleted)
            {              
                if (img_finger != null)
                    img_finger.gameObject.SetActive(false);

                if (img_pen != null)
                    img_pen.gameObject.SetActive(false);
                if (btn_pen != null)
                    btn_pen.gameObject.SetActive(false);

                if (img_delete != null)
                    img_delete.gameObject.SetActive(true);
                if (btn_delete != null)
                    btn_delete.gameObject.SetActive(true);

            }
            else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Added)
            {
                if (img_finger != null)
                    img_finger.gameObject.SetActive(false);

                if (img_pen != null)
                    img_pen.gameObject.SetActive(true);
                if (btn_pen != null)
                    btn_pen.gameObject.SetActive(true);

                if (img_delete != null)
                    img_delete.gameObject.SetActive(false);
                if (btn_delete != null)
                    btn_delete.gameObject.SetActive(false);
            }
            else if (MapEditorMgr.ins.touchBehaviour == TouchBehaviour.Select)
            {
                if (img_finger != null)
                    img_finger.gameObject.SetActive(true);

                if (img_pen != null)
                    img_pen.gameObject.SetActive(false);

                if (img_delete != null)
                    img_delete.gameObject.SetActive(false);
            }
        }

        void RefreshTabImages(MapEditorComponentType tabType)
        {
            foreach (var item in dicTabImages)
            {
                if (item.Key == tabType)
                    item.Value.gameObject.SetActive(true);
                else
                    item.Value.gameObject.SetActive(false);
            }

        }

        void OnClickTab(MapEditorComponentType tabType)
        {
            this.RefreshTabImages(tabType);
            MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorDown>();
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>().LoadListView((int)tabType);
        }
        
        //脚本类
        void OnClickScript()
        {
            this.OnClickTab(MapEditorComponentType.Script);
        }

        //地形类
        void OnClickGround()
        {
            this.OnClickTab(MapEditorComponentType.Ground);
        }

        //跳台类
        void OnClickPlatform()
        {
            this.OnClickTab(MapEditorComponentType.Platform);
        }
        
        //装饰类
        void OnClickDecoration()
        {
            this.OnClickTab(MapEditorComponentType.Decoration);
        }

        //道具类
        void OnClickItem()
        {
            this.OnClickTab(MapEditorComponentType.Item);
        }

        //运动器类
        void OnClickMotive()
        {
            this.OnClickTab(MapEditorComponentType.Motive);
        }

        //普通机关类
        void OnClickOrgan()
        {
            this.OnClickTab(MapEditorComponentType.Organ);
        }

        //伤害机关类
        void OnClickOrganAttack()
        {
            this.OnClickTab(MapEditorComponentType.OrganAttack);
        }

        //帮助
        void OnClickHelp()
        {
        
        }

		
		void Start()
		{

            if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour
		        || MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Share)
		    {
                //*****暂时未用
		        //main_camera = Camera.main;
		        //obj_MapEdge = GameObject.Find("MapEdge").gameObject;
		        //background = GameObject.FindObjectOfType<BackGround>();
		        //Reset();

		        MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorParkour>();
		    }

        }

        public void Reset()
        {

            {
                var pos = obj_MapEdge.transform.Find("left").transform.position;
                pos.z = MapEditor.MapEditorStroageData.edgebox_left;
                obj_MapEdge.transform.Find("left").transform.position = pos;
            }
            {
                var pos = obj_MapEdge.transform.Find("right").transform.position;
                pos.z = MapEditor.MapEditorStroageData.edgebox_right;
                obj_MapEdge.transform.Find("right").transform.position = pos;
            }
            {
                var pos = obj_MapEdge.transform.Find("down").transform.position;
                pos.y = MapEditor.MapEditorStroageData.edgebox_down;
                obj_MapEdge.transform.Find("down").transform.position = pos;
            }
            {
                var pos = obj_MapEdge.transform.Find("up").transform.position;
                pos.y = MapEditor.MapEditorStroageData.edgebox_up;
                obj_MapEdge.transform.Find("up").transform.position = pos;
            }

        }

        public void OnShowAdjustMapSizeBtn(bool flag)
        {
            if (btn_width_left_plus != null)
                btn_width_left_plus.gameObject.SetActive(flag);

            if (btn_width_left_reduce != null)
                btn_width_left_reduce.gameObject.SetActive(flag);

            if (btn_width_right_plus != null)
                btn_width_right_plus.gameObject.SetActive(flag);

            if (btn_width_right_reduce != null)
                btn_width_right_reduce.gameObject.SetActive(flag);

            if (btn_height_up_plus != null)
                btn_height_up_plus.gameObject.SetActive(flag);

            if (btn_height_up_reduce != null)
                btn_height_up_reduce.gameObject.SetActive(flag);
        }


        public void ChangeToPenMode()
        {
            this.OnClickDelete();
        }

        public void ChangeToFingerMode()
        {
            this.OnClickFinger();
        }


        void Update()
        {
            MapEditorInputMgr.ins.OnUpdate();
        }

        //退出武器点和出生点模式，进入正常组件编辑模式
        public void ChangeToEditObjectMode()
        {
            if (MapEditorMgr.ins.CurrentStep == MapEditorStep.WeaponSpawn)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.WeaponSpawn].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.MapObject].OnEnter();
                MapEditorMgr.ins.CurrentStep = MapEditorStep.MapObject;
            }
            else if (MapEditorMgr.ins.CurrentStep == MapEditorStep.SpawnPoint)
            {
                MapEditorMgr.ins.list_step[(int)MapEditorStep.SpawnPoint].OnExit();
                MapEditorMgr.ins.list_step[(int)MapEditorStep.MapObject].OnEnter();

                MapEditorMgr.ins.CurrentStep = MapEditorStep.MapObject;
            }
        }

        //重置网格位置
        public void ResetGridPos()
        {
            if (objGrid != null)
                objGrid.transform.position = originGridPos;
        }

        //调整位置，是网格不要移出屏幕
        public void AjustGridPos(Vector3 offset)
        {
            if (objGrid != null)
            {
                //水平方向 (是z没错！)
                var offsetZ = (int)Math.Floor(offset.z);
                var offsetY = (int)Math.Floor(offset.y);

                if (offsetZ % 2 == 1)
                    offsetZ -= 1;

                if (offsetY % 2 == 1)
                    offsetY -= 1;

                objGrid.transform.position = originGridPos + new Vector3(objGrid.transform.position.x, offsetY, offsetZ);
            }

        }

        public void RefreshUndoBtnState()
        {
            if (EditorUndo.GetStackCount() > 0)
            {
                if (img_cancel != null)
                    img_cancel.gameObject.SetActive(true);
            }
            else
            {
                if (img_cancel != null)
                    img_cancel.gameObject.SetActive(false);
            }
        }
    }
}