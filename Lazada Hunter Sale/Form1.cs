using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.IO;

namespace Lazada_Hunter_Sale
{
    public partial class Form1 : Form
    {
        public Form1()
        {
        InitializeComponent();
        comboBox1.DisplayMember = "Text";
        comboBox1.ValueMember = "Value";

        var items = new[] {
        new { Text = "Điện thoại Máy tính bảng", Value = "dien-thoai-may-tinh-bang" },
        new { Text = "Điện thoại", Value = "dien-thoai-di-dong" },
        new { Text = "Đồ gia dụng", Value = "do-gia-dung" },
        new { Text = "Tủ lạnh", Value = "tu-lanh" }
        };
        comboBox1.DataSource = items;
        textBox4.Hide();
        radioButton1.Checked = true;
        comboBox1.Show();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            HtmlWeb htmlweb = new HtmlWeb();
            htmlweb.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
            String url = "http://www.lazada.vn/";
            if (radioButton1.Checked)
            {
                url += comboBox1.SelectedValue.ToString();
            }
            else
            {
                if (textBox4.Text != "")
                {
                    url += textBox4.Text.ToString();
                }
                else
                {
                    MessageBox.Show("Bạn hãy nhập danh mục (Có thể nhấp Help để xem thông tin) !");
                    url = "";

                }
            }
            if(url !="" && url !=null)
            {
                try
            {
                if (textBox2.Text != null && textBox3.Text != null)
                {
                    dataGridView1.Rows.Clear();
                    int count = 0;
                    progressBar1.Minimum = 0;
                    progressBar1.Value = 1;
                    progressBar1.Step = 1;
                    for (int i=Int32.Parse(textBox2.Text); i<Int32.Parse(textBox3.Text); i++)
                    {
                        HtmlAgilityPack.HtmlDocument htmldoc = htmlweb.Load(url+"/?page="+i);
                        // Lấy page cuối
                        var itemList = htmldoc.DocumentNode.QuerySelectorAll("div.c-paging__wrapper > a").ToList();
                        var last = itemList[itemList.Count - 2];
                        var link = last.Attributes["href"].Value;
                        var text = last.InnerText;
                        label5.Text = text.ToString();
                        // Lấy giảm giá theo %
                        var Listgiamgia = htmldoc.DocumentNode.QuerySelectorAll("div[data-qa-locator=product-item]").ToList();

                        foreach (var item in Listgiamgia)
                        {
                            count++;
                            // lấy link
                            var linkNode = item.QuerySelector("a");
                            var linkgiamgia = linkNode.Attributes["href"].Value;
                            // Lấy giá cũ
                            var getgiacu = item.QuerySelector("div.product-card__old-price");
                            var giacu = "Not Detect";
                            if (getgiacu!=null)
                            {
                               giacu = getgiacu.InnerText;
                            }
                           
                            // Lấy Giá sau khi giảm SP
                            var getgiasp = item.QuerySelector("div.product-card__price");
                            var giasp = "Not Detect";
                            if (getgiasp != null)
                            {
                                giasp = getgiasp.InnerText;
                            }

                            // Lấy sale
                            var getsale = item.QuerySelector("div.price-block--grid > div.product-card__sale");
                            var sale= "- 0%";
                            if (getsale != null)
                            {
                                sale = getsale.InnerText;
                            }
                            // So sánh với giá trị Sale trong textbox
                            if (textBox1.Text != null && textBox1.Text !="")
                            {
                               string justNumbers = new String(sale.Where(Char.IsDigit).ToArray());
                               int sale_txt = Int32.Parse(textBox1.Text);
                               if (Int32.Parse(justNumbers) >= sale_txt)
                                {
                                    dataGridView1.Rows.Add(new string[] { count.ToString(), linkgiamgia, giacu, giasp, sale });
                                    DataGridViewLinkColumn dgvColDeletion = new DataGridViewLinkColumn();
                                    dgvColDeletion.UseColumnTextForLinkValue = true;
                                    dgvColDeletion.Text = "Delete";
                                }
                            }
                            else
                            {
                                dataGridView1.Rows.Add(new string[] { count.ToString(), linkgiamgia, giacu, giasp, sale });
                                //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Descending);
                               
                            }
                            progressBar1.Maximum = Listgiamgia.Count;
                            progressBar1.PerformStep();
                        }
                    }//end for
                  
                }
               
            } //end try
                
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi trong quá trình xử lý, xin vui lòng kiểm tra lại");
            }
            }
        }
       

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch)) { e.Handled = true; }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch)) { e.Handled = true; }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string str = ((System.Windows.Forms.DataGridView)(sender)).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            System.Diagnostics.Process.Start(str);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Program Files (x86)\Lazada Hunter Sale\Lazada Hunter Sale\help.txt"))
            {
                System.Diagnostics.Process.Start("notepad.exe", @"C:\Program Files (x86)\Lazada Hunter Sale\Lazada Hunter Sale\help.txt");
            }
            else
            {
                System.Diagnostics.Process.Start("notepad.exe", @"C:\Program Files\Lazada Hunter Sale\Lazada Hunter Sale\help.txt");
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox4.Hide();
                comboBox1.Show();
            }
            if (radioButton2.Checked)
            {
                textBox4.Show();
                comboBox1.Hide();
            }
        }
        private void textBox4_Enter(object sender, EventArgs e)
        {
            toolTip1.Show("Ví dụ:http://www.lazada.vn/dien-thoai-di-dong/ \n Các bạn Nhập vào là [dien-thoai-di-dong]", textBox4,999999);

        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            toolTip1.Hide(textBox4);
        }
    }
}
