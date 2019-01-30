using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqToSQLGiris
{
    public partial class Form1 : Form
    {
        NorthWindDataContext ctx = new NorthWindDataContext();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Product p = new Product();
            p.ProductName = txtProductName.Text;
            p.UnitPrice = nudFiyat.Value;
            p.UnitsInStock = Convert.ToInt16(nudStok.Value);
            p.CategoryID = (int)comboKategori.SelectedValue;
            p.SupplierID = (int)comboTedarikci.SelectedValue;

            ctx.Products.InsertOnSubmit(p);
            MessageBox.Show($"SubmitChanges oncesi ProductID={p.ProductID}");
            ctx.SubmitChanges();// değişiklikleri ADO.Net koduna çevirerek veritabanına gönder
            MessageBox.Show($"SubmitChanges sonrası ProductID={p.ProductID}");

            //refresh the grid
            //GetNewBindingList-> eklediğimiz datagridviewde görünmesi için.
            dataGridView1.DataSource = ctx.Products.GetNewBindingList();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //data gridvievde productsu listeledi otomatik.
            dataGridView1.DataSource = ctx.Products;

            comboKategori.DisplayMember = "CategoryName";
            comboKategori.ValueMember = "CategoryID";
            comboKategori.DataSource = ctx.Categories;

            comboTedarikci.DisplayMember = "CompanyName";
            comboTedarikci.ValueMember = "SupplierID";
            comboTedarikci.DataSource = ctx.Suppliers;

            //Kategori ve tedarikci adnın grigcieve gelmesi için
            //kategori ve tedarikçi adinin adatgridview'a gelemesi için sunları ekledik:
            //product ile category tablolarını join et;
            var sonuc = from urun in ctx.Products
                        join kat in ctx.Categories
                        on urun.CategoryID equals kat.CategoryID
                        select new // asağıdaki bes kolondan olusan yeni bir entity tipi.(anonim)
                        {
                            urun.ProductID,
                            urun.ProductName,
                            urun.UnitPrice,
                            urun.UnitsInStock,
                            kat.CategoryName
                        };
            //dataGridView1.DataSource = null;
            //dataGridView1.DataSource = sonuc;
            dataGridView1.DataSource = ctx.Products.GetNewBindingList();
          
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            //once contex ten sil sonra onayla veritabanından sil
            int urunID = (int)dataGridView1.CurrentRow.Cells["ProductId"].Value;
            Product p = ctx.Products.SingleOrDefault(urun => urun.ProductID == urunID);
            ctx.Products.DeleteOnSubmit(p);
            ctx.SubmitChanges();

            //refresh the grid
            dataGridView1.DataSource = ctx.Products.GetNewBindingList();
          
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow r = dataGridView1.CurrentRow;
            txtProductName.Text = r.Cells["ProductName"].Value.ToString();
            nudFiyat.Value = Convert.ToDecimal(r.Cells["UnitPrice"].Value);
            nudStok.Value = Convert.ToDecimal(r.Cells["UnitsInStock"].Value);
            comboKategori.SelectedValue = (int)r.Cells["CategoryId"].Value;
            comboTedarikci.SelectedValue = (int)r.Cells["SupplierId"].Value;
            txtProductName.Tag = r.Cells["ProductId"].Value;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            Product p = ctx.Products.SingleOrDefault(urun => urun.ProductID == (int)txtProductName.Tag);
            p.ProductName = txtProductName.Text;
            p.UnitPrice = nudFiyat.Value;
            p.UnitsInStock = Convert.ToInt16(nudStok.Value);
            p.CategoryID = (int)comboKategori.SelectedValue;
            p.SupplierID = (int)comboTedarikci.SelectedValue;
            ctx.SubmitChanges();//değişiklikleri ADO.net koduna çevirerek veritabanına gönder
                                //refresh the grid
            dataGridView1.DataSource = ctx.Products.GetNewBindingList();
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ctx.Products.Where(x => x.ProductName.Contains(txtAra.Text));
        }
    }
}
