/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class PreviewRunner : MonoBehaviour
    {
        GameObject player=null;
        void Start()
        {
            Time.timeScale = 1f;
            return;
            var obj = GameObject.Instantiate<GameObject>(PrefabsMgr.Load("Prefabs/Game/Character"));

            obj.transform.position = Vector3.one;

            obj.SetActive(true);
            obj.layer = 20;
            foreach (var p in obj.GetComponentsInChildren<Collider>(true))
            {
                p.gameObject.layer = 20;
            }

            foreach (var p in obj.GetComponentsInChildren<LineRendererPositionModifier>())
            {
                p.SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
            }
            obj.GetComponentInChildren<PlayerHeadVisual>().SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
            var sync = obj.GetComponent<SyncablePlayer>();
            GameObject.DestroyObject(sync as Component);

            obj.GetComponent<PlayerRootScript>().SetHasControl();
            obj.GetComponent<PlayerInfo>().layer = 20;
            player = obj;
        }
        void OnDestroy()
        {
            GameObject.Destroy(player);
        }
    }
}