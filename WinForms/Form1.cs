using System;
using System.Data;
using System.Windows.Forms;

namespace Kursach_AK_47
{
    public partial class Form1 : Form
    {
        string path = "";
        OpenFileDialog loadFile = new();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LoadFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartAnalyzer();
        }

        private void LoadFile()
        {
            if (loadFile.ShowDialog() == DialogResult.Cancel)
                return;
            path = loadFile.FileName;
            textBox8.Text = "";
            textBox8.Text = System.IO.File.ReadAllText(path);
        }

        private void StartAnalyzer()
        {
            dataGridView1.ClearSelection(); // _tableServiceWords
            dataGridView2.ClearSelection(); // _tableLimiters
            dataGridView3.ClearSelection(); // _tableIdentification
            dataGridView4.ClearSelection(); // _tableNumbers
            textBox9.Clear();
            listBox1.Items.Clear();

            if (path != "")
            {
                dataGridView1.DataSource = DataTokens.TableServiceWords();
                dataGridView2.DataSource = DataTokens.TableLimiters();
                
                LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer(path);
                foreach (string str in lexicalAnalyzer.GetErrorMessage())
                {
                    textBox9.Text += str + "\r\n";
                }
                
                
                if (!lexicalAnalyzer.GetErrors())
                {
                    dataGridView3.DataSource = DataTokens.TableIdentification();
                    dataGridView4.DataSource = DataTokens.TableNumbers();

                    listBox1.Items.AddRange(lexicalAnalyzer.GetResult().ToArray());
                    SyntacticAnalyzer syntacticAnalyzer = new SyntacticAnalyzer(lexicalAnalyzer.GetResult());
                    foreach (string str in syntacticAnalyzer.GetErrorMessage())
                    {
                        textBox9.Text += str + "\r\n";
                    }

                    if (!syntacticAnalyzer.GetErrors())
                    {
                        /*SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer();

                        foreach (string str in semanticAnalyzer.GetErrorMessage())
                        {
                            textBox9.Text += str + "\r\n";
                        }*/
                    }
                }
            }
        }
    }
}