using ForestFireSimulator.Common;
using System;

namespace ForestFireSimulator.Model
{
    /// <summary>
    /// The ForestCell class represents a single forest cell.
    /// </summary>
    public class ForestCell : NotificationBase
    {
        #region Fields

        private CellState _cellState;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForestCell()
        {
            _cellState = CellState.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Creates a forest cell with the provided state.
        /// </summary>
        public ForestCell(CellState cellState)
        {
            _cellState = cellState;
        }

        /// <summary>
        /// Create a forest cell from an existing forest cell.
        /// </summary>
        /// <param name="forestCell"></param>
        public ForestCell(ForestCell forestCell)
        {
            try
            {
                if (forestCell == null)
                {
                    throw new Exception("forestCell can not be null");
                }

                _cellState = forestCell.CellState;
            }
            catch (Exception ex)
            {
                throw new Exception("ForestCell(ForestCell forestCell): " + ex.ToString());
            }
        }

        #endregion

        #region Events
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cell state.
        /// </summary>
        public CellState CellState
        {
            get
            {
                return _cellState;
            }
            set
            {
                _cellState = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Methods
        #endregion
    }
}
