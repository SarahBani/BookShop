using BookShop.Models;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Controls;

namespace BookShop.Models
{
    public class ExtendedDataColumn : DataColumn
    {

        #region Properties

        public Type ContentType { get; set; }

        public ColumnType ColumnType { get; set; }

        #endregion /Properties

        #region Constructors

        public ExtendedDataColumn()
            : base()
        {
        }

        public ExtendedDataColumn(string columnName)
            : base(columnName)
        {
        }

        public ExtendedDataColumn(string columnName, Type contentType, Type dataType, ColumnType columnType)
            : base(columnName, dataType)
        {
            this.ContentType = contentType;
            this.ColumnType = columnType;
        }

        #endregion /Constructors

    }
}
