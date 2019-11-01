using UnityEngine;
using System.Collections;
using LuaInterface;

public class ComponentConfig : IConfigItem
{
    public int id;
    public string name;
    public string brief;
    //主题 用string是有可能需要拆分
    public string theme_id;
    //组
    public int group_id;
    //游戏模式
    public string game_mode;

    public string prefab_name;
    public string icon;

    //拷贝
    public int copy_allow;
    //删除
    public int delete_allow;
    //旋转
    public int rotate_zoom_allow;
    
    //是否允许手指模式
    public int finger_allow;

    //是否是编辑器模式独用
    public int GM_only;

    public int GetKey()
    {
        return this.id;
    }

    public void SetKey(int key)
    {
        this.id = key;
    }

    public void CreateByLuaTable(LuaTable luaTable)
    {
        this.name = NHelper.ParseObjectToString(luaTable["name"]);
        this.brief = NHelper.ParseObjectToString(luaTable["brief"]);
        this.theme_id = NHelper.ParseObjectToString(luaTable["theme_id"]);
        this.group_id = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["group_id"]));
        this.game_mode = NHelper.ParseObjectToString(luaTable["game_mode"]);
        this.prefab_name = NHelper.ParseObjectToString(luaTable["prefab_name"]);
        this.icon = NHelper.ParseObjectToString(luaTable["icon"]);

        this.copy_allow = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["copy_allow"]));
        this.delete_allow = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["delete_allow"]));
        this.rotate_zoom_allow = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["rotate_zoom_allow"]));

        this.finger_allow = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["finger_allow"]));

        this.GM_only = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["GM_only"]));
    }


}
