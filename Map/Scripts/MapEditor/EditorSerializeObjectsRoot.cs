/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{
    public class EditorSerializeObjectsRoot : EditorSerializeBase
    {
        //scene-->map
        public override bool SerializeObject(Serializable.Map map)
        {
            //

            int count = transform.childCount;
            if (count <= 0) return false;

            //是否支持多层
            bool supportMultiLayer = false;
            string name = transform.GetChild(count - 1).gameObject.name;
            if (name.Contains("layer"))
            {
                supportMultiLayer = true;
            }

            if (!supportMultiLayer)
            {
                for (int i = 0; i < count; i++)
                {
                    Serializable.MapObject obj = new Serializable.MapObject();
                    if (obj == null) return false;
                    var child = transform.GetChild(i);
                    if (child.GetComponent<DontSerializeThisToJson>() != null) continue;

                    //如果是隐藏的就不序列化
                    if (!child.gameObject.activeSelf) continue;

                    obj.Fill<GameObject>(child.gameObject);

                    map.objects.Add(obj);
                }
            }
            else//多层地图
            {
                for (int i = 0; i < count; i++) //遍历层
                {
                    var child = transform.GetChild(i);

                    //过滤掉出生点(OneSpawnPoint, OneWeaponSpawnPoint)
                    if (child.GetComponent<DontSerializeThisToJson>() != null) continue;

                    Serializable.MapLayer layer = new Serializable.MapLayer();
                    layer.Fill<GameObject>(child.gameObject);

                    //如果是隐藏的就不序列化
                    if (!child.gameObject.activeSelf) continue;

                    int childchildcount = child.childCount;
                    int layerIndex = 0;
                    int.TryParse(child.gameObject.name.Substring(5), out layerIndex);

                    map.layers.Add(layer);

                    for (int j = 0; j < childchildcount; j++) //遍历层里的物件
                    {

                        Serializable.MapObject obj = new Serializable.MapObject();
                        if (obj == null) return false;
                        var childchild = child.GetChild(j);
                        if (childchild.GetComponent<DontSerializeThisToJson>() != null) continue;

                        //如果是隐藏的就不序列化
                        if (!childchild.gameObject.activeSelf) continue;

                        obj.Fill<GameObject>(childchild.gameObject);

                        map.objects.Add(obj);
                    }

                }
            }

            //需要合并就合并
            if (TerrainMergeMgr.ins.needMerge)
            {
                TerrainMergeMgr.ins.MergeAndUpdate(map);
            }

            //if (count != map.objects.Count) return false;
            return base.SerializeObject(map);//default ok
        }
        //map-->scene
        public override bool DeSerializeObject(Serializable.Map map)
        {
            RuntimeLayerMgr.ins.Clear();
            RuntimeLayerMgr.ins.transform = transform;
            
            foreach (var l in map.layers)
            {
                RuntimeLayerMgr.ins.CreateLayer(l.layerIndex, l.moveFactorX, l.moveFactorY);
            }

            RuntimeLayerMgr.ins.RefreshData();

            //判断版本号
            bool isOldVersion = true;
            if (MapEditorMgr.ins.IsNewMapVersion(map.version))
                isOldVersion = false;

            foreach (var p in map.objects)
            {
                GameObject obj = null;
                if (isOldVersion)
                {
                    obj = MapLoader.ins.LoadMapObjectV1(map.theme, p.prefab);

                    p.transform.rotation = MapEditor.MapEditorMgr.ins.CorrentRotation(p.prefab, map.theme, p.transform.rotation);
                    p.transform.scale = MapEditor.MapEditorMgr.ins.CorrentScale(p.prefab, map.theme, p.transform.scale);

                }
                else
                {
                    int id = int.Parse(p.prefab);
                    obj = MapLoader.ins.LoadMapObjectById(id);
                }
                if (obj == null)
                {
                    if (map.theme >= 1 && map.theme <= 3 || map.theme == 8 || map.theme == 11)
                    {

                    }
                    else
                    {
                        Debug.LogError("Can not find map object=" + map.theme + "  " + p.prefab);
                    }
                }
                else
                {
                    if ((p.layerIndex > 0) && (p.layerIndex <= RuntimeLayerMgr.MAX_LAYER_COUNT))
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, RuntimeLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        p.Emplace<GameObject>(obj);
                    }
                    else
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, transform);
                        p.Emplace<GameObject>(obj);
                    }

                    if (p.decorateIndex > 0)
                    {
                        var path = "change/" + p.decorateIndex.ToString();
                        var dec = obj.transform.Find(path);
                        if (dec != null)
                            dec.gameObject.SetActive(true);
                    }

                }
            }

            ////////////////////////////////////////////////////////////////////
            //纯图形
            foreach (var p in map.terrain)
            {
                GameObject obj = null;
                if (isOldVersion)
                {
                    obj = MapLoader.ins.LoadMapObjectV1(map.theme, p.prefab);
                }
                else
                {
                    int id = int.Parse(p.prefab);
                    obj = MapLoader.ins.LoadMapObjectById(id);
                }
                if (obj == null)
                {
                    if (map.theme >= 1 && map.theme <= 3 || map.theme == 8 || map.theme == 11)
                    {

                    }
                    else
                    {
                        Debug.LogError("Can not find map object=" + map.theme + "  " + p.prefab);
                    }
                }
                else
                {
                    if ((p.layerIndex > 0) && (p.layerIndex <= RuntimeLayerMgr.MAX_LAYER_COUNT))
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, RuntimeLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        p.Emplace<GameObject>(obj);
                    }
                    else
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, transform);
                        p.Emplace<GameObject>(obj);
                    }

                    if (p.decorateIndex > 0)
                    {
                        var path = "change/" + p.decorateIndex.ToString();
                        var dec = obj.transform.Find(path);
                        if (dec != null)
                            dec.gameObject.SetActive(true);
                    }

                    //Terrain禁掉物理
                    var colliders = obj.transform.GetComponentsInChildren<Collider>();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].enabled = false;
                    }

                }
            }


            ////////////////////////////////////////////////////////////////////
            //纯物理(碰撞)
            foreach (var p in map.colliders)
            {
                GameObject obj = null;
                if (isOldVersion)
                {
                    obj = MapLoader.ins.LoadMapObjectV1(map.theme, p.prefab);
                }
                else
                {
                    int id = int.Parse(p.prefab);
                    obj = MapLoader.ins.LoadMapObjectById(id);
                }
                if (obj == null)
                {
                    if (map.theme >= 1 && map.theme <= 3 || map.theme == 8 || map.theme == 11)
                    {

                    }
                    else
                    {
                        Debug.LogError("Can not find map object=" + map.theme + "  " + p.prefab);
                    }
                }
                else
                {
                    if ((p.layerIndex > 0) && (p.layerIndex <= RuntimeLayerMgr.MAX_LAYER_COUNT))
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, RuntimeLayerMgr.ins.GetLayerByIndex(p.layerIndex));
                        p.Emplace<GameObject>(obj);
                    }
                    else
                    {
                        obj = GameObject.Instantiate<GameObject>(obj, transform);
                        p.Emplace<GameObject>(obj);
                    }

                    //Collide禁掉图形显示
                    var renders = obj.transform.GetComponentsInChildren<MeshRenderer>();
                    for (int i = 0; i < renders.Length; i++)
                    {
                        renders[i].enabled = false;
                    }

                }
            }
            return base.DeSerializeObject(map);//default ok
        }
    }

}