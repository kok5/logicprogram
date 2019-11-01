using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelMapEditorParkour : MonoBehaviour
    {
        private Vector3 originCameraPos;
        private UIPanelMapEditor uiMainMapPanel;

		//---Auto Generate Code Start---
        //自动声明变量
        //描述
        private Button btn_width_left;
        //描述
        private Button btn_width_right;
        //描述
        private Button btn_height_up;
        //描述
        private Button btn_height_down;
        //描述
        private InputField InputField;
        //描述
        private Button btn_left;
        //描述
        private Button btn_right;
        //描述
        private Button btn_up;
        //描述
        private Button btn_down;
        //描述
        private Button btn_reset;
        void Awake() 
        {
            //生成自动绑定代码

            btn_width_left = transform.Find("WidthHeight/btn_width_left").GetComponent<Button>();
            if (btn_width_left != null)
                btn_width_left.onClick.AddListener(OnClickWidthLeft);

            btn_width_right = transform.Find("WidthHeight/btn_width_right").GetComponent<Button>();
            if (btn_width_right != null)
                btn_width_right.onClick.AddListener(OnClickWidthRight);

            btn_height_up = transform.Find("WidthHeight/btn_height_up").GetComponent<Button>();
            if (btn_height_up != null)
                btn_height_up.onClick.AddListener(OnClickHeightUp);

            btn_height_down = transform.Find("WidthHeight/btn_height_down").GetComponent<Button>();
            if (btn_height_down != null)
                btn_height_down.onClick.AddListener(OnClickHeightDown);

            InputField = transform.Find("WidthHeight/InputField").GetComponent<InputField>();

            btn_left = transform.Find("CameraView/btn_left").GetComponent<Button>();
            if (btn_left != null)
                btn_left.onClick.AddListener(OnClickLeft);

            btn_right = transform.Find("CameraView/btn_right").GetComponent<Button>();
            if (btn_right != null)
                btn_right.onClick.AddListener(OnClickRight);

            btn_up = transform.Find("CameraView/btn_up").GetComponent<Button>();
            if (btn_up != null)
                btn_up.onClick.AddListener(OnClickUp);

            btn_down = transform.Find("CameraView/btn_down").GetComponent<Button>();
            if (btn_down != null)
                btn_down.onClick.AddListener(OnClickDown);

            btn_reset = transform.Find("CameraView/btn_reset").GetComponent<Button>();
            if (btn_reset != null)
                btn_reset.onClick.AddListener(OnClickReset);

            this.OnAwake();
        }
//---Auto Generate Code End---


        void OnAwake()
        {
            ins = this;

            InputField.onEndEdit.AddListener(delegate
            {
                float.TryParse(InputField.text, out inputNumber);
            });
        }

        void OnClickLeft()
        {
            this.OnClickCameraLeft();
        }
        void OnClickRight()
        {
            this.OnClickCameraRight();
        }
        void OnClickUp()
        {
            this.OnClickCameraUp();
        }
        void OnClickDown()
        {
            this.OnClickCameraDown();
        }
        void OnClickReset()
        {
            this.OnClickCameraReset();
        }

		
		void Start()
		{

            if (MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Parkour
		             || MapEditor.MapEditorConfig.CurrentMapGameMode == MapGameMode.Share)
		    {
		        main_camera = Camera.main;
		        obj_MapEdge = GameObject.Find("MapEdge").gameObject;
		        background = GameObject.FindObjectOfType<SceneLevelBackwardsRenderer>();
		        Reset();

		        originCameraPos = main_camera.transform.position;
		    }

		    uiMainMapPanel = MapEditorUIMgr.ins.GetPanel<UIPanelMapEditor>();
		}


#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                this._panel.SetActive(!this._panel.activeSelf);
            }
            //摄像机控制
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.OnClickCameraLeft();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.OnClickCameraRight();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.OnClickCameraUp();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.OnClickCameraDown();
            }
        }
