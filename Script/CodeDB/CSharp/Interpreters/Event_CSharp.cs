/****************************************************************************

Functions for interpreting c# code for blocks.

Copyright 2019 gxpack@qq.com
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


    [CodeInterpreter(BlockType = "event_touch")]
    public class Event_Touch_Cmdtor : VoidCmdtor
    {
        protected override void Execute(Block block)
        {
            CSharp.Interpreter.RunningWorkspace = block.Workspace;
            Base.Events.ins.Add("event_touch", "event1", (object xx) =>
            {
                Workspace wp = xx as Workspace;
                if (wp != null)
                {
                    UnityEngine.Debug.LogFormat("<color=green>{0}.</color>", "Event传送workspace");

                    CSharp.Interpreter.OnEventFired(wp);
                    CSharp.Interpreter.RunningWorkspace = null;
                }
                

                return true;
            });

            
        }
    }
}
