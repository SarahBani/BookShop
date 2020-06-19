namespace BookShop.Models
{
    public enum BookBinding
    {
        Unknown = -1,
        Paperback = 1,
        Hardcover = 2,
        Coalwood = 3
    }

    /// <summary>
    /// The type of column which is displayed for each property
    /// </summary>
    public enum ColumnType
    {
        Text,
        Label,
        ComboBox,
        CheckBox,
        Button
    }

}
