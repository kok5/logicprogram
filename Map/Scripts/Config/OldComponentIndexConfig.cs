using UnityEngine;
using System.Collections;
using LuaInterface;

public class OldComponentIndexConfig : IConfigItem
{
    public int id;
    public string prefab_name;
    public int index_id;

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
        this.prefab_name = NHelper.ParseObjectToString(luaTable["prefab_name"]);
        this.index_id = NHelper.ParseInt(NHelper.ParseObjectToString(luaTable["index_id"]));
    }
}
