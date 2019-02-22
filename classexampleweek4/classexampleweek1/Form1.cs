using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace classexampleweek1
{
    public partial class Form1 : Form
    {
        Timer gameloop;
        sprite player;
        vehicle cars, trucks;
        public Rectangle safeGrass, startingArea;
        public Label lives, livesLbl, scores, scoresLbl;
        public Image bgForComputer = Image.FromFile("bgToComp.png");
        TextBox playerInitials = new TextBox();
        Button submitInitials = new Button();
        int score;
        public string pInitials;
        DateTime theDate = DateTime.Today;
        DateTime theTime = DateTime.Now;

        bool[] keys;

        // Form1() function
        //      The constructor for Form1
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            score = 0;

            // set up the gameloop timer
            gameloop = new Timer();
            gameloop.Interval = 25;
            gameloop.Tick += new EventHandler(gameloop_Tick);
            gameloop.Enabled = true;

            // instantiate the car and truck
            cars = new vehicle(new Point(75, 145), 50, 0, 0);
            trucks = new vehicle(new Point(150, 220), 50, 0, 0);

            // instantiate the player
            player = new sprite(new Point((this.Width / 2) - 62, 15), 45, 0, 0);

            // load the image map of the player, and the images of the car and truck, respectively
            player.loadimagemap();
            cars.loadimages();
            trucks.loadimages();

            // set up the lives label for the user to see how many lives the user have left (next 13 lines)
            lives = new Label();
            lives.Font = new Font("Arial", 14, FontStyle.Bold);
            lives.BackColor = Color.Transparent;
            lives.ForeColor = Color.White;
            lives.Text = "Lives:";
            lives.Location = new Point(5, 25);

            livesLbl = new Label();
            livesLbl.Font = new Font("Arial", 14, FontStyle.Bold);
            livesLbl.BackColor = Color.Transparent;
            livesLbl.ForeColor = Color.White;
            livesLbl.Text = player.livesLeft.ToString();
            livesLbl.Location = new Point(75, 25);
            livesLbl.Width = this.Width;

            // set up the score label for the user to show the player's score (next 13 lines)
            scores = new Label();
            scores.Font = new Font("Arial", 14, FontStyle.Bold);
            scores.BackColor = Color.Transparent;
            scores.ForeColor = Color.White;
            scores.Text = "Score:";
            scores.Location = new Point(5, 46);

            scoresLbl = new Label();
            scoresLbl.Font = new Font("Arial", 14, FontStyle.Bold);
            scoresLbl.BackColor = Color.Transparent;
            scoresLbl.ForeColor = Color.White;
            scoresLbl.Text = score.ToString();
            scoresLbl.Location = new Point(75, 46);

            // add the two lives labels and score labels to the screen
            this.Controls.Add(scoresLbl);
            this.Controls.Add(scores);
            this.Controls.Add(livesLbl);
            this.Controls.Add(lives);

            // 3 = Right, 2 = Left, 1 = Down, 0 = Up
            keys = new bool[4];

            // set all the keys bool values to false
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = false;
            }

            // the ending point - if the player reaches here, they are safe
            safeGrass = new Rectangle();
            safeGrass.Location = new Point(0, this.Height / 2 + 82);
            safeGrass.Height = 100;
            safeGrass.Width = this.Width;

            // the starting point - player must leave this area and cross the road to safeGrass
            startingArea = new Rectangle();
            startingArea.Location = new Point(0, 0);
            startingArea.Height = 110;
            startingArea.Width = this.Width;

            // set up if the user presses a key down, and then lifts up, while in the form
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);
        }

        // Form1_KeyDown() function
        //      Set all the keys pressed bool values to true, meaning the user has pressed a key and the user has not lifted a finger yet
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // if the user is moving to the right (pressing the right arrow key on keyboard)
                case Keys.Right:
                    keys[3] = true;
                    break;
                // if the user is moving to the left (pressing the left arrow key on keyboard)
                case Keys.Left:
                    keys[2] = true;
                    break;
                // if the user is moving to the down (pressing the down arrow key on keyboard)
                case Keys.Down:
                    keys[1] = true;
                    break;
                // if the user is moving to the up (pressing the up arrow key on keyboard)
                case Keys.Up:
                    keys[0] = true;
                    break;
            }

            // send the keys and score to the movePlayer function to move the player around the screen
            player.movePlayer(ref keys, ref score);
        }

        // Form1_KeyUp() function
        //      Set all the keys pressed bool values to false, meaning the user is no longer pressing any key
        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // set all the keys bool values to false
            for (int k = 0; k < 4; k++)
            {
                keys[k] = false;
            }
        }

        // gameloop_Tick() function
        //      On every tick of the timer, perform all the functions referenced within this function, and Invalidate() the graphics
        void gameloop_Tick(object sender, EventArgs e)
        {
            // update function of cars and trucks, to reset x position of the cars and trucks if they go past the form window's width
            cars.update(this.Width, 'c');
            trucks.update(this.Width, 't');

            // update function for the player, as well as checking if the player was hit by a vehicle or if they are safe (interesects with safeGrass rectangle)
            player.update();
            player.isHit(cars, trucks, ref score);
            player.isSafe(safeGrass);

            if (!player.beenWon)
            {
                // set the form's title to the user's score
                scoresLbl.Text = score.ToString();
            }
            
            Invalidate();
        }

        // OnPaint() function
        //      Override the built-in OnPaint() function to draw the background, safe and starting areas, vehicles, and players
        protected override void OnPaint(PaintEventArgs e)
        {
            // draw the background - computer will see black rectangle at bottom, that's safe area; user sees safeGrass rectangle (lime green rectangle)
            e.Graphics.DrawImage(bgForComputer, 0, 0);

            // draw the safe area and starting area rectangles to the form window
            e.Graphics.DrawRectangle(new Pen(Color.Green), safeGrass); // draw the safe area (goal for player)
            e.Graphics.FillRectangle(new SolidBrush(Color.LimeGreen), safeGrass); // fill in the safe area rectangle with a lime green color
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(128, 89, 19)), startingArea); // draw the starting area (brown-ish rectangle at top)
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 89, 19)), startingArea); // fill in the starting area rectangle with a brown color

            // draw the car and truck to the form window
            e.Graphics.DrawImage(cars.trucks, cars.r.Location);
            e.Graphics.DrawImage(trucks.cars, trucks.r.Location);

            // draw the player to the form window
            e.Graphics.DrawImage(player.player, player.r.Location);

            // set the text of livesLbl to the number of lives the user has left (starts at 3 lives)
            livesLbl.Text = player.livesLeft.ToString();

            if (player.beenWon)
            {
                livesLbl.Text = "YOU WON!! Click 'File' 'Save Score' to Save!";
            }
        }

        // Form1_Load() function
        //      Set some basic things up for the form on its load
        private void Form1_Load(object sender, EventArgs e)
        {
            // initially set the form's title to "Jogger!", and set the background color of the form
            this.Text = "Jogger!";
            this.BackColor = Color.FromArgb(195, 195, 195);

            // play the sound effect as the background noise upon the form's loading
            player.playEffect("traffic");
        }

        // saveScoreToolStripMenuItem_Click() function
        //      Create the text box for the user to enter initials, as well as set up a "Save Score" button, which will
        //      be written to the screen when the user clicks the submitInitials button
        private void saveScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // set up a text box to come on the screen for the user to enter their initials
            playerInitials.Location = new Point((this.Width / 2) - 80, (this.Height / 2) - 20);
            playerInitials.Font = new Font("Arial", 20);
            playerInitials.Width = 75;
            playerInitials.Text = "";

            // set up a button for the user to press to save their score and initials to an HTML score file
            submitInitials.Location = new Point(playerInitials.Location.X + 80, (this.Height / 2) - 21);
            submitInitials.BackColor = Color.AntiqueWhite;
            submitInitials.Text = "Save Score";
            submitInitials.TextAlign = ContentAlignment.MiddleCenter;
            submitInitials.Height = 40;
            submitInitials.Width = 75;

            // add the text box and button to the form window
            this.Controls.Add(playerInitials);
            this.Controls.Add(submitInitials);

            // create a click event for the submitInitials button
            submitInitials.Click += new EventHandler(submitInitials_Click);
        }

        // submitInitials() function
        //      If the user clicks on "Save Score" from the form, perform this function, which sets up the HTML scores file
        void submitInitials_Click(object sender, EventArgs e)
        {
            // set up the HTML scores file
            StreamWriter hsFile = new StreamWriter("highscores.html", append:true);

            // set the time (hour and minute) that the user played the game
            string todaysDate = theDate.ToString("d");
            int hour = theTime.Hour;
            int min = theTime.Minute;
            bool won = player.beenWon;
            string didWin;

            // shows the user on the HTML scores page if the user actually won or not
            if (won)
            {
                didWin = "YES";
            }
            else
            {
                didWin = "NO";
            }

            // write the HTML code to create and style the high scores page
            hsFile.WriteLine("<html>");
            hsFile.WriteLine("\t<head>");
            hsFile.WriteLine("\t\t<title>Jogger High Scores</title>");
            hsFile.WriteLine("\t\t<style type=\"text/css\">\n\t\t\tbody {\n\t\t\t\tbackground: #FFE533;\n\t\t\t}\n\t\t</style>");
            hsFile.WriteLine("\t</head>");
            hsFile.WriteLine("<body>");
            hsFile.WriteLine("<p>&nbsp;</p>");
            hsFile.WriteLine("<table border=\"1\" style=\"width: 45%; background: #F9F9F9;\">\n\t<tr>\n\t\t<td><strong>Player Initials</strong></td><td><strong>Player Score</strong></td><td><strong>Actually Won?</strong></td><td><strong>Lives Left</strong</td><td><strong>Date</strong></td>\n\t</tr>");
            hsFile.WriteLine("\t<tr>\n\t\t<td>" + playerInitials.Text.ToUpper() + "</td><td>" + score + "</td><td>" + didWin + "</td><td>" + player.livesLeft + "</td><td>" + todaysDate + " " + hour + ":" + min + "</td>");
            hsFile.WriteLine("\t</tr>\n</table>");
            hsFile.WriteLine("</body>\n</html>");

            // close the HTML file
            hsFile.Close();

            // open the HTML file in the browser (IE by default)
            Process.Start("highscores.html");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void saveAndQuit_Click(object sender, EventArgs e)
        {

        }

        private void viewScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("highscores.html");
        }
    }
}