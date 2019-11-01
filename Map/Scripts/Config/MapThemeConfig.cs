using UnityEngine;
using System.Collections;
using LuaInterface;

public class MapThemeConfig : IConfigItem
{
    public int id;
    public string name;
    public string brief;

    public int bg_effect;
    public string weapon_id;

    public string GMweapon_id;

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
        this.bg_effect = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["bg_effect"]));
        this.weapon_id = NHelper.ParseObjectToString(luaTable["weapon_id"]);
        this.GMweapon_id = NHelper.ParseObjectToString(luaTable["GMweapon_id"]);
    }


}
