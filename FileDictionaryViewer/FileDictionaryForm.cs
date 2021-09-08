using System;
using System.Windows.Forms;
using Dictionary;
using System.IO;

namespace FileDictionaryViewer
{
    public partial class FileDictionaryForm : Form
    {
        readonly String nameOfParamDirectory = @"Param\";
        readonly String nameOfRequireDirectory = @"Require\";
        String[] requireFileEntries;
        private ComboBox cbFileKey;
        private ListBox lbFileValues;
        private Label requireFile;
        private Label suitableFiles;
        FileDictionary fileDictionary;

        public FileDictionaryForm()
        {
            try
            {
                InitializeComponent();
                fileDictionary = new FileDictionary(ref nameOfParamDirectory, ref nameOfRequireDirectory);
                requireFileEntries = Directory.GetFiles(nameOfRequireDirectory);
                cbFileKey.Items.AddRange(requireFileEntries);
                cbFileKey.SelectedIndex = 0;
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Отсутствует директория " + nameOfParamDirectory + " или " + nameOfRequireDirectory + ".");
            }
            catch (PathTooLongException)
            {
                MessageBox.Show("Длина пути к приложению слишком велика. Переместите папку с приложением, например, в корень диска С.");
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла редкая ошибка. Свяжитесь с отделом поддержки.");
            }
        }

        private void cbFileKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            String[] suitableParamFileEntries = fileDictionary.GetSuitableParamFiles(ref requireFileEntries[cbFileKey.SelectedIndex]);
            lbFileValues.Items.Clear();
            lbFileValues.Items.AddRange(suitableParamFileEntries);
        }
        private void InitializeComponent()
        {
            this.cbFileKey = new System.Windows.Forms.ComboBox();
            this.lbFileValues = new System.Windows.Forms.ListBox();
            this.requireFile = new System.Windows.Forms.Label();
            this.suitableFiles = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbFileKey
            // 
            this.cbFileKey.FormattingEnabled = true;
            this.cbFileKey.Location = new System.Drawing.Point(88, 60);
            this.cbFileKey.Name = "cbFileKey";
            this.cbFileKey.Size = new System.Drawing.Size(124, 21);
            this.cbFileKey.TabIndex = 0;
            this.cbFileKey.SelectedIndexChanged += new System.EventHandler(this.cbFileKey_SelectedIndexChanged);
            // 
            // lbFileValues
            // 
            this.lbFileValues.Enabled = false;
            this.lbFileValues.FormattingEnabled = true;
            this.lbFileValues.Location = new System.Drawing.Point(90, 104);
            this.lbFileValues.Name = "lbFileValues";
            this.lbFileValues.Size = new System.Drawing.Size(124, 121);
            this.lbFileValues.TabIndex = 1;
            // 
            // requireFile
            // 
            this.requireFile.AutoSize = true;
            this.requireFile.Location = new System.Drawing.Point(87, 44);
            this.requireFile.Name = "requireFile";
            this.requireFile.Size = new System.Drawing.Size(124, 13);
            this.requireFile.TabIndex = 2;
            this.requireFile.Text = "Файл с требованиями:";
            // 
            // suitableFiles
            // 
            this.suitableFiles.AutoSize = true;
            this.suitableFiles.Location = new System.Drawing.Point(90, 88);
            this.suitableFiles.Name = "suitableFiles";
            this.suitableFiles.Size = new System.Drawing.Size(111, 13);
            this.suitableFiles.TabIndex = 3;
            this.suitableFiles.Text = "Подходящие файлы:";
            // 
            // FileDictionaryForm
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.suitableFiles);
            this.Controls.Add(this.requireFile);
            this.Controls.Add(this.lbFileValues);
            this.Controls.Add(this.cbFileKey);
            this.MaximizeBox = false;
            this.Name = "FileDictionaryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
