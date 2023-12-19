public class MeteorFallArea : AreaAbstract
{
    public override void Action()
    {
        
    }

    public override void EnableArea(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}