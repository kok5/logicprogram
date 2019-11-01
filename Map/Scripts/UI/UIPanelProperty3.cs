using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelProperty3 : MonoBehaviour
    {
		//---Auto Generate Code Start---
        //自动声明变量
        //描述
        private Button btnTopLeft;
        //描述
        private Button btn_factory;
        //描述
        private Button btn_mapOwned;
        //描述
        private Image btnCheck;
        //描述
        private Button btnTopRight;
        //描述
        private Button btnBottomRight;
        //描述
        private Button btnBottomLeft;
        void Awake() 
        {
            //生成自动绑定代码
            btnTopLeft = transform.Find("btnTopLeft").GetComponent<Button>();
            btn_factory = transform.Find("body/btn_factory").GetComponent<Button>();
            btn_mapOwned = transform.Find("body/Scroll_View_1/Viewport/content/basic/btn_mapOwned").GetComponent<Button>();
            btnCheck = transform.Find("body/Scroll_View_1/Viewport/content/basic/btnCheck").GetComponent<Image>();
            btnTopRight = transform.Find("btnTopRight").GetComponent<Button>();
            btnBottomRight = transform.Find("btnBottomRight").GetComponent<Button>();
            btnBottomLeft = transform.Find("btnBottomLeft").GetComponent<Button>();
        }
//---Auto Generate Code End---
		
		void Start()
		{
		
		}
	}
}