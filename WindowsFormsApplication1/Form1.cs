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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //
        //programming variables
        //
        bool error1_exists = false; //if true it means that, although the user has selected the custom mode they haven't selected enough images (25)!!!
        bool defaultMode = true;    //it's true if the user doesn't select their own images!
        static String[] paths = new String[25];
        String[] customPaths = new String[25];  //it'll be used only if custom mode is selected
        picture[] arrayOfPictures = new picture[25];
        static PictureBox[] arrayOfPictureboxes = new PictureBox[50];
        static List<SelectedItem> selectedPictures = new List<SelectedItem>(2);
        List<picture> listOfPictures_Doubled = new List<picture>(50);
        List<User> userDetails = new List<User>(1);

        //
        //appearance variables
        //
        Color fontcolor=ColorTranslator.FromHtml("#162934");
        Color backcolorForButtons=ColorTranslator.FromHtml("#E31936");
        Image backimage = Image.FromFile("background.jpg");
        Image logo = Image.FromFile("memory-challenge.png");

        public Form1()
        {
            InitializeComponent();
        }

        private void formReload()
        {
            AppearanceSettings();
            InitializeImages();
            listOfPictures_Doubled.Clear();
            checkBox1.Checked = false;
            creationOfPathsForDefaultPictures();
            userDetails.Clear();
            userDetails.Add(new User());
            error1_exists = false;
            groupBox3.Visible = true;
            pictureBox0.Visible = true;
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            textBox1.Clear();

            flag = true;    //for debugging purposes
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            userDetails.Add(new User());
            AppearanceSettings();
            InitializeImages();
            MessageBox.Show("Being programmed and designed for Windows 8.1 Environment, the  game may not be dispayed correctly in other OS's.", "Memory Challenge v.2 - Display Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void AppearanceSettings()
        {
            this.BackgroundImage = backimage;
            this.ForeColor = fontcolor;
            label6.ForeColor = fontcolor;
            menuStrip1.ForeColor = fontcolor;
            groupBox1.ForeColor = fontcolor;
            groupBox2.ForeColor = fontcolor;
            groupBox3.ForeColor = fontcolor;
            textBox1.ForeColor = fontcolor;
            richTextBox1.ForeColor = fontcolor;
            menuStrip1.BackColor = backcolorForButtons;
            button1.BackColor= backcolorForButtons;
            button2.BackColor = backcolorForButtons;
            pictureBox0.Location = new Point(294, 50);
            groupBox3.Location = new Point(495, 342);
            groupBox2.Location = new Point(136, 31);
            pictureBox0.Image = logo;           
        }

        private void InitializeImages()
        {
            int i = 50;
            foreach (PictureBox picbox in groupBox1.Controls)
            {
                picbox.Enabled = true;  //because if you click New Game while an image is turned the picturebox will stay disabled
                arrayOfPictureboxes[i - 1] = picbox;
                set_back_side_of(picbox);
                i--;
            }
        }

        private void button1_Click(object sender, EventArgs e)  //"start" button
        {
            if (error1_exists)
            {
                MessageBox.Show("You have to select 25 pictures!", "Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (textBox1.Text.Length < 4)  //if true it means that Error2 exists, i.e. the user haven't given a valid name
            {
                MessageBox.Show("You have to type your name (at least 4 characters)!", "Error2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                userDetails[0].name = textBox1.Text;
                label3.Text = "Hello,  " + userDetails[0].name.ToString();
                label2.Text = "Clicks: " + userDetails[0].tries.ToString();
                groupBox3.Visible = false;  //hides the custom settings and the start button
                pictureBox0.Visible = false;    //hides the logo
                groupBox1.Location = new Point(81, 95);     //displays the groupbox of pictureboxes-
                groupBox1.Visible = true;                   //in the middle of the screen
                groupBox2.Visible = true;   //displays the groupbox of score,time,name,tries                
                if (!checkBox1.Checked)
                {
                    creationOfPathsForDefaultPictures();
                }
                else
                {
                    paths = customPaths;
                }
                create_pictures(arrayOfPictures, paths);
                shuffleCards();
                GameTimer.Enabled = true;
            }
        }

        private void creationOfPathsForDefaultPictures()
        {
            if (!checkBox1.Checked)
            {
                paths = Directory.GetFiles("images");
            }
        }

        private void create_pictures(picture[] array, String[] p)
        {
            for (int i = 0; i < 25; i++)
            {
                array[i] = new picture(p[i]);
                listOfPictures_Doubled.Add(array[i]);   //2 times because every picture is double 
                listOfPictures_Doubled.Add(array[i]);   //
            }
        }

        private void shuffleCards()    //either default or custom mode, it shuffles the the main list of pictures that will be used for displaying the images in the pictureboxes
        {
            FisherYates.Shuffle<picture>(listOfPictures_Doubled);            
        }

        private void set_back_side_of(PictureBox pbox)
        {
            pbox.SizeMode = PictureBoxSizeMode.Zoom;
            pbox.BackColor = Color.White;
            pbox.ImageLocation = "memory-challenge.png";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)   //options for the display of custom mode settings
        {
            if (checkBox1.Checked)
            {
                error1_exists = true;
                defaultMode = false;
                button2.Visible = true;
                richTextBox1.Visible = true;
                richTextBox1.Clear();
            }
            else
            {
                defaultMode = true;
                error1_exists = false;
                button2.Visible = false;
                richTextBox1.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)  //"choose files" button
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                customPaths = openFileDialog1.FileNames;
                richTextBox1.Text = "(" + customPaths.Length.ToString() + " file(s) selected)" + Environment.NewLine;
                foreach (String s in customPaths)
                {
                    richTextBox1.Text += "• " + s + Environment.NewLine;
                }
                if (customPaths.Length != 25)   //if true the user hasn't selected 25 images
                {
                    error1_exists = true;
                    MessageBox.Show("You have to select 25 pictures!", "Error1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else   //there have been selected 25 pictures
                {
                    error1_exists = false;
                    defaultMode = false;
                }
            }                      
        }

        private void check_if_the_pair_is_correct()
        {
            CardTimer.Enabled = false;
            if (selectedPictures.Count == 2 && selectedPictures[0].pic.path == selectedPictures[1].pic.path)    //user have found a correct pair
            {
                selectedPictures[0].picbox.Enabled = false; //if not when the user by accident click on an already opened(correct) picturebox-
                selectedPictures[1].picbox.Enabled = false; //and another one, it would set the back side of both pictureboxes!!! 
                selectedPictures.RemoveAt(1);   //clear the static list selectedPictures-
                selectedPictures.RemoveAt(0);   //so it can be used the next time.
                userDetails[0].CorrectPairs += 1;
                label5.Text = (25 - userDetails[0].CorrectPairs).ToString() + " Pairs left...";    //refresh of the left pairs
                if (userDetails[0].CorrectPairs == 25)
                {
                    GameTimer.Enabled = false;
                    Form2 endOfGame = new Form2(backimage, fontcolor, backcolorForButtons, userDetails[0].name, userDetails[0].time, userDetails[0].tries, true);
                    formReload();
                    endOfGame.Show();
                }
            }
            else    //so the selected pair it's not correct
            {
                set_back_side_of(selectedPictures[1].picbox);
                selectedPictures[1].picbox.Enabled = true;
                selectedPictures.RemoveAt(1);
                set_back_side_of(selectedPictures[0].picbox);
                selectedPictures[0].picbox.Enabled = true;
                selectedPictures.RemoveAt(0);
            }
        }

        private void select_NewItem(int i)  //save the clicked pictureboxes in a list of selectedItem s(see their class declaration) 
        {
            try {
                SelectedItem si = new SelectedItem(listOfPictures_Doubled[i], arrayOfPictureboxes[i]);
                selectedPictures.Add(si);
                arrayOfPictureboxes[i].ImageLocation = listOfPictures_Doubled[i].path;  //the selected picturebox display the corresponding image
                if (defaultMode)
                {
                    arrayOfPictureboxes[i].SizeMode = PictureBoxSizeMode.StretchImage;  //if the user selects their own images (i.e. defaultMode=false) the selected images will be displayed better in Zoom mode!
                }
                si.picbox.Enabled = false;  //disables the picturebox in case the user clicks on it by accident
            }
            catch (Exception ex)    //when debugging if you click too fast after find 2 correct pairs(as when ltest and correctpairs=23) there is the possibility to click on a enabled picturebox and be thrown indexOutOf... exception
            {}
        }

        private void picbox_OnClick(int index)
        {
            userDetails[0].tries += 1; //sums up all the tries that the user does
            label2.Text = "Clicks: " + userDetails[0].tries.ToString();
            label5.Text = (25 - userDetails[0].CorrectPairs).ToString() + " Pairs left...";
            index -= 1; //-1 because the parameter index of picturebox{i}_Click has the same index as the number of the picturebox
            if (selectedPictures.Count == 0)
            {
                select_NewItem(index);
            }
            else if (selectedPictures.Count == 1)
            {
                select_NewItem(index);
                CardTimer.Enabled = true;
            }
            else  //in case the user doen't want to wait ~1.7s (click before the CardTimer tick), i.e. the program does't wait the timer, to set the back side of the selected cards, but it allows the user to reselect new cards faster!!!
            {
                check_if_the_pair_is_correct();
                select_NewItem(index);
            }
        }

        //  
        //pictureBox{i}_Click (50 methods)
        //
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            picbox_OnClick(1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            picbox_OnClick(2);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            picbox_OnClick(3);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            picbox_OnClick(4);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            picbox_OnClick(5);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            picbox_OnClick(6);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            picbox_OnClick(7);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            picbox_OnClick(8);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            picbox_OnClick(9);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            picbox_OnClick(10);
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            picbox_OnClick(11);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            picbox_OnClick(12);
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            picbox_OnClick(13);
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            picbox_OnClick(14);
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            picbox_OnClick(15);
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            picbox_OnClick(16);
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            picbox_OnClick(17);
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {
            picbox_OnClick(18);
        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {
            picbox_OnClick(19);
        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {
            picbox_OnClick(20);
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {
            picbox_OnClick(21);
        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {
            picbox_OnClick(22);
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            picbox_OnClick(23);
        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {
            picbox_OnClick(24);
        }

        private void pictureBox25_Click(object sender, EventArgs e)
        {
            picbox_OnClick(25);
        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {
            picbox_OnClick(26);
        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {
            picbox_OnClick(27);
        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {
            picbox_OnClick(28);
        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {
            picbox_OnClick(29);
        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {
            picbox_OnClick(30);
        }

        private void pictureBox31_Click(object sender, EventArgs e)
        {
            picbox_OnClick(31);
        }

        private void pictureBox32_Click(object sender, EventArgs e)
        {
            picbox_OnClick(32);
        }

        private void pictureBox33_Click(object sender, EventArgs e)
        {
            picbox_OnClick(33);
        }

        private void pictureBox34_Click(object sender, EventArgs e)
        {
            picbox_OnClick(34);
        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {
            picbox_OnClick(35);
        }

        private void pictureBox36_Click(object sender, EventArgs e)
        {
            picbox_OnClick(36);
        }

        private void pictureBox37_Click(object sender, EventArgs e)
        {
            picbox_OnClick(37);
        }

        private void pictureBox38_Click(object sender, EventArgs e)
        {
            picbox_OnClick(38);
        }

        private void pictureBox39_Click(object sender, EventArgs e)
        {
            picbox_OnClick(39);
        }

        private void pictureBox40_Click(object sender, EventArgs e)
        {
            picbox_OnClick(40);
        }

        private void pictureBox41_Click(object sender, EventArgs e)
        {
            picbox_OnClick(41);
        }

        private void pictureBox42_Click(object sender, EventArgs e)
        {
            picbox_OnClick(42);
        }

        private void pictureBox43_Click(object sender, EventArgs e)
        {
            picbox_OnClick(43);
        }

        private void pictureBox44_Click(object sender, EventArgs e)
        {
            picbox_OnClick(44);
        }

        private void pictureBox45_Click(object sender, EventArgs e)
        {
            picbox_OnClick(45);
        }

        private void pictureBox46_Click(object sender, EventArgs e)
        {
            picbox_OnClick(46);
        }

        private void pictureBox47_Click(object sender, EventArgs e)
        {
            picbox_OnClick(47);
        }

        private void pictureBox48_Click(object sender, EventArgs e)
        {
            picbox_OnClick(48);
        }

        private void pictureBox49_Click(object sender, EventArgs e)
        {
            picbox_OnClick(49);
        }

        private void pictureBox50_Click(object sender, EventArgs e)
        {
            picbox_OnClick(50);
        }

        private void CardTimer_Tick(object sender, EventArgs e)    //timer1 is used on the procedure of checking if the selected images are a pair
        {
            check_if_the_pair_is_correct(); //the cards will be set in their back side after ~1.7s, so the user has time to see the selected cards
        }
        bool flag = true;   //for debbuging purposes
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            
            if (userDetails[0].name == "ltest" && flag) //for debbuging purposes
            {
                flag = false;
                userDetails[0].CorrectPairs = 24;
            }
            if (userDetails[0].CorrectPairs == 25)
            {
                GameTimer.Enabled = false;
                Form2 endOfGame = new Form2(backimage, fontcolor, backcolorForButtons, userDetails[0].name, userDetails[0].time, userDetails[0].tries, true);
                formReload();
                endOfGame.Show();
            }
            userDetails[0].time += 1;
            label1.Text = "Timer: " + (userDetails[0].time-1).ToString() + " seconds";
            label5.Text = (25 - userDetails[0].CorrectPairs).ToString() + " Pairs left...";
        }
        //
        //menuStrip(5)
        //
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e) //new game
        {
            formReload();
        }

        private void highScoresToolStripMenuItem_Click(object sender, EventArgs e)  //scores
        {
            GameTimer.Enabled = false;
            Form2 highScores = new Form2(backimage, fontcolor, backcolorForButtons, userDetails[0].name, userDetails[0].time, userDetails[0].tries,false);
            highScores.Show();
            if (groupBox1.Visible)  //so that the clock doesn't start ticking when the user clicks on the control while they haven't started the game yet (start screen)
            {
                GameTimer.Enabled = true;
            }            
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e) //credits
        {
            GameTimer.Enabled=false;
            MessageBox.Show("Made by LykourgosS. on February 2017", "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (groupBox1.Visible)  //so that the clock don't start ticking, in case the user clicks on the control while they haven't started the game yet (start screen)
            {
                GameTimer.Enabled = true;
            }
        }

        private void insrtuctionsToolStripMenuItem_Click(object sender, EventArgs e)    //instructions
        {
            GameTimer.Enabled = false;
            MessageBox.Show("Object of the game:" + Environment.NewLine + "The object of the game is to find all the matching pairs." + Environment.NewLine + "Winning the Game:" + Environment.NewLine + "Once all the cards have been played the game has finished! ;)", "Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (groupBox1.Visible)  //so that the clock don't start ticking, in case the user clicks on the control while they haven't started the game yet (start screen)
            {
                GameTimer.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)    //exit
        {
            this.Close();
        }
    }

    public class picture
    {
        public String path;

        public picture(String p)
        {
            path = p;
        }
    }

    public class User
    {
        public int time = 0;
        public int tries = 0;
        public int CorrectPairs = 0;
        public String name = "(null)";
    }

    public class SelectedItem
    {
        public picture pic;
        public PictureBox picbox;

        public SelectedItem(picture p, PictureBox pb)
        {
            pic = p;
            picbox = pb;
        }
    }
}

static public class FisherYates
{
    static Random r = new Random();
    //  Based on Java code from wikipedia:
    //  http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
    static public void Shuffle<T>(List<T> list)
    {
        for (int n = list.Count - 1; n > 0; --n)
        {
            int k = r.Next(n + 1);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }
}
