///
///Evan Anderson
///
///MINESWEEPER: 
///features of program: 
///     create minesweeper in WinForms. 
///     must have 10 bombs randomly placed on a 10X10 board
///     Game is over if user clicks a bomb or eliminates all other tiles. 
///     A save file is utilized to save statistics from previous games. 
///     Uses a menu to start game, view stats, read instructions, and read the AboutMe
///     User is able to start a new game after end of game, or quit application. 
///     


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eander17Minesweeper
{
    public partial class Form1 : Form
    {
        int runtime = 0;
        static int wins = 0;
        static int losses = 0;
        static float avgTime = 0;
        static int totalTime = 0; 

        Cell[,] grid = new Cell[10, 10];
        Random rand = new Random(); 

        public Form1()
        {
            InitializeComponent();
            ReadFile(); //initialize statistic variables
            InitializeTimer(); 
            InitializeGrid();  //set up grid objects
            newGameToolStripMenuItem.PerformClick(); 

        }

        #region Timer stuff

        /// <summary>
        /// sets timer interval to 1 second and subscribes timer to OnTimer_Click method. 
        /// </summary>
        private void InitializeTimer()
        {
            timer1.Interval = 1000;
            timer1.Tick += OnTimer_Tick; 
        }

        private void ResetTimer()
        {
            runtime = 0;
            timerLbl.Text = $"Timer: {runtime}"; 
        }

        /// <summary>
        /// every tick interval, increments the runtime variable by 1 (counts seconds) and updates the timer label on the status bar. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTimer_Tick(object sender, EventArgs e)
        {
            runtime++;
            timerLbl.Text = $"Timer: {runtime}"; 
        }

        #endregion

        #region Menu items

        /// <summary>
        /// method to initialize the grid or restart it on the "New Game" menu strip button click. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1. reset timer (dont start it)
            ResetTimer();
            
            //places 10 bombs on the board randomly. 
            for (int i = 0; i < 10; i++)
            {
                PlaceBomb(); 
            }
            PlaceNumbers(); //place numbers according to adjacency to bombs.  
        }

        /// <summary>
        /// exits the program when the Quit button in the menu strip is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        /// <summary>
        /// Event listener method for statistics menu item. 
        /// Opens a message box that tells user the basic statistics. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //stop the timer so user can look at stats. 
            timer1.Stop();
            //strings will contain the words written on messageBox. 
            string message = $"Wins: {wins} \nLosses: {losses} \nW/L ratio: {wins/losses} \nAverage Time: {avgTime}";
            string title = "Statistics";
            MessageBox.Show(message, title); //simple message box to display statistics. 
            if (runtime != 0) //if game has already started, this unpauses. Otherwise is not triggered. 
                timer1.Start();
        }

        /// <summary>
        /// event listener method that is triggered when the "About" button is pressed on the menu strip. 
        /// displays a message box telling the user who made the program. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pauses timer if it is running. 
            timer1.Stop();
            string message = $"Minesweeper by Evanio \nCreated for Malec's CS3020 ADVANCED OO programming \nest.2021";
            string title = "About the minesweeper";
            MessageBox.Show(message, title);

            if (runtime != 0) //restarts timer if game was already running, otherwise is not triggered. 
                timer1.Start();
        }

        /// <summary>
        /// event listener method that is triggered when user presses the "instructions" button. 
        /// displays a message box that tells the user how to play the game. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pause game. 
            timer1.Stop();
            string message = $"The goal of Minesweeper is to clear the field of empty spaces, leaving only the mines unclicked. " +
                $"\nStart by clicking on a square, if there is a mine adjacent to a box, it will show a number indicating the number of mines it is adjacent to." +
                $"\nUse EXPERT LOGIC to figure out where the bombs are, leaving them unclicked." +
                $"\nOnce all other spaces have been clicked, the game is over and you won." +
                $"\nIf you click a bomb, you will lose and the game will be over." +
                $"\nGOOD LUCK!";
            string title = "How to play minesweeper";
            MessageBox.Show(message, title);

            if (runtime != 0) //unpauses if in middle of game, isn't triggered otherwise. 
                timer1.Start();
        }

        /// <summary>
        /// event listener method that is triggered when user presses the "reset statistics" button on menu strip. 
        /// Wipes the UserSave text file and resets variables to zero. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset Statistics?", "WARNING", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ResetFile();
            }
        }


        #endregion


        #region grid stuff
        
        /// <summary>
        /// method that resets the Cell grid to default values. 
        /// </summary>
        private void ResetGrid()
        {
            for (int row = 0; row < grid.GetLength(1); row++)
            {
                for (int col = 0; col < grid.GetLength(0); col++)
                {
                    grid[col, row].MyButton.Visible = true; //makes buttons visible again. 
                    grid[col, row].MyButton.Enabled = true; //re-enables buttons. 
                    grid[col, row].MyLabel.Text = " ";      //makes all label text read a blank space. 
                    grid[col, row].CellColor = ColorTranslator.FromHtml("#77D970"); //change color of cell. 
                }
            }
        }

        /// <summary>
        /// Initializes the grid for the first time. 
        /// Sets the size, location, and creates the cell objects for entire grid. 
        /// </summary>
        private void InitializeGrid()
        {
            for (int row = 0; row < grid.GetLength(1); row++)
            {
                for (int col = 0; col < grid.GetLength(0); col++)
                {
                    Cell temp = new Cell(); //temporary cell object. 
                    temp.Location = new Point(col * temp.Size.Width, row * temp.Size.Height+40); //set location of cell object. 
                    temp.Row = row;
                    temp.Col = col;
                    temp.OnCellClick += OnCellClick; 
                    this.Controls.Add(temp);
                    grid[col, row] = temp;
                }
            }
        }

       
        /// <summary>
        /// Aggregates all methods that check different directions to cascade buttons. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckAllDirections(string targetString, int row, int col)
        {
            CheckLeft(targetString, row, col);
            CheckRight(targetString, row, col);
            CheckUp(targetString, row, col);
            CheckDown(targetString, row, col);
            CheckNE(targetString, row, col);
            CheckSE(targetString, row, col);
            CheckSW(targetString, row, col);
            CheckNW(targetString, row, col);
        }


        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckRight(string targetString, int row, int col)
        {
             
            if (col < grid.GetLength(0) - 1)
            {
                if (grid[col + 1, row].MyLabel.Text == " ")
                {
                    grid[col + 1, row].MyButton.PerformClick(); //clicks grid location if there is a space in the label field. 
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col + 1, row].MyLabel.Text != "B") 
                {
                    grid[col + 1, row].MyButton.Enabled = false;
                    grid[col + 1, row].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckLeft(string targetString, int row, int col)
        {
            if (col > 0)
            {
                if (grid[col - 1, row].MyLabel.Text == targetString)
                {
                    grid[col - 1, row].MyButton.PerformClick();
                }
                else if(checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col - 1, row].MyLabel.Text != "B")
                {
                    grid[col - 1, row].MyButton.Enabled = false;
                    grid[col - 1, row].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckDown(string targetString, int row, int col)
        {
            if (row < grid.GetLength(1) - 1)
            {
                if (grid[col, row + 1].MyLabel.Text == targetString)
                {
                    grid[col, row + 1].MyButton.PerformClick();
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col, row + 1].MyLabel.Text != "B")
                {
                    grid[col, row + 1].MyButton.Enabled = false;
                    grid[col, row + 1].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckUp(string targetString, int row, int col)
        {
            if (row > 0)
            {
                if (grid[col, row - 1].MyLabel.Text == targetString)
                {
                    grid[col, row - 1].MyButton.PerformClick();
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col, row - 1].MyLabel.Text != "B")
                {
                    grid[col, row - 1].MyButton.Enabled = false;
                    grid[col, row - 1].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckNE(string targetString, int row, int col)
        {
            if ((col < grid.GetLength(0) - 1) && (row < grid.GetLength(1) - 1))
            {
                if (grid[col + 1, row + 1].MyLabel.Text == targetString)
                {
                    grid[col + 1, row + 1].MyButton.PerformClick();
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col + 1, row + 1].MyLabel.Text != "B")
                {
                    grid[col + 1, row + 1].MyButton.Enabled = false;
                    grid[col + 1, row + 1].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckSE(string targetString, int row, int col)
        {
            if ((col < grid.GetLength(0) - 1) && (row > 0))
            {
                if (grid[col + 1, row - 1].MyLabel.Text == targetString)
                {
                    grid[col + 1, row - 1].MyButton.PerformClick();
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col + 1, row - 1].MyLabel.Text != "B")
                {
                    grid[col + 1, row - 1].MyButton.Enabled = false;
                    grid[col + 1, row - 1].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckSW(string targetString, int row, int col)
        {
            if (col > 0 && row > 0)
            {
                if (grid[col - 1, row - 1].MyLabel.Text == targetString)
                {
                    grid[col - 1, row - 1].MyButton.PerformClick();
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " &&  grid[col - 1, row - 1].MyLabel.Text != "B")
                {
                    grid[col - 1, row - 1].MyButton.Enabled = false;
                    grid[col - 1, row - 1].MyButton.Visible = false;
                }
            }
        }

        /// <summary>
        /// checks grid adjacent grid location for a bomb, performing a click if there is only a space there. 
        /// If there is a number in that location, button is disabled and made invisible, effectively clicking it but not allowing the continued cascading. 
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckNW(string targetString, int row, int col)
        {
            if (col > 0 && (row < grid.GetLength(1) - 1))
            {
                if (grid[col - 1, row + 1].MyLabel.Text == targetString)
                {
                    grid[col - 1, row + 1].MyButton.PerformClick();
                }
                else if (checkForBomb(row, col) == 0 && grid[col, row].MyLabel.Text == " " && grid[col - 1, row + 1].MyLabel.Text != "B")
                {
                    grid[col - 1, row + 1].MyButton.Enabled = false;
                    grid[col - 1, row + 1].MyButton.Visible = false;
                }
            }
        }

        #endregion

        #region game logic

        /// <summary>
        /// event listener is triggered when a cell is clicked. 
        /// Checks for win/lose conditions as well as adjacency. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCellClick(object sender, EventArgs e)
        {
            //starts timer if it hasn't started already. 
            timer1.Start();
            string targetString = " ";
            int row = ((Cell)sender).Row;
            int col = ((Cell)sender).Col;
            //if-else chain first checks if user clicked a bomb, then checks if user won, then checks if there are any adjacent bombs. 
            if (CheckClickedbomb(row, col))
                GameOver(row, col);
            else if (CheckForWin())
                GameWon(row, col);
            else
                CheckAllDirections(targetString, row, col);
        }

        /// <summary>
        /// Checks to see if clicked button contains a bomb underneath. 
        /// Returns true if there is a bomb and false if there is not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckClickedbomb(int row, int col)
        {
            return grid[col, row].MyLabel.Text == "B" ? true : false;
        }

        /// <summary>
        /// Method that checks to see if win conditions are met. 
        /// Win condition is: all tiles that are not bombs have been clicked. 
        /// Returns true if conditions are met, false otherwise. 
        /// </summary>
        /// <returns></returns>
        private bool CheckForWin()
        {
            //nested for loop iterates through grid. 
            for (int r = 0; r < grid.GetLength(1); r++)
            {
                for (int c = 0; c < grid.GetLength(0); c++)
                {
                    if (!CheckClickedbomb(r, c)) //if there is no bomb on this tile. 
                    {
                        //checking to see if the button at grid location has been pressed or not by checking whether it is visible. If visible, it has not been clicked and thus returns false. 
                        if (grid[c, r].MyButton.Visible == true)
                            return false;
                    }
                }
            }
            return true;  //all non-bomb tiles' buttons have been clicked, returning true
        }

        /// <summary>
        /// method that is triggered when CheckForWin returns true. 
        /// stops the timer, updates the statistics variables, and displays a message to the user letting them know if they won. 
        /// Message box asks user if they want to play again, resetting the game if yes and exiting game if no. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void GameWon(int row, int col)
        {
            //stops timer. 
            timer1.Stop();
            wins++;
            totalTime += runtime;
            avgTime = totalTime / (losses + wins);
            if (MessageBox.Show("Play again?", "You Won!", MessageBoxButtons.YesNo) == DialogResult.Yes) //message box to let user know they won, and asks them if they want to play again. 
            {
                //if yes, the grid is reset and a click is performed on the "new game" menu strip item, effectively restarting the game. 
                ResetGrid();
                newGameToolStripMenuItem.PerformClick();
            }
            else
            {
                //calls write file method and closes application. 
                WriteFile();
                quitToolStripMenuItem.PerformClick();
            }
        }

        /// <summary>
        /// Method that is triggered if user loses the game. 
        /// Method updates statistics variables, sets clicked bomb back color to red, and shows all bombs. 
        /// Finally asks user if they want to play again. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void GameOver(int row, int col)
        {
            timer1.Stop();
            losses++;
            totalTime += runtime;
            avgTime = totalTime / (losses + wins);
            grid[col, row].CellColor = Color.Red;
            for (int r = 0; r < grid.GetLength(1); r++)
            {
                for (int c = 0; c < grid.GetLength(0); c++)
                {
                    //finds all hidden bombs. 
                    if (CheckClickedbomb(r, c)) //checks grid location for a bomb, triggers if statement if true. 
                    {
                        grid[c, r].MyButton.Visible = false; //makes button at grid location invisible. 
                    }
                }
            }
            if (MessageBox.Show("Play again?", "Game Over!", MessageBoxButtons.YesNo) == DialogResult.Yes) //message box asking user if they want to play again. 
            {
                //resets the grid and starts game over. 
                ResetGrid();
                newGameToolStripMenuItem.PerformClick();
            }
            else
            {
                //writes statistics to UserSave file and exits application. 
                WriteFile();
                quitToolStripMenuItem.PerformClick();
            }
        }


        #endregion

        #region Save File Stuff

        /// <summary>
        /// method that opens file and reads the contents, saving them to respective variables. 
        /// </summary>
        public static void ReadFile()
        {
            //create file if there is none, does not alter text if file already exists. 
            StreamWriter w = File.AppendText("UserSave.txt");
            w.Close(); //closes streamwriter. 
            StreamReader reader = new StreamReader("UserSave.txt"); 
            
            try
            {
                //reader.ReadLine(); 
                while(!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split(','); //separates values between commas. 
                    wins = int.Parse(data[0]); //set value of wins
                    losses = int.Parse(data[1]); //set value of losses
                    avgTime = float.Parse(data[2]); //set value of average time
                    totalTime = int.Parse(data[3]); //set value of total time. 
                }
                reader.Close(); //closes streamreader. 
            }catch(Exception e) //just in case something breaks. 
            {
                Console.WriteLine(e.Message); 
            }
        }

        /// <summary>
        /// method that writes to file. Overwrites what was previously written. 
        /// </summary>
        public static void WriteFile()
        {
            StreamWriter writer = new StreamWriter("UserSave.txt"); //create streamwriter variable to write into UserSave text file. 
            writer.Write($"{wins},{losses},{avgTime},{totalTime}"); //writes variables into text file. 
            writer.Close(); //closes streamwriter. 
        }

        /// <summary>
        /// resets text file as well as all statistics variables. 
        /// </summary>
        private void ResetFile()
        {
            File.WriteAllText("UserSave.txt", string.Empty);
            wins = 0;
            losses = 0;
            avgTime = 0;
            totalTime = 0;
        }

        #endregion

        #region Placing things

        /// <summary>
        /// method that places a bomb at a random location. 
        /// Repeats until a valid (empty) location is found. 
        /// </summary>
        private void PlaceBomb()
        {
            int r, c; //coordinate variables. 
            r = rand.Next(10); 
            c = rand.Next(10); 

            while(!CheckEmptySpace(r,c)) //repeats while non-empty spaces are being picked. 
            {
                r = rand.Next(10);
                c = rand.Next(10); 
            }
            //set a label = to B. 
            grid[c, r].MyLabel.Text = "B"; 
        }

        /// <summary>
        /// checks to see whether there is a bomb at this location. returns true if there is no bomb, false if there is a bomb. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckEmptySpace(int row, int col)
        {
            return grid[col, row].MyLabel.Text == "B" ? false : true; 
        }

        /// <summary>
        /// Places numbers that correspond to number of adjacent bombs in grid coordinate. 
        /// </summary>
        private void PlaceNumbers()
        {
            int placedNum = 0; 
            for (int row = 0; row < grid.GetLength(1); row++)
            {
                for (int col = 0; col < grid.GetLength(0); col++)
                {
                    if(grid[col,row].MyLabel.Text != "B") //only triggered if grid location is not a B
                    {
                        //checkforbomb method returns number of adjacent bombs. 
                        placedNum = checkForBomb(row, col); 

                        if(placedNum != 0) //Only triggered if there is at least one bomb adjacent to grid coordinate. 
                        {
                            grid[col, row].MyLabel.Text = placedNum.ToString(); //places the string conversion of placedNum variable on label at grid coordinate. 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// checks every adjacent coordinate to given coordinate for a bomb returning the number of adjacent bombs found. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private int checkForBomb(int row, int col)
        {
            int numBombs = 0; 
            if(CheckBombRight(row, col))
            {
                numBombs++; 
            }
            if(CheckBombLeft(row, col))
            {
                numBombs++; 
            }
            if(CheckBombDown(row, col))
            {
                numBombs++; 
            }
            if(CheckBombUp(row, col))
            {
                numBombs++; 
            }
            if(CheckBombNE(row, col))
            {
                numBombs++; 
            }
            if(CheckBombSE(row, col))
            {
                numBombs++; 
            }
            if(CheckBombSW(row, col))
            {
                numBombs++; 
            }
            if(CheckBombNW(row, col))
            {
                numBombs++; 
            }
            return numBombs; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombRight(int row, int col)
        {

            if (col < grid.GetLength(0) - 1)
            {
                if (grid[col + 1, row].MyLabel.Text == "B") //if there is a bomb in this location. 
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombLeft(int row, int col)
        {
            if (col > 0)
            {
                if (grid[col - 1, row].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombDown(int row, int col)
        {
            if (row < grid.GetLength(1) - 1)
            {
                if (grid[col, row + 1].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombUp(int row, int col)
        {
            if (row > 0)
            {
                if (grid[col, row - 1].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombNE(int row, int col)
        {
            if ((col < grid.GetLength(0) - 1) && (row < grid.GetLength(1) - 1))
            {
                if (grid[col + 1, row + 1].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombSE(int row, int col)
        {
            if ((col < grid.GetLength(0) - 1) && (row > 0))
            {
                if (grid[col + 1, row - 1].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombSW(int row, int col)
        {
            if (col > 0 && row > 0)
            {
                if (grid[col - 1, row - 1].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        /// <summary>
        /// checks adjacent location for a bomb, returning true if said location contains a bomb and false if not. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool CheckBombNW(int row, int col)
        {
            if (col > 0 && (row < grid.GetLength(1) - 1))
            {
                if (grid[col - 1, row + 1].MyLabel.Text == "B")
                {
                    return true; 
                }
                return false; 
            }
            return false; 
        }

        #endregion


    }
}
