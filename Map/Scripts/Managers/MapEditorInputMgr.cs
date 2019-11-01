using MapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class MapEditorInputMgr
    {
        static MapEditorInputMgr _instance = null;

        public static MapEditorInputMgr ins
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapEditorInputMgr();

                    _instance.mask = LayerMask.GetMask("MapEditorObject");
                }

                return _instance;
            }
        }

        private LayerMask mask;
        private bool isTouchDown = false;
        private Bounds boundingBox;

        public Camera uiCamera = null;

        const int MAX_NUM = 9999;


#if (UNITY_ANDROID || UNITY_IOS|| UNITY_IPHONE ) && !UNITY_EDITOR
        public bool GetTouchDown()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)return true;
            }
            return false;
        }
        public bool GetTouchUp()
        {
            if (Input.touchCount >0)
            {
                var touch = Input.GetTouch(0);
                if (  touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) return false;
            }
            return true;
        }
        public Vector3 GetTouchPosition()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                return touch.position;
            }
            return Vector3.zero;
        }
#else
        public bool GetTouchDown()
        {
            return Input.GetMouseButtonDown(0);
        }
        public bool GetTouchUp()
        {
            return Input.GetMouseButtonUp(0);
        }
        public Vector3 GetTouchPosition()
        {
            return Input.mousePosition;
        }
