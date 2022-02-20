using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Binary_code_puzzle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<TextBox> textBoxes = new List<TextBox>();
        List<string> puzzles = new List<string>();
        int puzzle_index = 0;
        bool type_disabled = false;

        private static int CompareTabIndex(TextBox c1, TextBox c2)
        {
            return c1.TabIndex.CompareTo(c2.TabIndex);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(Control obj in panel1.Controls)
            {
                if (obj is TextBox)
                {
                    obj.KeyPress += Textbox_keypress;
                    obj.KeyDown += Textbox_keydown;
                    textBoxes.Add((TextBox)obj);
                }
            }

            textBoxes.Sort(new Comparison<TextBox>(CompareTabIndex));


            if (System.IO.File.Exists("puzzles.txt"))
            {
                System.IO.StreamReader file = new System.IO.StreamReader("puzzles.txt");
                bool puzzles_opened = false;

                string puzzle = "";
                while (!file.EndOfStream)
                {
                    string data = file.ReadLine();
                    if (data.IndexOf("//") != -1)
                        data = data.Substring(0, data.IndexOf("//"));
                    data = data.Replace(" ", "").Replace("\t", "");

                    if (data == "[")
                        puzzles_opened = true;
                    else if (data == "]")
                    {
                        puzzles_opened = false;
                        puzzles.Add(puzzle);
                        puzzle = "";
                    }
                    else if (puzzles_opened)
                        puzzle += data;
                }

                file.Dispose();
            }
            else
            {
                if (MessageBox.Show("File 'puzzles' not found, click Ok to see how to fix this problem", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                    System.Diagnostics.Process.Start("http://www.google.com");
            }
        }

        private void Textbox_keypress(object sender, KeyPressEventArgs e)
        {
            TextBox curr_textbox = (TextBox)sender;

             e.Handled = type_disabled || curr_textbox.Text.Length > 0;
        }

        private void Textbox_keydown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1 || e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                type_disabled = false;
            else
                type_disabled = true;
        }

        private void check_valid(object sender, EventArgs e)
        {
            clear_btn.PerformClick();
            string[] rows = new string[6];
            string[] columns = new string[6];


            foreach (TextBox obj in textBoxes)
            {
                //set rows 
                char index_row = char.Parse(obj.Name.Substring(0, 1));
                rows[index_row - 'A'] += obj.Text;

                //set columns
                int index_column = int.Parse(obj.Name.Substring(1, 1)) - 1;
                columns[index_column] += obj.Text;

            }

            for (int i = 0; i < 6; i++)
            {
                //check row
                if (check_string(rows[i]))
                    correct_line(i + 'A');
                else
                {
                    error_line(i + 'A');
                    rows[i] = "";
                }

                //check column
                if (check_string(columns[i]))
                    correct_line(i);
                else
                {
                    error_line(i);
                    columns[i] = "";
                }
            }

            //if rows and columns are unique
            check_unique(rows,true);
            check_unique(columns,false);
        }

        private void clear_btn_Click(object sender, EventArgs e)
        {
            //reset all textboxes to default background (except readonly cells)
            foreach (TextBox textBox in textBoxes)
                if (textBox.BackColor != SystemColors.Control)
                    textBox.BackColor = SystemColors.Window;
        }

        private void check_unique(string[] array,bool rows)
        {
            for (int i = 0; i < array.Length; i++)
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] == "" || array[j] == "")
                        continue;

                    if (array[i] == array[j])
                    {
                        //if we are checking for a row transform the index to the letter. For example 0 -> 65(ascii of A)
                        if (rows)
                        {
                            not_Unique_error(i + 'A');
                            not_Unique_error(j + 'A');
                        }
                        else
                        {
                            not_Unique_error(i);
                            not_Unique_error(j);
                        }
                       
                    }
                }
        }

        private void not_Unique_error(int line)
        {
            //row
            if (line > 6)
            {
                string index = (char)line + "";
                foreach (TextBox txtBox in textBoxes)
                    if (txtBox.Name.Substring(0, 1) == index && txtBox.BackColor != SystemColors.Control)
                        txtBox.BackColor = Color.FromArgb(255, 255, 192);
            }
            //column
            else
            {
                string index = (line + 1).ToString();
                foreach (TextBox txtBox in textBoxes)
                    if (txtBox.Name.Substring(1, 1) == index && txtBox.BackColor != SystemColors.Control)
                        txtBox.BackColor = Color.FromArgb(255, 255, 192);
            }
        }
        private void error_line(int line)
        {
            //row
            if (line > 6)
            {
                string index = (char)line + "";
                foreach (TextBox txtBox in textBoxes)
                    if (txtBox.Name.Substring(0, 1) == index && txtBox.BackColor != SystemColors.Control)
                        txtBox.BackColor = Color.FromArgb(255, 192, 192);
            }
            //column
            else
            {
                string index = (line + 1).ToString();
                foreach (TextBox txtBox in textBoxes)
                    if (txtBox.Name.Substring(1,1) == index && txtBox.BackColor != SystemColors.Control)
                        txtBox.BackColor = Color.FromArgb(255, 192, 192);
            }
        }

        private void correct_line(int line)
        {
            //row
            if (line > 6)
            {
                string index = (char)line + "";
                foreach (TextBox txtBox in textBoxes)
                    if (txtBox.Name.Substring(0, 1) == index && txtBox.BackColor != SystemColors.Control)
                        txtBox.BackColor = Color.FromArgb(192, 255, 192);
            }
            //column
            else
            {
                string index = (line + 1).ToString();
                foreach (TextBox txtBox in textBoxes)
                    if (txtBox.Name.Substring(1, 1) == index && txtBox.BackColor != SystemColors.Control)
                        txtBox.BackColor = Color.FromArgb(192, 255, 192);
            }

        }

        private static bool check_string(string s)
        {
            //if s.Lenght != 6 means that there is one or more empty cells
            if (s.Length != 6)
                return false;

            //check if s has more than two identical digits placed directly next to each other
            //if so return false
            int count = 0;
            char prev = ' ';
            foreach (char c in s)
            {
                if (c == prev)
                    count++;
                else
                    count = 1;

                if (count > 2)
                    return false;

                prev = c;
            }

            //check if s has the same number of zeros and ones.
            int count_diff_0_and_1 = 0;
            foreach (char c in s)
            {
                if (c == '1')
                    count_diff_0_and_1++;
                else
                    count_diff_0_and_1--;
            }
            if (count_diff_0_and_1 != 0)
                return false;

            return true;
        }

        bool rule_collapsed = false;
        private void Title_lbl_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            if (!rule_collapsed)
                collapse_rule.Start();
            else
                uncollapse_rule.Start();
        }
        private void collapse_rule_Tick(object sender, EventArgs e)
        {
            description_lbl.Height = description_lbl.Height - 4;
            panel1.Location = new Point(panel1.Location.X, panel1.Location.Y - 4);
            legend_title.Location = new Point(legend_title.Location.X, legend_title.Location.Y - 4);
            legend_description.Location = new Point(legend_description.Location.X, legend_description.Location.Y - 4);
            this.Height = this.Height - 4;

            if (description_lbl.Height == 0)
            {
                rule_collapsed = true;
                collapse_rule.Stop();
                this.Enabled = true;
            }
        }
        private void uncollapse__rule_Tick(object sender, EventArgs e)
        {
            description_lbl.Height = description_lbl.Height + 4;
            panel1.Location = new Point(panel1.Location.X, panel1.Location.Y + 4);
            legend_title.Location = new Point(legend_title.Location.X, legend_title.Location.Y + 4);
            legend_description.Location = new Point(legend_description.Location.X, legend_description.Location.Y + 4);
            this.Height = this.Height + 4;

            if (description_lbl.Height == 152)
            {
                rule_collapsed = false;
                uncollapse_rule.Stop();
                this.Enabled = true;
            }
        }
     
        bool legend_collapsed = false;
        private void legend_title_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            if (!legend_collapsed)
                collapse_legend.Start();
            else
                uncollapse_legend.Start();
        }
        private void collapse_legend_Tick(object sender, EventArgs e)
        {
            legend_description.Height = legend_description.Height - 4;
            panel1.Location = new Point(panel1.Location.X, panel1.Location.Y - 4);
            this.Height = this.Height - 4;

            if (legend_description.Height == 0)
            {
                legend_collapsed = true;
                collapse_legend.Stop();
                this.Enabled = true;
            }
        }
        private void uncollapse_legend_Tick(object sender, EventArgs e)
        {
            legend_description.Height = legend_description.Height + 4;
            panel1.Location = new Point(panel1.Location.X, panel1.Location.Y + 4);
            this.Height = this.Height + 4;

            if (legend_description.Height == 180)
            {
                legend_collapsed = false;
                uncollapse_legend.Stop();
                this.Enabled = true;
            }
        }

        private void next_btn_Click(object sender, EventArgs e)
        {
            clear_btn.PerformClick();
            puzzle_index++;
            if (puzzle_index == puzzles.Count())
                puzzle_index = 0;

            string curr_puzzle = puzzles[puzzle_index];
            string val = "";
            bool cell = false;

            int textboxes_index = -1;

            foreach (char c in curr_puzzle)
            {
                //open apostrophe
                if (c == '\'' && cell == false)
                {
                    cell = true;
                    val = "";
                }
                //close apostrophe
                else if (c == '\'' && cell == true)
                {
                    textboxes_index++;
                    cell = false;
                    if (val == "")
                    {
                        textBoxes[textboxes_index].Text = "";
                        textBoxes[textboxes_index].ReadOnly = false;
                        textBoxes[textboxes_index].BackColor = SystemColors.Window;
                    }
                    else
                    {
                        textBoxes[textboxes_index].Text = val;
                        textBoxes[textboxes_index].ReadOnly = true;
                        textBoxes[textboxes_index].BackColor = SystemColors.Control;
                    }
                }
                else if (cell)
                    val += c;
            }
            
        }
    }
}
