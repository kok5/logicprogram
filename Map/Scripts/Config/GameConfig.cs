using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LuaInterface;
using MapEditor;

public class GameConfig
{
    public static class ConfigName
    {
        public const string COMPONENT_CONFIG_NAME = "TBX.component_config";
        public const string OLD_COMPONENT_INDEX_CONFIG_NAME = "TBX.OldComponentIndex_config";
        public const string MAP_THEME_CONFIG_NAME = "TBX.MapTheme_config";
    }

    private static GameConfig s_instance;
    public static GameConfig instance
    {
        get
        {
            if( s_instance == null )
            {
                s_instance = new GameConfig();
            }
            return s_instance;
        }
    }

    private Dictionary<string, Object> _allConfigDict;
    private Dictionary<string, int> _ComponentName2IndexDict;

    private GameConfig()
    {
        //这里写入k-v
        _allConfigDict = new Dictionary<string, Object>();
        _allConfigDict.Add(ConfigName.COMPONENT_CONFIG_NAME, new Dictionary<int, ComponentConfig>());

        _allConfigDict.Add(ConfigName.OLD_COMPONENT_INDEX_CONFIG_NAME, new Dictionary<int, ComponentConfig>());

        _allConfigDict.Add(ConfigName.MAP_THEME_CONFIG_NAME, new Dictionary<int, ComponentConfig>());

        _ComponentName2IndexDict = new Dictionary<string, int>();
    }

    public void ReloadAllConfig()
    {
        _allConfigDict.Clear();
        _ComponentName2IndexDict.Clear();

        this.InitAllConfig();
    }

    public void InitAllConfig()
    {
        this.InitConfig<ComponentConfig>(LuaMgr.ins.GetTable(ConfigName.COMPONENT_CONFIG_NAME));
        this.InitConfig<OldComponentIndexConfig>(LuaMgr.ins.GetTable(ConfigName.OLD_COMPONENT_INDEX_CONFIG_NAME));
        this.InitConfig<MapThemeConfig>(LuaMgr.ins.GetTable(ConfigName.MAP_THEME_CONFIG_NAME));

        //处理映射关系
        this.ProcessPrefabIdRelation();

        LuaMgr.ins.CheckStack();
    }

    public Dictionary<int, T> GetAllConfig<T>() where T : IConfigItem, new()
    {
        string typeName = typeof(T).Name;
        return ( _allConfigDict[typeName] as Dictionary<int, T>);
    }

    public T GetConfig<T>(int key) where T : IConfigItem, new()
    {
        Dictionary<int, T> allConfig = GetAllConfig<T>();
        if( allConfig.ContainsKey(key) == true )
        {
            return ( T )( allConfig[key] );
        }
        return default(T);
    }

   

    public ComponentConfig GetComponentConfig(int key)
    {
        string typeName = "ComponentConfig";
        Dictionary<int, ComponentConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, ComponentConfig>;
        if (allConfig.ContainsKey(key) )
        {
            return (allConfig[key]);
        }
        return default(ComponentConfig);
    }

    public MapThemeConfig GetMapThemeConfig(int key)
    {
        string typeName = "MapThemeConfig";
        Dictionary<int, MapThemeConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, MapThemeConfig>;
        if (allConfig.ContainsKey(key))
        {
            return (allConfig[key]);
        }
        return default(MapThemeConfig);
    }

    public void ProcessPrefabIdRelation()
    {
        string typeName = "OldComponentIndexConfig";
        Dictionary<int, OldComponentIndexConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, OldComponentIndexConfig>;
        if (allConfig != null)
        {
            foreach (var kvp in allConfig)
            {
                if (!_ComponentName2IndexDict.ContainsKey(kvp.Value.prefab_name))
                {
                    _ComponentName2IndexDict.Add(kvp.Value.prefab_name, kvp.Value.index_id);
                }
            }
        }
    }

    public int GetComponentIdByName(string name)
    {
        int id = 0;
        if (_ComponentName2IndexDict.TryGetValue(name, out id))
            return id;
        else
        {
            UnityEngine.Debug.LogError(" 组件找不到索引， 组件路径: " + name);
            return 0;
        }
            
    }


    /// <summary>
    /// 获取所有分组信息
    /// </summary>
    /// <returns></returns>
    public int[] GetComponentGroupInfo()
    {
        Dictionary<int, int> groupDic = new Dictionary<int, int>();

        string typeName = "ComponentConfig";
        Dictionary<int, ComponentConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, ComponentConfig>;
        if (allConfig != null)
        {
            foreach(var kvp in allConfig)
            {
                if (!groupDic.ContainsKey(kvp.Value.group_id))
                {
                    groupDic.Add(kvp.Value.group_id, kvp.Value.group_id);
                }
            }
        }

        return groupDic.Values.ToArray();

    }

    /// <summary>
    /// 根据分组Id返回所有的组件
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public ComponentConfig[] GetComponentsByGroup(int groupId)
    {
        List<ComponentConfig> list = new List<ComponentConfig>();
        string typeName = "ComponentConfig";
        Dictionary<int, ComponentConfig> allConfig = _allConfigDict[typeName] as Dictionary<int, ComponentConfig>;
        if (allConfig != null)
        {
            foreach (var kvp in allConfig)
            {
                if (kvp.Value.group_id == groupId)
                {
                    //根据主题和模式过滤
                    if (MapEditor.MapEditorConfig.CurrentSelectTheme > 0)
                    {
                        if (MapEditorMgr.ins.IsHasTheme(kvp.Value.theme_id, MapEditor.MapEditorConfig.CurrentSelectTheme))
                            if (MapEditorMgr.ins.IsHasGameMode(kvp.Value.game_mode,
                                (int) MapEditor.MapEditorConfig.CurrentMapGameMode))
                            {
#if UNITY_EDITOR
                                list.Add(kvp.Value);
#else
                                if (kvp.Value.GM_only == 0)
                                {
                                    list.Add(kvp.Value);
                                }
#endif
                            }
                                
                    }
                    
                }
            }
        }

        return list.ToArray();
    }

    private void InitConfig<T>(LuaTable luaTable) where T : IConfigItem, new()
    {
        string typeName = typeof( T ).Name;
        if( _allConfigDict.ContainsKey( typeName ) )
        {
            return;
        }

        try
        {
            bool hasRecord = false;
            Dictionary<int, T> dict = new Dictionary<int, T>();
            LuaDictTable table = luaTable.ToDictTable();
            luaTable.Dispose();
            var iter2 = table.GetEnumerator();
            while (iter2 != null)
            {
                var one = iter2.Current.Key;
                LuaTable content = iter2.Current.Value as LuaTable;
                if (content != null)
                {
                    T configItem = new T();
                    configItem.SetKey(int.Parse(one.ToString()));
                    configItem.CreateByLuaTable(content);
                    if (dict.ContainsKey(configItem.GetKey()))
                    {
                        UnityEngine.Debug.LogError(string.Format("[{0}][{1}]配置表key重复：{2}", typeof(T), one, configItem.GetKey()));
                    }
                    else
                    {
                        hasRecord = true;
                        dict.Add(configItem.GetKey(), configItem);
                    }
                }

                //临时解决读表结束不退出循环
                if ((one == null) && hasRecord)
                {
                    break;
                }
                else
                {
                    iter2.MoveNext();
                }

            }
            _allConfigDict.Add(typeName, dict);
            table.Dispose();
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
     
    }

}