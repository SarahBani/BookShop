using System.Data;

namespace BookShop.Models
{
    public class ExtendedDataRow : DataRow
    {

        #region Properties

        /// <summary>
        /// for specifing this row is highlighted or not
        /// </summary>
        public bool IsHighlighted { get; set; }

        #endregion /Properties

        #region Constructors

        public ExtendedDataRow()
            : base(null)
        {
        }

        public ExtendedDataRow(DataRowBuilder rb)
            : base(rb)
        {
        }

        #endregion /Constructors

        #region Methods

        /// <summary>
        /// Clone the values of other ExtendedDataRow on current ExtendedDataRow
        /// </summary>
        /// <param name="extendedDataRow">the other ExtendedDataRow</param>
        public void Clone(ExtendedDataRow extendedDataRow)
        {
            this.ItemArray = extendedDataRow.ItemArray;
            this.IsHighlighted = extendedDataRow.IsHighlighted;
        }

        #endregion /Methods

    }
}
