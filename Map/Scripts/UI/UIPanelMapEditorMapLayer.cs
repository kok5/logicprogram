using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    enum LayerType
    {
        BackDecorate,
        Terrain,
        ForeDecorate,
    }

    public class UIPanelMapEditorMapLayer : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private GameObject _obj_content;
        private RectTransform curRecTran;
        private Vector3 offsetPos = Vector3.zero;

        //---Auto Generate Code Start---
        //自动声明变量
        //描述
        private Button btnSave;
        //描述
        private Button btnClose;
        //描述
        private Button btnAddBackDec;
        //描述
        private Button btnAddForeDec;
        //描述
        private Button btnAddTerrain;
        void Awake() 
        {
            //生成自动绑定代码

            btnSave = transform.Find("Top/btnSave").GetComponent<Button>();
            if (btnSave != null)
                btnSave.onClick.AddListener(OnClickSave);

            btnClose = transform.Find("Top/btnClose").GetComponent<Button>();
            if (btnClose != null)
                btnClose.onClick.AddListener(OnClickClose);

            btnAddBackDec = transform.Find("Mid/btnAddBackDec").GetComponent<Button>();
            if (btnAddBackDec != null)
                btnAddBackDec.onClick.AddListener(OnClickAddbackdec);

            btnAddForeDec = transform.Find("Mid/btnAddForeDec").GetComponent<Button>();
            if (btnAddForeDec != null)
                btnAddForeDec.onClick.AddListener(OnClickAddforedec);

            btnAddTerrain = transform.Find("Mid/btnAddTerrain").GetComponent<Button>();
            if (btnAddTerrain != null)
                btnAddTerrain.onClick.AddListener(OnClickAddterrain);

            this.OnAwake();
        }
//---Auto Generate Code End---
        void OnClickAddterrain()
        {
            EditorLayerMgr.ins.CreateTerrainLayer();
        }

        void OnAwake()
        {
            curRecTran = transform.GetComponent<RectTransform>();

            _obj_content = transform.Find("Mid/List/Viewport/ListContent").gameObject;
        }
        void OnClickSave()
        {
            Debug.Log("OnClickBtnSave");
        }
        void OnClickClose()
        {
            MapEditorUIMgr.ins.ClosePanel<UIPanelMapEditorMapLayer>();
        }

        //增加背景层
        void OnClickAddbackdec()
        {
            EditorLayerMgr.ins.CreateBackDecLayer();
        }
        //增加前景层
        void OnClickAddforedec()
        {
            EditorLayerMgr.ins.CreateForeDecLayer();
        }

		
		void Start()
		{
		    this.LoadListView();

            this.Refresh();
        }

        public void LoadListView()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p.gameObject);
            }
            _child.Clear();

            //加载装饰物
            for (int i = 0; i < EditorLayerMgr.MAX_LAYER_COUNT; i++)
            {
                var obj = MapLoader.ins.LoadEditorV1("LayerItem");

                if (obj == null) break;
                obj = GameObject.Instantiate<GameObject>(obj);
                obj.transform.SetParent(_obj_content.transform, false);
                //bool ok = obj.GetComponent<UIPanelLayerItem>().InitData(i+1);
                //if (!ok)
                //{
                //    GameObject.DestroyImmediate(obj);
                //    break;
                //}

                obj.SetActive(false);

                _child.Add(obj);
            }
            this.ResizeContent(GetLayerCount());
        }

        void ResizeContent(int num_of_one)
        {
            var trans = _obj_content.GetComponent<RectTransform>();
            var size = trans.sizeDelta;

            size.y = 60 * num_of_one + 20;//left pading is 20 
            trans.sizeDelta = size;
        }

        private int GetLayerCount()
        {
            int count = 0;
            for (int i = 0; i < _child.Count; i++)
            {
                if (_child[i].activeSelf)
                    count++;
            }

            return count;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                offsetPos = curRecTran.position - globalMousePos;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                curRecTran.position = globalMousePos + offsetPos;
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < EditorLayerMgr.MAX_LAYER_COUNT; i++)
            {
                var layerData = EditorLayerMgr.ins.GetLayerDataByIndex(i + 1);
                if ((layerData != null) && (layerData.visible))
                {
                    _child[i].SetActive(true);
                    _child[i].GetComponent<UIPanelLayerItem>().InitData(layerData);
                }
                else
                {
                    _child[i].SetActive(false);
                }
            }

            this.ResizeContent(GetLayerCount());
        }

        //void OnClickBtnDeleteCancel()
        //{
        //    if (objMsgbox != null)
        //        objMsgbox.SetActive(false);
        //}

        //void OnClickBtnDeleteOK()
        //{

        //    if (objMsgbox != null)
        //        objMsgbox.SetActive(false);

        //    if (EditorLayerMgr.ins.curEditLayer != -1)
        //    {
        //        EditorLayerMgr.ins.DeleteLayer(deletedLayerIndex);
        //    }
        //}

        List<GameObject> _child = new List<GameObject>();
        public OneMapObjectBase currentSelect = null;
    }
}