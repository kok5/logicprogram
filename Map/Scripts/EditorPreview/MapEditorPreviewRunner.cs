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
    public class MapEditorPreviewRunner : GameMgrExtensionBase
    {
        public static MapEditorPreviewRunner ins = null;
        GameMgr mgr = null;
        void Awake()
        {
            ins = this;
        }
        public override void OnStart(GameMgr mgr)
        {
            if (!BattleServer.IsNetwork)
            {
                this.StopAllCoroutines();
                this.StartCoroutine(StartWithIsNetworkFalse());

                mgr.topSceneLevelInfo.gameObject.SetActive(true);

                mgr.topSceneLevelInfo.GetComponent<MapSceneRoot>().AutoLoadSyncable(mgr);

            }
            this.mgr = mgr;

        }

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
            short visibleValue = currentCharacterSkin.GetJointVisibleValue();
            controller.SetJointRendererVisible(visibleValue);
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
            //    UIRoot.ins.EndPreView();
            //  UIRoot.ins.StartPreView();
            SceneMgr.LoadLevel("MapEditorPreview");
            SceneMgr.LoadLevelAdditive("MapEditorPreviewRuntime");
        }
        GameObject player = null;
        IEnumerator StartWithIsNetworkFalse()
        {
            yield return new WaitForSecondsRealtime(1.0f);
            if (this.player != null)
            {
                GameObject.DestroyImmediate(this.player);
                this.player = null;
            }
            mgr.GameHasStartForMap = true;
            if (mgr.topSceneLevelInfo == null)
            {
                //没有任何地图信息 那么需要 
                SceneMgr.LoadLevel("GameLogin");
                yield break;
            }
            // play count down
            do
            {
                if (mgr.topSceneLevelInfo.CurrentSerializeMap == null)
                {
                    if (UICountDown.ins != null)
                        UICountDown.ins.Play();
                    break;
                }

                if(mgr.topSceneLevelInfo.CurrentSerializeMap.map_expansion != null) // 大地图
                {
                    mgr.EnableWildMileeFollowCamera();
                    if (UICountDown.ins != null)
                        UICountDown.ins.Play();
                }
                else
                {
                    if(mgr.topSceneLevelInfo.CurrentSerializeMap.MapInfoMode == (int)MapGameMode.Parkour)
                    {
                        mgr.EnableParkourCameraFollowCharacter();
                        mgr.LazyCheckEdgeForParkourCameraSlider();
                        if (UICountDownGo.ins != null)
                            UICountDownGo.ins.Play();
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
            GameObject player1 = null;
            GameObject obj = null;
            if (MiscLoader.ins != null)
            {//修改加载点
                obj = MiscLoader.ins.Load<GameObject>("Game/Character", "Game/Character/Character.prefab");
            }
            else
            {
                player1 = OldResLoader.ins.Load("Prefabs/Game/Character"); ;
                obj = player1;
            }
            obj = GameObject.Instantiate<GameObject>(obj);
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
                info.characterName = StaticData.name;
                info.team_index = PlayerPrefs.GetInt("teamid");
            }

            //默认颜色是黄色 无需手动修改
            /* if (info != null)
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