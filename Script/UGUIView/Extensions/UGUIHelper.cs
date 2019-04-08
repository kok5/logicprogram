﻿/****************************************************************************

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

using UnityEngine;
using UnityEngine.UI;

namespace UBlockly.UGUI
{
    public static class UGUIHelper
    {
        /// <summary>
        /// Calculate the width of UI text
        /// </summary>
        /// <param name="textComponent">the UI.Text component where the text is put into</param>
        /// <param name="text">The text for calculation</param>
        public static int CalculateTextWidth(this Text textComponent, string text)
        {
            int width = 0;
            Font font = textComponent.font;
            int fontSize = textComponent.fontSize;
            font.RequestCharactersInTexture(text, fontSize, textComponent.fontStyle);
            CharacterInfo characterInfo;
            for (int i = 0; i < text.Length; i++)
            {
                font.GetCharacterInfo(text[i], out characterInfo, fontSize);
                width += characterInfo.advance;
            }
            return width;
        }
    }
}
