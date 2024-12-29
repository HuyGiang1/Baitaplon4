using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using e_excel = Microsoft.Office.Interop.Excel;
namespace Baitaplon
{

    public partial class Giaodienchinh : Form
    {

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-5DF9QTR\\SQLEXPRESS;Initial Catalog=BAITAPLON;Integrated Security=True");
        public static object DataGridView2 { get; internal set; }

        public Giaodienchinh()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            panel1.BackColor = ColorTranslator.FromHtml("#F29F05");
            label2.BackColor = panel1.BackColor;
        }

        private void Giaodienchinh_Load(object sender, EventArgs e)
        {
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            String sql = "Select * From Monhoc ";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable tb = new DataTable();
            da.Fill(tb);
            cmd.Dispose();
            con.Close();

            dataGridView1.DataSource = tb;
            dataGridView1.Refresh();
            LoadData();
            // Kiểm tra xem người dùng nhấn vào nút "Đăng ký"
            // Đếm số dòng hiện có trong DataGridView
            CheckDuplicateAndDisableButtons();
            FormatDataGridView2();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra chỉ số hàng và cột
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Kiểm tra nếu là cột nút
                if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    // Lấy giá trị nút hiện tại
                    var buttonCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (buttonCell.Value != null)
                    {
                        string buttonText = buttonCell.Value.ToString();

                        if (buttonText == "Đã đăng ký")
                        {
                            // Hiển thị thông báo nếu đã đăng ký
                            buttonCell.ReadOnly = true;
                        }
                        else
                        {
                            // Lấy thông tin từ hàng được chọn
                            string maHocPhan = dataGridView1.Rows[e.RowIndex].Cells["mahocphan"].Value.ToString();
                            string tenHocPhan = dataGridView1.Rows[e.RowIndex].Cells["tenhocphan"].Value.ToString();
                            int stc = int.Parse(dataGridView1.Rows[e.RowIndex].Cells["stc"].Value.ToString());
                            FormDangKy formDangKy = new FormDangKy(this, maHocPhan, stc);
                            formDangKy.Show();
                            LoadData();
                        }
                    }
                }
            }
        }

        public void AddRowToDataGridView2(object[] rowData)
        {
            dataGridView2.Rows.Add(rowData);
        }
        public void LoadData()
        {
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-5DF9QTR\\SQLEXPRESS;Initial Catalog=BAITAPLON;Integrated Security=True"))
            {
                con.Open();
                string query = "SELECT * FROM Ketquadangky";
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                DataTable tb = new DataTable();
                adapter.Fill(tb);

                // Xóa các dòng cũ trong DataGridView
                dataGridView2.Rows.Clear();

                // Tải dữ liệu từ DataTable vào DataGridView (bỏ qua cột stt trong database)
                foreach (DataRow row in tb.Rows)
                {
                    int newRowIndex = dataGridView2.Rows.Add();
                    DataGridViewRow newRow = dataGridView2.Rows[newRowIndex];

                    newRow.Cells["mahocphankq"].Value = row["mahocphan"]; // Cột "mahocphan"
                    newRow.Cells["loaikq"].Value = row["loai"];           // Cột "loai"
                    newRow.Cells["malhpkq"].Value = row["malhp"];         // Cột "malhp"
                    newRow.Cells["tenlhpkq"].Value = row["tenlhp"];       // Cột "tenlhp"
                    newRow.Cells["stckq"].Value = row["stc"];             // Cột "stc"
                    newRow.Cells["svkq"].Value = row["gv"];               // Cột "gv"
                    newRow.Cells["lichhockq"].Value = row["lichhoc"];     // Cột "lichhoc"
                    newRow.Cells["tungaykq"].Value = row["tungay"];       // Cột "tungay"
                    newRow.Cells["denngaykq"].Value = row["denngay"];     // Cột "denngay"
                    newRow.Cells["huy"].Value = "Hủy";                 // Cột nút "hủy" (nếu cần)
                }
                // Cập nhật giá trị cột "stt" (đánh số tăng dần hoặc giảm dần)
                int rowCount = dataGridView2.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    // Đánh số tăng dần
                    dataGridView2.Rows[i].Cells["sttkq"].Value = i + 1;

                    // Hoặc đánh số giảm dần
                    // dataGridView2.Rows[i].Cells["stt"].Value = rowCount - i;
                }
            }
            int rowCount1 = dataGridView1.Rows.Count;
            for (int i = 0; i < rowCount1; i++)
            {
                // Đánh số tăng dần
                dataGridView1.Rows[i].Cells["dangky"].Value = "Đăng ký";
                dataGridView1.Rows[i].Cells["stt"].Value = i + 1;
                // Hoặc đánh số giảm dần
                // dataGridView2.Rows[i].Cells["stt"].Value = rowCount - i;
            }
       

        }

        public class DataGridViewDisableButtonCell : DataGridViewButtonCell
        {
            public bool Enabled { get; set; } = true;

            protected override void Paint(
                Graphics graphics,
                Rectangle clipBounds,
                Rectangle cellBounds,
                int rowIndex,
                DataGridViewElementStates elementState,
                object value,
                object formattedValue,
                string errorText,
                DataGridViewCellStyle cellStyle,
                DataGridViewAdvancedBorderStyle advancedBorderStyle,
                DataGridViewPaintParts paintParts)
            {
                // Vẽ nút bình thường nếu Enabled = true
                if (this.Enabled)
                {
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                }
                else
                {
                    // Nếu bị vô hiệu hóa, đổi màu để thể hiện
                    ButtonRenderer.DrawButton(graphics, cellBounds, PushButtonState.Disabled);

                    // Vẽ nội dung văn bản
                    TextRenderer.DrawText(
                        graphics,
                        formattedValue?.ToString(),
                        cellStyle.Font,
                        cellBounds,
                        cellStyle.ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                    );
                }
            }
        }

        public void CheckDuplicateAndDisableButtons()
        {
            foreach (DataGridViewRow row1 in dataGridView1.Rows)
            {
                string maHocPhan1 = row1.Cells["mahocphan"]?.Value?.ToString()?.Trim();

                if (string.IsNullOrEmpty(maHocPhan1))
                    continue;

                bool isDuplicate = false;

                foreach (DataGridViewRow row2 in dataGridView2.Rows)
                {
                    string maHocPhan2 = row2.Cells["mahocphankq"]?.Value?.ToString()?.Trim();

                    if (maHocPhan1 == maHocPhan2)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (isDuplicate)
                {
                    // Thay đổi giao diện nút
                    var buttonCell = row1.Cells["dangky"] as DataGridViewButtonCell;
                    if (buttonCell != null)
                    {
                        buttonCell.Style.ForeColor = Color.Gray;
                        buttonCell.Style.BackColor = Color.LightGray;
                        buttonCell.Value = "Đã đăng ký";
                        buttonCell.ReadOnly = true;
                        // Vô hiệu hóa nút bằng cách hủy sự kiện
                        buttonCell.Tag = "disabled"; // Gắn cờ để biết nút này đã vô hiệu
                    }
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra chỉ số hàng và cột
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Kiểm tra nếu là cột nút "Hủy"
                if (dataGridView2.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    // Hiển thị thông báo xác nhận
                    DialogResult result = MessageBox.Show("Bạn có đồng ý hủy học phần này không?",
                                                          "Xác nhận",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            // Lấy dữ liệu từ dòng hiện tại
                            string mahocphankq = dataGridView2.Rows[e.RowIndex].Cells["mahocphankq"].Value?.ToString();
                            string malhp = dataGridView2.Rows[e.RowIndex].Cells["malhpkq"].Value?.ToString();

                            if (string.IsNullOrEmpty(mahocphankq) || string.IsNullOrEmpty(malhp))
                            {
                                MessageBox.Show("Dữ liệu không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                                // Xóa dữ liệu trong cơ sở dữ liệu
                                using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-5DF9QTR\\SQLEXPRESS;Initial Catalog=BAITAPLON;Integrated Security=True"))
                            {
                                conn.Open();
                                string query = "DELETE FROM Ketquadangky WHERE malhp = @malhp";
                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@malhp", malhp);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Cập nhật lại DataGridView1
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells["mahocphan"].Value?.ToString() == mahocphankq)
                                {
                                    DataGridViewButtonCell btnCell = row.Cells["dangky"] as DataGridViewButtonCell;
                                    if (btnCell != null)
                                    {
                                        btnCell.Value = "Đăng ký"; // Đổi lại text thành "Đăng ký"
                                        btnCell.ReadOnly = false;  // Kích hoạt lại nút
                                    }
                                }
                            }

                            // Xóa dòng trong DataGridView2
                            dataGridView2.Rows.RemoveAt(e.RowIndex);

                            // Hiển thị thông báo thành công
                            MessageBox.Show("Hủy học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Có lỗi xảy ra: {ex.Message}\n{ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        private void FormatDataGridView2()
        {
            // Đặt font cho toàn bộ DataGridView2
            Font font = new Font("Arial", 10, FontStyle.Regular);
            dataGridView2.DefaultCellStyle.Font = font;

            // Đặt font cho các tiêu đề cột
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = font;

            // Định dạng tự động chiều rộng cột
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Đặt màu nền cho các hàng trong DataGridView2
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Đặt lại định dạng cho các cột nếu cần
            dataGridView2.Columns["sttkq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["mahocphankq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns["loaikq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns["malhpkq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns["tenlhpkq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns["stckq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["svkq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["lichhockq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["tungaykq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns["denngaykq"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }



        private void UpdateRowDangky(DataGridView gridView)
        {
            for (int i = 0; i < gridView.Rows.Count; i++)
            {
                gridView.Rows[i].Cells["dangky"].Value = "Đăng ký"; // "stt" là tên cột chứa số thứ tự
            }
        }
        private void UpdateRowNumbers(DataGridView gridView)
        {
            for (int i = 0; i < gridView.Rows.Count; i++)
            {
                gridView.Rows[i].Cells["sttkq"].Value = i + 1; // "stt" là tên cột chứa số thứ tự
            }
        }
        private void dataGridView2_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            // Logic xử lý sau khi một hàng bị xóa khỏi dataGridView2
            UpdateRowNumbers(dataGridView2); // Cập nhật số thứ tự (STT) nếu cần
        }

        private void ExportExcel(DataTable tb, string sheetname)
        {
            //Tạo các đối tượng Excel 

            e_excel.Application oExcel = new e_excel.Application();
            e_excel.Workbooks oBooks;
            e_excel.Sheets oSheets;
            e_excel.Workbook oBook;
            e_excel.Worksheet oSheet;
            //Tạo mới một Excel WorkBook 
            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;
            oBooks = oExcel.Workbooks;
            oBook = (e_excel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            oSheets = oBook.Worksheets;
            oSheet = (e_excel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = sheetname;
            // Tạo phần đầu nếu muốn
            e_excel.Range head = oSheet.get_Range("A1", "H1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH MÔN HỌC ĐÃ ĐĂNG KÝ";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = 16;
            head.HorizontalAlignment = e_excel.XlHAlign.xlHAlignCenter;

            e_excel.Range head2 = oSheet.get_Range("A2", "H2");
            head2.MergeCells = true;
            head2.Value2 = "Năm học 2024-2025 - HK02";
            head2.Font.Bold = true;
            head2.Font.Name = "Tahoma";
            head2.Font.Size = 14;
            head2.HorizontalAlignment = e_excel.XlHAlign.xlHAlignCenter;
            // Tạo tiêu đề cột 
            e_excel.Range cl1 = oSheet.get_Range("A4", "A4");
            cl1.Value2 = "STT";
            cl1.ColumnWidth = 5;
            e_excel.Range cl2 = oSheet.get_Range("B4", "B4");
            cl2.Value2 = "MÃ HỌC PHẦN";
            cl2.ColumnWidth = 15;
            e_excel.Range cl3 = oSheet.get_Range("C4", "C4");
            cl3.Value2 = "LOẠI";
            cl3.ColumnWidth = 15.0;
            e_excel.Range cl4 = oSheet.get_Range("D4", "D4");
            cl4.Value2 = "MÃ LHP";
            cl4.ColumnWidth = 15.0;
            e_excel.Range cl5 = oSheet.get_Range("E4", "E4");
            cl5.Value2 = "TÊN LHP";
            cl5.ColumnWidth = 40;
            e_excel.Range cl6 = oSheet.get_Range("F4", "F4");
            cl6.Value2 = "STC";
            cl6.ColumnWidth = 5;
            e_excel.Range cl7 = oSheet.get_Range("G4", "G4");
            cl7.Value2 = "GIẢNG VIÊN";
            cl7.ColumnWidth = 25;
            e_excel.Range cl8 = oSheet.get_Range("H4", "H4");
            cl8.Value2 = "THỜI KHÓA BIỂU";
            cl8.ColumnWidth = 50;
            //Microsoft.Office.Interop.Excel.Range cl6 = oSheet.get_Range("F3", "F3");
            //cl6.Value2 = "NGÀY THI";
            //cl6.ColumnWidth = 15.0;
            //Microsoft.Office.Interop.Excel.Range cl6_1 = oSheet.get_Range("F4", "F1000");
            //cl6_1.Columns.NumberFormat = "dd/mm/yyyy";

            e_excel.Range rowHead = oSheet.get_Range("A4", "H4");
            rowHead.Font.Bold = true;
            // Kẻ viền
            rowHead.Borders.LineStyle = e_excel.Constants.xlSolid;
            // Thiết lập màu nền
            rowHead.Interior.ColorIndex = 15;
            rowHead.HorizontalAlignment = e_excel.XlHAlign.xlHAlignCenter;
            // Tạo mảng đối tượng để lưu dữ toàn bồ dữ liệu trong DataTable,
            // vì dữ liệu được được gán vào các Cell trong Excel phải thông qua object thuần.
            object[,] arr = new object[tb.Rows.Count, tb.Columns.Count];
            //Chuyển dữ liệu từ DataTable vào mảng đối tượng
            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                for (int c = 0; c < tb.Columns.Count; c++)

                {
                    if (c == 2 || c == 5)
                        arr[r, c] = "'" + dr[c].ToString();
                    else
                        arr[r, c] = dr[c];
                }
            }
            //Thiết lập vùng điền dữ liệu
            int rowStart = 5;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = tb.Columns.Count;
            // Ô bắt đầu điền dữ liệu
            e_excel.Range c1 = (e_excel.Range)oSheet.Cells[rowStart, columnStart];
            // Ô kết thúc điền dữ liệu
            e_excel.Range c2 = (e_excel.Range)oSheet.Cells[rowEnd, columnEnd];
            // Lấy về vùng điền dữ liệu
            e_excel.Range range = oSheet.get_Range(c1, c2);
            //Điền dữ liệu vào vùng đã thiết lập
            range.Value2 = arr;
            // Kẻ viền
            range.Borders.LineStyle = e_excel.Constants.xlSolid;
            // Căn giữa cột STT
            range.Borders.LineStyle = e_excel.Constants.xlSolid;
            range.HorizontalAlignment = e_excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = e_excel.XlVAlign.xlVAlignCenter;

            MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Inketquadangky_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo một DataTable để lưu dữ liệu
                DataTable dataTable = new DataTable();

                // Thêm các cột vào DataTable, trừ 3 cột cuối
                for (int i = 0; i < dataGridView2.Columns.Count - 3; i++)
                {
                    dataTable.Columns.Add(dataGridView2.Columns[i].Name);
                }

                // Duyệt qua từng hàng trong dataGridView2 và thêm vào DataTable
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (!row.IsNewRow) // Bỏ qua dòng trống cuối cùng
                    {
                        DataRow dataRow = dataTable.NewRow();

                        // Lấy dữ liệu từng ô, trừ 3 cột cuối
                        for (int i = 0; i < dataGridView2.Columns.Count - 3; i++)
                        {
                            dataRow[dataGridView2.Columns[i].Name] = row.Cells[i].Value ?? DBNull.Value;
                        }

                        dataTable.Rows.Add(dataRow);
                    }
                }

                // Gọi hàm xuất Excel với DataTable vừa tạo
                ExportExcel(dataTable, "DSMONHOCDANGKY");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi lấy dữ liệu từ DataGridView2: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
