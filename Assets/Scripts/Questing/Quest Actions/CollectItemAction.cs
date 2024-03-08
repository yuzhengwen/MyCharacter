using System;

public class CollectItemAction : BaseObjective
{
    private readonly ItemDataSO item;
    private readonly int amountToCollect;
    private readonly QuestManager qm;

    private int amountCollected;

    public CollectItemAction(BaseQuest parent, QuestManager qm, ItemDataSO item, int amountToCollect) : base(parent, "Collect Item")
    {
        this.item = item;
        this.amountToCollect = amountToCollect;
        this.qm = qm;
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
    public override void Complete()
    {
        base.Complete();
        qm.inventory.OnItemAdded -= CheckItem;
    }
}
