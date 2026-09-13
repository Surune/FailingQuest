using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FailingQuest.Combat
{
    public class CombatUnitView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform Rect;
        public Image Portrait;
        public Image HealthFill;
        public Image StressFill;
        public Image Highlight;
        public TMP_Text NameText;
        public TMP_Text HealthText;
        public TMP_Text StatusText;
        public TMP_Text FloatingText;
        public TMP_Text TargetMarker;
        public Button Button;
        public RankBattleController Controller;
        public int Id;

        public void OnPointerEnter(PointerEventData eventData) => Controller.Inspect(Id);
        public void OnPointerExit(PointerEventData eventData) => Controller.ClearInspection();
    }
}
