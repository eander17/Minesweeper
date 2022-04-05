using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eander17Minesweeper
{
    public partial class Cell : UserControl
    {
        public EventHandler OnCellClick;

        Panel myPanel;
        Button myButton;
        Label myLabel; 
        Size cellSize = new Size(50, 50);
        int row;
        int col;

        public Cell()
        {
            InitializeComponent();
            this.Size = cellSize;
            this.Padding = new Padding(0);
            SetUpButton();
            SetUpLabel();
            SetUpPanel();
 
        }



        public Color CellColor { get => myPanel.BackColor; set => myPanel.BackColor = value; }
        public Button MyButton { get => myButton; }
        public int Row { get => row; set => row = value; }
        public int Col { get => col; set => col = value; }
        public Label MyLabel { get => myLabel; set => myLabel = value; }

        /// <summary>
        /// sets up buttons giving them their size, location, color, and event listener. 
        /// </summary>
        private void SetUpButton()
        {
            myButton = new Button();
            myButton.Location = this.Location;
            myButton.Size = this.Size;
            myButton.BackColor = ColorTranslator.FromHtml("#FF0075");
            myButton.Click += OnButtonClick; 
         
            this.Controls.Add(myButton);
        }

        /// <summary>
        /// sets up panels giving them their size, location, borders, and background color. 
        /// </summary>
        private void SetUpPanel()
        {
            myPanel = new Panel();
            myPanel.Size = this.Size;
            myPanel.Location = this.Location;
            myPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; 
            myPanel.BackColor = ColorTranslator.FromHtml("#77D970");
            myPanel.Controls.Add(myLabel); 
            this.Controls.Add(myPanel);
        }

        /// <summary>
        /// sets up labels giving them their size, location, text, and alignment. 
        /// </summary>
        private void SetUpLabel()
        {
            myLabel = new Label();
            myLabel.Size = this.Size;
            myLabel.Location = this.Location;
            myLabel.Text = " "; 
            myLabel.Font = new Font("Sans Serif", 24);
            myLabel.TextAlign = ContentAlignment.MiddleCenter; 
            this.Controls.Add(myPanel); 
        }

        /// <summary>
        /// triggered when the button is clicked. Sets buttons visibility to false. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnButtonClick(object sender, EventArgs e)
        {
         
            ((Button)sender).Visible = false;
            if (OnCellClick != null)
            {
                OnCellClick(this, EventArgs.Empty);
            }
            
        }
    }
}
