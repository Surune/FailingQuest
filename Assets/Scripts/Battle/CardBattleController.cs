using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FailingQuest.Combat;

namespace FailingQuest.Cards
{
    public class CardBattleController : MonoBehaviour
    {
        public CardCatalog Catalog;
        public int StartingMana = CardCombatModel.DefaultStartingMana;
        public CombatAppearance[] Heroes, EnemyArt;
        public TMP_Text Heading, Status, HeroStatus, Hint, Log, Piles;
        public Image HeroPortrait;
        public Button[] EnemyButtons, CardButtons, RewardButtons;
        public Image[] EnemyPortraits, EnemyBars;
        public Image[] CardIcons;
        public Sprite[] AbilityIcons;
        public TMP_Text[] CardNames, CardCosts;
        public TMP_Text[] EnemyLabels, IntentLabels, CardLabels, RewardLabels;
        public Button EndButton, DeckButton, CloseButton, ContinueButton;
        public GameObject DeckPanel, ResultPanel;
        public TMP_Text DeckText, ResultText, ContinueText;
        public CardCombatModel Model { get; private set; }
        public int Selected { get; private set; } = -1;
        private UserData data;
        private bool standalone, resolved, chosen;
        private Map.NodeType encounter;
        private Card[] rewards;
        private CharacterType hero;

        private void Start()
        {
            standalone = FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length == 0;
            data = standalone ? new UserData { characters = new() { Heroes[0].CharacterType } } : GameManager.Instance.userData;
            hero = data.characters[0];
            encounter = Map.NodeType.Normal;
            if (!standalone && GameManager.Instance.currentMap.userPath.Count > 0)
                encounter = GameManager.Instance.currentMap.GetNode(GameManager.Instance.currentMap.userPath.Last()).nodeType;
            int maxHealth = 80;
            int strength = 0;
            if (!standalone)
                foreach (int id in data.myTreasureIndex)
                {
                    var treasure = GameManager.Instance.treasureCatalog.treasures[id];
                    maxHealth += treasure.healthBonus;
                    strength += treasure.attackBonus;
                }
            Model = new CardCombatModel(data.deck, Mathf.CeilToInt(maxHealth * data.partyHealth[hero]), maxHealth, System.Environment.TickCount, StartingMana);
            Model.Strength = strength;
            HeroPortrait.sprite = Heroes.First(h => h.CharacterType == hero).Sprite;
            int count = encounter == Map.NodeType.Boss ? 1 : encounter == Map.NodeType.Elite ? 3 : 2;
            for (int i = 0; i < count; i++)
            {
                int hp = encounter == Map.NodeType.Boss ? 150 : encounter == Map.NodeType.Elite ? 36 : 24 + Mathf.Min(12, data.battlesWon * 2);
                Model.Enemies.Add(new CardEnemy { Name = encounter == Map.NodeType.Boss ? "폐허의 군주" : EnemyArt[i].Template.Name,
                    Health = hp, MaxHealth = hp, Power = encounter == Map.NodeType.Boss ? 13 : 5 + i, Pattern = i });
            }
            for (int i = 0; i < EnemyButtons.Length; i++)
            {
                int id = i;
                EnemyPortraits[i].sprite = EnemyArt[i].Sprite;
                EnemyButtons[i].onClick.AddListener(() => Target(id));
            }
            for (int i = 0; i < CardButtons.Length; i++) { int slot = i; CardButtons[i].onClick.AddListener(() => Select(slot)); }
            for (int i = 0; i < RewardButtons.Length; i++) { int slot = i; RewardButtons[i].onClick.AddListener(() => ChooseReward(slot)); }
            EndButton.onClick.AddListener(EndTurn);
            DeckButton.onClick.AddListener(ShowDeck);
            CloseButton.onClick.AddListener(() => DeckPanel.SetActive(false));
            ContinueButton.onClick.AddListener(Continue);
            DeckPanel.SetActive(false); ResultPanel.SetActive(false);
            Heading.text = encounter == Map.NodeType.Boss ? "마지막 관문 / 폐허의 군주" : encounter == Map.NodeType.Elite ? "잊힌 지하묘지 / 정예 전투" : "잊힌 지하묘지 / 전투";
            Model.BeginTurn(); Refresh(); Resolve();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { Selected = -1; DeckPanel.SetActive(false); Refresh(); }
            if (DeckPanel.activeSelf || resolved) return;
            for (int i = 0; i < 9; i++) if (Input.GetKeyDown(KeyCode.Alpha1 + i)) Select(i);
            if (Input.GetKeyDown(KeyCode.Space)) EndTurn();
            if (Input.GetKeyDown(KeyCode.D)) ShowDeck();
        }

