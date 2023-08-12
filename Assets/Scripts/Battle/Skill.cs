public abstract class Skill
{
    private int _coolTime=100;
    private bool isUsed;
    //TODO 스킬의 효과
    
    public Skill(int coolTime)
    {
        _coolTime = coolTime;
        isUsed = false;
    }

    public bool Available(int time)
    {
        //스킬 사용 가능한지 판단
        return !isUsed && time >= _coolTime;
    }
    
    public abstract void use();
    //TODO 스킬 사용시 효과
    

    public void Reset()
    {
        isUsed = false;
    }
}
