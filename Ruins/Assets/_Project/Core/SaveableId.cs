using UnityEngine;

[DisallowMultipleComponent]
public class SaveableId : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    // Вызывается при добавлении компонента в редакторе
    void Reset() => id = System.Guid.NewGuid().ToString("N");

    [ContextMenu("Сгенерировать новый ID")]
    private void Regenerate() => id = System.Guid.NewGuid().ToString("N");
}