        public void Select(int index)
        {
            if (!Model.PlayerTurn || resolved || index >= Model.Hand.Count || DeckPanel.activeSelf) return;
            if (Model.Hand[index].Cost > Model.Mana) { Hint.text = "마나가 부족합니다. 회복 카드도 마나 5가 필요하며 턴이 바뀌어도 회복되지 않습니다."; return; }
            Selected = index;
            if (!Model.Hand[index].RequiresTarget) Target(0);
            else Refresh();
        }

        public void Target(int index)
        {
            if (DeckPanel.activeSelf || Selected < 0 || resolved) return;
            if (!Model.Play(Selected, index)) return;
            Selected = -1; Refresh(); Resolve();
        }

        public void EndTurn()
        {
            if (!Model.PlayerTurn || resolved || DeckPanel.activeSelf) return;
            Selected = -1; Model.EndTurn(); Refresh(); StartCoroutine(EnemyPhase());
        }

        private IEnumerator EnemyPhase()
        {
            for (int i = 0; i < Model.Enemies.Count; i++)
            {
                if (!Model.Enemies[i].Alive) continue;
                yield return new WaitForSeconds(0.55f);
                Model.EnemyAction(i); Refresh();
                if (Model.Finished) { Resolve(); yield break; }
            }
            Model.BeginTurn(); Refresh();
        }

        public void Refresh()
        {
            Status.text = $"TURN {Model.Turn:00}     마나 {Model.Mana} / {CardCombatModel.MaxMana}";
            HeroStatus.text = $"생명력 {Model.Health} / {Model.MaxHealth}\n방어 {Model.Block}    힘 {Model.Strength}";
            Piles.text = $"뽑기 {Model.DrawPile.Count}   /   버림 {Model.Discard.Count}   /   소멸 {Model.Exhaust.Count}";
            Log.text = Model.Message;
            Hint.text = !Model.PlayerTurn ? "적의 행동 중…" : Selected >= 0 ? $"{Model.Hand[Selected].Name} → 공격할 적을 클릭하세요 · ESC 취소" : "카드 선택 [1–9] → 적 클릭   ·   방어/스킬은 즉시 사용   ·   턴 종료 [Space]";
            for (int i = 0; i < EnemyButtons.Length; i++)
            {
                EnemyButtons[i].gameObject.SetActive(i < Model.Enemies.Count);
                if (i >= Model.Enemies.Count) continue;
                var enemy = Model.Enemies[i];
                EnemyLabels[i].text = $"{enemy.Name}\n{enemy.Health} / {enemy.MaxHealth}   방어 {enemy.Block}";
                IntentLabels[i].text = !enemy.Alive ? "처치" : $"다음 행동 · {enemy.Intent}" + (enemy.Vulnerable > 0 ? $"\n취약 {enemy.Vulnerable}턴 · 받는 피해 +50%" : "");
                EnemyBars[i].fillAmount = (float)enemy.Health / enemy.MaxHealth;
                EnemyPortraits[i].color = enemy.Alive ? Color.white : RankBattleController.Tone(0.25f, 0.25f, 0.25f);
                EnemyButtons[i].interactable = Model.PlayerTurn && Selected >= 0 && enemy.Alive && !resolved;
            }
            float width = Mathf.Min(190, 1140f / Mathf.Max(1, Model.Hand.Count) - 10);
            float start = 720 - Model.Hand.Count * (width + 10) / 2;
            for (int i = 0; i < CardButtons.Length; i++)
            {
                var button = CardButtons[i]; button.gameObject.SetActive(i < Model.Hand.Count);
                if (i >= Model.Hand.Count) continue;
                var card = Model.Hand[i];
                var rect = (RectTransform)button.transform;
                rect.anchoredPosition = RankBattleController.V2(start + i * (width + 10), i == Selected ? 53 : 35);
                rect.sizeDelta = RankBattleController.V2(width, 237);
                CardNames[i].text = card.Name;
                CardNames[i].fontSize = width < 140 ? 14 : 19;
                CardCosts[i].text = card.Cost + "";
                CardIcons[i].sprite = card.Defined ? card.Icon : AbilityIcons[card.Id % AbilityIcons.Length];
                CardLabels[i].text = card.Description;
                CardLabels[i].enableAutoSizing = true;
                CardLabels[i].fontSizeMin = 7;
                CardLabels[i].fontSizeMax = width < 140 ? 13 : 17;
                button.interactable = Model.PlayerTurn && card.Cost <= Model.Mana && !resolved;
                button.GetComponent<Image>().color = i == Selected ? RankBattleController.Tone(.39f,.3f,.13f) : card.IsAttack ? RankBattleController.Tone(.27f,.13f,.14f) : RankBattleController.Tone(.11f,.23f,.26f);
            }
            EndButton.interactable = Model.PlayerTurn && !resolved;
        }

