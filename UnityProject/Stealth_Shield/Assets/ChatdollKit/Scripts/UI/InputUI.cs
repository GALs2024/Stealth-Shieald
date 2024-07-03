using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ChatdollKit.Dialog;
using ChatdollKit.IO;
using ChatdollKit.LLM.ChatGPT;

namespace ChatdollKit.UI
{
    public class InputUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject chatdollKitObject;
        private DialogController dialogController;
        [SerializeField]
        private SimpleCamera simpleCamera;

        // Input UI
        [SerializeField]
        private InputField requestInput;
        [SerializeField]
        private Image imagePreview;
        [SerializeField]
        private Image imageIcon;
        [SerializeField]
        private GameObject imagePathPanel;
        [SerializeField]
        private InputField imagePathInput;

        private void Start()
        {
            if (chatdollKitObject == null)
            {
                chatdollKitObject = FindObjectOfType<ChatdollKit>()?.gameObject;
                if (chatdollKitObject == null)
                {
                    Debug.LogError("ChatdollKit is not found in this scene.");
                }
            }
            dialogController = chatdollKitObject.GetComponent<DialogController>();

            if (simpleCamera == null)
            {
                simpleCamera = FindObjectOfType<SimpleCamera>();
                if (simpleCamera == null)
                {
                    Debug.LogWarning("SimpleCamera is not found in this scene.");
                }
            }
        }

        // Conversation UI
        public void OnWakeButton()
        {
            _ = dialogController.StartDialogAsync();
        }

        public void OnSubmitRequestInput()
        {
            var inputText = requestInput.text.Trim();
            requestInput.text = string.Empty;
            if (string.IsNullOrEmpty(inputText)) return;

            if (dialogController.OnRequestAsync == null)
            {
#pragma warning disable CS1998
                dialogController.OnRequestAsync = async (request, token) =>
                {
                    if (request.Type == RequestType.Voice)
                    {
                        var imageBytes = GetImageBytes();
                        if (imageBytes != null)
                        {
                            request.Payloads["imageBytes"] = imageBytes;
                            ClearImage();
                        }
                    }
                };
#pragma warning restore CS1998
            }

            if (dialogController.Status == DialogController.DialogStatus.Idling)
            {
                var dialogRequest = new DialogRequest("_", new WakeWord() { Text = inputText, SkipPrompt = true }.CloneWithRecognizedText(inputText), true);
                _ = dialogController.StartDialogAsync(dialogRequest);
            }
            else
            {
                ((IVoiceRequestProvider)dialogController.RequestProviders[RequestType.Voice]).TextInput = inputText;
            }
        }

        // Camera
        public void OnCameraButton()
        {
            if (simpleCamera != null)
            {
                simpleCamera.ToggleCamera();

                var chatGPTService = chatdollKitObject.GetComponent<ChatGPTService>();
                if (chatGPTService != null && chatGPTService.CaptureImage == null)
                {
                    chatGPTService.CaptureImage = async (string source) =>
                    {
                        try
                        {
                            return await simpleCamera.CaptureImageAsync();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error at CaptureImageAsync: {ex.Message}\n{ex.StackTrace}");
                        }
                        return null;
                    };
                }
            }
            else
            {
                Debug.LogWarning("SimpleCamera is not available.");
            }
        }

        // Image UI
        public void OnImageButton()
        {
            ActivateImagePathPanel(!imagePathPanel.activeSelf);
        }

        public void OnSubmitImagePath()
        {
            var path = imagePathInput.text;
            ActivateImagePathPanel(false);

            if (string.IsNullOrEmpty(path))
            {
                // Clear image when path is empty
                imagePreview.sprite = null;
                imagePreview.gameObject.SetActive(false);
                imageIcon.gameObject.SetActive(true);
                return;
            }

            // Load image from file
            var imageBytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);

            // Resize image
            var resizedTexture = ResizeTexture(texture, 640);

            // Set image to preview
            var sprite = Sprite.Create(resizedTexture, new Rect(0.0f, 0.0f, resizedTexture.width, resizedTexture.height), new Vector2(0.5f, 0.5f));
            imagePreview.preserveAspect = true;
            imagePreview.sprite = sprite;
            imageIcon.gameObject.SetActive(false);
            imagePreview.gameObject.SetActive(true);
        }

        public byte[] GetImageBytes()
        {
            if (imagePreview.sprite != null)
            {
                return imagePreview.sprite.texture.EncodeToJPG();
            }
            else
            {
                return null;
            }
        }

        public void ClearImage()
        {
            imagePreview.sprite = null;
            imagePreview.gameObject.SetActive(false);
            imageIcon.gameObject.SetActive(true);
        }

        private void ActivateImagePathPanel(bool activate)
        {
            imagePathInput.text = string.Empty;
            imagePathPanel.SetActive(activate);
            requestInput.enabled = !activate;

            if (activate)
            {
                imagePathInput.Select();
            }
            else
            {
                requestInput.Select();
            }
        }

        private static Texture2D ResizeTexture(Texture2D originalTexture, int maxLength)
        {
            var width = originalTexture.width;
            var height = originalTexture.height;

            if (Mathf.Max(width, height) < maxLength)
            {
                // Use original texture if smaller than the max
                return originalTexture;
            }

            // Calculate the resized size keeping aspect ratio
            var aspect = (float)width / height;
            if (width > height)
            {
                width = maxLength;
                height = Mathf.RoundToInt(maxLength / aspect);
            }
            else
            {
                height = maxLength;
                width = Mathf.RoundToInt(maxLength * aspect);
            }

            // Make resized texture
            var resizedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float u = (float)x / (width - 1);
                    float v = (float)y / (height - 1);
                    resizedTexture.SetPixel(x, y, originalTexture.GetPixelBilinear(u, v));
                }
            }
            resizedTexture.Apply();

            return resizedTexture;
        }
    }
}
