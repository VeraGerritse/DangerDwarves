
[System.Serializable]
public class Gold : Item {
    public int amount;

    public override void StartGold(int amount_)
    {
        amount = amount_;
        itemName = amount.ToString() + " Gold";
    }
}
