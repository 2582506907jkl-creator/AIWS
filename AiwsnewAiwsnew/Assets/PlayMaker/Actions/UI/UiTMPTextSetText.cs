
// (c) Copyright HutongGames, LLC 2010-2025. All rights reserved.

using UnityEngine;
using TMPro;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Sets the text value of a TextMeshProUGUI component.")]
    public class UiTMPTextSetText : ComponentAction<TextMeshProUGUI>
    {
        [RequiredField]
        [CheckForComponent(typeof(TextMeshProUGUI))]
        [Tooltip("The GameObject with the TextMeshProUGUI component.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.TextArea)]
        [Tooltip("The text of the TextMeshProUGUI component.")]
        public FsmString text;

        [Tooltip("Reset when exiting this state.")]
        public FsmBool resetOnExit;

        [Tooltip("Repeats every frame.")]
        public bool everyFrame;

        private TextMeshProUGUI tmpText;
        private string originalString;

        public override void Reset()
        {
            gameObject = null;
            text = null;
            resetOnExit = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (UpdateCache(go))
            {
                tmpText = cachedComponent;
            }

            originalString = tmpText != null ? tmpText.text : string.Empty;

            DoSetTextValue();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetTextValue();
        }

        private void DoSetTextValue()
        {
            if (tmpText == null) return;

            tmpText.text = text.Value;
        }

        public override void OnExit()
        {
            if (tmpText == null) return;

            if (resetOnExit.Value)
            {
                tmpText.text = originalString;
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("UISetTMPText", text);
        }
#endif
    }
}
