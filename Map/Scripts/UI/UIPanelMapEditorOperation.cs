using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace MapEditor
{
    public class UIPanelMapEditorOperation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //包围盒高宽比
        public float boundBoxRatio = 1.0f;
        public const float MaxScale = 4;
        public const float Offset = 0.0f;
        private RectTransform curRecTran;
        private RectTransform backRecTran;
        // 0 left 1 right 2 up(delete) 3 down (copy)
        public GameObject[] childObjs = new GameObject[4];
        private RectTransform[] childRecTrans = new RectTransform[4];
        private Vector3[] childOriginPostions = new Vector3[4];

        //目标
        private Transform targetTs;

        private float rotatedAngle = 0f;

        //当前缩放
        //private Vector3 scale = Vector3.one;
        //当前旋转
        private Vector3 originRotate = Vector3.zero;
        //
        private Vector3 originPos = Vector3.zero;

        private Vector3 originScale = Vector3.one;

        //两个旋转点之间的距离
        private float pivotDis = 1.0f;

        private Vector3 offsetPos = Vector3.zero;
        private Vector3 offsetTransPos = Vector3.zero;

        //---Auto Generate Code Start---
        //自动声明变量
        //组件手势操作蒙皮
        private Image Image_mask;
        //描述
        private Button btn_left;
        //描述
        private Button btn_right;
        //描述
        private Button btn_up;
        //描述
        private Button btn_down;
        void Awake() 
        {
            //生成自动绑定代码

            Image_mask = transform.Find("Image_mask").GetComponent<Image>();

            btn_left = transform.Find("btn_left").GetComponent<Button>();
            if (btn_left != null)
                btn_left.onClick.AddListener(OnClickLeft);

            btn_right = transform.Find("btn_right").GetComponent<Button>();
            if (btn_right != null)
                btn_right.onClick.AddListener(OnClickRight);

            btn_up = transform.Find("btn_up").GetComponent<Button>();
            if (btn_up != null)
                btn_up.onClick.AddListener(OnClickUp);

            btn_down = transform.Find("btn_down").GetComponent<Button>();
            if (btn_down != null)
                btn_down.onClick.AddListener(OnClickDown);

            this.OnAwake();
        }
//---Auto Generate Code End---
        void OnAwake()
        {
            curRecTran = transform.GetComponent<RectTransform>();
            backRecTran = Image_mask.GetComponent<RectTransform>();
            string[] btnNames = {"btn_left", "btn_right", "btn_up", "btn_down"};

            for (int i = 0; i < 4; i++)
            {
                childObjs[i] = transform.Find(btnNames[i]).gameObject;
                childRecTrans[i] = childObjs[i].transform.GetComponent<RectTransform>();
                childOriginPostions[i] = childRecTrans[i].position;
            }
        }

        public void SetBackgroundSize(Vector2 newSize)
        {
            RectTransform trans = backRecTran;
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }

        void OnClickLeft()
        {
        
        }
        void OnClickRight()
        {
        
        }
        void OnClickUp()
        {
        
        }
        void OnClickDown()
        {
        
        }

        public void InitData(Transform ts)
        {
            Bounds objBoundingBox;

            var main = ts.Find("main");
            if (main != null)
                objBoundingBox = main.gameObject.CalculateBounds();
            else
                objBoundingBox = ts.gameObject.CalculateBounds();

            targetTs = ts;

            originScale = targetTs.localScale;
            originRotate = targetTs.rotation.eulerAngles;
            originPos = targetTs.position;

            boundBoxRatio = objBoundingBox.size.y / objBoundingBox.size.z;

            Vector3 leftPos = targetTs.position + new Vector3(0, 0, objBoundingBox.extents.z + Offset);
            Vector3 rightPos = targetTs.position - new Vector3(0, 0, objBoundingBox.extents.z + Offset);
            Vector3 upPos = targetTs.position + new Vector3(0, objBoundingBox.extents.y + Offset, 0);
            Vector3 downPos = targetTs.position - new Vector3(0, objBoundingBox.extents.y + Offset, 0);

            //世界坐标转屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTs.position);

            Vector3 screenLeftPos = Camera.main.WorldToScreenPoint(leftPos);
            Vector3 screenRightPos = Camera.main.WorldToScreenPoint(rightPos);
            Vector3 screenUpPos = Camera.main.WorldToScreenPoint(upPos);
            Vector3 screenDownPos = Camera.main.WorldToScreenPoint(downPos);

            Vector3 pos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, screenPos,
                MapEditorInputMgr.ins.uiCamera, out pos))
            {
                curRecTran.position = pos;
            }

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(childRecTrans[0], screenLeftPos,
                MapEditorInputMgr.ins.uiCamera, out pos))
            {
                childRecTrans[0].position = pos;
            }

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(childRecTrans[1], screenRightPos,
                MapEditorInputMgr.ins.uiCamera, out pos))
            {
                childRecTrans[1].position = pos;
            }

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(childRecTrans[2], screenUpPos,
                MapEditorInputMgr.ins.uiCamera, out pos))
            {
                childRecTrans[2].position = pos;
            }

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(childRecTrans[3], screenDownPos,
                MapEditorInputMgr.ins.uiCamera, out pos))
            {
                childRecTrans[3].position = pos;
            }

            pivotDis = Vector3.Distance(childRecTrans[0].position, childRecTrans[1].position);

            this.RefreshButtonState();
            this.ResizeSelectBackground();

        }

        //子控件开始拖动
        public void OnBeginChildDrag(int index)
        {
            //InitData(targetTs);

            originScale = targetTs.localScale;
            originRotate = targetTs.rotation.eulerAngles;
            originPos = targetTs.position;

            childObjs[2].SetActive(false);
            childObjs[3].SetActive(false);

            for (int i = 0; i < 4; i++)
            {
                childOriginPostions[i] = childRecTrans[i].position;
            }
        }

        //子控件拖动
        public void OnChildDrag(int index, Vector3 pos)
        {
            childRecTrans[index].position = pos;

            Vector3 originDir = Vector3.one;
            Vector3 newDir = Vector3.one;

            Vector3 pivotPos = Vector3.one;

            if (index == 1)
            {
                originDir = childOriginPostions[1] - childOriginPostions[0];
                newDir = pos - childOriginPostions[0];

                pivotPos = childOriginPostions[0];
            }
            else if (index == 0)
            {
                originDir = childOriginPostions[0] - childOriginPostions[1];
                newDir = pos - childOriginPostions[1];

                pivotPos = childOriginPostions[1];
            }

            Quaternion qt = Quaternion.FromToRotation(originDir, newDir);
            //Debug.Log("=============================EluerAngles: " + qt.eulerAngles);

            //rotatedAngle = Vector3.Angle(originDir, newDir);

            rotatedAngle = qt.eulerAngles.z;

            //Debug.Log("@@@@@@@@@@@@@@@@@@@@@angle: " + rotatedAngle);
            float tmpDis = Vector3.Distance(childRecTrans[0].position, childRecTrans[1].position);
            float scale = tmpDis / pivotDis;
            if (scale >= MaxScale)
                scale = MaxScale;


            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(MapEditorInputMgr.ins.uiCamera, pivotPos);

            //Vector3 screenPos = new Vector3(pivotPos.x, pivotPos.y, 0);
            //屏幕坐标转世界坐标
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);


            //worldPos.x = 0;
            //这里的旋转是总的角度，旋转时要重置到旋转前的矩阵
            targetTs.rotation = Quaternion.Euler(originRotate);
            targetTs.localScale = originScale * scale;
            targetTs.position = originPos;
            targetTs.RotateAround(worldPos, new Vector3(1, 0, 0), rotatedAngle);
        }

        //拖动结束，按需重置拖动点
        public void OnEndChildDrag(int index)
        {
            float tmpDis = Vector3.Distance(childRecTrans[0].position, childRecTrans[1].position);
            float scale = tmpDis / pivotDis;
            if (scale >= MaxScale)
            {
                scale = MaxScale;
                if (index == 1)
                {
                    childRecTrans[1].position = childRecTrans[0].position + (childRecTrans[1].position - childRecTrans[0].position) / tmpDis * MaxScale;

                }
                else if (index == 0)
                {
                    childRecTrans[0].position = childRecTrans[1].position + (childRecTrans[0].position - childRecTrans[1].position) / tmpDis * MaxScale;
                }

            }

            Vector3 centerPos = (childRecTrans[0].position + childRecTrans[1].position) / 2;
            Vector3 rotatedPos = RotateRound(childRecTrans[1].position, centerPos, new Vector3(0, 0, 1), 90);
            childRecTrans[2].position = centerPos + (rotatedPos - centerPos) * boundBoxRatio;
            childRecTrans[3].position = childRecTrans[0].position + childRecTrans[1].position - childRecTrans[2].position;

            this.RefreshButtonState();
            this.ResizeSelectBackground();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                this.offsetTransPos = curRecTran.position - globalMousePos;
                curRecTran.position = globalMousePos;

                Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, 0);
                var pos = Camera.main.ScreenToWorldPoint(screenPos);

                if (EditorSelection.activeGameObject != null)
                {
                    //拖动原始的深度坐标X保持不变
                    pos.x = EditorSelection.activeGameObject.transform.position.x;

                    this.offsetPos = EditorSelection.activeGameObject.transform.position - pos;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                curRecTran.position = globalMousePos + this.offsetTransPos;

                Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, 0);
                var pos = Camera.main.ScreenToWorldPoint(screenPos);

                if (EditorSelection.activeGameObject != null)
                {
                    //拖动原始的深度坐标X保持不变
                    pos.x = EditorSelection.activeGameObject.transform.position.x;
                    EditorSelection.activeGameObject.transform.position = pos + this.offsetPos;
                }
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                curRecTran.position = globalMousePos + this.offsetTransPos;

                Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, 0);
                var pos = Camera.main.ScreenToWorldPoint(screenPos);
                
                if (EditorSelection.activeGameObject != null)
                {
                    //拖动原始的深度坐标X保持不变
                    pos.x = EditorSelection.activeGameObject.transform.position.x;
                    EditorSelection.activeGameObject.transform.position = pos + this.offsetPos;
                    if (MapEditorMgr.ins.EnableAutoGrid)
                        MapObjectRoot.ins.OnEndMoveObject(EditorSelection.activeGameObject);
                    this.InitData(EditorSelection.activeGameObject.transform);
                }
            }

        }

        public Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }

        void Start()
		{
		
		}


        public void ResizeSelectBackground()
        {
            var disx = Math.Abs(childRecTrans[1].anchoredPosition.x - childRecTrans[0].anchoredPosition.x);
            var disy = Math.Abs(childRecTrans[1].anchoredPosition.y - childRecTrans[0].anchoredPosition.y);
            if (disx > disy)
                this.SetBackgroundSize(new Vector2(disx, disx));
            else
                this.SetBackgroundSize(new Vector2(disy, disy));
        }


        public void RefreshButtonState()
        {
            if (MapEditorInputMgr.ins.CurrentSelectObject != null)
            {
                int id = 0;
                var striId = MapEditorInputMgr.ins.CurrentSelectObject.name;

                int.TryParse(striId, out id);
                if (id > 0)
                {
                    var rec = GameConfig.instance.GetComponentConfig(id);
                    if (rec != null)
                    {
                        // 0 left 1 right 2 up(delete) 3 down (copy)
                        if (rec.copy_allow == 1)
                            childObjs[3].SetActive(true);
                        else
                            childObjs[3].SetActive(false);

                        if (rec.delete_allow == 1)
                            childObjs[2].SetActive(true);
                        else
                            childObjs[2].SetActive(false);

                        if (rec.rotate_zoom_allow == 1)
                        {
                            childObjs[0].SetActive(true);
                            childObjs[1].SetActive(true);
                        }
                        else
                        {
                            childObjs[0].SetActive(false);
                            childObjs[1].SetActive(false);
                        }

                    }
                }
            }
        }
	}
}