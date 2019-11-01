using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class Text2Image : MonoBehaviour
{
    [SerializeField]
    Camera camera;
    [SerializeField]
    TextMesh txtContent;


    //step 1
    public static void AsyncGetTextImage(string uuid, string text, VoidFuncObject cb_sprite, int width = 1136, int height = 640)
    {
        if (string.IsNullOrEmpty(uuid) || width <= 0 || height <= 0)
        {
            if (cb_sprite != null)
            {
                try
                {
                    cb_sprite(null);
                }
                catch (Exception e)
                {

                }
            }
            return;
        }
        _tasks.Enqueue(new TextImageTask
        {
            uuid = uuid,
            cb_sprite = cb_sprite,
            width = width,
            height = height,
            text = text,
        });
        if (_tasks.Count >= 1)
        {
            CheckTask();
        }
        else
        {
        }
    }


    void Awake()
    {
        is_running = true;
    }

    //Step 3
    void Start()
    {
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();

        if (_tasks.Count <= 0)
        {
            Application.UnloadLevel("text2image");
            //yield break;
        }
        TextImageTask task = _tasks.Dequeue();
        if (task == null)
        {
            Application.UnloadLevel("text2image");
           // yield break;
        }

        is_running = true;

        txtContent.text =  task.text;
        //StartCoroutine(BeingText2Image(task.cb_sprite, task.width, task.height));

        BeingText2Image(task.cb_sprite, task.width, task.height);

    }

    static bool is_running = false;
    static Queue<TextImageTask> _tasks = new Queue<TextImageTask>();

    class TextImageTask
    {
        public string uuid;
        public VoidFuncObject cb_sprite;
        public int width;
        public int height;
        public string text;
    }

    //Step 2
    static void CheckTask()
    {
        //if (is_running)
        //{
        //}
        //else
        {
            if (_tasks.Count <= 0) return;
            SceneMgr.LoadLevelAdditiveAsync("text2image");
        }
    }



    public void BeingText2Image(VoidFuncObject cb, int WIDTH, int HEIGHT)
    {
        //yield return new WaitForEndOfFrame();
        //string file_name = Application.dataPath + "/../Cache" + "/" + "tmp12345678" + "_" + WIDTH + "_" + HEIGHT + ".jpg";
        string file_name = LocalStorageMapCaptureImage.ins.GetRootDirectory() + "/" + "tmp" + StaticData.uuid+ "_" + WIDTH + "_" + HEIGHT + ".jpg";

        //开始捕捉图像
        RenderTexture tex = new RenderTexture(WIDTH, HEIGHT, 20);
        camera.targetTexture = tex;
        camera.Render();
        RenderTexture.active = tex;

        Texture2D tex2d = new Texture2D(WIDTH, HEIGHT);

        tex2d.ReadPixels(new Rect(0, 0, WIDTH, HEIGHT), 0, 0);
        tex2d.Apply();
        byte[] save_data = tex2d.EncodeToJPG();

        File.WriteAllBytes(file_name, save_data);
        RenderTexture.active = null;
        camera.targetTexture = null;
        if (cb != null)
        {
            cb(tex2d);
        }
        else
        {
            GameObject.DestroyImmediate(tex2d);
        }
        GameObject.DestroyImmediate(tex);
        tex2d = null;
        tex = null;

        Application.UnloadLevel("text2image");


    }
}