        private void Resolve()
        {
            if (!Model.Finished || resolved) return;
            resolved = true;
            data.partyHealth[hero] = (float)Model.Health / Model.MaxHealth;
            data.battleRounds += Model.Turn;
            ResultPanel.SetActive(true);
            if (Model.Victory)
            {
                int gold = encounter == Map.NodeType.Boss ? 150 : encounter == Map.NodeType.Elite ? 60 : 30;
                data.money += gold; data.battlesWon++;
                if (encounter == Map.NodeType.Elite) data.elitesWon++;
                ResultText.text = $"전투 승리 / {gold} 골드 획득\n덱에 추가할 카드 한 장을 선택하세요.";
                rewards = Catalog.CreateRewardPool().OrderBy(_ => Random.value).Take(3).ToArray();
                for (int i = 0; i < 3; i++) RewardLabels[i].text = $"{rewards[i].Cost} 마나\n\n{rewards[i].Name}\n\n{rewards[i].Description}";
                ContinueText.text = "카드 보상 건너뛰기";
            }
            else
            {
                ResultText.text = $"원정 실패\n{Model.Turn}턴 동안 싸웠습니다.";
                foreach (var button in RewardButtons) button.gameObject.SetActive(false);
                ContinueText.text = "원정 결과 보기";
            }
            Refresh();
        }

        public void ChooseReward(int index)
        {
            if (!resolved || !Model.Victory || chosen) return;
            chosen = true; data.deck.Add(rewards[index].Copy());
            ResultText.text = rewards[index].Name + " 카드를 덱에 추가했습니다.";
            foreach (var button in RewardButtons) button.interactable = false;
            ContinueText.text = "계속하기";
        }

        private void ShowDeck()
        {
            DeckText.text = "보유 덱 · " + data.deck.Count + "장\n" + string.Join("\n", data.deck.GroupBy(c => c.Name + " · 마나 " + c.Cost + (c.Crafted ? " — " + c.Description.Replace("\n", " / ") : "")).Select(g => $"{g.Key} ×{g.Count()}"))
                + "\n\n현재 버림: " + string.Join(", ", Model.Discard.Select(c => c.Name))
                + "\n소멸: " + string.Join(", ", Model.Exhaust.Select(c => c.Name));
            DeckPanel.SetActive(true);
        }

        private void Continue()
        {
            if (standalone) { UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene"); return; }
            SceneLoader.LoadScene(!Model.Victory ? "GameOverScene" : encounter == Map.NodeType.Boss ? "GameClearScene" : "MapScene");
        }
    }
}
