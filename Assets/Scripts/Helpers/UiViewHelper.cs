using System.Collections;
using Enums;
using Services;
using UISystem;
using UnityEngine;

namespace Helpers
{
    public static class UiViewHelper
    {
        public static IEnumerator ActivateHandlerOnStartGame(UIElementBase view, GameMatchService service)
        {
            yield return new WaitUntil(() => service.EGameModeStatus == EGameModeStatus.Play);

            view.BeginShow();
        }
    }
}