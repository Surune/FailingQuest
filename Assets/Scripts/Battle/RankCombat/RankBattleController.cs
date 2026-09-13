using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FailingQuest.Combat
{
    public class RankBattleController : MonoBehaviour
    {
        public const int MaxPlayerCount = 1;
        public const int MaxEnemyCount = 5;
        public CombatAppearance[] Heroes;
        public CombatAppearance[] Enemies;
        public CombatUnitView[] Views;
        public Button[] SkillButtons;
        public Image[] SkillIcons;
        public Sprite[] AbilityIcons;
        public CombatSkillCatalog SkillCatalog;
        public TMP_Text[] SkillLabels;
        public TMP_Text RoundText;
        public TMP_Text TurnText;
        public TMP_Text HeroText;
        public TMP_Text StatsText;
        public TMP_Text SkillText;
        public TMP_Text TargetText;
        public TMP_Text LogText;
        public TMP_Text PromptText;
        public Image HeroPortrait;
        public GameObject ResultPanel;
        public TMP_Text ResultTitle;
        public TMP_Text ResultBody;
        public TMP_Text ContinueLabel;
        public Button ContinueButton;
        public Button PassButton;
        public Button RetreatButton;
        public Button CancelButton;
        public RectTransform Stage;
        public GameObject HelpPanel;
        public Button HelpButton;
        public Button CloseHelpButton;
        public int Seed;
        public CombatModel Model { get; private set; }
        public int SelectedSkill { get; private set; } = -1;
        public bool Busy { get; private set; }
        private bool resultShown;
        private Map.NodeType encounterType = Map.NodeType.Normal;
        private int Reward => encounterType == Map.NodeType.Boss ? 250 : encounterType == Map.NodeType.Elite ? 150 : 100;
        private string EncounterName => encounterType == Map.NodeType.Boss ? "최종 보스" : encounterType == Map.NodeType.Elite ? "엘리트 전투" : "전투";
        private int inspected = -1;
        private readonly List<CombatAppearance> party = new();
        private readonly List<CombatAppearance> appearance = new();

        private void Start()
        {
            var managers = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
            if (managers.Length > 0)
            {
                var map = managers[0].currentMap;
                if (map.userPath.Count > 0) encounterType = map.GetNode(map.userPath.Last()).nodeType;
            }
            if (managers.Length > 0)
                foreach (var type in managers[0].userData.characters.Take(MaxPlayerCount)) party.Add(Heroes.First(h => h.CharacterType == type));
            else
                party.AddRange(Heroes.Take(MaxPlayerCount));
            Model = new CombatModel(Seed == 0 ? System.Environment.TickCount : Seed);
            if (managers.Length > 0) Model.QuestProgress = RunEffects.Progress;
            for (int i = 0; i < party.Count; i++)
            {
                Model.Add(managers.Length > 0 ? RunEffects.Prepare(party[i]) : party[i].Template, false, i + 1);
                if (managers.Length > 0)
                    Model.Units[i].Health = Mathf.CeilToInt(Model.Units[i].Template.Health * managers[0].userData.partyHealth[party[i].CharacterType]);
                appearance.Add(party[i]);
            }
            for (int i = 0; i < Mathf.Min(MaxEnemyCount, Enemies.Length); i++)
            {
                var template = Newtonsoft.Json.JsonConvert.DeserializeObject<CombatTemplate>(Newtonsoft.Json.JsonConvert.SerializeObject(Enemies[i].Template));
                if (encounterType == Map.NodeType.Elite || encounterType == Map.NodeType.Boss)
                {
                    bool boss = encounterType == Map.NodeType.Boss && i == 0;
                    template.Name = (boss ? "폐허의 군주 · " : "정예 · ") + template.Name;
                    template.Health = Mathf.CeilToInt(template.Health * (boss ? 2f : 1.25f));
                    template.Speed += boss ? 2 : 1;
                    foreach (var skill in template.Skills.Where(s => s.Max > 0))
                    {
                        skill.Min += boss ? 2 : 1;
                        skill.Max += boss ? 2 : 1;
                    }
                }
                Model.Add(template, true, i + 1);
                appearance.Add(Enemies[i]);
            }
            for (int i = 0; i < Views.Length; i++)
            {
                Views[i].gameObject.SetActive(i < Model.Units.Count);
                if (i >= Model.Units.Count) continue;
                int id = i;
                Views[i].Id = id;
                Views[i].Controller = this;
                Views[i].Portrait.sprite = appearance[i].Sprite;
                Views[i].Button.onClick.AddListener(() => Target(id));
            }
            for (int i = 0; i < SkillButtons.Length; i++)
            {
                int index = i;
                SkillButtons[i].onClick.AddListener(() => SelectSkill(index));
            }
            PassButton.onClick.AddListener(Pass);
            RetreatButton.onClick.AddListener(Retreat);
            CancelButton.onClick.AddListener(Cancel);
            ContinueButton.onClick.AddListener(Continue);
            HelpButton.onClick.AddListener(() => HelpPanel.SetActive(true));
            CloseHelpButton.onClick.AddListener(() => HelpPanel.SetActive(false));
            ResultPanel.SetActive(false);
            HelpPanel.SetActive(false);
            Model.Record($"{EncounterName} · 원정대가 폐허에 진입했습니다.");
            Advance();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (HelpPanel.activeSelf) HelpPanel.SetActive(false);
                else Cancel();
            }
            if (HelpPanel.activeSelf || Busy || resultShown) return;
            for (int i = 0; i < 4; i++) if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SelectSkill(i);
            if (Input.GetKeyDown(KeyCode.Space)) Pass();
        }

        private bool PlayerTurn => !Busy && !resultShown && Model.AwaitingAction && !Model.Active.Enemy;

        private void Advance()
        {
            SelectedSkill = -1;
            inspected = -1;
            Busy = false;
            if (!Model.Next()) { ShowResult(); return; }
            if (Model.Active.Enemy) { Busy = true; StartCoroutine(EnemyTurn()); }
            Refresh();
        }

        private IEnumerator EnemyTurn()
        {
            yield return new WaitForSeconds(0.85f);
            while (HelpPanel.activeSelf) yield return null;
            int actor = Model.Active.Id;
            var health = Model.Units.Select(u => u.Health).ToArray();
            Model.EnemyAction();
            yield return AnimateAction(actor, health);
            Advance();
        }

        public void SelectSkill(int index)
        {
            if (!PlayerTurn) return;
            var skill = Model.Active.Template.Skills[index];
            if (!Model.CanUse(Model.Active, skill)) return;
            SelectedSkill = index;
            Refresh();
        }

        public void Target(int id)
        {
            if (!PlayerTurn || SelectedSkill < 0) return;
            int actor = Model.Active.Id;
            var health = Model.Units.Select(u => u.Health).ToArray();
            if (!Model.Use(SelectedSkill, id)) return;
            Busy = true;
            SelectedSkill = -1;
            StartCoroutine(FinishAction(actor, health));
        }

        private IEnumerator FinishAction(int actor, int[] health)
        {
            yield return AnimateAction(actor, health);
            Advance();
        }

        private IEnumerator AnimateAction(int actor, int[] health)
        {
            Refresh();
            var rect = Views[actor].Portrait.rectTransform;
            var origin = rect.anchoredPosition;
            float direction = Model.Units[actor].Enemy ? -1 : 1;
            for (int i = 0; i < Model.Units.Count; i++)
            {
                int delta = Model.Units[i].Health - health[i];
                Views[i].FloatingText.text = delta == 0 ? "" : delta > 0 ? $"+{delta}" : delta.ToString();
                Views[i].FloatingText.color = delta > 0 ? Tone(0.4f, 0.85f, 0.65f) : Tone(1f, 0.35f, 0.25f);
            }
            float time = 0;
            while (time < 0.6f)
            {
                time += Time.deltaTime;
                float pulse = Mathf.Sin(Mathf.Clamp01(time / 0.6f) * Mathf.PI);
                rect.anchoredPosition = origin + V2(direction * 30 * pulse, 8 * pulse);
                rect.localScale = V3(direction * (1 + pulse * 0.1f), 1 + pulse * 0.1f, 1);
                yield return null;
            }
            rect.anchoredPosition = origin;
            rect.localScale = V3(direction, 1, 1);
            foreach (var view in Views) view.FloatingText.text = "";
        }

        public void Pass() { if (PlayerTurn && Model.Pass()) Advance(); }
        public void Retreat() { if (PlayerTurn && Model.Retreat()) Advance(); }
        public void Cancel() { SelectedSkill = -1; if (!resultShown) Refresh(); }
        public void Inspect(int id) { inspected = id; RefreshTarget(); }
        public void ClearInspection() { inspected = -1; RefreshTarget(); }

        private void Refresh()
        {
            RoundText.text = $"ROUND  {Model.Round:00}";
            TurnText.text = string.Join("   ›   ", Model.Order.Skip(Mathf.Max(0, Model.Cursor)).Where(u => u.Living).Select(u => u.Name));
            var actor = Model.Active;
            HeroPortrait.sprite = appearance[actor.Id].Sprite;
            HeroText.text = actor.Name + (actor.Enemy ? "  /  적의 차례" : "  /  행동 선택");
            StatsText.text = $"체력  {actor.Health}/{actor.Template.Health}     스트레스  {actor.Stress}/200\n속도  {actor.Speed}     회피  {actor.Dodge}     보호  {actor.Protection}%";
            var targets = SelectedSkill >= 0 ? Model.Targets(actor, actor.Template.Skills[SelectedSkill]) : new List<Combatant>();
            foreach (var unit in Model.Units)
            {
                var view = Views[unit.Id];
                view.gameObject.SetActive(unit.Living);
                float x = unit.Enemy ? 590 + (unit.Rank - 1) * 155 : 260;
                view.Rect.anchoredPosition = V2(x, 355);
                view.NameText.text = unit.Name;
                view.HealthFill.fillAmount = (float)unit.Health / unit.Template.Health;
                view.StressFill.fillAmount = unit.Enemy ? 0 : unit.Stress / 200f;
                view.HealthText.text = unit.AtDeathsDoor ? "죽음의 문턱" : $"{unit.Health} / {unit.Template.Health}";
                view.StatusText.text = Status(unit);
                view.Portrait.color = Color.white;
                view.Portrait.rectTransform.localScale = V3(unit.Enemy ? -1 : 1, 1, 1);
                bool valid = targets.Contains(unit);
                view.TargetMarker.text = valid ? "[ 대상 ]" : unit == actor ? "[ 행동 중 ]" : "";
                view.Highlight.color = valid ? Tone(0.85f, 0.3f, 0.18f, 0.9f) : unit == actor ? Tone(0.83f, 0.65f, 0.31f, 0.95f) : Tone(0.26f, 0.23f, 0.22f, 0.65f);
                view.Button.interactable = PlayerTurn && valid;
            }
            for (int i = 0; i < SkillButtons.Length; i++)
            {
                bool hasSkill = i < actor.Template.Skills.Length;
                SkillButtons[i].gameObject.SetActive(hasSkill);
                if (!hasSkill) continue;
                var skill = actor.Template.Skills[i];
                int cooldown = Model.RemainingCooldown(actor, skill);
                SkillLabels[i].text = $"{i + 1}  {skill.Name}"
                    + (cooldown > 0 ? $" · 재사용 {cooldown}R" : "");
                SkillButtons[i].interactable = PlayerTurn && Model.CanUse(actor, skill);
                SkillButtons[i].GetComponent<Image>().color = SelectedSkill == i ? Tone(0.45f, 0.3f, 0.13f) : Tone(0.17f, 0.15f, 0.14f);
                SkillIcons[i].sprite = skill.Id > 0 ? SkillCatalog.Get(skill.Id.ToString("000")).Icon
                    : AbilityIcons[((int)skill.Effect) % AbilityIcons.Length];
            }
            PassButton.interactable = PlayerTurn;
            RetreatButton.interactable = PlayerTurn;
            CancelButton.interactable = PlayerTurn && SelectedSkill >= 0;
            PromptText.text = Busy ? $"{actor.Name}의 행동…" : SelectedSkill >= 0 ? "밝게 표시된 대상을 클릭하세요  ·  ESC 선택 취소" : "스킬 선택 [1–4]  →  대상 클릭";
            SkillText.text = SelectedSkill < 0 ? "스킬을 선택하고 강조된 대상을 클릭하세요.\n단일 대상과 전체 대상 스킬을 활용하세요.\n대기는 한 번의 행동을 소모합니다."
                : Describe(actor.Template.Skills[SelectedSkill]);
            LogText.text = string.Join("\n", Model.Log.TakeLast(2));
            RefreshTarget();
        }

        private string Describe(CombatSkill skill)
            => $"<color=#E8C781>{skill.Name}</color>\n{skill.Description}\n대상: {(skill.SelfOnly ? "자신" : skill.BothTeams ? "양 진영" : skill.Friendly ? "아군" : "적")}{(skill.SelfOnly ? "" : skill.All ? " 전체" : " 한 명")}"
                + (skill.Max > 0 ? skill.Effect == Effect.Heal ? $"\n회복 {skill.Min}~{skill.Max}"
                    : $"\n기본 피해 {CombatModel.ScaleDamage(skill.Min)}~{CombatModel.ScaleDamage(skill.Max)}" : "")
                + (skill.Cooldown > 0 ? $"\n사용 후 {skill.Cooldown}개 라운드 동안 재사용 불가" : "");

        private void RefreshTarget()
        {
            if (inspected < 0)
            {
                TargetText.text = "대상 정보\n캐릭터 위에 마우스를 올려\n능력치와 명중률을 확인하세요.";
                return;
            }
            var unit = Model.Units[inspected];
            TargetText.text = $"<color=#E8C781>{unit.Name}</color>\nHP {unit.Health}/{unit.Template.Health}  |  회피 {unit.Dodge}\n보호 {unit.Protection}%  |  저항 {unit.Template.Resistance}%";
            if (SelectedSkill >= 0)
            {
                var skill = Model.Active.Template.Skills[SelectedSkill];
                bool valid = Model.Targets(Model.Active, skill).Contains(unit);
                TargetText.text += valid ? skill.Friendly ? "\n아군 지원 대상" : $"\n명중 {Model.HitChance(Model.Active, skill, unit)}%  |  치명타 {skill.Critical}%" : "\n대상 지정 불가";
            }
        }

        private static string Status(Combatant unit)
        {
            var parts = unit.Ailments.Select(a => $"{CombatModel.EffectName(a.Effect)} {a.Turns}").ToList();
            if (unit.Afflicted) parts.Add("붕괴");
            if (unit.Virtuous) parts.Add("각성");
            if (unit.AtDeathsDoor) parts.Add("죽음의 문턱");
            return string.Join(" · ", parts);
        }

        private void ShowResult()
        {
            if (resultShown) return;
            resultShown = true;
            var session = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
            if (session.Length > 0)
            {
                session[0].userData.battleRounds += Model.Round;
                if (Model.Outcome == Outcome.Retreated) session[0].userData.retreats++;
                for (int i = 0; i < party.Count; i++)
                    session[0].userData.partyHealth[party[i].CharacterType] = Mathf.Max(0.1f, (float)Model.Units[i].Health / Model.Units[i].Template.Health);
                if (Model.Outcome == Outcome.Victory)
                {
                    session[0].userData.battlesWon++;
                    if (encounterType == Map.NodeType.Elite) session[0].userData.elitesWon++;
                    RunEffects.Progress(1, 1);
                    if (encounterType == Map.NodeType.Elite) RunEffects.Progress(3, 1);
                }
            }
            Busy = false;
            Refresh();
            ResultPanel.SetActive(true);
            bool victory = Model.Outcome == Outcome.Victory;
            ResultTitle.text = victory ? EncounterName + " 승리" : Model.Outcome == Outcome.Defeat ? "원정대 전멸" : "원정대 후퇴";
            int survivors = Model.Units.Count(u => !u.Enemy && u.Living);
            ResultBody.text = $"{Model.Round} 라운드  ·  생존 {survivors}/{party.Count}\n" + (victory ? $"폐허에 잠시 고요가 찾아옵니다.\n보상: {Reward} 골드" : "어둠은 쉽게 물러서지 않습니다.");
            ContinueLabel.text = FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length == 0 ? "시작 화면으로"
                : Model.Outcome == Outcome.Defeat || victory && encounterType == Map.NodeType.Boss ? "원정 결과 보기" : "지도로 돌아가기";
            if (victory)
            {
                var managers = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
                if (managers.Length > 0) managers[0].userData.money += Reward;
            }
        }

        private void Continue()
        {
            bool standalone = FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length == 0;
            if (standalone) { SceneManager.LoadScene("StartScene"); return; }
            var map = GameManager.Instance.currentMap;
            if (Model.Outcome == Outcome.Victory && map.userPath.Count > 0 && map.userPath.Last().Equals(map.GetBossNode().point))
            { GameManager.Instance.userData.nodesVisited = map.userPath.Count; SceneLoader.LoadScene("GameClearScene"); return; }
            if (Model.Outcome == Outcome.Retreated && map.userPath.Count > 0)
            {
                map.userPath.RemoveAt(map.userPath.Count - 1);
            }
            SceneLoader.LoadScene(standalone ? "StartScene" : Model.Outcome == Outcome.Defeat ? "GameOverScene" : "MapScene");
        }

        public static Vector2 V2(float x, float y) { Vector2 value = default; value.x = x; value.y = y; return value; }
        public static Vector3 V3(float x, float y, float z) { Vector3 value = default; value.x = x; value.y = y; value.z = z; return value; }
        public static Color Tone(float r, float g, float b, float a = 1) { Color value = default; value.r = r; value.g = g; value.b = b; value.a = a; return value; }
    }
}
