﻿/*
 *   Yocto-Visualization, a free application to visualize Yoctopuce Sensors.
 * 
 *  properties editor
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
 */



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace YoctoVisualisation
{
  public delegate void SetValueCallBack(PropertyValueChangedEventArgs e);



  public partial class PropertiesForm : Form
  {
    SetValueCallBack setCallback = null;
    XmlNode initNode;

    public PropertiesForm(XmlNode initDataNode)
    {
      InitializeComponent();
      // Set this property to intercept all events
      KeyPreview = true;
      initNode = initDataNode;


   }





    private void PropertiesForm_Load(object sender, EventArgs e)
    {
      StartForm.RestoreWindowPosition(this, initNode);


    }

    public void showWindow(GenericProperties prop, SetValueCallBack valueCallBack, bool ForceToTshow)
    {
      try
      {
        setCallback = valueCallBack;
     
        propertyGrid1.SelectedObject = prop;
        Text = "Properties of " + prop.Form_Text;
      }
      catch (Exception e){ LogManager.Log("Properties Windows error: " + e.Message); }

      if (ForceToTshow) Show();

    }

    public string getConfigData()
    {
      return "<PropertiesForm>\n"
            + "<location x='" + this.Location.X.ToString() + "' y='" + this.Location.Y.ToString() + "'/>\n"
            + "<size     w='" + this.Size.Width.ToString() + "' h='" + this.Size.Height.ToString() + "'/>\n"
            + "</PropertiesForm>\n";

    }


    private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      setCallback(e);



    }

    private void PropertiesForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (!propertyGrid1.RectangleToScreen(propertyGrid1.ClientRectangle).Contains(Cursor.Position))
      {
        return;
      }

      bool backward = false;
      // Handle tab key

      if (e.KeyCode == Keys.Tab && (e.Shift)) backward = true;
      else if (e.KeyCode != Keys.Tab) { return; }


      e.Handled = true;
      e.SuppressKeyPress = true;

      // Get selected griditem
      GridItem gridItem = propertyGrid1.SelectedGridItem;
      if (gridItem == null) { return; }

      // Create a collection all visible child griditems in propertygrid
      GridItem root = gridItem;
      while (root.GridItemType != GridItemType.Root)
      {
        root = root.Parent;
      }
      List<GridItem> gridItems = new List<GridItem>();
      FindItems(root, gridItems);

      // Get position of selected griditem in collection
      int index = gridItems.IndexOf(gridItem);
      int nextIndex;
      if (backward)

      {   // Select next griditem in collection
        nextIndex  = index - 1;
        if (nextIndex < 0)nextIndex = gridItems.Count - 1;
        // Select next griditem in collection
       


      }
      else
      {
        // Select next griditem in collection
        nextIndex = index + 1;
        if (nextIndex >= gridItems.Count) nextIndex = 0; 
        // Select next griditem in collection
       
      }
      propertyGrid1.SelectedGridItem = gridItems[nextIndex];
    }
 

    private void FindItems(GridItem item, List<GridItem> gridItems)
    {
      switch (item.GridItemType)
      {
        case GridItemType.Root:
        case GridItemType.Category:
          foreach (GridItem i in item.GridItems)
          {
            this.FindItems(i, gridItems);
          }
          break;
        case GridItemType.Property:
          gridItems.Add(item);
          if (item.Expanded)
          {
            foreach (GridItem i in item.GridItems)
            {
              this.FindItems(i, gridItems);
            }
          }
          break;
        case GridItemType.ArrayValue:
          break;
      }
    }

    private void PropertiesForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason != CloseReason.ApplicationExitCall)
      {
        e.Cancel = true;
        Hide();
      }
    }

   
  }
}
