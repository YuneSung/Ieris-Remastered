﻿//Copyright 2017 Andrea Fioraldi <andreafioraldi@gmail.com>

//Aeiris is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 3 of the License, or
//(at your option) any later version.

//Stout Compiler is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
//MA 02110-1301, USA.

using Be.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace Aeiris
{
    public partial class MainForm : Form
    {
        private Be.Windows.Forms.HexBox hexBox1;
        private bool _default = true;
        private bool _unity = false;
        private bool _unreal = false;
        private bool _FCL = false;
        private bool _cpp = true;
        private bool _cs = false;

        private void hexBoxInit()
        {
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.hexBox1.AllowDrop = true;
            this.hexBox1.BackColor = System.Drawing.SystemColors.Window;
            this.hexBox1.ColumnInfoVisible = true;
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox1.InfoForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(3, 3);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(520, 292);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 0;
            this.hexBox1.VScrollBarVisible = true;
            this.hexBox1.SelectionStartChanged += new System.EventHandler(this.hexBox1_SelectionStartChanged);
            this.hexBox1.SelectionLengthChanged += new System.EventHandler(this.hexBox1_SelectionLengthChanged);

            this.EditPage.Controls.Add(hexBox1);
        }

        public MainForm()
        {
            InitializeComponent();
            hexBoxInit();
        }

        public MainForm(String file)
        {

            InitializeComponent();
            hexBoxInit();
            Open(file);
        }

        private FindForm findFm;
        private ReplaceForm replFm;
        private GotoForm gotoFm;
        private ConverterForm convertFm;

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists("log.txt"))
            {
                File.WriteAllText("log.txt", "");
            }
            textBox1.Text = File.ReadAllText("log.txt");
            string settings = File.ReadAllText("log.txt");

            if (settings.Contains("ec:gray")) { EditPage.ForeColor = Color.Gray; ec_gray.Checked = true; }
            else if (settings.Contains("ec:black")) { EditPage.ForeColor = Color.Black; ec_black.Checked = true; }
            else if (settings.Contains("ec:blue")) { EditPage.ForeColor = Color.Blue; ec_blue.Checked = true; }
            else if (settings.Contains("ec:pink")) { EditPage.ForeColor = Color.Pink; ec_pink.Checked = true; }
            else if (settings.Contains("ec:yellow")) { EditPage.ForeColor = Color.Yellow; ec_yellow.Checked = true; }

            if (settings.Contains("ls:enabled")) { ls_enabled.Checked = true; }
            else if (settings.Contains("ls:disabled")) { ls_disabled.Checked = true; }

            if (settings.Contains("fpt:default")) { fpt_default.Checked = true; }
            else if (settings.Contains("fpt:fcl")) { fpt_fcl.Checked = true; }
            else if (settings.Contains("fpt:lua/j")) { fpt_luaoj.Checked = true; }
            else if (settings.Contains("fpt:unitydevh")) { fpt_unitydevh.Checked = true; }
            else if (settings.Contains("fpt:unrealdev")) { fpt_unrealdev.Checked = true; }

            if (settings.Contains("dk:c++")) { dk_cpp.Checked = true; }
            else if (settings.Contains("dk:c#")) { dk_cs.Checked = true; }


            findFm = new FindForm(hexBox1);
            replFm = new ReplaceForm(hexBox1);
            gotoFm = new GotoForm(hexBox1);
            convertFm = new ConverterForm();
            hexBox1.InsertActive = menuInsert.Checked;
            hexBox1.Select();
        }

        private DynamicFileByteProvider provider = null;
        private String path = null;

        private static bool[] printables_ASCII = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        private static bool[] printables_extASCII = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        private static bool[] printables_alnumASCII = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

        private List<long> stringsIndexes;

        private void searchStrBtn_Click(object sender, EventArgs e)
        {
            stringsList.Items.Clear();
            stringsIndexes = new List<long>();

            bool[] printables = null;
            if (AsciiBox.Checked)
                printables = printables_ASCII;
            else if (extAsciiBox.Checked)
                printables = printables_extASCII;
            else if (alphanumAsciiBox.Checked)
                printables = printables_alnumASCII;

            long min = (long) strMinimumBox.Value;
            
            long strIndex = -1;
            String str = "";
            for (long i = 0; i < provider.Length; ++i)
            {
                byte b = provider.ReadByte(i);
                if ((b == 0 || b == '\n' || b == '\r') && strIndex != -1)
                {
                    if (i - strIndex >= min)
                    {
                        str = "";
                        for (long k = strIndex; k < i; ++k)
                            str += (char)provider.ReadByte(k);
                        stringsList.Items.Add(str);
                        stringsIndexes.Add(strIndex);
                    }
                    strIndex = -1;
                }
                else if (printables[b])
                {
                    if (strIndex == -1)
                    {
                        strIndex = i;
                    }
                }
                else if (strIndex != -1)
                {
                    strIndex = -1;
                }
                
            }
        }

        private void stringsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (stringsList.SelectedIndex != -1)
            {
                hexBox1.Select(stringsIndexes[stringsList.SelectedIndex], ((String)stringsList.SelectedItem).Length);
                tabControl1.SelectedIndex = 0;
                hexBox1.Select();
            }
        }

        private byte[] interpArray = null;
        private bool selectionChanged = true;

        private void refreshInterpretationTab()
        {
            long size = 8;
            if (hexBox1.SelectionLength > 8)
                size = hexBox1.SelectionLength;

            interpArray = new byte[size];

            if ((BitConverter.IsLittleEndian && !littleEndianBox.Checked)
                || (!BitConverter.IsLittleEndian && littleEndianBox.Checked))
            {
                long idx = hexBox1.SelectionLength - 1;
                for (long i = 0; i < hexBox1.SelectionLength; ++i, --idx)
                {
                    interpArray[idx] = provider.ReadByte(hexBox1.SelectionStart + i);
                }
            }
            else
            {
                for (long i = 0; i < hexBox1.SelectionLength; ++i)
                {
                    interpArray[i] = provider.ReadByte(hexBox1.SelectionStart + i);
                }
            }

            selectedLbl.Text = BitConverter.ToString(interpArray);

            i8Box.Text = ((sbyte)interpArray[0]).ToString();
            u8Box.Text = interpArray[0].ToString();
            short i16 = BitConverter.ToInt16(interpArray, 0);
            i16Box.Text = i16.ToString();
            u16Box.Text = ((ushort)i16).ToString();
            int i32 = BitConverter.ToInt32(interpArray, 0);
            i32Box.Text = i32.ToString();
            u32Box.Text = ((uint)i32).ToString();
            long i64 = BitConverter.ToInt64(interpArray, 0);
            i64Box.Text = i64.ToString();
            u64Box.Text = ((ulong)i64).ToString();

            f32Box.Text = BitConverter.ToSingle(interpArray, 0).ToString();
            f64Box.Text = BitConverter.ToDouble(interpArray, 0).ToString(); binBox.Text = "";
        }

        private void Interpretation_Enter(object sender, EventArgs e)
        {
            if (selectionChanged)
            {
                refreshInterpretationTab();
                selectionChanged = false;
                binBox.Text = "";
                unicodeBox.Text = "";
            }
        }

        private void littleEndianBox_CheckedChanged(object sender, EventArgs e)
        {
            refreshInterpretationTab();
            binBox.Text = "";
            unicodeBox.Text = "";
        }

        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            selectionChanged = true;
        }

        private void hexBox1_SelectionStartChanged(object sender, EventArgs e)
        {
            selectionChanged = true;
        }

        private void calcUTFnBinBox_Click(object sender, EventArgs e)
        {
            binBox.Text = "";
            for (long i = 0; i < interpArray.Length; ++i)
                binBox.Text += Convert.ToString(interpArray[i], 2).PadLeft(8, '0') + " ";

            unicodeBox.Text = Encoding.Unicode.GetString(interpArray);
        }

        public void Open(String file)
        {
            try
            {
                provider = new DynamicFileByteProvider(file);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            path = file;
            hexBox1.ByteProvider = provider;

            menuEdit.Enabled = true;
            menuSave.Enabled = true;
            menuSaveAs.Enabled = true;
            menuReset.Enabled = true;
            menuClose.Enabled = true;
            menuOpen.Enabled = false;

            this.Text = "Aeiris v1.15";

            tabControl1.Visible = true;
            dragDropLbl.Visible = true;
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Open(openFileDialog1.FileName);
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            provider.ApplyChanges();
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                for (long i = 0; i < provider.Length; ++i)
                {
                    fs.WriteByte(provider.ReadByte(i));
                }
                fs.Close();
            }
        }

        private void menuReset_Click(object sender, EventArgs e)
        {
            if(provider.HasChanges())
            {
                ((IDisposable)provider).Dispose();
                Open(path);
            }
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            path = null;
            ((IDisposable)provider).Dispose();
            provider = null;
            hexBox1.ByteProvider = provider;
            
            menuEdit.Enabled = false;
            menuSave.Enabled = false;
            menuSaveAs.Enabled = false;
            menuReset.Enabled = false;
            menuClose.Enabled = false;
            menuOpen.Enabled = true;
            tabControl1.SelectedIndex = 0;
            selectionChanged = true;

            dragDropLbl.Visible = true;
            tabControl1.Visible = false;
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            hexBox1.Cut();
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            hexBox1.Copy();
        }

        private void menuPaste_Click(object sender, EventArgs e)
        {
            hexBox1.Paste();
        }

        private void menuSelectAll_Click(object sender, EventArgs e)
        {
            hexBox1.SelectAll();
        }

        private void menuCopyHex_Click(object sender, EventArgs e)
        {
            hexBox1.CopyHex();
        }

        private void menuFind_Click(object sender, EventArgs e)
        {
            findFm.Show();
        }

        private void menuCalculator_Click(object sender, EventArgs e)
        {
            Process.Start("calc");
        }

        private void menuReplace_Click(object sender, EventArgs e)
        {
            replFm.Show();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Aeiris is an hex editor written in C# by Andrea Fioraldi.\n\nFor bugs or info write to andreafioraldi@gmail.com");
        }

        private void menuLicense_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Copyright 2017 Andrea Fioraldi <andreafioraldi@gmail.com>

