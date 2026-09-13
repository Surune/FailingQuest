using System;
using System.Linq;
using FailingQuest.Cards;

public static class CardCombatChecks
{
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception(label);
    }
    private static CardCombatModel Battle()
    {
        var model = new CardCombatModel(Card.Starter(),80,80,17);
        model.Enemies.Add(new CardEnemy { Name="Dummy",Health=200,MaxHealth=200,Power=8 });
        model.BeginTurn(); return model;
    }
    public static string Run()
    {
        var m=Battle();
        Check(m.Hand.Count==5 && m.Mana==100,"Opening hand and battle mana");
        m.Mana=30;
        m.Hand.Clear(); m.Hand.Add(new Card { Id=3 }); m.Hand.Add(new Card { Id=3 });
        Check(m.Play(0,0) && m.Mana==10 && m.Enemies[0].Health==186,"Damage and mana cost");
        Check(!m.Play(0,0) && m.Mana==10 && m.Hand.Count==1 && m.Enemies[0].Health==186,"Insufficient mana must not mutate");
        m=Battle(); m.Hand.Clear(); m.Hand.Add(new Card { Id=1 });
        m.Play(0,0); m.EndTurn(); m.EnemyAction(0);
        Check(m.Health==77 && m.Block==0,"Block absorbs damage");
        m.BeginTurn(); Check(m.Block==0 && m.Mana==90 && m.Hand.Count==5,"Turn resets block and draws without resetting mana");
        m.Mana=0; m.EndTurn(); m.BeginTurn();
        Check(m.Mana==0,"Zero mana stays zero next turn");
        m.Hand.Clear(); m.Hand.Add(new Card {Id=10});
        Check(!m.Play(0,0) && m.Mana==0 && m.Hand.Count==1,"Recovery requires its mana cost at zero");
        m.Mana=5;
        Check(m.Play(0,0) && m.Mana==30 && m.Discard.Last().Id==10,"Recovery pays five mana before restoring thirty");
        m.EndTurn(); m.BeginTurn(); Check(m.Mana==30,"Recovered mana persists");
        m.Hand.Clear(); m.Hand.Add(new Card {Id=10,Upgraded=true});
        Check(m.Play(0,0) && m.Mana==75,"Upgraded recovery pays five and restores fifty mana");
        m.Mana=100; m.Hand.Add(new Card {Id=10}); m.Play(0,0);
        Check(m.Mana==100,"Recovery cannot exceed maximum mana");
        m.EndTurn(); m.BeginTurn(); Check(m.Mana==100,"Maximum mana persists");
        Check(Battle().Mana==100,"New battle receives fresh starting mana");
        var custom=new CardCombatModel(Card.Starter(),80,80,17,7);
        Check(custom.Mana==7,"Configured battle starting mana");
        custom = new CardCombatModel(Card.Starter(),80,80,17,150);
        Check(custom.Mana==100,"Starting mana respects maximum");
        m=Battle(); m.Mana=90; m.Hand.Clear(); m.Hand.Add(new Card {Id=10,Upgraded=true});
        Check(m.Play(0,0) && m.Mana==100,"Upgraded recovery caps at 100");
        int[] costs = {10,10,15,20,15,10,15,20,10,15,5};
        for(int id=0;id<costs.Length;id++)
            Check(new Card {Id=id}.Cost==costs[id] && new Card {Id=id,Upgraded=true}.Cost==costs[id],"Rescaled normal/upgraded card costs");
        Check(Card.Starter().Count(c=>c.Id==10)==2,"Starter deck contains two recovery cards");
        foreach(int id in Enumerable.Range(0,Card.Names.Length))
            Check(new Card {Id=id}.Description.Length>0,"Every card has a valid description");
        m=Battle(); m.Hand.Clear(); m.Hand.Add(new Card { Id=2 }); m.Hand.Add(new Card { Id=0 });
        m.Play(0,0); m.Play(0,0); Check(m.Enemies[0].Health==183,"Vulnerable amplifies following attack");
        m=Battle(); m.DrawPile.Clear(); m.Hand.Clear(); m.Discard.Clear();
        m.Hand.Add(new Card { Id=5 }); m.Discard.Add(new Card { Id=0 }); m.Discard.Add(new Card { Id=1 });
        m.Play(0,0); Check(m.Hand.Count==2 && m.Discard.Count==1 && m.Discard[0].Id==5,"Reshuffle excludes resolving draw card");
        m=Battle(); m.Hand.Clear(); m.Hand.Add(new Card { Id=6 }); m.Play(0,0);
        Check(m.Exhaust.Count==1 && m.Block==12,"Exhaust card leaves deck for battle");
        m=Battle(); m.Draw(20); Check(m.Hand.Count==10,"Hand limit");
        m=Battle(); m.Hand.Clear(); m.Hand.Add(new Card { Id=7 });
        m.Enemies.Add(new CardEnemy {Health=10,MaxHealth=10}); m.Enemies[0].Health=10;
        m.Play(0,0); Check(m.Victory,"Area attack and victory");
        m=Battle(); m.Health=1; m.EndTurn(); m.EnemyAction(0);
        Check(m.Health==0 && m.Finished,"Defeat at zero health");
        m.BeginTurn(); Check(!m.PlayerTurn,"No turn after defeat");
        var deck=Card.Starter(); m=new CardCombatModel(deck,80,80,4);
        m.DrawPile[0].Upgraded=true; Check(deck.All(c=>!c.Upgraded),"Combat copies persistent deck");
        m=Battle();
        for(int i=0;i<20;i++)
        {
            m.EndTurn(); m.BeginTurn();
            Check(m.DrawPile.Count+m.Hand.Count+m.Discard.Count+m.Exhaust.Count==Card.Starter().Count,"Card conservation across repeated turns");
        }
        return "PASS: battle starting mana, cross-turn persistence (zero/spent/recovered/capped), normal/upgraded recovery, new battle reset, configured mana, starter recovery cards, costs, block, draw, vulnerable, reshuffle, exhaust, victory/defeat, 20-turn card conservation";
    }
}
