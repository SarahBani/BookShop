using BookShop.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace BookShop.Models
{
    public class ExtendedDataTable : DataTable
    {

        #region Constructors

        public ExtendedDataTable()
            : base()
        {
        }

        public ExtendedDataTable(string tableName)
            : base(tableName)
        {
        }

        public ExtendedDataTable(string tableName, string tableNamespace)
            : base(tableName, tableNamespace)
        {
        }

        #endregion /Constructors

        #region Methods

        // Return the RowType as ExtendedDataRow instead of DataRow
        protected override Type GetRowType() => typeof(ExtendedDataRow);

        // Use the RowBuilder to return an ExtendedDataRow instead of DataRow
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) => new ExtendedDataRow(builder);

        public ExtendedDataRow GetNewRow() => (ExtendedDataRow)base.NewRow();

        /// <summary>
        /// For applying custom sort on DataTable
        /// </summary>
        /// <param name="columnName">name of the sort column</param>
        /// <param name="comparer">the comparer which is appropriate for comparing the content of the column</param>
        public void ApplySort(string columnName, BaseCustomComparer comparer)
        {
            var rows = new List<ExtendedDataRow>();
            foreach (DataRow row in this.Rows)
            {
                ExtendedDataRow newRow = this.GetNewRow();
                newRow.Clone((row as ExtendedDataRow));
                rows.Add(newRow);
            }
            rows.Sort((row1, row2) =>
            {
                return comparer.Compare(row1[columnName], row2[columnName]);
            });
            for (int i = 0; i < Rows.Count; i++)
            {
                (this.Rows[i] as ExtendedDataRow).Clone(rows[i]);
            }
        }

        #endregion /Methods

    }
}
