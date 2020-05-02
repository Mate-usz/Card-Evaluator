using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;

namespace CardPricer_YuGiOh
{
    class Program : Form
    {
        // TODO
        // Window app
        // Buttons Code or Name ( Radio?! )
        // Texts fields to insert strings
        // Browse for a file?!
        // Print in window or save to a file

        private Button button1;
        private Button button2;
        private OpenFileDialog fileDialog;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private RadioButton radioButton4;

        int searchType = 0;
        int saveMode = 0;

        private Label textFile;
        private Label text0;

        private TextBox textBox0;

        GroupBox groupBox0;
        GroupBox groupBox1;
        GroupBox groupBox2;

        public Program()
        {

            fileDialog = new OpenFileDialog();
            button1 = new Button();

            fileDialog.Title = "Browse Text Files";
            fileDialog.DefaultExt = "txt";
            fileDialog.ReadOnlyChecked = true;
            fileDialog.Filter = "txt files (*.txt) | *.txt";

            button1.AutoSize = true;
            button1.Location = new Point(700, 70);
            button1.Text = "Browse...";
            button1.Click += new EventHandler(button1_Click);

            // Group box 0, contains the name of file browsed
            groupBox0 = new GroupBox();
            textFile = new Label();

            groupBox0.Controls.Add(textFile);
            groupBox0.Size = new Size(600, 100);
            groupBox0.Location = new Point(30, 30);
            groupBox0.Text = "File name";

            textFile.AutoSize = true;
            textFile.Location = new Point(30, 50);

            // Group box 1, 2 radio buttons that determine
            // were to put research results
            groupBox1 = new GroupBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Location = new Point(30, 175);
            groupBox1.Size = new Size(390, 100);
            groupBox1.Text = "Save Mode";

            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(30, 50);
            radioButton1.Text = "Save to file?";
            radioButton1.CheckedChanged += new EventHandler(radioButton_CheckedChanged);

            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(200, 50);
            radioButton2.Text = "Display here";
            radioButton2.Checked = true;
            radioButton2.CheckedChanged += new EventHandler(radioButton_CheckedChanged);

            // Group box 2, 2 radio buttons that determine
            // which type of search has to do by Code or Card Name
            groupBox2 = new GroupBox();
            radioButton3 = new RadioButton();
            radioButton4 = new RadioButton();

            groupBox2.Controls.Add(radioButton3);
            groupBox2.Controls.Add(radioButton4);
            groupBox2.Size = new Size(390, 100);
            groupBox2.Location = new Point(450, 175);
            groupBox2.Text = "Search Type";

            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(30, 50);
            radioButton3.Text = "Card Code";
            radioButton3.CheckedChanged += new EventHandler(radioButton_CheckedChanged);

            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(200, 50);
            radioButton4.Text = "Name";
            radioButton4.Checked = true;
            radioButton4.CheckedChanged += new EventHandler(radioButton_CheckedChanged);

            // Search button, appears at bottom of the form
            button2 = new Button();
            button2.Size = new Size(150, 100);
            button2.Location = new Point(150, 350);
            button2.Text = "Search";
            button2.Click += new EventHandler(OnButtonSearchClick);

            // Results where should show up
            // Create an instance of a TextBox control.
            textBox0 = new TextBox();

            // Set the Multiline property to true.
            textBox0.Multiline = true;
            // Add vertical scroll bars to the TextBox control.
            textBox0.ScrollBars = ScrollBars.Vertical;
            textBox0.AcceptsReturn = true;
            // Set WordWrap to true to allow text to wrap to the next line.
            textBox0.WordWrap = true;
            // Set the default text of the control.
            textBox0.Text = "";
            textBox0.Enabled = false;
            textBox0.Location = new Point(450, 300);
            textBox0.Size = new Size(400, 200);
            textBox0.ReadOnly = true;

            // Assemble the form
            this.Text = "Card Evaluator";
            this.ClientSize = new Size(900, 600);
            this.SetAutoSizeMode(AutoSizeMode.GrowOnly);
            this.Controls.Add(button1);
            this.Controls.Add(groupBox0);
            this.Controls.Add(groupBox1);
            this.Controls.Add(groupBox2);
            this.Controls.Add(button2);
            this.Controls.Add(textBox0);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textFile.Text = fileDialog.FileName;
            }
            
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Program());
        }

        void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
            {
                MessageBox.Show("Sender is not a RadioButton");
                return;
            }

            // Ensure that the RadioButton.Checked property
            // changed to true.

            // Keep track of the selected RadioButton by saving a value 
            // to save mode or search type.
            // Save Mode
            // 1 = Save to file | 0 = Display inside program
            // Search Type
            // 1 = Code | 0 = Name

            if (rb.Checked && rb.Text.Contains("?"))
            {
                saveMode = 1;
            }
            else if (rb.Checked && rb.Text.Contains("Display"))
            {
                saveMode = 0;
            }
            else if (rb.Checked && rb.Text.Contains("Name"))
            {
                searchType = 0;
            }
            else if (rb.Checked && rb.Text.Contains("Code"))
            {
                searchType = 1;
            }

        }

        void OnButtonSearchClick(object sender, EventArgs args)
        {
            SearchManager sm = new SearchManager();

            string[] cardsFromFile = GetCardsFromFile();

            string txt = sm.Search(searchType, saveMode, cardsFromFile);

            if (saveMode == 0)
            {
                textBox0.Text = txt;

                textBox0.Enabled = true;
            }
            else
                CreateExcelFile(txt);      

        }

        string[] GetCardsFromFile()
        {
            List<string> cards = new List<string>();

            //Read the contents of the file into a stream
            Stream fileStream = fileDialog.OpenFile();

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line;
                while( (line = reader.ReadLine()) != null)
                {
                    cards.Add(line);
                }
            }

            return cards.ToArray();
        }

        void CreateExcelFile(string infos)
        {
            string[] values = infos.Split(',');

            string fileName = @"..\..\Carte.xlsx";

            // Create a spreadsheet document for editing.  
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Create(fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadSheet.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook.
                Sheets sheets = spreadSheet.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = spreadSheet.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "mySheet"
                };
                sheets.Append(sheet);

                // Create a worksheet and data that will be added at the end
                Worksheet worksheet = new Worksheet();
                SheetData sheetData = new SheetData();


                // Creating a row and 2 cell per row and fill with values
                // A1 = Card Name | B1 = Price
                Row row = new Row();

                Cell cell1 = new Cell();
                cell1.CellValue = new CellValue("Card Name");
                cell1.DataType = CellValues.String;
                cell1.CellReference = "A1";

                Cell cell2 = new Cell();
                cell2.CellValue = new CellValue("Price");
                cell2.DataType = CellValues.String;
                cell2.CellReference = "B1";

                row.Append(cell1);
                row.Append(cell2);

                // Append the first row
                sheetData.Append(row);

                int indexCell = 1;

                int i = 0;

                // Looping throught all values and adding them to cells which
                // will be in a row lately append to sheetdata
                // Every loop creates a new row 
                for (; i < values.Length-1; i++)
                {
                    indexCell++;
                    
                    row = new Row();

                    cell1 = new Cell();
                    cell1.CellValue = new CellValue(values[i]);
                    cell1.DataType = CellValues.String;
                    cell1.CellReference = "A" + indexCell;

                    i++;

                    cell2 = new Cell();
                    cell2.CellValue = new CellValue(values[i]);
                    cell2.DataType = CellValues.String;
                    cell2.CellReference = "B" + indexCell;

                    row.Append(cell1);
                    row.Append(cell2);

                    sheetData.Append(row);
                }

                worksheet.Append(sheetData);

                worksheetPart.Worksheet = worksheet;
            }
        }
    }
}
