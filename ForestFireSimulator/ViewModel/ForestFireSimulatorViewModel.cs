using ForestFireSimulator.Common;
using ForestFireSimulator.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForestFireSimulator.ViewModel
{
    /// <summary>
    /// The ForestFireSimulatorViewModel class represents the View Model for the Forest Fire Simulator.
    /// </summary>
    public class ForestFireSimulatorViewModel : NotificationBase
    {
        #region Fields
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ForestFireSimulatorViewModel()
        {
            try
            {
                Forest = new Forest();  // Initialise the model class.
                Forest.PropertyChanged += OnForestPropertyChanged;

                // Initialise commands.
                StartSimulationCommand = new DelegateCommand(OnStartSimulation, CanStartSimulation);
                StopSimulationCommand = new DelegateCommand(OnStopSimulation, CanStopSimulation);
                StepSimulationCommand = new DelegateCommand(OnStepSimulation, CanStepSimulation);
                ResetSimulationCommand = new DelegateCommand(OnResetSimulation);
                StartFireCommand = new DelegateCommand(OnStartFire, CanStartFire);
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel(): " + ex.ToString());
            }
        }
        
        #endregion

        #region Events
        #endregion

        #region Properties

        /// <summary>
        /// Gets the Forest model class.
        /// </summary>
        public Forest Forest { get; }

        /// <summary>
        /// Gets or sets the start simulation command.
        /// </summary>
        public DelegateCommand StartSimulationCommand { get; private set; }

        /// <summary>
        /// Gets or sets the stop simulation command.
        /// </summary>
        public DelegateCommand StopSimulationCommand { get; private set; }

        /// <summary>
        /// Gets or sets the step simulation command.
        /// </summary>
        public DelegateCommand StepSimulationCommand { get; private set; }

        /// <summary>
        /// Gets or sets the reset simulation command.
        /// </summary>
        public DelegateCommand ResetSimulationCommand { get; private set; }

        /// <summary>
        /// Gets or sets the start fire command.
        /// </summary>
        public DelegateCommand StartFireCommand { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The OnStartSimulation method is called to start the simulation.
        /// </summary>
        /// <param name="arg"></param>
        public void OnStartSimulation(object arg)
        {
            try
            {
                if (Forest != null)
                {
                    Forest.StartSimulation();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel.OnStartSimulation(object arg): " + ex.ToString());
            }
        }

        /// <summary>
        /// The CanStartSimulation method is callled to determine if the simulation can start.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool CanStartSimulation(object arg)
        {
            return Forest != null && !Forest.IsSimulationRunning;
        }

        /// <summary>
        /// The OnStopSimulation method is called to stop the simulation.
        /// </summary>
        /// <param name="arg"></param>
        public void OnStopSimulation(object arg)
        {
            try
            {
                if (Forest != null)
                {
                    Forest.StopSimulation();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel.OnStopSimulation(object arg): " + ex.ToString());
            }
        }

        /// <summary>
        /// The CanStopSimulation method is callled to determine if the simulation can stop.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool CanStopSimulation(object arg)
        {
            return Forest != null && Forest.IsSimulationRunning;
        }

        /// <summary>
        /// The OnStepSimulation method is called to step the simulation.
        /// </summary>
        /// <param name="arg"></param>
        public async void OnStepSimulation(object arg)
        {
            try
            {
                if (Forest != null)
                {
                    await Forest.StepSimulation();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel.OnStepSimulation(object arg): " + ex.ToString());
            }
        }

        /// <summary>
        /// The CanStepSimulation method is callled to determine if the simulation can step.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool CanStepSimulation(object arg)
        {
            return Forest != null && !Forest.IsSimulationRunning;
        }

        /// <summary>
        /// The OnResetSimulation method is called to reset the simulation.
        /// </summary>
        /// <param name="arg"></param>
        public void OnResetSimulation(object arg)
        {
            try
            {
                if (Forest != null)
                {
                    Forest.ResetSimulation();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel.OnResetSimulation(object arg): " + ex.ToString());
            }
        }

        /// <summary>
        /// The OnStartFire method is start a fire at the left mouse button click position.
        /// </summary>
        /// <param name="arg"></param>
        public void OnStartFire(object arg)
        {
            try
            {
                if (Forest != null)
                {
                    Point mousePoint = Mouse.GetPosition((IInputElement)arg);       // Get the mouse position (in pixels).
                    double forestWidthPixels = ((ItemsControl)arg).ActualWidth;     // Get the forest width (in pixels).
                    double forestHeightPixels = ((ItemsControl)arg).ActualHeight;   // Get the forest height (in pixels).

                    Forest.StartFireAtPoint(mousePoint, forestWidthPixels, forestHeightPixels);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel.OnStartFire(object arg): " + ex.ToString());
            }
        }

        /// <summary>
        /// The CanStartFire method is callled to determine if a fire can be started.
        /// </summary>
        public bool CanStartFire(object arg)
        {
            return Forest != null;
        }

        /// <summary>
        /// The OnForestPropertyChanged method is called when a property in the Forest model class changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnForestPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                StartSimulationCommand.RaiseCanExecuteChanged();
                StopSimulationCommand.RaiseCanExecuteChanged();
                StepSimulationCommand.RaiseCanExecuteChanged();
                ResetSimulationCommand.RaiseCanExecuteChanged();
                StartFireCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                throw new Exception("ForestFireSimulatorViewModel.OnForestPropertyChanged(object sender, PropertyChangedEventArgs e): " + ex.ToString());
            }
        }

        #endregion
    }
}
