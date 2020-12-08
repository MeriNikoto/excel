using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace NewExcel
{
    public partial class Form1 : Form
    {

       // DataGridView dataGridView2 = new DataGridView();
        Class26BasedSys f = new Class26BasedSys();
        MyHashTable myHash = MyHashTable.GetInstance();
        ToPolReverseNotationConverter converter = new ToPolReverseNotationConverter();
        int row = 0, column = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView(10, 10);
            dataGridView2.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            //dataGridView2.CellMouseEnter += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress1);
            button1.Click += new EventHandler(Button1_Click);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
          //  this.Controls.Add(dataGridView2);
        }
        private void InitializeDataGridView(int rowsCount, int columns)
        {
            // Create an unbound DataGridView by declaring a column count. 
            dataGridView2.ColumnCount = columns;
            dataGridView2.ColumnHeadersVisible = true;
            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Times new roman", 12, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            // Set the column header names.	
            
            for (int i = 0; i < columns; ++i)
            {
                dataGridView2.Columns[i].Name = f.ToSys(i);
            }
            dataGridView2.RowCount = rowsCount;
            dataGridView2.RowHeadersVisible = true;
            DataGridViewCellStyle rowHeaderStyle = new DataGridViewCellStyle();
            rowHeaderStyle.BackColor = Color.Beige;
            rowHeaderStyle.Font = new Font("Times new roman", 12, FontStyle.Bold);
            dataGridView2.RowHeadersDefaultCellStyle = rowHeaderStyle;
            for (int i = 0; i < rowsCount; ++i)
            {
                dataGridView2.Rows[i].HeaderCell.Value = (i).ToString();
            }
            // Populate the rows.
            dataGridView2.Location = new Point(0, 75);
            dataGridView2.Width = 1470;
            dataGridView2.Height = 1470;
            

            dataGridView2.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
          

        }
        bool isRecalculated = true;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Хочете зберегти зміни?", "Зберегти файл", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                Save();
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Clear(ref isRecalculated);
            dataGridView2.Refresh();
            textBox1.Clear();
         
        }
       
        string names;
        private void dataGridView1_CellClick(object sender,
            DataGridViewCellEventArgs e)
        {
            names = f.ToSys(e.ColumnIndex) + "." + e.RowIndex.ToString();
            row = e.RowIndex;
            column = e.ColumnIndex;
            textBox1.Text = myHash.ShowFormula(names).ToString();

          
        }
        private void textBox1_KeyPress1(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == 13)
                {
                    
                    if (textBox1.Text == "")
                    {
                       
                        myHash.DeleteHash(names);
                        dataGridView2[column, row].Value = "";
                    }
                    else
                    {
                        string val = converter.Calculate(textBox1.Text);
                        if (!myHash.formulas.ContainsKey(names))
                        {
                            myHash.AddBoth(names, textBox1.Text, val);
                            dataGridView2[column, row].Value = val;
                        }
                        else
                        {
                            string temp = myHash.formulas[names].ToString();
                            try
                            {
                                myHash.formulas[names] = textBox1.Text;
                                myHash.ReCalculate(names, ref isRecalculated, converter);
                                Rewrite(ref isRecalculated);
                            }
                            catch(TokenEx)
                            {
                                MessageBox.Show("Помилка: посилання клітинки саму на себе");
                                myHash.formulas[names] = temp;
                            }
                        }
                    }
                }
                
            }
            catch(DivideByZeroException a)
            {
                MessageBox.Show(a.Message + "2");
                myHash.AddFormula(names, textBox1.Text);
                myHash.AddValue(names, "Error");
                dataGridView2[column, row].Value = "Error";
            }
            catch (TokenEx)
            {
                MessageBox.Show("Неправильний формат запису");
            }
        }
        public void Rewrite(ref bool isRecalculated)
        {
            isRecalculated = false;
            foreach (DictionaryEntry pair in myHash.Values)
            {
                string index = pair.Key.ToString();
                string columnNum = index.Split('.')[0];
                string rowNum = index.Split('.')[1];
                dataGridView2.Rows[Convert.ToInt32(rowNum)].Cells[f.FromSys(columnNum)].Value = pair.Value;

            }
            isRecalculated = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int rowId = dataGridView2.Rows.Add();

            // Grab the new row!
            DataGridViewRow row = dataGridView2.Rows[rowId];
            int rows = dataGridView2.Rows.Count;
            dataGridView2.Rows[rows - 1].HeaderCell.Value = (rows - 1).ToString();
            /*for (int i = Convert.ToInt32(rowNum); i <= rows; i++ )
            {
                dataGridView1.Rows[i].HeaderCell.Value = (i).ToString();
            }*/
        }

        private void AddColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int columnNum = dataGridView2.Columns.Count;
            int columnId = dataGridView2.Columns.Add("",f.ToSys(columnNum));
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int column = dataGridView2.Columns.Count;
            int row = dataGridView2.Rows.Count;
            string r = (row-1).ToString();
            for (int i = 0; i < column; i++)
            {
               
                myHash.ReEvaluate(f.ToSys(i) + "." + r, converter);
                myHash.DeleteHash(f.ToSys(i) + "." + r);
            }
            Rewrite(ref isRecalculated);
            if(row > 1)
                dataGridView2.Rows.RemoveAt(row - 1);
        }

        private void DeleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int column = dataGridView2.Columns.Count;
            int row = dataGridView2.Rows.Count;
            string r =f.ToSys(column - 1);
            for (int i = 0; i < row; i++)
            {

                myHash.ReEvaluate(r + "." + i, converter);
                myHash.DeleteHash(r + "." + i);
            }
            Rewrite(ref isRecalculated);
            if (column > 1)
                dataGridView2.Columns.RemoveAt(column - 1);
        }
        private void Save()
        {
            Stream mystream;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((mystream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter sw = new StreamWriter(mystream);
                    var filepath = saveFileDialog1.FileName;
                    if (filepath == "")
                    {
                        filepath = "SomeFile.bin";
                    }
                    myHash.AddFormula("num", dataGridView2.Rows.Count.ToString() + "." + dataGridView2.Columns.Count.ToString());
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    //Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(mystream, MyHashTable.GetInstance().formulas);
                    mystream.Close();
                }
            }
        }
        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
                var result = MessageBox.Show("Ви хочете зберегти зміни?", "Збереження", MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    Save();
                }
                Stream mystr = null;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((mystr = openFileDialog1.OpenFile()) != null)
                    {
                        StreamReader sr = new StreamReader(mystr);


                        Clear(ref isRecalculated);
                        var ds = new DataSet();
                        var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        MyHashTable.GetInstance().formulas = formatter.Deserialize(mystr) as Hashtable;
                        int rows = Convert.ToInt32(myHash.formulas["num"].ToString().Split('.')[0]);
                        int columns = Convert.ToInt32(myHash.formulas["num"].ToString().Split('.')[1]);
                        myHash.DeleteHash("num");
                        foreach (DictionaryEntry pair in myHash.formulas)
                        {
                            myHash.Re(pair.Key.ToString(), ref isRecalculated, converter);
                        }
                        
                        InitializeDataGridView(rows, columns);
                        mystr.Close();
                        Rewrite(ref isRecalculated);
                    }
                }
            
            
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Доступні дії:\n" +
                "+, -, /, *, >, <, min, max, inc, dec\n" +
                "Ім'я клітинки зписується у форматі <буква>.<число>. Наприклад, А.0\n" +
                "усі числа та операції пишуться через пропуск\n" +
                "Clear - очищує таблицю\n" +
                "Add row/column - додає рядок/стовпчик\n" +
                "Delete row/column - видаляє рядок/стовпчик\n" +
                "Save - зберігає поточну таблицю\n" +
                "Load - відкриває створну раніше таблицю\n";
            MessageBox.Show(message);

        }

        public void Clear(ref bool isRecalculated)
        {
            isRecalculated = false;
            foreach(DictionaryEntry pair in myHash.Values)
            {
                string index = pair.Key.ToString();
                string columnNum = index.Split('.')[0];
                string rowNum = index.Split('.')[1];
                dataGridView2.Rows[Convert.ToInt32(rowNum)].Cells[f.FromSys(columnNum)].Value = "";
            }
            myHash.values.Clear();
            myHash.formulas.Clear();
            isRecalculated = true;
        }
      
    }
}
