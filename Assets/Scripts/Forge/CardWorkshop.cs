using System.Collections.Generic;
using System.Linq;
using FailingQuest.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardWorkshop : MonoBehaviour
{
    public Button UpgradeTab, DismantleTab, CombineTab, Previous, Next, Confirm, Clear, Return;
    public Button[] Items;
    public TMP_Text[] Labels;
    public TMP_Text Status, Preview, Page;
    private UserData data;
    private int mode, page, selectedCard = -1;
    private bool upgraded;
    private readonly List<CardPhrase> selected = new();

    private void Start()
    {
        data = GameManager.Instance.userData;
        UpgradeTab.onClick.AddListener(() => SetMode(0));
        DismantleTab.onClick.AddListener(() => SetMode(1));
        CombineTab.onClick.AddListener(() => SetMode(2));
        Previous.onClick.AddListener(() => { page--; Refresh(); });
        Next.onClick.AddListener(() => { page++; Refresh(); });
        Clear.onClick.AddListener(() => { selected.Clear(); selectedCard = -1; Refresh(); });
        Confirm.onClick.AddListener(Apply);
        Return.onClick.AddListener(() => SceneLoader.LoadScene("MapScene"));
        for (int i = 0; i < Items.Length; i++) { int slot = i; Items[i].onClick.AddListener(() => Select(slot)); }
        SetMode(0);
    }

    private void SetMode(int value)
    {
        mode = value; page = 0; selectedCard = -1; selected.Clear(); Refresh();
    }

    private void Select(int slot)
    {
        int index = page * Items.Length + slot;
        if (mode == 2)
        {
            var phrase = data.cardPhrases[index];
            if (!selected.Remove(phrase)) selected.Add(phrase);
        }
        else selectedCard = index;
        Refresh();
    }

    private void Apply()
    {
        if (!Confirm.interactable) return;
        if (mode == 0) { data.deck[selectedCard].Upgraded = true; upgraded = true; }
        else if (mode == 1) CardCrafting.Dismantle(data.deck, data.cardPhrases, data.deck[selectedCard]);
        else CardCrafting.Combine(data.deck, data.cardPhrases, selected);
        selectedCard = -1; selected.Clear();
        Refresh();
    }

    public void Refresh()
    {
        int count = mode == 2 ? data.cardPhrases.Count : data.deck.Count;
        int pages = Mathf.Max(1, Mathf.CeilToInt((float)count / Items.Length));
        page = Mathf.Min(page, pages - 1);
        Page.text = $"{page + 1} / {pages}";
        Previous.interactable = page > 0; Next.interactable = page + 1 < pages;
        Status.text = $"대장간 · 보유 카드 {data.deck.Count}장 / 재료 {data.cardPhrases.Count}개\n"
            + (mode == 0 ? "방문당 1장 무료 강화" : mode == 1 ? "카드 1장을 소모해 단어·문장으로 분해합니다. 덱에 최소 1장은 남겨야 합니다." : "재료를 선택한 순서대로 효과가 발동합니다. 선택된 재료는 조합 시 소모됩니다.");
        for (int i = 0; i < Items.Length; i++)
        {
            int index = page * Items.Length + i;
            Items[i].gameObject.SetActive(index < count);
            if (index >= count) continue;
            if (mode == 2)
            {
                var p = data.cardPhrases[index];
                int order = selected.IndexOf(p);
                Labels[i].text = (order >= 0 ? $"[선택 {order + 1}] " : "") + p.Text + $"\n마나 {p.ManaCost}";
            }
            else
            {
                var c = data.deck[index];
                Labels[i].text = (selectedCard == index ? "[선택] " : "") + $"{c.Name} · 마나 {c.Cost}\n{c.Description}";
            }
        }
        Confirm.GetComponentInChildren<TMP_Text>().text = mode == 0 ? "강화하기" : mode == 1 ? "분해하기" : "조합하기";
        Confirm.interactable = mode == 2
            ? selected.Count >= 2 && selected.Any(p => p.Effect != PhraseEffect.Word && p.Effect != PhraseEffect.Exhaust)
            : selectedCard >= 0 && (mode == 0 ? !upgraded && !data.deck[selectedCard].Upgraded : data.deck.Count > 1);
        if (mode == 2)
            Preview.text = $"조합 미리보기 · {selected.Count}개\n총 소모 마나: {selected.Sum(p => p.ManaCost)}\n\n"
                + string.Join("\n", selected.Select((p, i) => $"{i + 1}. {p.Text}  [마나 {p.ManaCost}]"));
        else if (selectedCard >= 0)
        {
            var c = data.deck[selectedCard].Copy();
            if (mode == 0) c.Upgraded = true;
            Preview.text = mode == 0 ? $"강화 결과 · 마나 {c.Cost}\n\n{c.Description}" : "분해 시 획득 재료\n\n" + string.Join("\n", c.GetPhrases().Select(p => $"{p.Text}  [마나 {p.ManaCost}]"));
        }
        else Preview.text = mode == 0 && upgraded ? "이번 방문의 강화를 완료했습니다.\n분해와 조합은 계속 이용할 수 있습니다." : "카드를 선택하면 결과를 미리 볼 수 있습니다.";
    }
}
