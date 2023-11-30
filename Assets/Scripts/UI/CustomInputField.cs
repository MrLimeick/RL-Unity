using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RL.UI
{
    public static class CustomInputField
    {
        public static void SetText(this TMPro.TMP_InputField IF, string text)
        {
            if (IF.text != text && !IF.isFocused)
            {
                IF.text = text;
            }
        }
    }
}
