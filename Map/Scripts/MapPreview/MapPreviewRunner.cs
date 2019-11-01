/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using Skin;
using UnityEngine;
namespace MapEditor
{
    //浏览地图 打分 主runner
    //TODO把 个人地图数据 缓存在本地 通过时间戳 来标记出来 是有上传成功才更新本地缓存 否则都读取本地缓存 而非服务器请求
    //缓存要注意多账号问题
    public class MapPreviewRunner : GameMgrExtensionBase
    {
        public static MapPreviewRunner ins = null;
        //预览的 地图id 全局静态变量 该场景一加载 即读取该变量 外部 只需要写入 并且 加载该场景即可
        public static string map_preview_uuid = "";

        GameMgr mgr = null;
        //     public static HashTable kv_cache = null;
        //       public static bool just_reload = false;
        //缓存当前自己的地图 数据

        void LoadNext()
        {
            this.StopAllCoroutines();
            StartCoroutine(Download(map_preview_uuid));
        }
        public override void OnStart(GameMgr mgr)
        {
            if (string.IsNullOrEmpty(map_preview_uuid))
            {
                //没有 地图 id 提供预览
                SceneMgr.LoadLevel("GameLogin");
                return;
            }
            this.LoadNext();
            this.mgr = mgr;
            ins = this;
        }

