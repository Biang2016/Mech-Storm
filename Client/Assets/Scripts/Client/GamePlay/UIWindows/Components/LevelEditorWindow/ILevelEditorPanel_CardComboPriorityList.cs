using UnityEngine.Events;

public interface ILevelEditorPanel_CardComboPriorityCardContainer
{
    bool IsSelected { get; set; }

    UnityAction<ILevelEditorPanel_CardComboPriorityCardContainer> OnSelect { get; set; }

    void AddCard(int cardID);
    void RemoveCard(int cardID);
    void MoveUpCard(int cardID);
    void MoveDownCard(int cardID);
}