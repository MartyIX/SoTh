using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Sokoban.Solvers;
using Sokoban.Solvers.XmlGetRoundSolution;
using Sokoban.Lib.Exceptions;

namespace Sokoban.View
{
    public partial class Solvers
    {
        private void SolutionFromWeb_Click(object sender, RoutedEventArgs e)
        {
            if (gameServerCommunication != null)
            {
                int roundsID = 0;
                object mazeID = null;
                string movementsSoFar = null;
                string maze = null;
                bool success = true;
                uint width = 0;
                uint height = 0;

                dataGridItemsSource.Clear();

                try
                {
                    roundsID = (int)solverProvider.GetIdentifier(SolverProviderIdentifierType.RoundsID);
                    mazeID = solverProvider.GetIdentifier(SolverProviderIdentifierType.DocumentPaneInstance);
                    width = solverProvider.GetMazeWidth();
                    height = solverProvider.GetMazeHeight();
                    maze = solverProvider.SerializeMaze();
                    
                    movementsSoFar = solverProvider.MovementsSoFar;
                }
                catch (NoRoundIsOpenException)
                {
                    showMessage("No round is opened.");
                    this.SolverStatus = "No round is opened.";
                    success = false;
                }
                catch (NotStandardSokobanVariantException)
                {
                    showMessage("The opened round is not standard Sokoban variant and cannot be solved.");
                    this.SolverStatus = "The opened round is not standard Sokoban variant and cannot be solved.";
                    success = false;
                }

                if (success)
                {
                    string output = gameServerCommunication.GetRequestOnServer("Solvers",
                        "/remote/GetRoundSolution/?rounds_id=" + roundsID + "&movementsSoFar=" + movementsSoFar, null);

                    if (gameServerCommunication.LastCommunicationError != String.Empty)
                    {
                        showMessage(gameServerCommunication.LastCommunicationError);
                    }
                    else
                    {
                        XmlGetRoundSolution xml = new XmlGetRoundSolution();
                        bool isParsed = true;

                        try
                        {
                            xml.Parse(output);
                        }
                        catch (InvalidStateException)
                        {
                            this.SolverStatus = "The answer of game server is malformed.";
                            isParsed = false;
                        }

                        if (isParsed == true)
                        {
                            if (xml.Success)
                            {
                                solverWorkCompleted("Game server", mazeID, width, height, maze, xml.Solution);
                            }
                            else
                            {
                                showMessage("Unfortunately, no solution was found on the game server.");
                                this.SolverStatus = "No solution was found on the game server.";
                            }
                        }
                    }
                }
            }
            else
            {
                showMessage("Game server communication is not initialized in `solvers' module'.");
            }
        }
    }
}
