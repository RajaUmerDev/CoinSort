public interface IScorer
{
    void OnMergeSlot(int coins);

    void OnDrawCoins(int coins);

    int GetCurrentScore();

    void OnClickUnLockBtn(int coins);

    void OnMergeSlotForTarget(int addedScore);

    void Initialize();
}