#endif


        //编辑器下允许连续 增加物件
        public void UpdateWithAdded(Vector3 touchPos)
        {
            if (true)
            {
                var ray = Camera.main.ScreenPointToRay(touchPos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 30000f, mask))
                {// hit somethig
                    if (CurrentSelectObject != null)
                    {
                        CurrentSelectObject.GetComponentFully<MapObjectBase>().SetBright(false);
                    }
                    if (hit.collider.gameObject.GetComponentFully<MapObjectBase>() != null)
                    {
                        CurrentSelectObject = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;

                        //如果选中普通的组件，开始计时长按
                        if ((CurrentSelectObject.GetComponent<MapObjectWeaponSpawnPoint>() == null) &&
                            (CurrentSelectObject.GetComponent<MapObjectSpawnPoint>() == null))
                        {
                            this.BeginMapObjectPressed();
                        }
                    }
                    /* if (hit.collider.gameObject.GetComponentFully<MapObjectDecorate>() != null)
                     {
                         CurrentSelectObject = hit.collider.gameObject;
                     }*/

                    _position_delta = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position - Camera.main.ScreenToWorldPoint(touchPos);
                    pos_begin_touch = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position;
                    // Debug.LogError("hit " + hit.collider.gameObject.name + "    " + CurrentSelectObject.gameObject.name);

                    this.CurrentSelectWeapon = CurrentSelectObject.GetComponent<MapObjectWeaponSpawnPoint>();
                    if (this.CurrentSelectWeapon != null)
                    {//sync view
                        var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorDown>();
                        if (dlg != null)
                            dlg.Sync(this.CurrentSelectWeapon);
                        foreach (var p in CurrentSelectObject.transform.parent.GetComponentsInChildren<MapObjectWeaponSpawnPoint>())
                        {
                            p.transform.GetChild(0).gameObject.SetActive(false);
                        }
                        CurrentSelectWeapon.transform.GetChild(0).gameObject.SetActive(true);
                        MapObjectRoot.ins.CheckAllConflict();
                        if (UITips.ins != null)
                        {
                            int order = CurrentSelectWeapon.GetComponent<MapObjectWeaponSpawnPoint>().Order;
                            if (order > 0)
                            {
                                UITips.ins.ShowTips("您正在设置武器" + order + "出生点枪支，请点击底部枪械设置。");
                            }
                            else
                            {
                                UITips.ins.ShowTips("您正在设置武器出生点枪支，请点击底部枪械设置。");
                            }
                        }
                    }
                }
                else
                {
                    this.CurrentSelectWeapon = null;// if hit nothing  will cancel weapon spawn point select status
                    if (CurrentSelectObject != null)
                    {
                        CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(false);
                    }
                    // not hit some thind  -   if select  then added

                    if (this.currentSelectId > 0)
                    {
                        if (!this.IsExceedMaxObjectCount(this.currentSelectId))
                        {
                            //   if (root.CurrentStep == MapEditorStep.MapObject)
                            {
                                ///////////////////////////////////////////////
                                //点击空地时创建
                                
                                var pos = Camera.main.ScreenToWorldPoint(touchPos);
                               // pos.x = 0f;
                                pos.x = EditorLayerMgr.ins.GetCurLayerTransform().position.x;

                                if (MapObjectRoot.ins.OnPreCreateObject(pos, this.currentSelectId))
                                {
                                    var obj = MapObjectRoot.ins.CreateObjectById(this.currentSelectId, EditorLayerMgr.ins.GetCurLayerTransform());
                                    //      Debug.LogError(pos);
                                    obj.transform.position = pos;

                                    //因组件规格的不一致，层节点X方向20倍的缩放 默认创建后有得组件的localscalX为0.05 有得localscaleZ为0.05，
                                    //要得到缩放的效果对应的方向localscale必须归一化（多个方向都必须考虑）
                                    Vector3 scale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
                                    scale = this.CheckLocslScale(scale);
                                    obj.transform.localScale = scale;

                                    MapObjectRoot.ins.OnPostCreateObject(obj);
                                    MapObjectRoot.ins.PlayClickEffect(obj.transform.position);
                                    CurrentSelectObject = obj;
                                    //计算包围盒, 有"main"子节点就按main计算，没有就按整个计算
                                    var objMain = CurrentSelectObject.transform.Find("main");
                                    if (objMain != null)
                                        boundingBox = objMain.gameObject.CalculateBounds();
                                    else
                                        boundingBox = CurrentSelectObject.CalculateBounds();
                                }

                                _position_delta = Vector3.zero;

                                return;
                            }
                        }
                    }
                }
            }

        }

        //拖动跟随或连续生成
        public void OnDragMoveOrAdd(Vector3 touchPos)
        {
            var ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30000f, mask))
            {
                // hit somethig
                if (CurrentSelectObject != null)
                {
                    CurrentSelectObject.GetComponentFully<MapObjectBase>().SetBright(false);
                }

                if (hit.collider.gameObject.GetComponentFully<MapObjectBase>() != null)
                {
                    CurrentSelectObject = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject;
                }

            }

            //拖动连续生成
            if ((CurrentSelectObject != null) && (CurrentSelectObject.GetComponent<MapObjectSpawnPoint>() == null) && (CurrentSelectObject.GetComponent<MapObjectWeaponSpawnPoint>() == null))
            {
                if (!this.IsExceedMaxObjectCount(this.currentSelectId))
                {
                    var pos = Camera.main.ScreenToWorldPoint(touchPos);
                    //pos.x = 0f;
                    pos.x = EditorLayerMgr.ins.GetCurLayerTransform().position.x;
                    if (boundingBox != null)
                    {

                        //竖直方向超过足够距离
                        if (Mathf.Abs(boundingBox.center.y - pos.y) >= boundingBox.size.y)
                        {
                            if (pos.y > boundingBox.center.y)
                            {
                                pos.y = boundingBox.center.y + boundingBox.size.y;
                                if (boundingBox.size.y < 1)
                                {
                                    pos.y = boundingBox.center.y + 1;
                                }
                                else
                                {
                                    pos.y = boundingBox.center.y + boundingBox.size.y;
                                }
                                     
                            }
                            else
                            {
                                if (boundingBox.size.y < 1)
                                {
                                    pos.y = boundingBox.center.y - 1;
                                }
                                else
                                {
                                    pos.y = boundingBox.center.y - boundingBox.size.y;
                                }

                            }

                            if (MapObjectRoot.ins.OnPreCreateObject(pos, this.currentSelectId))
                            {

                                if (this.currentSelectId > 0)
                                {
                                    var obj = MapObjectRoot.ins.CreateObjectById(this.currentSelectId, EditorLayerMgr.ins.GetCurLayerTransform());
                                    obj.transform.position = pos;
                                    //因组件规格的不一致，层节点X方向20倍的缩放 默认创建后有得组件的localscalX为0.05 有得localscaleZ为0.05，
                                    //要得到缩放的效果对应的方向localscale必须归一化（多个方向都必须考虑）
                                    Vector3 scale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
                                    scale = this.CheckLocslScale(scale);
                                    obj.transform.localScale = scale;
                                    MapObjectRoot.ins.OnPostCreateObject(obj);
                                    MapObjectRoot.ins.PlayClickEffect(obj.transform.position);
                                    CurrentSelectObject = obj;
                                    boundingBox.center = obj.transform.position;
                                }
                            }
                            else
                            {
                                boundingBox.center = pos;
                            }

                        }
                        //水平方向超过足够距离
                        else if (Mathf.Abs(boundingBox.center.z - pos.z) >= boundingBox.size.z)
                        {
                            if (pos.z > boundingBox.center.z)
                            {
                                if (boundingBox.size.z < 1)
                                {
                                    pos.z = boundingBox.center.z + 1;
                                }
                                else
                                {
                                    pos.z = boundingBox.center.z + boundingBox.size.z;
                                }

                            }
                            else
                            {
                                if (boundingBox.size.z < 1)
                                {
                                    pos.z = boundingBox.center.z - 1;
                                }
                                else
                                {
                                    pos.z = boundingBox.center.z - boundingBox.size.z;
                                }
                                
                            }

                            if (MapObjectRoot.ins.OnPreCreateObject(pos, this.currentSelectId))
                            {

                                if (this.currentSelectId > 0)
                                {
                                    var obj = MapObjectRoot.ins.CreateObjectById(this.currentSelectId, EditorLayerMgr.ins.GetCurLayerTransform());
                                    obj.transform.position = pos;
                                    //因组件规格的不一致，层节点X方向20倍的缩放 默认创建后有得组件的localscalX为0.05 有得localscaleZ为0.05，
                                    //要得到缩放的效果对应的方向localscale必须归一化（多个方向都必须考虑）
                                    Vector3 scale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
                                    scale = this.CheckLocslScale(scale);
                                    obj.transform.localScale = scale;

                                    MapObjectRoot.ins.OnPostCreateObject(obj);
                                    MapObjectRoot.ins.PlayClickEffect(obj.transform.position);
                                    CurrentSelectObject = obj;
                                    boundingBox.center = obj.transform.position;
                                }

                            }
                            else
                            {
                                boundingBox.center = pos;
                            }
                        }
                    }

                }
            }
        }

        Vector3 pos_begin_touch;
        public void UpdateWithDelete(Vector3 touchPos)
        {
            if (true)
            {
                var ray = Camera.main.ScreenPointToRay(touchPos);
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 30000f, mask))
                {// hit somethig
                    //hit map object
                    var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                    if (obj != null)
                    {
                        CurrentSelectObject = obj.gameObject;
                    }
                    //hit map dec
                    //  if (hit.collider.gameObject.GetComponentFully<MapObjectDecorate>() != null)
                    {
                        //  CurrentSelectObject = hit.collider.gameObject;
                    }
                    //     Debug.LogError("hit " + hit.collider.gameObject.name);
                    if (CurrentSelectObject != null)
                    {
                        this.CurrentSelectWeapon = CurrentSelectObject.GetComponentFully<MapObjectWeaponSpawnPoint>();
                    }


                }

            }
            if (CurrentSelectObject != null)
            {
                var type = CurrentSelectObject.GetComponent<MapObjectBase>();
                if ((type as MapObjectSpawnPoint == null) && (type as MapObjectWeaponSpawnPoint == null))
                {// hit spasm will ignore
                    // GameObject.DestroyImmediate(CurrentSelectObject);
                    // MapObjectRoot.ins.DestroyObjectImmediate(CurrentSelectObject);
                    MapObjectRoot.ins.PlayClickEffect(CurrentSelectObject.transform.position);
                    CurrentSelectObject.SetActive(false);
                    MapObjectRoot.ins.DeleteCheckDecrate(CurrentSelectObject);
                    EditorRuntime.Delete(CurrentSelectObject);
                    MapEditorUIMgr.ins.GetPanel<UIPanelMapEditor>().RefreshUndoBtnState();

                }
                MapObjectRoot.ins.CheckAllConflict();
                CurrentSelectObject = null;
                //cancle
                //  root._panel_up.OnBtnDeleteClick();
                return;

            }
            CurrentSelectObject = null;
            ClearSelectObject();
        }

        public void UpdateWithSelected(Vector3 touchPos)
        {
            if (true)
            {
                var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 30000f, mask))
                {
                    var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                    if (obj != null)
                    {
                        var type = obj.gameObject.GetComponent<MapObjectBase>();

                        if ((type as MapObjectSpawnPoint == null) && (type as MapObjectWeaponSpawnPoint == null) && this.CanChangeToFingerMode(obj.gameObject))
                        {
                            SelectObject(obj.gameObject);
                        }
                        else//出生武器点
                        {
                            CurrentSelectObject = obj.gameObject;
                            _position_delta = hit.collider.gameObject.GetComponentFully<MapObjectBase>().gameObject.transform.position - Camera.main.ScreenToWorldPoint(touchPos);
                        }
                    }
                    else
                    {
                        ClearSelectObject();
                    }
                }
                else
                {
                    ClearSelectObject();
                }
            }
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="obj"></param>
        public void SelectObject(GameObject obj)
        {
            if (obj != null)
            {
                CurrentSelectObject = obj;
                //UIPanelProperty.ins.Show();
#if UNITY_EDITOR
                MapEditorUIMgr.ins.ShowPanel<UIPanelProperty>();
#endif


                //MapEditorUIMgr.ins.ShowPanel<UIPanelProperty2>();
                //MapEditorUIMgr.ins.GetPanel<UIPanelProperty2>().InitData(obj);
                EditorSelection.activeObject = CurrentSelectObject;
                //UIPanelOperation.ins.InitData(CurrentSelectObject.transform);

                MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditorOperation>();
                var dlg = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorOperation>();
                if (dlg != null)
                    dlg.InitData(CurrentSelectObject.transform);
            }
        }

        public void ClearSelect()
        {
            this.currentSelectId = 0;
            this.ClearSelectObject();
        }

        /// <summary>
        ///  取消选择
        /// </summary>
        public void ClearSelectObject()
        {
            if (CurrentSelectObject != null)
            {
                CurrentSelectObject = null;
                //UIPanelProperty.ins.Hide();
                //MapEditorUIMgr.ins.ClosePanel<UIPanelProperty2>();
#if UNITY_EDITOR
                MapEditorUIMgr.ins.ClosePanel<UIPanelProperty>();
#endif
                EditorSelection.activeObject = null;

                MapEditorUIMgr.ins.ClosePanel<UIPanelMapEditorOperation>();
                //UIPanelOperation.ins.Hide();
            }

        }



        bool IsTouchPositionHasObject()
        {
            var ray = Camera.main.ScreenPointToRay(this.GetTouchPosition());
#if UNITY_EDITOR
            Debug.DrawLine(ray.origin, ray.GetPoint(100f), Color.red, 10f);
#endif
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30000f, mask))
            {
                var obj = hit.collider.gameObject.GetComponentFully<MapObjectBase>();
                if (obj != null)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnUpdate()
        {
            if (isMapObjectPressed)
            {
                if ((Time.time -  lastMapObjectPressedTime) > PRESS_TIME)
                {
                    //选择当前按住的组件
                    if (this.CurrentSelectObject && this.CanChangeToFingerMode(this.CurrentSelectObject))
                    {
                        MapEditorUIMgr.ins.GetPanel<UIPanelMapEditor>().ChangeToFingerMode();
                        this.SelectObject(this.CurrentSelectObject);
                    }

                    isMapObjectPressed = false;
                }
            }
        }


        public void BeginMapObjectPressed()
        {
            isMapObjectPressed = true;
            lastMapObjectPressedTime = Time.time;
        }

        public void EndMapObjectPressed()
        {
            isMapObjectPressed = false;
        }

        //移动角色出生点和武器出生点
        public bool MoveSpawnOrWeaponPosition(GameObject CurrentSelectObject, Vector3 touchPos)
        {
            //这里只处理武器和出生点跟随移动
            if (CurrentSelectObject != null)
            {

                var pos_pre = CurrentSelectObject.transform.position;

                var pos = Camera.main.ScreenToWorldPoint(touchPos) + _position_delta;
                pos.x = 0f;

                CurrentSelectObject.transform.position = pos;
                if (MapObjectRoot.ins.CheckConflict(CurrentSelectObject))
                {
                    MapObjectRoot.ins.SetBrightAll(true);
                    CurrentSelectObject.GetComponent<MapObjectBase>().SetBright(true);
                }
                else
                {
                    MapObjectRoot.ins.SetBrightAll(false);
                }
                //move 
                if (CurrentSelectObject.transform.hasChanged)
                {
                    //   MapObjectRoot.ins.CheckAllConflict();
                    CurrentSelectObject.transform.hasChanged = false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 当前组件能否切换到 手指模式
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool CanChangeToFingerMode(GameObject curSelectObject)
        {
            if (curSelectObject != null)
            {
                int id = 0;
                var striId = curSelectObject.name;
                int.TryParse(striId, out id);
                if (id > 0)
                {
                    var rec = GameConfig.instance.GetComponentConfig(id);
                    if (rec != null)
                    {
                        if (rec.finger_allow == 1)
                            return true;
                        else
                            return false;
                    }

                    return false;
                }

                return false;
            }

            return false;
        }

        public bool OnSpawnOrWeaponPointMove(Vector3 touchPos)
        {
            if (CurrentSelectObject != null)
            {
                var type = CurrentSelectObject.GetComponent<MapObjectBase>();
                if ((type as MapObjectSpawnPoint != null) || (type as MapObjectWeaponSpawnPoint != null))
                {
                    if (this.MoveSpawnOrWeaponPosition(CurrentSelectObject, touchPos))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            return false;
        }

        public Vector3 CheckLocslScale(Vector3 scale)
        {
            if (scale.x < 1.0f && scale.x >= 0f)
                scale.x = 1.0f;
            else if(scale.x < 0f && scale.x > -1f)
                 scale.x = -1.0f;

            if (scale.y < 1.0f && scale.y >= 0f)
                scale.y = 1.0f;
            else if (scale.y < 0f && scale.y > -1f)
                scale.y = -1.0f;

            if (scale.z < 1.0f && scale.z >= 0f)
                scale.z = 1.0f;
            else if (scale.z < 0f && scale.z > -1f)
                scale.z = -1.0f;

            return scale;
        }

        public bool IsExceedMaxObjectCount(GameObject obj)
        {
            var strId = obj.name;
            int id = 0;
            int.TryParse(strId, out id);
            return IsExceedMaxObjectCount(id);
        }
        public bool IsExceedMaxObjectCount(int curSelectId)
        {
#if UNITY_EDITOR
                        const int MAX_NUM = 9999;
                        const int GRID_MAX_NUM = 9999;
#else
            int MAX_NUM = DevConfig.MapEditorMaxAllowMapObjectNumber;
            int GRID_MAX_NUM = DevConfig.MapEditorMaxAllowGridMapObjectNumber;
#endif

            if (MapObjectRoot.ins.IsGridObjectById(curSelectId))
            {
                if (MapObjectRoot.ins.GetGridMapObjectCount() >= GRID_MAX_NUM)
                {
                    UIPopMsg.Show("地形刷组件数量达到上限!");

                    return true;
                }
            }
            else
            {
                if (MapObjectRoot.ins.GetNormalMapObjectCount() >= MAX_NUM)
                {
                    UIPopMsg.Show("组件数量达到上限!");

                    return true;
                }
            }

            return false;

        }


        public GameObject CurrentSelectObject = null;
        //{ set { EditorSelection.activeGameObject = CurrentSelectObject; }
        //  get { return EditorSelection.activeGameObject; }
        //}
        private MapObjectWeaponSpawnPoint CurrentSelectWeapon = null; // if current select is weapon spaen point  , use to update  UIPanelUp 's view ui
        Vector3 _position_delta;

        public int currentSelectId = 0;

        public TouchBehaviour touchBehaviour = TouchBehaviour.Added;

        private bool isMapObjectPressed = false;
        private float lastMapObjectPressedTime;

        public const float PRESS_TIME = 0.5f;
    }

}

