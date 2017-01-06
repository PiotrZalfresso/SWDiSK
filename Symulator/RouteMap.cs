using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Symulator
{
    public partial class RouteMap : Form
    {
        public RouteMap(string start, string end)
        {
            InitializeComponent();
            string url = "http://www.google.pl/maps/dir/"+ start + "/" + end;
            webBrowser1.Navigate(new Uri(url));
        }
    }
}
