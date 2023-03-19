using System.Collections;
using System.Collections.Generic;
using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using InstantGamesBridge.Modules.Player;
using InstantGamesBridge.Modules.Social;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SDK
{
    /// <summary>
    /// Реклама для ВК и Яндекс игр
    /// </summary>
    public class HelperForInstantGamesBridge: MonoBehaviour
    {

        #region Advertisement

        [Header("All about interstitial video")]
        [SerializeField] private bool showInterstitial = false; // Показывать ли вообще interstitial видео
        [SerializeField] private int delaysSecondsVK = 0; // Задержка между рекламами в ВК (секунды)
        [SerializeField] private int delaySecondsYG = 0; // Задержка между рекламами в Яндекс Играх (секунды)
        [SerializeField] private bool ignoreDelayIntVK = false; // Игнорировать задержку между рекламами в ВК
        [SerializeField] private bool ignoreDelayIntYG = false; // Игнорировать задержку между рекламами в Яндекс играх

        [Header("All about rewarded video")]
        [SerializeField] private bool showRewarded = false; // Показывать ли вообще rewarded видео
        
        /// <summary>
        /// Вернет состояние rewarded видео
        /// </summary>
        /// <returns>По дефолту вернет Closed</returns>
        public RewardedState GetStateRewarded()
        {
            RewardedState stateRewardedVideo = RewardedState.Closed;
            Bridge.advertisement.rewardedStateChanged += state => { stateRewardedVideo = state; };
            return stateRewardedVideo;
        }
        
        /// <summary>
        /// Вернет состояние interstitial видео
        /// </summary>
        /// <returns>По дефолту вернет Closed</returns>
        public InterstitialState GetStateInterstitial()
        {
            InterstitialState stateInterstitialVideo = InterstitialState.Closed;
            Bridge.advertisement.interstitialStateChanged += state => { stateInterstitialVideo = state; };
            return stateInterstitialVideo;
        }

        /// <summary>
        /// Показывает interstitial видео
        /// </summary>
        public void ShowInterstitial()
        {
            if (showInterstitial)
            {
                Bridge.advertisement.SetMinimumDelayBetweenInterstitial(
                    new SetMinimumDelayBetweenInterstitialVkOptions(delaysSecondsVK),
                    new SetMinimumDelayBetweenInterstitialYandexOptions(delaySecondsYG));
                
                Bridge.advertisement.ShowInterstitial(
                    new ShowInterstitialVkOptions(ignoreDelayIntVK),
                    new ShowInterstitialYandexOptions(ignoreDelayIntYG));
            }
            
        }
        
        /// <summary>
        /// Показывает rewarded видео
        /// </summary>
        public void ShowRewarded()
        {
            if (showRewarded)
            {
                Bridge.advertisement.ShowRewarded();
            }
        }

        #endregion

        #region Authorization

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
        
        #endregion

        #region Share

        [Header("All about share")] 
        [SerializeField] private string vkShareLink = "https://vk.com/mewton.games";

        public void ShareLink()
        {
            Bridge.social.Share(
                success => { 
                    if (success)
                    {
                        // Success
                        Debug.LogAssertion($"Share is {success}");
                    }
                    else
                    {
                        // Error
                        Debug.LogError($"Share is {success}");
                    }
                },
                new ShareVkOptions(vkShareLink));
        }
        
        #endregion
    }
}
