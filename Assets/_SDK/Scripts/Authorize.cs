using System.Collections;
using System.Collections.Generic;
using InstantGamesBridge;
using InstantGamesBridge.Modules.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SDK
{
    public class Authorize : MonoBehaviour
    {
        [Header("All about Authorization")]
        [SerializeField] private bool setAuthVK = false;
        [SerializeField] private bool setAuthYG = false;
        [SerializeField] private bool yandexScopes = true; // Запросить доступ к имени и фото, по умолчанию = true

        public void Authorization()
        {
            if ((setAuthVK && Bridge.platform.id == "vk") || (setAuthYG && Bridge.platform.id == "yandex"))
            {
                if (Bridge.player.isAuthorized && !Bridge.player.isAuthorizationSupported)
                {
                    Debug.LogWarning($"Player is authorized or authorization is not supported");
                    return;
                }
                
                AuthorizeYandexOptions
                    authorizeYandexOptions = new AuthorizeYandexOptions(yandexScopes); // Необязательно
                Bridge.player.Authorize(
                    success =>
                    {
                        if (success)
                        {
                            // Success
                            Debug.LogAssertion($"Authorize success: {success}");
                        }
                        else
                        {
                            // Error
                            Debug.LogError($"Authorize success: {success}");
                        }
                    },
                    authorizeYandexOptions);
            }
        }

        /// <summary>
        /// Ставим первое фотография профиля игрока
        /// </summary>
        /// <param name="playerImage">Фото, которое нужно заменить</param>
        public void SetFirstPlayerPhoto(Image playerImage)
        {
            if (Bridge.player.photos.Count > 0)
            {
                string url = Bridge.player.photos[0];
                StartCoroutine(LoadPhoto(url, playerImage));
            }
            else
            {
                Debug.LogWarning("The player has no photo");
            }
        }

        private IEnumerator LoadPhoto(string url, Image playerImage)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                playerImage.sprite = sprite;
            }
        }
        
        /// <summary>
        /// Вставляет id игрока в текст
        /// </summary>
        /// <param name="text">Объект TextMeshProUGUI</param>
        public void SetPlayerId(TextMeshProUGUI tmp)
        {
            string id = Bridge.player.id;
            if (id == null)
            {
                Debug.LogWarning("Can't find id");
                return;
            }
            
            tmp.text = id;
        }

        /// <summary>
        /// Вставляет имя игрока в текст
        /// </summary>
        /// <param name="text">Объект TextMeshProUGUI</param>
        public void SetPlayerName(TextMeshProUGUI tmp)
        {
            string playerName = Bridge.player.name;
            if (playerName == null)
            {
                Debug.LogWarning("Can't find name");
                return;
            }
            
            tmp.text = playerName;
        }
    }
}
