using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Sokoban.Lib.Exceptions;
using Sokoban.Solvers;

namespace Sokoban.View
{
    public partial class Solvers
    {
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            dataGridItemsSource.Clear();

            if (cbCurrentSolver.Items.Count == 0)
            {
                SolverStatus = "No solver is available";
            }
            else
            {
                SolverStatus = "Solver is running";

                disableStartOfSolver();

                try
                {
                    // We specify that we want to get results via method solverWorkCompleted
                    solversManager.SolveRound(new SolversManager.SolverWorkCompletedDel(solverWorkCompleted));
                }
                catch (SolverException ex)
                {
                    enableStartOfSolver();
                    SolverStatus = "Solver cannot finish its work.";
                    addItemToMessageLog(0, CurrentSolver, ex.Message);
                }
                catch (NotStandardSokobanVariantException ex)
                {
                    enableStartOfSolver();
                    SolverStatus = "Solver cannot solve this game variant.";
                    addItemToMessageLog(0, CurrentSolver, ex.Message);
                    this.showMessage("The opened round is not standard Sokoban variant. Solvers can solve only the variant. We're sorry for the inconvenience.");
                }
                catch (NoRoundIsOpenException)
                {
                    enableStartOfSolver();
                    SolverStatus = "No round is opened.";
                    this.showMessage("No round is opened. For solving rounds open a standard variant round and click \"Start\" button again.");
                }
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                solversManager.TerminateSolver();

            }
            catch (NotImplementedException)
            {
                showMessage("This solver doesn't support termination.");
            }
        }


        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            solversManager.ShowConfigure();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            solversManager.ShowAbout();
        }
    }
}
