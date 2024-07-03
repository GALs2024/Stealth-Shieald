﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ChatdollKit.LLM
{
    public class LLMServiceBase : MonoBehaviour, ILLMService
    {
        public bool _IsEnabled;
        public virtual bool IsEnabled {
            get {
#if UNITY_WEBGL && !UNITY_EDITOR
                return false;
#else
                return _IsEnabled;
#endif
            }
            set
            {
                _IsEnabled = value;
                if (value == true)
                {
                    OnEnabled?.Invoke();
                }
            }
        }
        [Header("Debug")]
        public bool DebugMode = false;

        [Header("Context configuration")]
        [TextArea(1, 6)]
        public string SystemMessageContent;
        public string ErrorMessageContent;
        public int HistoryTurns = 10;

        public Action OnEnabled { get; set; }

        protected List<ILLMTool> llmTools = new List<ILLMTool>();

        public virtual void AddTool(ILLMTool tool)
        {
            llmTools.Add(tool);
        }

#pragma warning disable CS1998
        public virtual ILLMMessage CreateMessageAfterFunction(string role = null, string content = null, ILLMSession llmSession = null, Dictionary<string, object> arguments = null)
        {
            throw new NotImplementedException("LLMServiceBase.CreateMessageAfterFunction must be implemented");
        }

        public virtual async UniTask<List<ILLMMessage>> MakePromptAsync(string userId, string inputText, Dictionary<string, object> payloads, CancellationToken token = default)
        {
            throw new NotImplementedException("LLMServiceBase.MakePromptAsync must be implemented");
        }

        public virtual async UniTask<ILLMSession> GenerateContentAsync(List<ILLMMessage> messages, Dictionary<string, object> payloads, bool useFunctions = true, int retryCounter = 1, CancellationToken token = default)
        {
            throw new NotImplementedException("LLMServiceBase.GenerateContentAsync must be implemented");
        }
#pragma warning restore CS1998

        protected virtual Dictionary<string, string> ExtractTags(string text)
        {
            var tagPattern = @"\[(\w+):([^\]]+)\]";
            var matches = Regex.Matches(text, tagPattern);
            var result = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    result[key] = value;
                }
            }

            return result;
        }
    }

    public class LLMSession : ILLMSession
    {
        public bool IsResponseDone { get; set; } = false;
        public string StreamBuffer { get; set; }
        public string CurrentStreamBuffer { get; set; }
        public bool IsVisionAvailable { get; set; } = true;
        public ResponseType ResponseType { get; set; } = ResponseType.None;
        public UniTask StreamingTask { get; set; }
        public string FunctionName { get; set; }
        public List<ILLMMessage> Contexts { get; set; }

        public LLMSession()
        {
            IsResponseDone = false;
            StreamBuffer = string.Empty;
            CurrentStreamBuffer = string.Empty;
            ResponseType = ResponseType.None;
            Contexts = new List<ILLMMessage>();
        }
    }

    public class LLMTool : ILLMTool
    {
        public string name { get; set; }
        public string description { get; set; }
        public ILLMToolParameters parameters { get; set; }

        public LLMTool(string name, string description)
        {
            this.name = name;
            this.description = description;
            parameters = new LLMToolParameters();
        }

        public void AddProperty(string key, Dictionary<string, object> value)
        {
            parameters.properties.Add(key, value);
        }
    }

    public class LLMToolParameters : ILLMToolParameters
    {
        public string type { get; set; }
        public Dictionary<string, Dictionary<string, object>> properties { get; set; }

        public LLMToolParameters()
        {
            type = "object";
            properties = new Dictionary<string, Dictionary<string, object>>();
        }
    }
}
