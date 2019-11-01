using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{

    public class UIPanelLayer : MonoBehaviour, IBeginDragHandler, IDragHandler
    {

        public static UIPanelLayer ins = null;
        private GameObject _obj_content;

        private Button btnSave;
        private Button btnClose;

        private Button btnAddBackDec;
        private Button btnAddForeDec;

        private GameObject objMsgbox;

        private Button btnDeleteOk;
        private Button btnDeleteCancel;

        private RectTransform curRecTran;
        private Vector3 offsetPos = Vector3.zero;

        //UIRoot root;

        public int deletedLayerIndex { get; set; }

        void Awake()
        {
            ins = this;
            //this.root = this.GetComponentInParent<UIRoot>();

            curRecTran = transform.GetComponent<RectTransform>();
        }

        void Start()
        {
            _obj_content = transform.Find("Mid/List/Viewport/ListContent").gameObject;
            this.LoadListView();

            btnSave = transform.Find("Top/btnSave").GetComponent<Button>();
            btnClose = transform.Find("Top/btnClose").GetComponent<Button>(); 

            btnAddBackDec = transform.Find("Mid/btnAddBackDec").GetComponent<Button>(); 
            btnAddForeDec = transform.Find("Mid/btnAddForeDec").GetComponent<Button>();

            objMsgbox = transform.Find("Bottom/MessageBox").gameObject;

            btnDeleteOk = transform.Find("Bottom/MessageBox/btnOK").GetComponent<Button>() ;
            btnDeleteCancel = transform.Find("Bottom/MessageBox/btnCancel").GetComponent<Button>();

            if (btnSave != null)
                btnSave.onClick.AddListener(OnClickBtnSave);

            if (btnClose != null)
                btnClose.onClick.AddListener(OnClickBtnClose);

            if (btnAddBackDec != null)
                btnAddBackDec.onClick.AddListener(OnClickAddBackDec);

            if (btnAddForeDec != null)
                btnAddForeDec.onClick.AddListener(OnClickAddForeDec);

            if (btnDeleteOk != null)
                btnDeleteOk.onClick.AddListener(OnClickBtnDeleteOK);

            if (btnDeleteCancel != null)
                btnDeleteCancel.onClick.AddListener(OnClickBtnDeleteCancel);
        }

        void OnClickBtnSave()
        {
            Debug.Log("OnClickBtnSave");
        }


        void OnClickBtnClose()
        {
            this.Hide();
        }

        /// <summary>
        /// 增加背景装饰层
        /// </summary>
        void OnClickAddBackDec()
        {
            EditorLayerMgr.ins.CreateBackDecLayer();
        }

        /// <summary>
        /// 增加前景装饰层
        /// </summary>
        void OnClickAddForeDec()
        {
            EditorLayerMgr.ins.CreateForeDecLayer();
        }

        void OnClickBtnDeleteCancel()
        {
            if (objMsgbox != null)
                objMsgbox.SetActive(false);
        }

        void OnClickBtnDeleteOK()
        {
            
            if (objMsgbox != null)
                objMsgbox.SetActive(false);

            if (EditorLayerMgr.ins.curEditLayer != -1)
            {
                EditorLayerMgr.ins.DeleteLayer(deletedLayerIndex);
            }
        }

        public void ShowMsgBox()
        {
            if (objMsgbox != null)
            {
                objMsgbox.SetActive(true);
            }
        }

        public void Clear()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p);
            }
            _child.Clear();
        }


        public void LoadListView()
        {
            foreach (var p in _child)
            {
                GameObject.Destroy(p.gameObject);
            }
            _child.Clear();
            
            //加载装饰物
            for (int i =0; i< EditorLayerMgr.MAX_LAYER_COUNT; i++)
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

            size.y = 100 * num_of_one + 20;//left pading is 20 
            trans.sizeDelta = size;
        }

        public void Show()
        {
            this.transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.transform.gameObject.SetActive(false);
        }

   
        public void OnUnSelectLayer(int index)
        {
            if (index >= 1)
                _child[index-1].GetComponent<UIPanelLayerItem>().OnUnSelect();
        }


        public void Refresh()
        {
            for (int i=0; i< EditorLayerMgr.MAX_LAYER_COUNT; i++)
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

        private int GetLayerCount()
        {
            int count = 0;
            for (int i=0; i<_child.Count; i++)
            {
                if (_child[i].activeSelf)
                    count++;
            }

            return count;
        }

        List<GameObject> _child = new List<GameObject>();
        public OneMapObjectBase currentSelect = null;
    }
}
