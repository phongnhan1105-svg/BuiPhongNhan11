using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTapBuoi4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Gắn sự kiện cho nút
            btnThemSua.Click += btnThemSua_Click;
            btnXoa.Click += btnXoa_Click;
            dgvSinhVien.CellClick += dgvSinhVien_CellClick;
            this.FormClosing += Form1_FormClosing;
        }

        // ===== Khi form load =====
        private void Form1_Load(object sender, EventArgs e)
        {
            // Căn giữa 1 số cột
            dgvSinhVien.Columns["ColMSSV"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSinhVien.Columns["colGioiTinh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSinhVien.Columns["colDTB"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Format điểm TB
            dgvSinhVien.Columns["colDTB"].DefaultCellStyle.Format = "N2";
        }

        // ===== Kiểm tra dữ liệu nhập =====
        private bool KiemTraHopLe()
        {
            if (!Regex.IsMatch(txtMSSV.Text, @"^\d{10}$"))
            {
                MessageBox.Show("MSSV phải gồm đúng 10 chữ số!");
                txtMSSV.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || !Regex.IsMatch(txtHoTen.Text, @"^[\p{L}\s]+$"))
            {
                MessageBox.Show("Họ tên chỉ chứa chữ và khoảng trắng!");
                txtHoTen.Focus();
                return false;
            }

            if (!rdoNam.Checked && !rdoNu.Checked)
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return false;
            }

            if (!float.TryParse(txtDTB.Text, out float dtb) || dtb < 0 || dtb > 10)
            {
                MessageBox.Show("Điểm TB phải là số từ 0 đến 10!");
                txtDTB.Focus();
                return false;
            }

            if (cboKhoa.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn khoa!");
                return false;
            }

            return true;
        }

        // ===== Nút Thêm/Sửa =====
        private void btnThemSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraHopLe()) return;

            string mssv = txtMSSV.Text.Trim();
            string hoten = txtHoTen.Text.Trim();
            string gioitinh = rdoNam.Checked ? "Nam" : "Nữ";
            string dtb = float.Parse(txtDTB.Text).ToString("0.00");
            string khoa = cboKhoa.Text;

            // Kiểm tra xem MSSV đã có chưa
            DataGridViewRow existingRow = null;
            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                if (row.Cells["ColMSSV"].Value?.ToString() == mssv)
                {
                    existingRow = row;
                    break;
                }
            }

            if (existingRow == null) // Thêm mới
            {
                dgvSinhVien.Rows.Add(mssv, hoten, gioitinh, dtb, khoa);
            }
            else // Sửa
            {
                existingRow.Cells["colHoTen"].Value = hoten;
                existingRow.Cells["colGioiTinh"].Value = gioitinh;
                existingRow.Cells["colDTB"].Value = dtb;
                existingRow.Cells["colKhoa"].Value = khoa;
            }

            CapNhatThongKe();
            DatLaiForm();
        }

        // ===== Nút Xóa =====
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgvSinhVien.Rows.RemoveAt(dgvSinhVien.SelectedRows[0].Index);
                    CapNhatThongKe();
                    DatLaiForm();
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn một sinh viên để xóa!");
            }
        }

        // ===== Khi bấm vào 1 dòng trong DataGridView =====
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
                txtMSSV.Text = row.Cells["ColMSSV"].Value?.ToString();
                txtHoTen.Text = row.Cells["colHoTen"].Value?.ToString();
                string gt = row.Cells["colGioiTinh"].Value?.ToString();
                rdoNam.Checked = (gt == "Nam");
                rdoNu.Checked = (gt == "Nữ");
                txtDTB.Text = row.Cells["colDTB"].Value?.ToString();
                cboKhoa.Text = row.Cells["colKhoa"].Value?.ToString();
            }
        }

        // ===== Reset form =====
        private void DatLaiForm()
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            txtDTB.Clear();
            rdoNam.Checked = false;
            rdoNu.Checked = false;
            cboKhoa.SelectedIndex = -1;
        }

        // ===== Cập nhật số lượng Nam/Nữ =====
        private void CapNhatThongKe()
        {
            int nam = 0, nu = 0;
            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                string gt = row.Cells["colGioiTinh"].Value?.ToString();
                if (gt == "Nam") nam++;
                else if (gt == "Nữ") nu++;
            }
            txtNam.Text = nam.ToString();
            txtNu.Text = nu.ToString();
        }

        // ===== Xác nhận khi thoát =====
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}