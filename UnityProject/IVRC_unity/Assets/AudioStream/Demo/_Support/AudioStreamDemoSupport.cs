// (c) 2016-2024 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using AudioStreamSupport;
using System.Collections;
using UnityEngine;

namespace AudioStreamDemo
{
    public static class AudioStreamDemoSupport
    {
        #region common OnGUI
        public static void OnGUI_GUIHeader(string fmodVersion)
        {
            var fullVersion = AudioStream.About.versionString + AudioStream.About.fmodNotice + (!string.IsNullOrWhiteSpace(fmodVersion) ? " " + fmodVersion : "");
            AudioStreamSupport.UX.OnGUI_Header(fullVersion);

            if (AudioStream.DevicesConfiguration.Instance.ASIO
                && !string.IsNullOrWhiteSpace(fmodVersion)
                )
                GUILayout.Label(string.Format("ASIO enabled, buffer size: {0}", AudioStream.DevicesConfiguration.Instance.ASIO_bufferSize), AudioStreamSupport.UX.guiStyleLabelMiddle);

            if (!string.IsNullOrWhiteSpace(RuntimeBuildInformation.Instance.downloadsOverHTTPMessage)) { GUI.color = Color.magenta; GUILayout.Label(RuntimeBuildInformation.Instance.downloadsOverHTTPMessage); GUI.color = Color.white; }
        }
        #endregion
    }
}