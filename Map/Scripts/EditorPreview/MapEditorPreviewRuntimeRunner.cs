/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class MapEditorPreviewRuntimeRunner : MonoBehaviour
    {
        void Awake()
        {
            var s = this.GetComponent<MapEditor.RuntimeSerialize>();
            s.LoadFromJson(MapObjectRoot.record_json);
        }
    }
}