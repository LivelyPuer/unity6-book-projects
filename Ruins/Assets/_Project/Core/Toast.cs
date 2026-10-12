using System;

// Всплывающие сообщения: логика публикует, интерфейс показывает
public static class Toast
{
    public static event Action<string> Shown;
    public static void Show(string message) => Shown?.Invoke(message);
}
