using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class MapEditorUIMgr : MonoBehaviour
    {
        public static MapEditorUIMgr ins = null;
        //创建了的panel
        Dictionary<string, GameObject> _acvtive_panels = new Dictionary<string, GameObject>();

        void Start()
        {
            MapEditorUIMgr.ins.ShowPanel<UIPanelMapEditor>();
            //打开地编界面重置之前选中的组件
            MapEditorInputMgr.ins.currentSelectId = 0;
        }

        List<Transform> _sort_panels = new List<Transform>();
        Transform root = null;
        void Awake()
        {
            ins = this;
            root = transform.Find("Root");
            int count = root.childCount;
            for (int i = 0; i < count; i++)
            {
                _sort_panels.Add(root.GetChild(i));
            }

            UICanvas = this.GetComponent<RectTransform>();
        }

         void OnDestroy()
        {
            if (ins != null)
            {
                ins = null;

                //reset ui 
                foreach (var p in root.GetComponents<MonoBehaviour>())
                {
                    GameObject.DestroyImmediate(p);
                }
                int count = root.childCount;
                for (int i = 0; i < count; i++)
                {
                    var _panel_sort = root.GetChild(i);
                    //clear script
                    foreach (var p in _panel_sort.GetComponents<MonoBehaviour>())
                    {
                        GameObject.DestroyImmediate(p);
                    }

                    //clear child-child
                    while (_panel_sort.childCount > 0)
                    {
                        var _child_child = _panel_sort.GetChild(0);
                        GameObject.DestroyImmediate(_child_child.gameObject);
                    }
                }
                _acvtive_panels.Clear();
                this.root = null;
            }
        }

        public RectTransform UICanvas { get; private set; }



        public void ShowOrCreatePanel(string name, int sort = 2)
        {
            Debug.Log("@@@ShowOrCreatePanel: " + name);
  
            if (string.IsNullOrEmpty(name)) return;
            {
                GameObject panel = null;
                if (_acvtive_panels.TryGetValue(name, out panel) && panel != null)
                {
                    panel.gameObject.SetActive(true);
                    return;
                }
            }
            {
                Debug.Log("@@@LoadPanel: " + name);
                var panel = UIResourcesLoader.ins.LoadPanel(name, true);
                //faild to load panel
                if (panel == null)
                {
                    Debug.LogError("UIMgr faild to load panel" + name);
                    return;
                }

                if (sort >= 0 && sort < _sort_panels.Count)
                {
                    panel.transform.SetParent(_sort_panels[sort], false);
                }
                else
                {
                    if (_sort_panels.Count >= 3)
                    {
                        panel.transform.SetParent(_sort_panels[2], false);
                    }
                    else if (_sort_panels.Count > 0)
                    {
                        panel.transform.SetParent(_sort_panels[0], false);
                    }
                }

                _acvtive_panels[name] = panel;
            }
        }

        public bool DestroyPanel(string name)
        {
            GameObject panel = null;
            if (_acvtive_panels.TryGetValue(name, out panel) && panel != null)
            {
                UIResourcesLoader.ins.DestroyPanel(panel.gameObject);
                return true;
            }
            return false;
        }


        public bool ShowPanel<T>()
        {
            ShowOrCreatePanel(GetClassName<T>());
            return true;
        }

        public T GetPanel<T>()
        {
            var name = GetClassName<T>();
            GameObject panel = null;

            if (_acvtive_panels.TryGetValue(name, out panel) && panel != null)
            {
                return panel.gameObject.GetComponent<T>();
            }
            else
            {
                return default(T);
            }

            
        }

        public void ClosePanel<T>()
        {
            ClosePanel(GetClassName<T>());
        }

        //public bool ShowPanel(string name)
        //{
        //    ShowOrCreatePanel(name);
        //    return true;
        //}

        public bool ClosePanel(string name)
        {
            GameObject panel = null;
            if (_acvtive_panels.TryGetValue(name, out panel) && panel != null)
            {
                panel.gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        public string GetClassName<T>()
        {
            var fullname = typeof(T).ToString();
            int markIndex = fullname.LastIndexOf(".");

            if (markIndex != -1)
            {
                //过滤掉命名空间
                var name = fullname.Substring(markIndex + 1, (fullname.Length - markIndex - 1)); 

                Debug.Log("new class name: " + name);

                return name;
            }
            else
            {
                return fullname;
            }

        }

        // Update is called once per frame

    }
}

