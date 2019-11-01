/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//just for diff  touch is UI or GameObject
public class IsTouchUI : UnityEngine.UI.Graphic, ICancelHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public static bool _IsTouchUI = false;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("_IsTouchUI == true");
        //_IsTouchUI = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("_IsTouchUI == true");
        //_IsTouchUI = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("_IsTouchUI == false");
        //_IsTouchUI = false;
    }
    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("_IsTouchUI == true");
        //_IsTouchUI = true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("_IsTouchUI == false");
        //_IsTouchUI = false;
    }
    public void OnCancel(BaseEventData eventData)
    {
        //Debug.Log("_IsTouchUI == true");
        //_IsTouchUI = true;
    }
}