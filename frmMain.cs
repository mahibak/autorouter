﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autorouter
{
    public partial class frmMain : Form
    {
        Map map = new Map();

        public frmMain()
        {
            InitializeComponent();
            mapView1.SetMap(map);
        }
        
        private void mapView1_MouseMove(object sender, MouseEventArgs e)
        {
            Text = map.PixelToTile(e.X, e.Y).ToString();
        }
    }
}