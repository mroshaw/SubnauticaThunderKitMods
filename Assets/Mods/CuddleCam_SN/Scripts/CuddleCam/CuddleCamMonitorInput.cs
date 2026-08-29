using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DaftAppleGames.CuddleCam_SN
{
    /// <summary>
    /// Manages player focus and controller navigation for a CuddleCam monitor.
    /// </summary>
    internal class CuddleCamMonitorInput : uGUI_InputGroup, uGUI_IButtonReceiver, IPointerHoverHandler,
        IPointerClickHandler
    {
        private const string HoverTextKey = "CuddleCam Monitor";
        private const float TerminationSqrDistance = 9f;

        [SerializeField] private CanvasGroup headingCanvasGroup;
        [SerializeField] private CanvasGroup buttonControlsCanvasGroup;
        [SerializeField] [Min(0f)] private float controlsFadeDuration = 0.25f;

        private Coroutine controlsFadeCoroutine;
        private Player player;
        private uGUI_NavigableControlGrid navigationGrid;
        private RectTransform rectTransform;

        /// <summary>
        /// Initialises the monitor input group.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            navigationGrid = GetComponent<uGUI_NavigableControlGrid>();
            rectTransform = GetComponent<RectTransform>();
            SetControlsAlpha(0f);
        }

        /// <summary>
        /// Resets the controls when the monitor input group is disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StopControlsFade();
            SetControlsAlpha(0f);
        }

        /// <summary>
        /// Releases monitor focus when the player moves out of interaction range.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (focused && player &&
                (player.transform.position - rectTransform.position).sqrMagnitude >= TerminationSqrDistance)
            {
                Deselect();
            }
        }

        /// <summary>
        /// Gives controller navigation focus to the monitor controls.
        /// </summary>
        public override void OnSelect(bool lockMovement)
        {
            base.OnSelect(lockMovement);

            player = Player.main;
            FadeControlsTo(1f);
            if (navigationGrid)
            {
                GamepadInputModule.current.SetCurrentGrid(navigationGrid);
            }
        }

        /// <summary>
        /// Clears the current player reference when monitor focus is released.
        /// </summary>
        public override void OnDeselect()
        {
            base.OnDeselect();
            player = null;
            FadeControlsTo(0f);
        }

        /// <summary>
        /// Displays the monitor interaction prompt while the pointer is over its UI.
        /// </summary>
        public void OnPointerHover(PointerEventData eventData)
        {
            if (enabled && !selected)
            {
                HandReticle.main.SetText(
                    HandReticle.TextType.Hand,
                    HoverTextKey,
                    true,
                    GameInput.Button.LeftHand);
                HandReticle.main.SetText(
                    HandReticle.TextType.HandSubscript,
                    string.Empty,
                    false,
                    GameInput.Button.None);
                HandReticle.main.SetIcon(HandReticle.IconType.Interact, 1f);
            }
        }

        /// <summary>
        /// Selects the monitor when the player interacts anywhere on its screen.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (enabled && !selected && IsAcceptedToOpenWithButton(eventData.button))
            {
                Select();
            }
        }

        /// <summary>
        /// Releases monitor focus when the player presses the secondary interaction button.
        /// </summary>
        public bool OnButtonDown(GameInput.Button button)
        {
            if (button == GameInput.Button.RightHand)
            {
                Deselect();
                return true;
            }

            return false;
        }

        private void FadeControlsTo(float targetAlpha)
        {
            if (!buttonControlsCanvasGroup && !headingCanvasGroup)
            {
                return;
            }

            StopControlsFade();
            if (!isActiveAndEnabled || controlsFadeDuration <= 0f)
            {
                SetControlsAlpha(targetAlpha);
                return;
            }

            controlsFadeCoroutine = StartCoroutine(FadeControlsCoroutine(targetAlpha));
        }

        private IEnumerator FadeControlsCoroutine(float targetAlpha)
        {
            float buttonStartAlpha = buttonControlsCanvasGroup
                ? buttonControlsCanvasGroup.alpha
                : targetAlpha;
            float headingStartAlpha = headingCanvasGroup
                ? headingCanvasGroup.alpha
                : targetAlpha;
            float elapsedTime = 0f;

            while (elapsedTime < controlsFadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float fadeProgress = Mathf.Clamp01(elapsedTime / controlsFadeDuration);
                if (buttonControlsCanvasGroup)
                {
                    buttonControlsCanvasGroup.alpha = Mathf.Lerp(
                        buttonStartAlpha,
                        targetAlpha,
                        fadeProgress);
                }

                if (headingCanvasGroup)
                {
                    headingCanvasGroup.alpha = Mathf.Lerp(
                        headingStartAlpha,
                        targetAlpha,
                        fadeProgress);
                }

                yield return null;
            }

            SetControlsAlpha(targetAlpha);
            controlsFadeCoroutine = null;
        }

        private void StopControlsFade()
        {
            if (controlsFadeCoroutine == null)
            {
                return;
            }

            StopCoroutine(controlsFadeCoroutine);
            controlsFadeCoroutine = null;
        }

        private void SetControlsAlpha(float alpha)
        {
            if (buttonControlsCanvasGroup)
            {
                buttonControlsCanvasGroup.alpha = alpha;
            }

            if (headingCanvasGroup)
            {
                headingCanvasGroup.alpha = alpha;
            }
        }
    }
}
