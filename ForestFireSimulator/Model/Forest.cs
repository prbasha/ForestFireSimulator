using ForestFireSimulator.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ForestFireSimulator.Model
{
    /// <summary>
    /// The Forest class represents a simulation of a forest.
    /// It is made up of a square grid of forest cells.
    /// At any point in time, a forest cell will have one of three states: empty, tree, or burning.
    /// </summary>
    public class Forest : NotificationBase
    {
        #region Fields

        private ObservableCollection<ForestCell> _forestCells;
        private int _regrowthProbability = Constants.DefaultProbability;
        private int _lightingProbability = Constants.DefaultProbability;
        private int _stepIntervalMilliSeconds = Constants.DefaultStepIntervalMilliSeconds;
        private DispatcherTimer _stepTimer;
        private bool _isSimulationRunning = false;
        private Random _random;
        private static object _updateLock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Forest()
        {
            try
            {
                BuildForest();          // Build the forest.
                _random = new Random(); // Initialise the random number generator.
            }
            catch (Exception ex)
            {
                throw new Exception("Forest(): " + ex.ToString());
            }
        }

        #endregion

        #region Events
        #endregion

        #region Properties

        /// <summary>
        /// Gets the width of the forest (in cells).
        /// </summary>
        public int ForestWidthCells
        {
            get
            {
                return Constants.ForestWidth;
            }
        }

        /// <summary>
        /// Gets the height of the forest (in cells).
        /// </summary>
        public int ForestHeightCells
        {
            get
            {
                return Constants.ForestHeight;
            }
        }
        
        /// <summary>
        /// Gets the collection of forest cells.
        /// </summary>
        public ObservableCollection<ForestCell> ForestCells
        {
            get
            {
                if (_forestCells == null)
                {
                    _forestCells = new ObservableCollection<ForestCell>();
                }

                return _forestCells;
            }
            private set
            {
                if (value != null)
                {
                    _forestCells = new ObservableCollection<ForestCell>(value);
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the regrowth probability.
        /// </summary>
        public int RegrowthProbability
        {
            get
            {
                return _regrowthProbability;
            }
            set
            {
                if (value >= Constants.ZeroPercentProbability && value <= Constants.OneHundredPercentProbability)
                {
                    _regrowthProbability = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the lightning probability.
        /// </summary>
        public int LightningProbability
        {
            get
            {
                return _lightingProbability;
            }
            set
            {
                if (value >= Constants.ZeroPercentProbability && value <= Constants.OneHundredPercentProbability)
                {
                    _lightingProbability = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the step interval in milli-seconds.
        /// </summary>
        public int StepIntervalMilliSeconds
        {
            get
            {
                return _stepIntervalMilliSeconds;
            }
            set
            {
                if (value >= Constants.MinimumStepIntervalMilliSeconds && value <= Constants.MaximumStepIntervalMilliSeconds)
                {
                    _stepIntervalMilliSeconds = value;
                    RaisePropertyChanged();

                    if (_stepTimer != null && _stepTimer.IsEnabled)
                    {
                        // Update the step timer interval.
                        _stepTimer.Interval = TimeSpan.FromMilliseconds(_stepIntervalMilliSeconds);
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the simulation running boolean flag.
        /// </summary>
        public bool IsSimulationRunning
        {
            get
            {
                return _isSimulationRunning;
            }
            private set
            {
                _isSimulationRunning = value;
                RaisePropertyChanged();
            }
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// The BuildForest method is called to build (initialise) the forest.
        /// </summary>
        private void BuildForest()
        {
            try
            {
                if (!IsSimulationRunning)
                {
                    _forestCells = new ObservableCollection<ForestCell>();
                    while (_forestCells.Count != Constants.ForestWidth * Constants.ForestHeight)
                    {
                        _forestCells.Add(new ForestCell(CellState.Tree));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.BuildForest(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The BuildForest method is called to reset the forest.
        /// All forest cells are set to trees.
        /// </summary>
        private void ResetForest()
        {
            try
            {
                if (!IsSimulationRunning)
                {
                    foreach (ForestCell forestCell in _forestCells)
                    {
                        forestCell.CellState = CellState.Tree;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.ResetForest(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The StartSimulation method is called to start the forest simulation.
        /// </summary>
        public void StartSimulation()
        {
            try
            {
                if (!IsSimulationRunning)
                {
                    // Start the step timer.
                    _stepTimer = new DispatcherTimer();
                    _stepTimer.Interval = TimeSpan.FromMilliseconds(_stepIntervalMilliSeconds);
                    _stepTimer.Tick += new EventHandler(StepTimerEventHandler);
                    _stepTimer.Start();
                    
                    IsSimulationRunning = true; // Set the simulation running flag.
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.StartSimulation(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The StopSimulation method is called to stop the forest simulation.
        /// </summary>
        public void StopSimulation()
        {
            try
            {
                // Stop the step timer.
                if (_stepTimer != null && _stepTimer.IsEnabled)
                {
                    _stepTimer.Stop();
                }

                // Clear the simulation running flag.
                if (IsSimulationRunning)
                {
                    IsSimulationRunning = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.StopSimulation(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The StepSimulation method is called to step through one iteration of the forest simulation.
        /// </summary>
        public async Task StepSimulation()
        {
            try
            {
                // Clear the simulation running flag.
                if (!IsSimulationRunning)
                {
                    await UpdateForest();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.StepSimulation(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The ResetSimulation method is called to reset the forest simulation.
        /// </summary>
        public void ResetSimulation()
        {
            try
            {
                StopSimulation();   // Stop the simulation (if it is running).  
                ResetForest();      // Reset the forest.
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.ResetSimulation(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The StartFireAtPoint method is called to start a fire at a specific point in the forest.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="forestWidthPixels"></param>
        /// <param name="forestHeightPixels"></param>
        public void StartFireAtPoint(Point point, double forestWidthPixels, double forestHeightPixels)
        {
            try
            {
                if (point != null && forestWidthPixels > 0 && forestHeightPixels > 0)
                {
                    // Retrieve the x/y coordinates in pixels.
                    double xPositionPixels = point.X;
                    double yPositionPixels = point.Y;

                    // Convert the x/y coordinates from pixels to cells.
                    int xPosition = (int)((xPositionPixels / forestWidthPixels) * Constants.ForestWidth);
                    int yPosition = (int)((yPositionPixels / forestHeightPixels) * Constants.ForestHeight);

                    // Determine the cell index from the x/y coordinates.
                    int cellIndex = xPosition + (yPosition*Constants.ForestWidth);

                    // Retrieve the forest cell and set it to burning.
                    if (IsCellIndexValid(cellIndex))
                    {
                        ForestCell forestCell = _forestCells.ElementAt(cellIndex);
                        if (forestCell.CellState == CellState.Tree)
                        {
                            forestCell.CellState = CellState.Burning;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.StartFireAtPoint(Point point, double forestWidthPixels, double forestHeightPixels): " + ex.ToString());
            }
        }

        /// <summary>
        /// The StepTimerEventHandler method is called when the step timer expires.
        /// It updates the current state of the forest.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StepTimerEventHandler(object sender, EventArgs e)
        {
            try
            {
                if (IsSimulationRunning)
                {
                    await UpdateForest();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.StepTimerEventHandler(object sender, EventArgs e): " + ex.ToString());
            }
        }

        /// <summary>
        /// The UpdateForest method is called to update the forest.
        /// An empty cell may become a tree cell, as per the regrowth probability.
        /// A burning cell becomes an empty cell.
        /// A tree cell becomes a burning cell, if any of its neighbours are burning.
        /// A tree cell may become a burning cell, even with no burning neighbours, as per the lightning probability.
        /// </summary>
        private async Task UpdateForest()
        {
            try
            {
                await Task.Run(() =>
                {
                    lock (_updateLock)
                    {
                    
                        ObservableCollection<ForestCell> updatedForest = new ObservableCollection<ForestCell>();    // The updated forest.

                        // Update the forest cells, one cell at a time.
                        foreach (ForestCell forestCell in _forestCells)
                        {
                            ForestCell updatedForestCell = new ForestCell(forestCell);  // Make a copy of the current forest cell.

                            // Update the forest cell.
                            if (forestCell.CellState == CellState.Empty)
                            {
                                // If the cell is empty, apply regrowth.
                                updatedForestCell.CellState = ApplyRegrowth() ? CellState.Tree : updatedForestCell.CellState;
                            }
                            else if (forestCell.CellState == CellState.Tree)
                            {
                                // If the cell is a tree, check for any burning neighbours.

                                int cellIndex = _forestCells.IndexOf(forestCell);   // Retrieve the index of the tree cell.

                                // Determine the indexes for all of the neighbours.
                                int topNeighbourIndex = cellIndex - Constants.ForestWidth;
                                int topRightNeighbourIndex = cellIndex - Constants.ForestWidth + 1;
                                int rightNeighbourIndex = cellIndex + 1;
                                int bottomRightNeighbourIndex = cellIndex + Constants.ForestWidth + 1;
                                int bottomNeighbourIndex = cellIndex + Constants.ForestWidth;
                                int bottomLeftNeighbourIndex = cellIndex + Constants.ForestWidth - 1;
                                int leftNeighbourIndex = cellIndex - 1;
                                int topLeftNeighbourIndex = cellIndex - Constants.ForestWidth - 1;

                                // Determine if the cell is on the top/right/bottom/left edge of the forest - certain neighbours must be ignored if a cell is on an edge.
                                bool topEdge = cellIndex < Constants.ForestWidth ? true : false;
                                bool rightEdge = ((cellIndex + 1) % Constants.ForestWidth) == 0 ? true : false;
                                bool leftEdge = (cellIndex % Constants.ForestWidth) == 0 ? true : false;
                                bool bottomEdge = (cellIndex + Constants.ForestWidth) >= (Constants.ForestWidth * Constants.ForestHeight) ? true : false;

                                // Check each neighbour.
                                if (!topEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(topNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!rightEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(topRightNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!rightEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(rightNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!rightEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(bottomRightNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!bottomEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(bottomNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!leftEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(bottomLeftNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!leftEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(leftNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }
                                if (!leftEdge && updatedForestCell.CellState != CellState.Burning && IsCellBurning(topLeftNeighbourIndex))
                                {
                                    updatedForestCell.CellState = CellState.Burning;
                                }

                                // If the cell is still a tree, apply a lighting strike.
                                if (updatedForestCell.CellState == CellState.Tree)
                                {
                                    updatedForestCell.CellState = ApplyLightningStrike() ? CellState.Burning : updatedForestCell.CellState;
                                }
                            }
                            else if (forestCell.CellState == CellState.Burning)
                            {
                                // If the cell is burning, it becomes an empty cell.
                                updatedForestCell.CellState = CellState.Empty;
                            }

                            updatedForest.Add(updatedForestCell);   // Add the updated forest cell to the updated forest.
                        }

                        // Update the forest.
                        if (updatedForest.Count == _forestCells.Count)
                        {
                            foreach (ForestCell forestCell in _forestCells)
                            {
                                CellState updatedCellState = updatedForest.ElementAt(_forestCells.IndexOf(forestCell)).CellState;
                                if (updatedCellState != forestCell.CellState)
                                {
                                    forestCell.CellState = updatedCellState;
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Forest.UpdateForest(): " + ex.ToString());
            }
        }

        /// <summary>
        /// The IsCellIndexValid method is called to determine if a provided cell index is within range of the forest cell collection.
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        private bool IsCellIndexValid(int cellIndex)
        {
            return cellIndex >= 0 && cellIndex < _forestCells.Count;
        }

        /// <summary>
        /// The IsCellBurning method is called to determine if a provided cell is burning.
        /// The cell is checked via its index.
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        private bool IsCellBurning(int cellIndex)
        {
            return IsCellIndexValid(cellIndex) && _forestCells.ElementAt(cellIndex).CellState == CellState.Burning;
        }

        /// <summary>
        /// The ApplyRegrowth method is called to apply regrowth to a forest cell, as per the regrowth probability.
        /// Return true is regrowth was successful. Returns false otherwise.
        /// </summary>
        /// <returns></returns>
        private bool ApplyRegrowth()
        {
            if (_regrowthProbability == Constants.ZeroPercentProbability)
            {
                // Zero percent chance of regrowth.
                return false;
            }
            else if (_regrowthProbability == Constants.OneHundredPercentProbability)
            {
                // One hundred percent chance of regrowth.
                return true;
            }
            else if (_regrowthProbability >= Constants.MinimumProbability && _regrowthProbability < Constants.MaximumProbability)
            {
                // Select a random number between the min/max range (1-99).
                int randomNumber = _random.Next(Constants.MinimumProbability, Constants.MaximumProbability);

                if (randomNumber <= _regrowthProbability)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// The ApplyLightningStrike method is called to apply a lightning strike to a forest cell, as per the lighting probability.
        /// Return true is lightning strikes. Returns false otherwise.
        /// </summary>
        /// <returns></returns>
        private bool ApplyLightningStrike()
        {
            if (_lightingProbability == Constants.ZeroPercentProbability)
            {
                // Zero percent chance of lightning.
                return false;
            }
            else if (_lightingProbability == Constants.OneHundredPercentProbability)
            {
                // One hundred percent chance of lightning.
                return true;
            }
            else if (_lightingProbability >= Constants.MinimumProbability && _lightingProbability < Constants.MaximumProbability)
            {
                // Select a random number between the min/max range (1-99).
                int randomNumber = _random.Next(Constants.MinimumProbability, Constants.MaximumProbability);

                if (randomNumber <= _lightingProbability)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
