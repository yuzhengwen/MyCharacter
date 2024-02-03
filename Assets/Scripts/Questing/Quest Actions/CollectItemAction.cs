using System;

public class CollectItemAction : QuestAction
{
    private readonly ItemDataSO item;
    private readonly int amountToCollect;
    private readonly QuestManager qm;

    private int amountCollected;

    public CollectItemAction(QuestManager qm, ItemDataSO item, int amountToCollect) : base(qm)
    {
        this.item = item;
        this.amountToCollect = amountToCollect;
    }

    private void CheckItem(ItemDataSO itemAdded, int amt)
    {
        if (itemAdded == item)
        {
            amountCollected += amt;
            if (amountCollected >= amountToCollect)
                Complete();
        }
    }
    public override void Start()
    {
        qm.inventory.OnItemAdded += CheckItem;
    }
    protected override void Complete()
    {
        base.Complete();
        qm.inventory.OnItemAdded -= CheckItem;
    }
}
