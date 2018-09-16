using ForestFireSimulator.ViewModel;
using System.Windows;

namespace ForestFireSimulator.View
{
    /// <summary>
    /// The ForestFireSimulatorView class represents the View for the Forest Fire Simulator.
    /// </summary>
    public partial class ForestFireSimulatorView : Window
    {
        #region Fields
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForestFireSimulatorView()
        {
            InitializeComponent();

            // Create the View Model.
            ForestFireSimulatorViewModel viewModel = new ForestFireSimulatorViewModel();
            DataContext = viewModel;    // Set the data context for all data binding operations.
        }

        #endregion

        #region Events
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}
