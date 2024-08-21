﻿// (c) 2016-2024 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using AudioStream;
using AudioStreamSupport;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioStreamDemo
{
    [ExecuteInEditMode()]
    public class ResonanceSourceDemo : MonoBehaviour
    {
        /// <summary>
        /// Demo references
        /// </summary>
        public AudioStream.ResonanceSource resonanceSource;
        List<FMOD_SystemW.OUTPUT_DEVICE_INFO> availableOutputs;
        /// <summary>
        /// User selected audio output driver id
        /// </summary>
        int selectedOutput = 0; // 0 is system default
        int previousSelectedOutput = -1;

        #region UI events

        Dictionary<string, string> streamsStatesFromEvents = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, string>> tags = new Dictionary<string, Dictionary<string, string>>();

        public void OnPlaybackStarted(string goName)
        {
            this.streamsStatesFromEvents[goName] = "playing";
        }

        public void OnPlaybackPaused(string goName, bool paused)
        {
            this.streamsStatesFromEvents[goName] = paused ? "paused" : "playing";
        }
        /// <summary>
        /// Invoked when decoding has finished
        /// </summary>
        /// <param name="goName"></param>
        /// <param name="_"></param>
        public void OnPlaybackStopped(string goName, string _)
        {
            this.streamsStatesFromEvents[goName] = "stopped";
        }

        public void OnTagChanged(string goName, string _key, object _value)
        {
            // care only about 'meaningful' tags
            var key = _key.ToLowerInvariant();

            if (key == "artist" || key == "title")
            {
                // little juggling around dictionaries..

                if (this.tags.ContainsKey(goName))
                    this.tags[goName][_key] = _value as string;
                else
                    this.tags[goName] = new Dictionary<string, string>() { { _key, _value as string } };
            }
        }

        public void OnError(string goName, string msg)
        {
            this.streamsStatesFromEvents[goName] = msg;
        }

        #endregion

        IEnumerator Start()
        {
            while (!this.resonanceSource.ready
                )
                yield return null;

            this.availableOutputs = FMOD_SystemW.AvailableOutputs(LogLevel.INFO, "ResonanceSourceDemo", null);
        }

        Vector2 scrollPosition = Vector2.zero;
        void OnGUI()
        {
            AudioStreamDemoSupport.OnGUI_GUIHeader(this.resonanceSource ? " " + this.resonanceSource.fmodVersion : "");

            GUILayout.Label("Streamed audio is being played via FMOD's provided Google Resonance plugin.");
            GUILayout.Label(">> W/S/A/D/Arrows to move || Left Shift/Ctrl to move up/down || Mouse to look || 'R' to reset listener position <<");

            GUILayout.Label("Press Play to play entered stream.\r\nURL can be pls/m3u/8 playlist, file URL, or local filesystem path (with or without the 'file://' prefix)");
            GUILayout.Label("You can select system output on which it is played");

            GUI.color = Color.yellow;

            foreach (var p in this.streamsStatesFromEvents)
                GUILayout.Label(p.Key + " : " + p.Value, AudioStreamSupport.UX.guiStyleLabelNormal);

            GUI.color = Color.white;

            FMOD.RESULT lastError;
            string lastErrorString = this.resonanceSource.GetLastError(out lastError);

            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUIStyle());

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(Screen.width / 3f * 2)))
                {
                    GUILayout.Label(this.resonanceSource.GetType() + "   ========================================", AudioStreamSupport.UX.guiStyleLabelSmall);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Stream: ", AudioStreamSupport.UX.guiStyleLabelNormal, GUILayout.MaxWidth(Screen.width / 2));
                        this.resonanceSource.url = GUILayout.TextField(this.resonanceSource.url, GUILayout.MaxWidth(Screen.width / 2));
                    }

                    GUILayout.Label(string.Format("State = {0} {1}"
                        , this.resonanceSource.isPlaying ? "Playing" + (this.resonanceSource.isPaused ? " / Paused" : "") : "Stopped"
                        , lastError + " " + lastErrorString
                        )
                        , AudioStreamSupport.UX.guiStyleLabelNormal);

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(string.Format("Volume: {0} dB", Mathf.RoundToInt(this.resonanceSource.gain)), AudioStreamSupport.UX.guiStyleLabelNormal, GUILayout.MaxWidth(Screen.width / 2));
                        this.resonanceSource.gain = GUILayout.HorizontalSlider(this.resonanceSource.gain, -80f, 24f, GUILayout.MaxWidth(Screen.width / 2));
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Stream format: ", AudioStreamSupport.UX.guiStyleLabelNormal, GUILayout.MaxWidth(Screen.width / 2));
                        this.resonanceSource.streamType = (AudioStreamBase.StreamAudioType)ComboBoxLayout.BeginLayout(0, System.Enum.GetNames(typeof(AudioStreamBase.StreamAudioType)), (int)this.resonanceSource.streamType, 10, AudioStreamSupport.UX.guiStyleButtonNormal, GUILayout.MaxWidth(Screen.width / 2));
                    }

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button(this.resonanceSource.isPlaying ? "Stop" : "Play", AudioStreamSupport.UX.guiStyleButtonNormal))
                        if (this.resonanceSource.isPlaying)
                            this.resonanceSource.Stop();
                        else
                            this.resonanceSource.Play();

                    if (this.resonanceSource.isPlaying)
                    {
                        if (GUILayout.Button(this.resonanceSource.isPaused ? "Resume" : "Pause", AudioStreamSupport.UX.guiStyleButtonNormal))
                            if (this.resonanceSource.isPaused)
                                this.resonanceSource.Pause(false);
                            else
                                this.resonanceSource.Pause(true);
                    }

                    GUILayout.EndHorizontal();

                    Dictionary<string, string> _tags;
                    if (this.tags.TryGetValue(this.resonanceSource.name, out _tags))
                        foreach (var d in _tags)
                            GUILayout.Label(d.Key + ": " + d.Value, AudioStreamSupport.UX.guiStyleLabelNormal);


                    ComboBoxLayout.EndAllLayouts();
                }

                using (new GUILayout.VerticalScope(GUILayout.MaxWidth(Screen.width / 3)))
                {
                    GUILayout.Label("Listener Available outputs:", AudioStreamSupport.UX.guiStyleLabelNormal);

                    if (this.availableOutputs != null)
                        this.selectedOutput = GUILayout.SelectionGrid(this.selectedOutput, this.availableOutputs.Select((output, index) => string.Format("[Output #{0}]: {1}", index, output.name)).ToArray()
                            , 1, AudioStreamSupport.UX.guiStyleButtonNormal);

                    // GUILayout.Label(string.Format("-- user requested {0}, running on {1}", this.audioStreamListener1.outputDevice.name, this.audioStreamListener1.RuntimeOutputDriverID), AudioStreamSupport.UX.guiStyleLabelNormal);

                    if (this.selectedOutput != this.previousSelectedOutput)
                    {
                        if ((Application.isPlaying
                            // Indicate correct device in the list, but don't call output update if it was not due user changing / clicking it
                            // && Event.current.type == EventType.Used
                            )
                            )
                        {
                            this.resonanceSource.SetOutput(this.selectedOutput);
                        }

                        this.previousSelectedOutput = this.selectedOutput;
                    }
                }
            }

            GUILayout.Space(40);

            GUILayout.EndScrollView();
        }
    }
}