#endif
        //从data 里面恢复 edgebox位置
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
//#if UNITY_EDITOR
        // 地图大小 控制
        public void OnClickWidthLeft()
        {
            var pos = obj_MapEdge.transform.Find("left").transform.position;
            if (Input.GetKey(KeyCode.Minus))
            {
                pos.z -= (2 * inputNumber);
            }
            else
            {
                pos.z += (2 * inputNumber);
            }

            if (pos.z >= 24f)
            {
                MapEditor.MapEditorStroageData.edgebox_left = pos.z;
                obj_MapEdge.transform.Find("left").transform.position = pos;
            }

        }
        public void OnClickWidthRight()
        {
            var pos = obj_MapEdge.transform.Find("right").transform.position;
            if (Input.GetKey(KeyCode.Minus))
            {
                pos.z += (2 * inputNumber);
            }
            else
                pos.z -= (2 * inputNumber);

            if (pos.z <= -24f)
            {
                MapEditor.MapEditorStroageData.edgebox_right = pos.z;
                obj_MapEdge.transform.Find("right").transform.position = pos;
            }
        }
        public void OnClickHeightUp()
        {
            var pos = obj_MapEdge.transform.Find("up").transform.position;
            if (Input.GetKey(KeyCode.Minus))
            {
                pos.y -= (2 * inputNumber);
            }
            else
            {
                pos.y += (2 * inputNumber);
            }

            if (pos.y >= 22f)
            {
                MapEditor.MapEditorStroageData.edgebox_up = pos.y;
                obj_MapEdge.transform.Find("up").transform.position = pos;
            }
        }
        public void OnClickHeightDown()
        {
            var pos = obj_MapEdge.transform.Find("down").transform.position;
            if (Input.GetKey(KeyCode.Minus))
            {
                pos.y += (2 * inputNumber);
            }
            else
                pos.y -= (2 * inputNumber);

            if (pos.y <= -22f)
            {
                MapEditor.MapEditorStroageData.edgebox_down = pos.y;
                obj_MapEdge.transform.Find("down").transform.position = pos;
            }
        }
        //摄像机位置控制
        public void OnClickCameraLeft()
        {
            var pos = this.main_camera.transform.position;
            pos.z += (5 * inputNumber);
            this.main_camera.transform.position = pos;
            var pos_background = background.transform.position;
            pos_background.z = pos.z;
            pos_background.y = pos.y;
            background.enabled = false;
            background.transform.position = pos_background;

            uiMainMapPanel.AjustGridPos(this.main_camera.transform.position - this.originCameraPos);
        }
        public void OnClickCameraRight()
        {
            var pos = this.main_camera.transform.position;
            pos.z -= (5 * inputNumber);
            this.main_camera.transform.position = pos;
            var pos_background = background.transform.position;
            pos_background.z = pos.z;
            pos_background.y = pos.y;
            background.enabled = false;
            background.transform.position = pos_background;

            uiMainMapPanel.AjustGridPos(this.main_camera.transform.position - this.originCameraPos);
        }
        public void OnClickCameraUp()
        {
            var pos = this.main_camera.transform.position;
            pos.y += (5 * inputNumber);
            this.main_camera.transform.position = pos;
            var pos_background = background.transform.position;
            pos_background.z = pos.z;
            pos_background.y = pos.y;
            background.enabled = false;
            background.transform.position = pos_background;

            uiMainMapPanel.AjustGridPos(this.main_camera.transform.position - this.originCameraPos);
        }
        public void OnClickCameraDown()
        {
            var pos = this.main_camera.transform.position;
            pos.y -= (5 * inputNumber);
            this.main_camera.transform.position = pos;
            var pos_background = background.transform.position;
            pos_background.z = pos.z;
            pos_background.y = pos.y;
            background.enabled = false;
            background.transform.position = pos_background;

            uiMainMapPanel.AjustGridPos(this.main_camera.transform.position - this.originCameraPos);
        }
        //复位
        public void OnClickCameraReset()
        {
            var pos = new Vector3(-10000, 0, 0);
            this.main_camera.transform.position = pos;
            var pos_background = background.transform.position;
            pos_background.z = pos.z;
            pos_background.y = pos.y;
            background.enabled = false;
            background.transform.position = pos_background;

            uiMainMapPanel.ResetGridPos();
        }

        float inputNumber = 1;
//#else
    // 地图大小 控制
    //public void OnClickWidthLeft()
    //{
    //}
    //public void OnClickWidthRight()
    //{
    //}
    //public void OnClickHeightUp()
    //{
    //}
    //public void OnClickHeightDown()
    //{
    //}
    ////摄像机位置控制
    //public void OnClickCameraLeft()
    //{
    //}
    //public void OnClickCameraRight()
    //{
    //}
    //public void OnClickCameraUp()
    //{
    //}
    //public void OnClickCameraDown()
    //{
    //}
    ////复位
    //public void OnClickCameraReset()
    //{
    //}

//#endif


        public static UIPanelMapEditorParkour ins = null;
        [SerializeField]
        GameObject _panel;

        Camera main_camera;
        GameObject obj_MapEdge;
        SceneLevelBackwardsRenderer background = null;
    }
}