Aeiris is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

Stout Compiler is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
MA 02110-1301, USA. Remastered and edited by ");
        }

        private void menuGoto_Click(object sender, EventArgs e)
        {
            gotoFm.Show();
        }

        private void dragDropLbl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if(files.Length > 0)
            {
                Open(files[0]);
            }
        }

        private void dragDropLbl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void menuInsert_Click(object sender, EventArgs e)
        {
            menuInsert.Checked = !menuInsert.Checked;
            hexBox1.InsertActive = menuInsert.Checked;
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            provider.DeleteBytes(hexBox1.SelectionStart, hexBox1.SelectionLength);
        }

        private void menuFindNext_Click(object sender, EventArgs e)
        {
            findFm.Next();
        }

        private void menuConverter_Click(object sender, EventArgs e)
        {
            convertFm.Show();
        }

        private void saveListBtn_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter fs = new StreamWriter(saveFileDialog1.OpenFile());
                for (int i = 0; i < stringsList.Items.Count; ++i)
                {
                    fs.WriteLine(stringsList.Items[i]);
                }
                fs.Close();
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void dragDropLbl_Click(object sender, EventArgs e)
        {

        }

        private void stringsList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            stringsIndexes = new List<long>();

            bool[] printables = null;
            if (AsciiBox.Checked)
                printables = printables_ASCII;
            else if (extAsciiBox.Checked)
                printables = printables_extASCII;
            else if (alphanumAsciiBox.Checked)
                printables = printables_alnumASCII;

            long min = (long)strMinimumBox.Value;

            long strIndex = -1;
            String str = "";
            for (long i = 0; i < provider.Length; ++i)
            {
                byte b = provider.ReadByte(i);
                if ((b == 0 || b == '\n' || b == '\r') && strIndex != -1)
                {
                    if (i - strIndex >= min)
                    {
                        str = "";
                        for (long k = strIndex; k < i; ++k)
                            str += (char)provider.ReadByte(k);
                        stringsList.Items.Add(str);
                        stringsIndexes.Add(strIndex);
                    }
                    strIndex = -1;
                }
                else if (printables[b])
                {
                    if (strIndex == -1)
                    {
                        strIndex = i;
                    }
                }
                else if (strIndex != -1)
                {
                    strIndex = -1;
                }

            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            MessageBox.Show("Saved!");
        }

        string nl = Environment.NewLine;
        private void button2_Click(object sender, EventArgs e)
        {
            File.WriteAllText("log.txt", "");
            button2.ForeColor = Color.Green;
            if (ec_gray.Checked == true) { File.AppendAllText("log.txt", "ec:gray" + nl); }
            else if (ec_black.Checked == true) { File.AppendAllText("log.txt", "ec:black" + nl ); }
            else if (ec_blue.Checked == true) { File.AppendAllText("log.txt", "ec:blue" + nl ); }
            else if (ec_pink.Checked == true) { File.AppendAllText("log.txt", "ec:pink" + nl ); }
            else if (ec_yellow.Checked == true) { File.AppendAllText("log.txt", "ec:yellow" + nl); }
           
            if (ls_disabled.Checked == true) { File.AppendAllText("log.txt", "ls:disabled" + nl); }
            else if (ls_enabled.Checked == true) { File.AppendAllText("log.txt", "ls:enabled" + nl); }

            if (fpt_default.Checked == true) { File.AppendAllText("log.txt", "fpt:default" + nl); }
            else if (fpt_fcl.Checked == true) { File.AppendAllText("log.txt", "fpt:fcl" + nl); }
            else if (fpt_luaoj.Checked == true) { File.AppendAllText("log.txt", "fpt:lua/j" + nl); }
            else if (fpt_unitydevh.Checked == true) { File.AppendAllText("log.txt", "fpt:unitydevh" + nl); }
            else if (fpt_unrealdev.Checked == true) { File.AppendAllText("log.txt", "fpt:unrealdev" + nl); }

            if (dk_cpp.Checked == true) { File.AppendAllText("log.txt", "dk:c++" + nl); }
            else if (dk_cs.Checked == true) { File.AppendAllText("log.txt", "dk:c#" + nl); }

            MessageBox.Show("Saved! Please restart your program for the changes to take place!");
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
          //MessageBox.Show("Warning: there might be problems with launching! Please also be aware that settings wont show as 'checked' but will still be saved.");
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            File.WriteAllText("log.txt", "");
            textBox1.Text = "";
            MessageBox.Show("Deleted!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                           //
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("#language: <lang> @include: <code>");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sent!");
        }
    }
}