        public static string json = "";
        IEnumerator Download(string name)
        {
            string json = string.Empty;

            yield return MapHttpTask.Download(name, (string json1) =>
                {
                    json = json1;
                }, () =>
                {
                    //地图下载失败  直接回退
                    SceneMgr.LoadLevel("GameLogin");
                }, true);
            if (!string.IsNullOrEmpty(json))
            {
                yield return SceneMgr.LoadLevelAdditiveAsync("MapPreviewRuntime");
           //     mgr.latestLoadedMap = GameObject.FindObjectOfType<MapInfo>();
                var run = GameObject.FindObjectOfType<MapPreviewRuntimeRunner>();
                run.LoadFromJson(json);
                this.StartCoroutine(StartWithIsNetworkFalse());

                mgr.topSceneLevelInfo.gameObject.SetActive(true);

                mgr.topSceneLevelInfo.GetComponent<MapSceneRoot>().AutoLoadSyncable(mgr);

                if (GameMgr.mode == BattleMode.Guide && GuideMgr.ins != null)
                {
                    GuideMgr.ins.TriggerDrama(GuideTriggerType.MapCreate, null);
                    GuideMgr.ins.HideBlackMask();
                }
            }
        }
        public override void OnIsNetworkFalseKillPlayerDone()
        {
            this.StopAllCoroutines();
            this.StartCoroutine(CoroOnIsNetworkFalseKillPlayerDone());
        }
        public IEnumerator CoroOnIsNetworkFalseKillPlayerDone()
        {
            yield return new WaitForSecondsRealtime(1.5f);
            Time.timeScale = 1f;
            //do some thing
            yield return new WaitForSecondsRealtime(0.5f);
            //reload
            SceneMgr.LoadLevel("MapPreview");

            // UIRoot.ins.EndPreView();
            //  UIRoot.ins.StartPreView();
        }
        GameObject player = null;
        void AddSkinToCharacter(GameObject player1)
        {
            CharacterSkin currentCharacterSkin = player1.GetComponent<CharacterSkin>();
            if (currentCharacterSkin != null && StaticData.CanLoadSkin())
            {
                Transform[] amounts = new Transform[(int)SkinPartType.Max];
                var rigbods = player1.transform.Find("Rigidbodies");
                if (rigbods != null)
                {
                    int max = (int)SkinPartType.Max;
                    amounts = new Transform[max];
                    for (int i = 1; i < max; i++)
                    {
                        SkinPartType partType = (SkinPartType)i;
                        amounts[i] = rigbods.Find(partType.ToString());
                        if (amounts[i] == null)
                        {
                            //Debug.LogError("CharacterSkin can not find this part " + partType);
                        }
                    }
                }
                var typetoskin = StaticData.GetSelfSkins();
                for (int i = 1; i < (int)Skin.SkinPartType.Max; i++)
                {
                    if (typetoskin[i] != null)
                    {
                        foreach (var id in typetoskin[i])
                        {
                            currentCharacterSkin.LoadSkinPart(i, id);
                        }
                    }
                }
            }
            var controller = player1.GetComponent<PlayerRootScript>();
            //controller.SetLineRendererVisible(StaticData.LineRendererVisible);
            StaticData.JointRenderVisibleVue = currentCharacterSkin.GetJointVisibleValue();
            controller.SetJointRendererVisible(StaticData.JointRenderVisibleVue);
        }
        IEnumerator StartWithIsNetworkFalse()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player);
                this.player = null;
            }
            mgr.GameHasStartForMap = true;
            // play count down
            do
            {
                if(mgr.topSceneLevelInfo.CurrentSerializeMap == null)
                {
                    if (UICountDown.ins != null)
                        UICountDown.ins.Play();
                    break;
                }

                if (mgr.topSceneLevelInfo.CurrentSerializeMap.map_expansion != null)
                {
                    mgr.EnableWildMileeFollowCamera();
                    if (UICountDown.ins != null)
                    {
                        UICountDown.ins.Play();
                    }
                }
                else
                {
                    if (mgr.topSceneLevelInfo.CurrentSerializeMap.MapInfoMode == (int)MapGameMode.Parkour)
                    {
                        mgr.EnableParkourCameraFollowCharacter();
                        mgr.LazyCheckEdgeForParkourCameraSlider();

                        if (UICountDownGo.ins != null)
                        {
                            UICountDownGo.ins.Play();
                        }
                        this.gameObject.FetchComponent<ParkourGameMgr>();
                    }
                    else
                    {
                        if (UICountDown.ins != null)
                            UICountDown.ins.Play();
                    }
                }
            } while (false);
            //  yield return new WaitForSecondsRealtime(1.5f);

            //create player automatic

            var obj = MiscLoader.ins.LoadAndInstantiate<GameObject>("Game/Character", "Game/Character/Character.prefab");
            this.player = obj;
            obj.SetActive(true);
            //   yield return new WaitForEndOfFrame();
            obj.layer = 20;
            foreach (var p in obj.GetComponentsInChildren<Collider>(true))
            {
                p.gameObject.layer = 20;
            }
            var info = obj.GetComponent<PlayerInfo>();
            if (info != null)
            {
                info.layer = 20;
                info.uuid = StaticData.uuid;
                info.luuid = StaticData.luuid;
                info.characterName = PlayerPrefs.GetString("PlayerName");
                info.team_index = PlayerPrefs.GetInt("teamid");
            }
            //默认颜色是黄色 无需手动修改
            /*   if (info != null)
               {
                   foreach (var p in obj.GetComponentsInChildren<SetLinePositions>())
                   {
                       p.SetColor(info.myColor);
                   }
                   obj.GetComponentInChildren<HeadRenderer>().SetColor(info.myColor,info.id_in_room);
                   //   var sync = obj.GetComponent<SyncablePlayer>();
                   //    GameObject.DestroyObject(sync as Component);
                   info.myLayer = 20;
               }
               else
               {
                   foreach (var p in obj.GetComponentsInChildren<SetLinePositions>())
                   {
                       p.SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
                   }
                   obj.GetComponentInChildren<HeadRenderer>().SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
                   //   var sync = obj.GetComponent<SyncablePlayer>();
                   //    GameObject.DestroyObject(sync as Component);
               }*/
            var controller = obj.GetComponent<PlayerRootScript>();
            controller.SetHasControl();
            if (mgr != null)
            {
                mgr.characterAllList.Add(controller);
            }
            //set player position
            Vector3 target = mgr.topSceneLevelInfo.playerInitedPos[UnityEngine.Random.Range(0, mgr.topSceneLevelInfo.playerInitedPos.Length)].position;
            var rigs = obj.GetComponentsInChildren<Rigidbody>();
            TagPlayerHip hip = obj.GetComponentInChildren<TagPlayerHip>(true);
            var delta = target - hip.transform.position;
            //   obj.transform.position = target;
            foreach (var pp in rigs)
            {
                if (pp == null) continue;//TODO when player remove this coroutine will missing reference
                pp.transform.position += delta;
            }
            AddSkinToCharacter(this.player);
            //TODO why this code do not work ?
            foreach (Rigidbody rig in rigs)
            {
                rig.isKinematic = true;// disable physics              
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSecondsRealtime(1f);

            foreach (Rigidbody rig in rigs)
            {
                rig.isKinematic = false;// disable physics      
            }
            if (GameMgr.mode == BattleMode.Guide && GuideMgr.ins != null)
            {
                GuideMgr.ins.TriggerDrama(GuideTriggerType.PlayerCreate, null);
            }
        }
        void OnDestroy()
        {
            if (ins == this)
            {
                ins = null;
            }
            //do something clean up
            foreach (var p in GameObject.FindObjectsOfType<DestroyWhenSceneChanged>())
            {
                GameObject.DestroyImmediate(p.gameObject);
            }
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player);
                this.player = null;
            }

        }
    }
}