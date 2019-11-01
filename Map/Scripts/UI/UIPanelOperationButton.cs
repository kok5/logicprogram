using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIPanelOperationButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public int index;
        private RectTransform curRecTran;

        public void OnBeginDrag(PointerEventData eventData)
        {
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorOperation>().OnBeginChildDrag(index);
        }

        public void OnDrag(PointerEventData eventData)
        {

            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                //curRecTran.position = globalMousePos;
                MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorOperation>().OnChildDrag(index, globalMousePos);
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            MapEditorUIMgr.ins.GetPanel<UIPanelMapEditorOperation>().OnEndChildDrag(index);
        }

        void Awake()
        {
            curRecTran = transform.GetComponent<RectTransform>();
        }

    }
}