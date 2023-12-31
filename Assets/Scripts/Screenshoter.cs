﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace RL.UX
{
    public class Screenshoter : MonoBehaviour
    {
        private Screenshoter Instance;

        [RuntimeInitializeOnLoadMethod]
        static void Init() => InstanceObject.InstantiatePrefab("Screenshoter");
        private void Awake() => this.SetInstance(ref Instance, true);

        public static void Take()
        {
            if (!Directory.Exists("Screenshots")) Directory.CreateDirectory("Screenshots");
            ScreenCapture.CaptureScreenshot($"Screenshots/{DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}.png");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12)) Take();
        }
    }
}