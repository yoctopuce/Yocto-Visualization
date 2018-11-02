
/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  Raw data window
 * 
 *  
 *   - - - - - - - - - License information: - - - - - - - - -
 *
 *  Copyright (C) 2017 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace YoctoVisualisation
{
  public partial class RawDataForm : Form
  {

    BackgroundWorker exportProcess;
    bool refreshDisabled = false;

    public RawDataForm()
    {
      InitializeComponent();
      timer1.Enabled = true;
    }

    public void showData()
    {
      Show();
      checkedListBox1.Items.Clear();
      foreach (CustomYSensor s in sensorsManager.sensorList)
        if (!(s is NullYSensor))
        {
          checkedListBox1.Items.Add(s);
        }
      refeshGridGeometry(null);

    }

    public void refeshGridGeometry(ItemCheckEventArgs e)
    {
      int sensCount = 0;
      int persensor = 0;

      List<CustomYSensor> slist = new List<CustomYSensor>();
      for (int i = 0; i < checkedListBox1.Items.Count; i++)
      {
        bool toAdd = false;
        if (checkedListBox1.GetItemChecked(i)) toAdd = true;
        if (e != null)
        {
          if (e.Index == i)
            toAdd = (e.NewValue == CheckState.Checked);
        }
        if (toAdd)
        {

          slist.Add((CustomYSensor)(checkedListBox1.Items[i]));
        }
      }
      sensCount = slist.Count;
      if (showMin.Checked) persensor++;
      if (showAvg.Checked) persensor++;
      if (showMax.Checked) persensor++;

      dataGridView1.ColumnCount = persensor * sensCount;
      dataGridView1.ColumnHeadersVisible = true;
      dataGridView1.RowHeadersWidth = 200;


      int n = 0;
      for (int i = 0; i < slist.Count; i++)
      {
        string sName = slist[i].ToString();
        if (showMin.Checked) dataGridView1.Columns[n++].Name = sName + "(min)";
        if (showAvg.Checked) dataGridView1.Columns[n++].Name = sName + "(avg)";
        if (showMax.Checked) dataGridView1.Columns[n++].Name = sName + "(max)";
      }

      for (int i = 0; i < dataGridView1.Columns.Count; i++)
      {
        dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      }
      RefreshContents(slist);
    }

    private void RefreshContents(List<CustomYSensor> slist)
    {
      dataGridView1.SuspendLayout();
      List<int> indexes = new List<int>();
      for (int i = 0; i < slist.Count; i++)
      {
        indexes.Add(slist[i].curData.Count - 1);
      }
      dataGridView1.Rows.Clear();
      bool sMin = false;
      bool sMax = false;
      bool sAvg = false;
      int toShow = 0;
      if (showMin.Checked) { sMin = true; toShow++; }
      if (showAvg.Checked) { sAvg = true; toShow++; }
      if (showMax.Checked) { sMax = true; toShow++; }

      int colcount = slist.Count * toShow;

      double MaxTimeStamp = 0;
      for (int i = 0; i < slist.Count; i++)
        if (indexes[i] >= 0)
          if (MaxTimeStamp < slist[i].curData[indexes[i]].DateTime)
            MaxTimeStamp = slist[i].curData[indexes[i]].DateTime;


      int rowcount = 0;
      while ((MaxTimeStamp > 0) && (rowcount < constants.MAXRAWDATAROWS))
      {
        rowcount++;
        string[] row = new string[colcount];


        double nextTimeStamp = 0;
        for (int i = 0; i < slist.Count; i++)
        {
          bool Shown = false;
          if (indexes[i] >= 0)
          { if (slist[i].curData[indexes[i]].DateTime == MaxTimeStamp)
            {
              int n = 0;
              if (sMin) { row[n + i * toShow] = slist[i].minData[indexes[i]].Value.ToString(); n++; }
              if (sAvg) { row[n + i * toShow] = slist[i].curData[indexes[i]].Value.ToString(); n++; }
              if (sMax) { row[n + i * toShow] = slist[i].maxData[indexes[i]].Value.ToString(); n++; }
              Shown = true;
              indexes[i]--;
            }
              if (indexes[i] >= 0)
              {
                if (slist[i].curData[indexes[i]].DateTime > nextTimeStamp)
                  nextTimeStamp = slist[i].curData[indexes[i]].DateTime;

              }

            }
          if (!Shown)
          {
            int n = 0;
            if (sMin) { row[n + i * toShow] = ""; n++; }
            if (sAvg) { row[n + i * toShow] = ""; n++; }
            if (sMax) { row[n + i * toShow] = ""; n++; }
          }

        //  if (indexes[i] > 0)
        //  {
        //    double t = slist[i].minData[indexes[i]].DateTime;
        //    if ((t > nextTimeStamp) && (t < MaxTimeStamp)) nextTimeStamp = t;
        //  }
        }

        if (colcount > 0)
        {
          int idx = dataGridView1.Rows.Add(row);
          dataGridView1.Rows[idx].HeaderCell.Value = constants.UnixTimeStampToDateTime(MaxTimeStamp).ToString("yyyy/MM/dd HH:mm:ss.ff");
        }

        MaxTimeStamp = nextTimeStamp;
      }
      if (toShow == 0) label1.Text = "No data shown, select at least Min, Avg or Max options in the top-right checkboxes";
      else if ((rowcount) == 0) label1.Text = "No data shown, select at least one sensor in the list above.";
      else if (rowcount < constants.MAXRAWDATAROWS) label1.Text = "Here are the last " + rowcount.ToString() + " data rows.";
      else label1.Text = "Here are the last " + rowcount.ToString() + " data rows.  Use the export feature to get the whole data set.";

      CsvBtn.Enabled = ((rowcount > 0) && (toShow > 0));
      dataGridView1.ResumeLayout();
    }


    private List<string> getCvsdata(List<CustomYSensor> slist, bool sMin, bool sAvg, bool sMax)
    {

      int toShow = 0;

      List<int> indexes = new List<int>();
      for (int i = 0; i < slist.Count; i++) indexes.Add(0);


      if (sMin) toShow++;
      if (sAvg) toShow++;
      if (sMax) toShow++;
      List<string> res = new List<string>();
      string ColumnHeader = "Timestamp";

      for (int i = 0; i < slist.Count; i++)
      {
        string sName = slist[i].ToString();
        if (sMin) ColumnHeader += ";" + sName + "(min)";
        if (sAvg) ColumnHeader += ";" + sName + "(avg)";
        if (sMax) ColumnHeader += ";" + sName + "(max)";
      }
      res.Add(ColumnHeader);

      int colcount = slist.Count * toShow;

      double MinTimeStamp = Double.MaxValue;
      for (int i = 0; i < slist.Count; i++)
        if (indexes[i] < slist[i].curData.Count)
          if (MinTimeStamp > slist[i].curData[indexes[i]].DateTime)
            MinTimeStamp = slist[i].curData[indexes[i]].DateTime;



      while ((MinTimeStamp < Double.MaxValue))
      {
        string line = "";
        string[] row = new string[colcount];
        double nextTimeStamp = Double.MaxValue;
        for (int i = 0; i < slist.Count; i++)
        {
          bool Shown = false;
          if (indexes[i] < slist[i].curData.Count)
          { if (slist[i].curData[indexes[i]].DateTime == MinTimeStamp)
            {

              if (sMin) line += ";" + slist[i].minData[indexes[i]].Value.ToString();
              if (sAvg) line += ";" + slist[i].curData[indexes[i]].Value.ToString();
              if (sMax) line += ";" + slist[i].maxData[indexes[i]].Value.ToString();
              Shown = true;
              indexes[i]++;
            }
              if (indexes[i] < slist[i].curData.Count)
                if (slist[i].curData[indexes[i]].DateTime < nextTimeStamp)
                  nextTimeStamp = slist[i].curData[indexes[i]].DateTime;

            }
          if (!Shown)
          {

            if (sMin) line += " ; ";
            if (sAvg) line += " ; ";
            if (sMax) line += " ; ";
          }

          //  if (indexes[i] < slist[i].curData.Count)
          //  {
          //    double t = slist[i].minData[indexes[i]].DateTime;
          //    if ((t < nextTimeStamp) && (t > MinTimeStamp)) nextTimeStamp = t;
          //  }
        }

        if (colcount > 0)
        {

          line = constants.UnixTimeStampToDateTime(MinTimeStamp).ToString("yyyy/MM/dd HH:mm:ss.ff") + line;

        }
        res.Add(line);
        MinTimeStamp = nextTimeStamp;
      }
      return res;
    }


    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {









    }



    private void CheckedChanged(object sender, EventArgs e)
    {
      refeshGridGeometry(null);
    }



    private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
    {
     if (!refreshDisabled) refeshGridGeometry(e);
    }

    private void button2_Click(object sender, EventArgs e)
    {

    }

    private void RawDataForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing)
      {
        e.Cancel = true;
        Hide();
      }
    }


    private List<CustomYSensor> selection()
    {
      List<CustomYSensor> slist = new List<CustomYSensor>();
      for (int i = 0; i < checkedListBox1.Items.Count; i++)
      {
        if (checkedListBox1.GetItemChecked(i))
          slist.Add((CustomYSensor)(checkedListBox1.Items[i]));

      }
      return slist;
    }

    private void button2_Click_1(object sender, EventArgs e)
    {

      RefreshContents(selection());
    }


    protected void export_DoWork(object sender, DoWorkEventArgs e)
    {
      object[] p = (object[])e.Argument;
      Boolean[] s = (Boolean[])(p[2]);

      List<String> data = getCvsdata((List<CustomYSensor>)p[1], s[0], s[1], s[2]);

      StreamWriter sw = File.CreateText((String)p[0]);

      int n = 0;
      foreach (string line in data)
      {
        n++;
        sw.WriteLine(line);
        ((BackgroundWorker)sender).ReportProgress(((int)(50 + 50.0 * n / data.Count)));
      }
      sw.Close();
    }

    protected void export_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
      progressPanel.Visible = false;
      progressBar1.Visible = false;
    }

    protected void export_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;

    }


    private void button1_Click(object sender, EventArgs e)
    {

      if (saveFileDialog1.ShowDialog() == DialogResult.OK)
      {

        progressPanel.Visible = true;
        progressBar1.Visible = true;
        progressBar1.Value = 0;
        exportProcess = new BackgroundWorker();
        exportProcess.WorkerReportsProgress = true;
        exportProcess.DoWork += new DoWorkEventHandler(export_DoWork);
        exportProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(export_Completed);
        exportProcess.ProgressChanged += new ProgressChangedEventHandler(export_ProgressChanged);
        exportProcess.RunWorkerAsync(new object[] { saveFileDialog1.FileName, selection(), new Boolean[] { showMin.Checked, showAvg.Checked, showMax.Checked } });




      }


    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      double total = 0;
      double count = 0;

      for (int i = 0; i < checkedListBox1.Items.Count; i++)
      {
        if (checkedListBox1.GetItemChecked(i))
        {
          total += ((CustomYSensor)(checkedListBox1.Items[i])).getGetaLoadProgress(); ;
          count++;
        }
      }

      int percent = (int)(total / count);
      if ((count == 0) || (total >= 100)) Text = "Raw data";
      else Text = "Raw data (Loading from dataloggers " + percent.ToString() + "%)";


    }

    private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      refreshDisabled = true;
      for (int i = 0; i < checkedListBox1.Items.Count; i++)
      {
        checkedListBox1.SetItemChecked(i, true);

      }
      refreshDisabled = false;
      refeshGridGeometry(null);
    }

    private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
    {
      refreshDisabled = true;
      for (int i = 0; i < checkedListBox1.Items.Count; i++)
      {
        checkedListBox1.SetItemChecked(i, false);

      }
      refreshDisabled = false;
      refeshGridGeometry(null);
    }
  }
  }
