using System;

namespace BookShop.Models
{
    /// <summary>
    /// Define the column which is displayed for each property
    /// </summary>
    public class ColumnAttribute : Attribute
    {

        #region Properties

        public ColumnType Type { get; set; }

        #endregion /Properties

        #region Constructors

        public ColumnAttribute(ColumnType type = ColumnType.Text)
        {
            this.Type = type;
        }

        #endregion /Constructors

    }
}
