// (c) 2016-2024 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using AudioStream;
using AudioStreamSupport;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioStreamDemo
{
    //namespace AudioStreamDemo
    [ExecuteInEditMode]
    public class AudioStreamListenerDemo : MonoBehaviour
    {
        [SerializeField] AudioStreamListener audioStreamListener1;
        [SerializeField] AudioStreamListener audioStreamListener2;
        List<FMOD_SystemW.OUTPUT_DEVICE_INFO> availableOutputs;
        /// <summary>
        /// User selected audio output driver id
        /// </summary>
        int selectedOutput1 = 0; // 0 is system default
        int previousSelectedOutput1 = -1;
        int selectedOutput2 = 0; // 0 is system default
        int previousSelectedOutput2 = -1;

        IEnumerator Start()
        {
            while (!this.audioStreamListener1.ready
                || !this.audioStreamListener2.ready
                )
                yield return null;

            this.availableOutputs = FMOD_SystemW.AvailableOutputs(LogLevel.INFO, "AudioStreamListenerDemo", null);
        }

        Vector2 scrollPosition1 = Vector2.zero, scrollPosition2 = Vector2.zero;
        void OnGUI()
        {
            AudioStreamDemoSupport.OnGUI_GUIHeader(this.audioStreamListener1.fmodVersion);

            var color = GUI.color;
            GUI.color = Color.yellow;

            GUILayout.Label("Two sound sources placed near the middle of the scene have narrow emitting sound fields facing in their foward direction,", AudioStreamSupport.UX.guiStyleLabelNormal);
            GUILayout.Label("(left one plays network radio using AudioStream component, right one plays looping Unity AudioClip)", AudioStreamSupport.UX.guiStyleLabelNormal);
            GUILayout.Label("There are two audio listeners - you can select output device of each one (both receive audio of both audio sources)", AudioStreamSupport.UX.guiStyleLabelNormal);
            GUILayout.Label("you can also move the left one around the scene  using W | A | S | D and mouse, press 'R' to reset its position", AudioStreamSupport.UX.guiStyleLabelNormal);

            // wait until outputs list is retrieved
            if (this.availableOutputs == null)
                return;

            GUI.color = color;

            using (new GUILayout.HorizontalScope())
            {
                // outputs for listener 1
                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(Screen.width / 3)))
                {
                    this.scrollPosition1 = GUILayout.BeginScrollView(this.scrollPosition1, new GUIStyle());

                    GUILayout.Label("Listener1 Available outputs:", AudioStreamSupport.UX.guiStyleLabelNormal);

                    this.selectedOutput1 = GUILayout.SelectionGrid(this.selectedOutput1, this.availableOutputs.Select((output, index) => string.Format("[Output #{0}]: {1}", index, output.name)).ToArray()
                        , 1, AudioStreamSupport.UX.guiStyleButtonNormal);

                    // GUILayout.Label(string.Format("-- user requested {0}, running on {1}", this.audioStreamListener1.outputDevice.name, this.audioStreamListener1.RuntimeOutputDriverID), AudioStreamSupport.UX.guiStyleLabelNormal);

                    if (this.selectedOutput1 != this.previousSelectedOutput1)
                    {
                        if ((Application.isPlaying
                            // Indicate correct device in the list, but don't call output update if it was not due user changing / clicking it
                            // && Event.current.type == EventType.Used
                            )
                            )
                        {
                            this.audioStreamListener1.SetOutput(this.selectedOutput1);
                        }

                        this.previousSelectedOutput1 = this.selectedOutput1;
                    }

                    GUILayout.EndScrollView();

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Listener1 Master volume:");
                        this.audioStreamListener1.masterVolume = GUILayout.HorizontalSlider(this.audioStreamListener1.masterVolume, 0f, 1f);
                        GUILayout.Label(string.Format("{0:F2}", this.audioStreamListener1.masterVolume));
                    }
                }

                // screen middle
                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(Screen.width / 3)))
                {
                    GUILayout.Label("", AudioStreamSupport.UX.guiStyleLabelNormal);
                }

                // outputs for listener 2
                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(Screen.width / 3)))
                {
                    this.scrollPosition2 = GUILayout.BeginScrollView(this.scrollPosition2, new GUIStyle());

                    GUILayout.Label("Listener2 Available outputs:", AudioStreamSupport.UX.guiStyleLabelNormal);

                    this.selectedOutput2 = GUILayout.SelectionGrid(this.selectedOutput2, this.availableOutputs.Select((output, index) => string.Format("[Output #{0}]: {1}", index, output.name)).ToArray()
                        , 1, AudioStreamSupport.UX.guiStyleButtonNormal);

                    // GUILayout.Label(string.Format("-- user requested {0}, running on {1}", this.audioStreamListener1.outputDevice.name, this.audioStreamListener1.RuntimeOutputDriverID), AudioStreamSupport.UX.guiStyleLabelNormal);

                    if (this.selectedOutput2 != this.previousSelectedOutput2)
                    {
                        if ((Application.isPlaying
                            // Indicate correct device in the list, but don't call output update if it was not due user changing / clicking it
                            && Event.current.type == EventType.Used
                            )
                            )
                        {
                            this.audioStreamListener2.SetOutput(this.selectedOutput2);
                        }

                        this.previousSelectedOutput2 = this.selectedOutput2;
                    }

                    GUILayout.EndScrollView();

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Listener2 Master volume:");
                        this.audioStreamListener2.masterVolume = GUILayout.HorizontalSlider(this.audioStreamListener2.masterVolume, 0f, 1f);
                        GUILayout.Label(string.Format("{0:F2}", this.audioStreamListener2.masterVolume));
                    }
                }
            }
        }
    }
}