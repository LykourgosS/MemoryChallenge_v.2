using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        bool ThePlayerJustFinishedTheGame;  //status of the game

        OleDbConnection conn = new OleDbConnection();   //connections variables

        String Uname;                           //score details
        int seconds;                            //
        int Clicks;                             //
        DateTime dateOfScore = new DateTime();  //

        private void AppearanceSettings(Image backImage, Color fontColor, Color butColor, String btext, Size fsize)
        {
            this.Size = fsize;
            this.BackgroundImage = backImage;
            this.ForeColor = fontColor;
            richTextBox1.ForeColor = fontColor;
            pictureBox0.ImageLocation = "memory-challenge.png";
            button1.Text = btext;
            button1.BackColor = butColor;
        }
        public Form2(Image backImage, Color fontColor, Color butColor, String name, int secs, int c, bool endOfGame)
        {
            InitializeComponent();
            conn.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=HighScores.mdb;";
            Uname = name;
            seconds = secs - 1; //(because there is a slight delay after finishing the game)
            Clicks = c;
            ThePlayerJustFinishedTheGame = endOfGame;
            if (endOfGame)    //end of game constructor
            {
                this.Text += "Congrats you win!";
                label1.Visible = false;
                AppearanceSettings(backImage, fontColor, butColor, "Submit my score!", new Size(418, 479));
                richTextBox1.Text = "Name: " + name.ToString() + Environment.NewLine + c.ToString() + " tries in " + seconds.ToString() + " seconds ";
                
            }
            else    //high scores clicked
            {
                this.Text += "High Scores";
                label1.Visible = true;
                AppearanceSettings(backImage, fontColor, butColor, "Go back to the game...", new Size(418, 479));
                //
                //start of connecting to the database
                //
                conn.Open();
                string querySort = " SELECT* FROM HighScores ORDER BY seconds ASC;";
                OleDbCommand cmd = new OleDbCommand(querySort, conn);
                OleDbDataReader rdr = cmd.ExecuteReader();
                int count = 1;
                while (rdr.Read())
                {
                    Uname = rdr[1].ToString();
                    Clicks = Convert.ToInt32(rdr[2]);
                    seconds = Convert.ToInt32(rdr[3]);
                    dateOfScore = Convert.ToDateTime(rdr[4]);
                    richTextBox1.Text += count.ToString() + ".   " + Uname + Environment.NewLine + "( " + seconds.ToString() + " sec, " + Clicks.ToString() + " clicks on " + dateOfScore + " )" + Environment.NewLine;
                    count++;
                    if (count == 11)
                    {
                        break;
                    }
                }
                conn.Close();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ThePlayerJustFinishedTheGame)  //go back to the game
            {                
                this.Close();
            }
            else  //submit my score (insertion of the score to the database for statistical purposes)
            {
                dateOfScore = System.DateTime.Now;
                //MessageBox.Show(dateOfScore.ToShortDateString() + "  " + dateOfScore.ToShortTimeString());
                conn.Open();
                string queryInsert = "INSERT INTO HighScores (uname,clicks,seconds,score_date) VALUES('" + Uname + "','" + Clicks + "','" + seconds + "','" + dateOfScore.ToShortDateString() + "  " + dateOfScore.ToShortTimeString() + "');";
                OleDbCommand cmd = new OleDbCommand(queryInsert, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                this.Close();
            }
        }
    }
}