using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelMapEditorMapLayerProperty : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private RectTransform curRecTran;
        private Vector3 offsetPos = Vector3.zero;

        //---Auto Generate Code Start---
        //自动声明变量
        //描述
        private Button btnSave;
        //描述
        private Button btnClose;
        //描述
        private InputField input_Xspeed;
        //描述
        private InputField input_Yspeed;
        void Awake() 
        {
            //生成自动绑定代码

            btnSave = transform.Find("Top/btnSave").GetComponent<Button>();
            if (btnSave != null)
                btnSave.onClick.AddListener(OnClickSave);

            btnClose = transform.Find("Top/btnClose").GetComponent<Button>();
            if (btnClose != null)
                btnClose.onClick.AddListener(OnClickClose);

            input_Xspeed = transform.Find("Mid/input_Xspeed").GetComponent<InputField>();

            input_Yspeed = transform.Find("Mid/input_Yspeed").GetComponent<InputField>();

            this.OnAwake();
        }
//---Auto Generate Code End---
        void OnAwake()
        {
            curRecTran = transform.GetComponent<RectTransform>();
        }
        void OnClickSave()
        {
            Debug.Log("@@@@@@@@@@@@ input_Xspeed.text: " + input_Xspeed.text);
            Debug.Log("@@@@@@@@@@@@ input_Yspeed.text: " + input_Yspeed.text);

            float ratioX = 1.0f;
            if (!string.IsNullOrEmpty(input_Xspeed.text))
            {
                float.TryParse(input_Xspeed.text, out ratioX);
                if (ratioX > 1.0f)
                {
                    UITips.ins.ShowTips("数值不能超过1");
                    return;
                }
            }

            float ratioY = 1.0f;
            if (!string.IsNullOrEmpty(input_Yspeed.text))
            {
                float.TryParse(input_Yspeed.text, out ratioY);
                if (ratioY > 1.0f)
                {
                    UITips.ins.ShowTips("数值不能超过1");
                    return;
                }
            }

            EditorLayerMgr.ins.SetMoveFactor(ratioX, ratioY);

            UITips.ins.ShowTips("保存成功!");


        }
        void OnClickClose()
        {
            MapEditorUIMgr.ins.ClosePanel<UIPanelMapEditorMapLayerProperty>();
        }

		
		void Start()
		{

        }

        public void Init()
        {
            var data = EditorLayerMgr.ins.layerdatas[EditorLayerMgr.ins.curEditLayer-1];
            if (data != null)
            {
                if (input_Xspeed != null)
                    input_Xspeed.text = data.moveFactorX.ToString();

                if (input_Yspeed != null)
                    input_Yspeed.text = data.moveFactorY.ToString();
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
    }
}