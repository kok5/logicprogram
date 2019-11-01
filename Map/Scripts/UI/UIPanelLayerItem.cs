using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MapEditor
{
    public class UIPanelLayerItem : MonoBehaviour
    {
        public Button btnDelete;
        public Button btnMoveDown;
        public Button btnMoveUp;
        public Toggle toggleVisible;

        public Button btnLayerClick;

        public GameObject objSelectedBkg;

        public Text txtName; 

        private int layerIndex = -1;
        private LayerType layerType = LayerType.Terrain;

         public bool InitData(MapEditorStroageData.MapLayerData data)
        {
            layerIndex = data.layerIndex;

            if (layerIndex == EditorLayerMgr.TERRAIN_LAYER_INDEX)
                btnDelete.gameObject.SetActive(false);

            if (EditorLayerMgr.ins.curEditLayer == layerIndex)
                objSelectedBkg.SetActive(true);
            else
                objSelectedBkg.SetActive(false);

            
            if (EditorLayerMgr.ins.IsTerrainLayer(layerIndex))
            //if (layerIndex == EditorLayerMgr.TERRAIN_LAYER_INDEX)
            {
                txtName.text = "地形层" + (layerIndex- EditorLayerMgr.TERRAIN_LAYER_INDEX + 1).ToString();
            }
            else if (layerIndex < EditorLayerMgr.TERRAIN_LAYER_INDEX)
            {
                if (txtName.text == "")
                    txtName.text = "背景层" + layerIndex.ToString();
            }
            else
            {
                if (txtName.text == "")
                    txtName.text = "前景层" + (layerIndex - EditorLayerMgr.TERRAIN_LAYER_INDEX - 20 + 1).ToString();
            }

            return true;
        }

        // Start is called before the first frame update
        void Start()
        {

            btnDelete.onClick.AddListener(OnClickBtnDelete);
            btnMoveUp.onClick.AddListener(OnClickBtnMoveUp);
            btnMoveDown.onClick.AddListener(OnClickBtnMoveDown);

            if (btnLayerClick != null)
            {
                btnLayerClick.onClick.AddListener(() =>
                {
                    EditorLayerMgr.ins.SelectLayer(layerIndex);
                }
                    );
            }


            toggleVisible.onValueChanged.AddListener((bool isOn) => { OnToggleClick(toggleVisible, isOn); });
        }

        //public Button btnDelete;
        //public Button btnMoveDown;
        //public Button btnMoveUp;
        //public Toggle toggleVisible;

        //public Button btnLayerClick;

        //public GameObject objSelectedBkg;

        //public Text txtName;

        private void OnClickBtnDelete()
        {
            if (layerIndex != -1)
            {
                EditorLayerMgr.ins.DeleteLayer(layerIndex);
            }

        }


        private void OnClickBtnMoveUp()
        {
            int upLayerIndex = EditorLayerMgr.ins.GetUpLayerIndex(layerIndex);
            if (upLayerIndex != -1)
            {
                EditorLayerMgr.ins.SwitchLayer(layerIndex, upLayerIndex);
                EditorLayerMgr.ins.SelectLayer(upLayerIndex);
            }
        }

        private void OnClickBtnMoveDown()
        {
            int downLayerIndex = EditorLayerMgr.ins.GetDownLayerIndex(layerIndex);
            if (downLayerIndex != -1)
            {
                EditorLayerMgr.ins.SwitchLayer(layerIndex, downLayerIndex);
                EditorLayerMgr.ins.SelectLayer(downLayerIndex);
            }
        }

        private void OnClickBtnLayerClick()
        {
            if (EditorLayerMgr.ins.curEditLayer != layerIndex)
            {
                UIPanelLayer.ins.OnUnSelectLayer(EditorLayerMgr.ins.curEditLayer);
                objSelectedBkg.SetActive(true);
                EditorLayerMgr.ins.curEditLayer = layerIndex;
            }
        }



        private void OnToggleClick(Toggle toggle, bool isOn)
        {
            if (isOn)
            {
                Debug.Log("Layer: " + layerIndex + " On");
                EditorLayerMgr.ins.ShowLayer(layerIndex);
            }
            else
            {
                Debug.Log("Layer: " + layerIndex + " Off");
                EditorLayerMgr.ins.HideLayer(layerIndex);
            }
        }


        public void OnUnSelect()
        {
            objSelectedBkg.SetActive(false);
        }

    }

}
