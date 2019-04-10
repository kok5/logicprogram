/****************************************************************************

Functions for interpreting c# code for blocks.

Copyright 2016 dtknowlove@qq.com
Copyright 2016 sophieml1989@gmail.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

****************************************************************************/


using System;
using System.Collections;
using System.Linq;

namespace UBlockly
{


    [CodeInterpreter(BlockType = "appearance_show")]
    public class Appearance_Show_Cmdtor : VoidCmdtor
    {
        protected override void Execute(Block block)
        {
            //CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "TEXT", new DataStruct(""));
            //yield return ctor;
            //DataStruct input = ctor.Data; 
            ////todo: 暂时用Debug.Log，后面根据UI输出框再定
            //UnityEngine.Debug.Log("c# print: " + input.ToString());

            //UnityEngine.Debug.Log("c# gameobject name: " + block.Workspace.ObjectRoot.name);

            //UnityEngine.Debug.LogFormat("<color=green>{0}.</color>", "显示！！");

            //CSharp.Interpreter.RunningWorkspace = block.Workspace;

            UnityEngine.Debug.LogFormat("<color=green>{0}.</color>", "appearance_show！！");

        }
    }

    [CodeInterpreter(BlockType = "appearance_hide")]
    public class Appearance_Hide_Cmdtor : VoidCmdtor
    {
        protected override void Execute(Block block)
        {
            //CustomEnumerator ctor = CSharp.Interpreter.ValueReturn(block, "TEXT", new DataStruct(""));
            //yield return ctor;
            //DataStruct input = ctor.Data; 
            ////todo: 暂时用Debug.Log，后面根据UI输出框再定
            //UnityEngine.Debug.Log("c# print: " + input.ToString());

            //UnityEngine.Debug.Log("c# gameobject name: " + block.Workspace.ObjectRoot.name);

            UnityEngine.Debug.LogFormat("<color=green>{0}.</color>", "不要马上看到我！！ 隐藏！！");
        }
    }

    [CodeInterpreter(BlockType = "appearance_distance_get")]
    public class Appearance_Distance_Get_Cmdtor : ValueCmdtor
    {
        protected override DataStruct Execute(Block block)
        {
            UnityEngine.Debug.LogFormat("<color=green>{0}.</color>", "002 触发事件！！！");

            Base.Events.ins.Fire("event_touch", "event1", false, block.Workspace);

            int distance = 1001;
            return new DataStruct(distance);

            //UnityEngine.Debug.LogFormat("<color=green>{0}.</color>", "显示！！");
        }
    }

}
