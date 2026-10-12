public static class SaveMigrations
{
    public static SaveData Upgrade(SaveData data, int target)
    {
        while (data.version < target)
        {
            switch (data.version)
            {
                case 1: FromV1(data); break;
                case 2: FromV2(data); break;
            }
            data.version++;
        }
        return data;
    }

    // v1 → v2: появился инвентарь — пустой по умолчанию
    private static void FromV1(SaveData d) { /* добавить пустую запись */ }

    // v2 → v3: поле gold переехало в кошелёк
    private static void FromV2(SaveData d) { /* перенести значение */ }
}
