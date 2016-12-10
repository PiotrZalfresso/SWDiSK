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
    public partial class ShowDistMatrixForm : Form
    {
        public ShowDistMatrixForm()
        {
            InitializeComponent();
        }

        public void bindDataToGidView()
        {
            dataGridView1.ColumnCount = PackagesList.numberOfPackages + 1;

            for (int rowIndex = 0; rowIndex < PackagesList.numberOfPackages + 1; ++rowIndex)
            {
                var row = new DataGridViewRow();

                for (int columnIndex = 0; columnIndex < PackagesList.numberOfPackages + 1; ++columnIndex)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = Matrices.Distance[rowIndex, columnIndex]
                    });
                   
                }

                dataGridView1.Rows.Add(row);
                dataGridView1.Columns[rowIndex].HeaderText = rowIndex.ToString();

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }

            dataGridView1.Columns[PackagesList.numberOfPackages].HeaderText = "Hub";

        }
    }
}
