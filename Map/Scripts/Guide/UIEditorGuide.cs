/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class UIEditorGuide : MonoBehaviour
    {
        public static UIEditorGuide ins = null;
        public Text txt_center;
        public Text txt_tip;

        void Awake()
        {
            ins = this;
        }
        void OnDestroy()
        {
            ins = null;
        }
        void Start()
        {
            this.ShowGuideStep1();
        }


        public void OnClickOk()
        {
            this.SetVisible(false);

        }
        public GameObject hide;

        void SetVisible(bool visible)
        {
            hide.SetActive(visible);
        }
        bool s1 = false;
        bool s2 = false;
        bool s3 = false;

        public void ShowGuideStep1()
        {
            if (s1 || MapEditorMgr.ins.HasPreview) return;
            s1 = true;

            txt_center.text = "教程：顶部点击“画笔”可以在“画笔”与“橡皮”之间切换。“画笔”为添加组件，“橡皮”为删除组件，“手指”为编辑组件，“网格”为辅助对齐，“撤销”可以协助恢复误操作，“运行”可以试玩关卡。底部为各种类型组件展示待选。";

            txt_tip.text = "点击屏幕开始关卡设计";

            this.SetVisible(true);

        }
        public void ShowGuideStep2()
        {
            if (s2 || MapEditorMgr.ins.HasPreview) return;
            s2 = true;

            txt_center.text = "教程：你必须设置4个出生点位置，拖动图中1P，2P,3P,4P位置到你想设置的出生点位即可，尽量不要把出生点位设置在比较尴尬的位置哦";

            txt_tip.text = "点击屏幕开始设置出生点";

            this.SetVisible(true);


        }
        public void ShowGuideStep3()
        {
            if (s3 || MapEditorMgr.ins.HasPreview) return;
            s3 = true;

            txt_center.text = "你可以设置4个枪支掉落点，每个枪支掉落点可以设置0-5把武器掉落（武器掉落时间间隔为10秒）；点击武器掉落点后在底部点击你想要掉落的枪支，按顺序点击1-5把枪即设置完成。";

            txt_tip.text = "点击屏幕开始设置枪支掉落";


            this.SetVisible(true);

        }



    }
}