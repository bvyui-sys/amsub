using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using DPFP;
using DPFP.Capture;

namespace Attendance_Monitoring_System
{
    public partial class UserControlRegister : UserControl
    {
        private DPFP.Template Template;
        private string selectedEmpId = "";
        public UserControlRegister()
        {
            InitializeComponent();
            this.Controls.Add(lblEmpId);

        }

        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Action(delegate ()
            {
                Template = template;

                if (Template != null)
                {
                    MessageBox.Show("🎉 Fingerprint Template Ready!\n\n" +
                                  "The fingerprint template has been successfully created and saved.\n\n" +
                                  "✅ Template Status: Valid\n" +
                                  "✅ Database: Updated\n" +
                                  "✅ Employee: Ready for attendance\n\n" +
                                  "The employee can now use fingerprint authentication.",
                                  "Enrollment Complete", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("❌ Template Creation Failed!\n\n" +
                                  "The fingerprint template could not be created properly.\n\n" +
                                  "Possible reasons:\n" +
                                  "• Poor fingerprint quality\n" +
                                  "• Insufficient fingerprint samples\n" +
                                  "• Scanner connection issues\n\n" +
                                  "Please try scanning again with better finger placement.",
                                  "Enrollment Failed", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                }
            }));
        }

        private Label lblEmpId = new Label { Visible = false };
      


        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(pbSearchEmp, "Search");
        }

        private void UserControlRegister_Load(object sender, EventArgs e)
        {
            dgvDept.DataSource = MySQL.Pull("SELECT * FROM lasam_attendance.departments;");
            dgvDept.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDept.MultiSelect = false;

            LoadEmployees();
            LoadDepartments();
            LoadEmploymentTypes();
            SetupEmployeeGrid();
            SetupDepartmentGrid();
            LoadRoles();
            LoadShifts();
        }

        private void LoadEmployees()
        {
            string query = @"
        SELECT e.ID,
               e.employee_code,
               e.first_name,
               e.middle_name,
               e.last_name,
               e.birthdate,
               e.sex,
               e.address,
               d.description AS department,
               s.shift_name AS shift,
               e.date_added,
               et.Type AS employment_type,
               e.role,
               e.position,
               e.contact_number,
               e.photo
        FROM employees e
        LEFT JOIN departments d ON e.department_id = d.ID
        LEFT JOIN shift s ON e.shift_id = s.ID
        LEFT JOIN employmenttype et ON e.employmenttype_id = et.ID
        ORDER BY e.ID DESC;
    ";

            DataTable dt = MySQL.Pull(query);

            dgvReg.AutoGenerateColumns = false;
            dgvReg.DataSource = dt;

            if (dgvReg.Columns.Count == 0) 
            {
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID", HeaderText = "ID" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employee_code", HeaderText = "Employee Code" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "first_name", HeaderText = "First Name" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "middle_name", HeaderText = "Middle Name" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "last_name", HeaderText = "Last Name" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "birthdate", HeaderText = "Birthdate" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "sex", HeaderText = "Sex" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "address", HeaderText = "Address" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "department", HeaderText = "Department" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "shift", HeaderText = "Shift" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "date_added", HeaderText = "Date Added" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "employment_type", HeaderText = "Employment Type" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "role", HeaderText = "Role" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "position", HeaderText = "Position" });
                dgvReg.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "contact_number", HeaderText = "Contact Number" });

                DataGridViewImageColumn imgCol = new DataGridViewImageColumn
                {
                    DataPropertyName = "photo",
                    HeaderText = "Photo",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 80
                };
                dgvReg.Columns.Add(imgCol);
            }

            dgvReg.RowTemplate.Height = 60;
            dgvReg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReg.MultiSelect = false;
        }

        private void dgvDept_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDept.SelectedRows.Count > 0)
            {
                var row = dgvDept.SelectedRows[0];
                txtdeptcode.Text = row.Cells["code"].Value.ToString();
                txtdeptname.Text = row.Cells["description"].Value.ToString();
            }
        }

        private void btnClearDept_Click(object sender, EventArgs e)
        {
            txtdeptcode.Clear();
            txtdeptname.Clear();
            txtdeptcode.Focus();
        }

        private void btnAddDept_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtdeptcode.Text) || string.IsNullOrWhiteSpace(txtdeptname.Text))
            {
                MessageBox.Show("Please enter both Code and Description.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int count = MySQL.GetCount($"SELECT * FROM departments WHERE code='{txtdeptcode.Text}'");
            if (count > 0)
            {
                MessageBox.Show("Department code already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = $@"INSERT INTO departments (code, description)
                              VALUES ('{txtdeptcode.Text}', '{txtdeptname.Text}')";

            MySQL.Push(query);

            dgvDept.DataSource = MySQL.Pull("SELECT * FROM departments;");

            txtdeptcode.Clear();
            txtdeptname.Clear();
        }

        private void btnDeleteDept_Click(object sender, EventArgs e)
        {
            if (dgvDept.SelectedRows.Count > 0)
            {
                string id = dgvDept.SelectedRows[0].Cells["ID"].Value.ToString();
                string deptName = dgvDept.SelectedRows[0].Cells["description"].Value.ToString();

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete the department \"{deptName}\"?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = $"DELETE FROM departments WHERE ID='{id}'";
                    MySQL.Push(query);

                    dgvDept.DataSource = MySQL.Pull("SELECT * FROM departments;");

                    MessageBox.Show("Department deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a department to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadDepartments()
        {
            var dt = MySQL.Pull("SELECT ID, code, description FROM departments");
            cb_department.DataSource = dt;
            cb_department.DisplayMember = "description";
            cb_department.ValueMember = "ID";
            cb_department.SelectedIndex = -1;
        }

        private void LoadEmploymentTypes()
        {
            var dt = MySQL.Pull("SELECT ID, Type FROM lasam_attendance.employmenttype");
            cb_employmenttype.DataSource = dt;
            cb_employmenttype.DisplayMember = "Type";
            cb_employmenttype.ValueMember = "ID";
            cb_employmenttype.SelectedIndex = -1;
        }

        private void LoadRoles()
        {
            var dt = MySQL.Pull("SELECT DISTINCT role FROM lasam_attendance.users");
            cb_role.DataSource = dt;
            cb_role.DisplayMember = "role";
            cb_role.ValueMember = "role";
            cb_role.SelectedIndex = -1;
        }

        private void LoadShifts()
        {
            var dt = MySQL.Pull("SELECT ID, shift_name FROM shift;");
            cb_shift.DataSource = dt;
            cb_shift.DisplayMember = "shift_name";
            cb_shift.ValueMember = "ID";
            cb_shift.SelectedIndex = -1;
        }

        private void btnAddEmp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_Fname.Text) ||
                string.IsNullOrWhiteSpace(tb_Lname.Text) ||
                cb_department.SelectedIndex == -1 ||
                cb_shift.SelectedIndex == -1 ||
                cb_employmenttype.SelectedIndex == -1 ||
                cb_role.SelectedIndex == -1 ||
                (!rbMale.Checked && !rbFemale.Checked))
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string firstName = tb_Fname.Text.Trim();
                string middleName = tb_Mname.Text.Trim();
                string lastName = tb_Lname.Text.Trim();
                string birthdate = dtp_birthdate.Value.ToString("yyyy-MM-dd");
                string sex = rbMale.Checked ? "Male" : "Female";
                string address = tb_Address.Text.Trim();
                string departmentId = cb_department.SelectedValue?.ToString();
                string shiftId = cb_shift.SelectedValue?.ToString();
                string dateAdded = datetimepicker_datehired.Value.ToString("yyyy-MM-dd");
                string employmentTypeId = cb_employmenttype.SelectedValue?.ToString();
                string role = cb_role.SelectedValue?.ToString();
                string position = tb_Pos.Text.Trim();
                string contact = tb_Contact.Text.Trim();

                byte[] imgData = null;
                if (pb_image.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pb_image.Image.Save(ms, pb_image.Image.RawFormat);
                        imgData = ms.ToArray();
                    }
                }

                bool isNew = string.IsNullOrEmpty(selectedEmpId); 
                string employeeCode = "";

                using (MySqlConnection conn = new MySqlConnection(MySQL.ConnectionString))
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    if (isNew)
                    {

                        string deptCode = "";
                        if (cb_department.SelectedItem != null)
                        {
                            DataRowView drv = cb_department.SelectedItem as DataRowView;
                            if (drv != null)
                                deptCode = drv["code"].ToString();
                        }

                        if (string.IsNullOrEmpty(deptCode))
                        {
                            MessageBox.Show("Please select a department first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Get highest existing employee_code for this department
                        var dtMax = MySQL.Pull($@"
                        SELECT employee_code 
                        FROM employees 
                        WHERE employee_code LIKE '{deptCode}-%' 
                        ORDER BY employee_code DESC 
                        LIMIT 1;
                        ");

                        int nextNumber = 1;

                        if (dtMax.Rows.Count > 0)
                        {
                            string lastCode = dtMax.Rows[0]["employee_code"].ToString();  
                            string[] parts = lastCode.Split('-');

                            if (parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                                nextNumber = lastNumber + 1;
                        }

                        employeeCode = $"{deptCode}-{nextNumber:D3}";


                        cmd.CommandText = @"
                    INSERT INTO employees
                    (employee_code, first_name, middle_name, last_name, birthdate, sex, address, department_id, shift_id, date_added, employmenttype_id, role, position, contact_number, photo)
                    VALUES
                    (@employeeCode, @firstName, @middleName, @lastName, @birthdate, @sex, @address, @departmentId, @shiftId, @dateAdded, @employmentTypeId, @role, @position, @contact, @photo);
                    SELECT LAST_INSERT_ID();";

                        cmd.Parameters.AddWithValue("@employeeCode", employeeCode);
                    }
                    else
                    {
                        cmd.CommandText = @"
                    UPDATE employees
                    SET first_name = @firstName,
                        middle_name = @middleName,
                        last_name = @lastName,
                        birthdate = @birthdate,
                        sex = @sex,
                        address = @address,
                        department_id = @departmentId,
                        shift_id = @shiftId,
                        date_added = @dateAdded,
                        employmenttype_id = @employmentTypeId,
                        role = @role,
                        position = @position,
                        contact_number = @contact,
                        photo = @photo
                    WHERE ID = @id;";

                        cmd.Parameters.AddWithValue("@id", selectedEmpId);
                    }

                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@middleName", middleName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@birthdate", birthdate);
                    cmd.Parameters.AddWithValue("@sex", sex);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@departmentId", departmentId);
                    cmd.Parameters.AddWithValue("@shiftId", shiftId);
                    cmd.Parameters.AddWithValue("@dateAdded", dateAdded);
                    cmd.Parameters.AddWithValue("@employmentTypeId", employmentTypeId);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@position", position);
                    cmd.Parameters.AddWithValue("@contact", contact);
                    cmd.Parameters.Add("@photo", MySqlDbType.LongBlob).Value = (object)imgData ?? DBNull.Value;

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (isNew && result != null)
                    {
                        int newEmpId = Convert.ToInt32(result);
                        selectedEmpId = newEmpId.ToString();
                    }
                    conn.Close();
                }

                LoadEmployees();
                ClearEmployeeForm();

                if (isNew)
                {
                    MessageBox.Show($"Employee successfully added!\nGenerated Code: {employeeCode}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Employee successfully updated!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnDeleteEmp_Click(object sender, EventArgs e)
        {
            if (dgvReg.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int employeeId = Convert.ToInt32(dgvReg.SelectedRows[0].Cells[0].Value);

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this employee?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = $"DELETE FROM employees WHERE ID = {employeeId};";
                    MySQL.Push(query);

                    LoadEmployees(); 

                    MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ClearEmployeeForm()
        {
            tb_Fname.Clear();
            tb_Mname.Clear();
            tb_Lname.Clear();
            tb_Address.Clear();
            tb_Pos.Clear();
            tb_Contact.Clear();
            cb_department.SelectedIndex = -1;
            cb_employmenttype.SelectedIndex = -1;
            cb_role.SelectedIndex = -1;
            cb_shift.SelectedIndex = -1;
            rbMale.Checked = false;
            rbFemale.Checked = false;
            datetimepicker_datehired.Value = DateTime.Now;
            dtp_birthdate.Value = DateTime.Now;
        }

        private void btnClearEmp_Click(object sender, EventArgs e)
        {
            tb_Fname.Clear();
            tb_Mname.Clear();
            tb_Lname.Clear();
            tb_Address.Clear();
            tb_Pos.Clear();
            tb_Contact.Clear();
            cb_department.SelectedIndex = -1;
            cb_employmenttype.SelectedIndex = -1;
            cb_role.SelectedIndex = -1;
            cb_shift.SelectedIndex = -1;
            rbMale.Checked = false;
            rbFemale.Checked = false;
            datetimepicker_datehired.Value = DateTime.Now;
            dtp_birthdate.Value = DateTime.Now;
        }
        private void SetupEmployeeGrid()
        {
            dgvReg.AutoGenerateColumns = true;
            dgvReg.AllowUserToAddRows = false;
            dgvReg.ReadOnly = true;
            dgvReg.RowTemplate.Height = 60;
            dgvReg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReg.MultiSelect = false;
            dgvReg.BorderStyle = BorderStyle.None;
            dgvReg.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;


            dgvReg.EnableHeadersVisualStyles = false;
            dgvReg.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60);
            dgvReg.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReg.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvReg.ColumnHeadersHeight = 40;
            dgvReg.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReg.DefaultCellStyle.BackColor = Color.White;
            dgvReg.DefaultCellStyle.ForeColor = Color.Black;
            dgvReg.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 34, 34);
            dgvReg.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvReg.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 228, 225);
            dgvReg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SetupDepartmentGrid()
        {
            dgvDept.AutoGenerateColumns = true;
            dgvDept.AllowUserToAddRows = false;
            dgvDept.ReadOnly = true;
            dgvDept.RowTemplate.Height = 40;
            dgvDept.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDept.MultiSelect = false;
            dgvDept.BorderStyle = BorderStyle.None;
            dgvDept.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvDept.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvDept.EnableHeadersVisualStyles = false;
            dgvDept.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 20, 60); 
            dgvDept.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDept.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDept.ColumnHeadersHeight = 35;

            dgvDept.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvDept.DefaultCellStyle.BackColor = Color.White;
            dgvDept.DefaultCellStyle.ForeColor = Color.Black;
            dgvDept.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 34, 34); 
            dgvDept.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvDept.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 228, 225);
            dgvDept.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pb_image.Image = Image.FromFile(ofd.FileName);
            }
        }

        private void btnUpdateemp_Click(object sender, EventArgs e)
        {
            if (dgvReg.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an employee to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dgvReg.SelectedRows[0];
            selectedEmpId = row.Cells[0].Value.ToString();

            tb_Fname.Text = row.Cells[2].Value?.ToString();
            tb_Mname.Text = row.Cells[3].Value?.ToString();
            tb_Lname.Text = row.Cells[4].Value?.ToString();

            if (DateTime.TryParse(row.Cells[5].Value?.ToString(), out DateTime birth))
                dtp_birthdate.Value = birth;

            string sex = row.Cells[6].Value?.ToString();
            rbMale.Checked = sex == "Male";
            rbFemale.Checked = sex == "Female";

            tb_Address.Text = row.Cells[7].Value?.ToString();
            cb_department.Text = row.Cells[8].Value?.ToString();
            cb_shift.Text = row.Cells[9].Value?.ToString();

            if (DateTime.TryParse(row.Cells[10].Value?.ToString(), out DateTime hired))
                datetimepicker_datehired.Value = hired;

            cb_employmenttype.Text = row.Cells[11].Value?.ToString();
            cb_role.Text = row.Cells[12].Value?.ToString();
            tb_Pos.Text = row.Cells[13].Value?.ToString();
            tb_Contact.Text = row.Cells[14].Value?.ToString();

            if (row.Cells[15].Value != DBNull.Value)
            {
                byte[] imgBytes = (byte[])row.Cells[15].Value;
                using (MemoryStream ms = new MemoryStream(imgBytes))
                {
                    pb_image.Image = Image.FromStream(ms);
                }
            }
            else
            {
                pb_image.Image = null;
            }

            tbcReg.SelectedTab = tpEmp;
        }

        private void btnUpdateDept_Click(object sender, EventArgs e)
        {
            if (dgvDept.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a department to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = dgvDept.SelectedRows[0].Cells["ID"].Value.ToString();
            string newCode = txtdeptcode.Text.Trim();
            string newDesc = txtdeptname.Text.Trim();

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to update this department to:\n\nCode: {newCode}\nDescription: {newDesc}?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = $@"UPDATE departments 
                              SET code = '{newCode}', description = '{newDesc}'
                              WHERE ID = '{id}'";

                    MySQL.Push(query);

                    dgvDept.DataSource = MySQL.Pull("SELECT * FROM departments;");

                    MessageBox.Show("Department updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating department: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvDept_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvDept_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDept.SelectedRows.Count > 0)
            {
                txtdeptcode.Text = dgvDept.SelectedRows[0].Cells["code"].Value.ToString();
                txtdeptname.Text = dgvDept.SelectedRows[0].Cells["description"].Value.ToString();
            }
        }

            private void guna2TextBox1_TextChanged(object sender, EventArgs e)
            {
                string searchText = guna2TextBox1.Text.Trim();

                string query = @"
        SELECT e.ID,
               e.employee_code,
               e.first_name,
               e.middle_name,
               e.last_name,
               e.birthdate,
               e.sex,
               e.address,
               d.description AS department,
               s.shift_name AS shift,
               e.date_added,
               et.Type AS employment_type,
               e.role,
               e.position,
               e.contact_number,
               e.photo
        FROM employees e
        LEFT JOIN departments d ON e.department_id = d.ID
        LEFT JOIN shift s ON e.shift_id = s.ID
        LEFT JOIN employmenttype et ON e.employmenttype_id = et.ID
        WHERE e.first_name LIKE '%" + searchText + @"%'
           OR e.middle_name LIKE '%" + searchText + @"%'
           OR e.last_name LIKE '%" + searchText + @"%'
        ORDER BY e.ID DESC;
    ";

                DataTable dt = MySQL.Pull(query);
                dgvReg.DataSource = dt;

                if (dgvReg.Columns.Contains("photo") && dgvReg.Columns["photo"] is DataGridViewImageColumn imgCol)
                {
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                }
            }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            string searchText = guna2TextBox2.Text.Trim();

            string query = @"
        SELECT ID, code, description
        FROM departments
        WHERE code LIKE '%" + searchText + @"%'
           OR description LIKE '%" + searchText + @"%'
        ORDER BY ID DESC;
    ";

            DataTable dt = MySQL.Pull(query);
            dgvDept.DataSource = dt;
        }

        private void dgvReg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pbSearchEmp_Click(object sender, EventArgs e)
        {

        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (string.IsNullOrEmpty(selectedEmpId))
                {
                    MessageBox.Show("⚠️ Employee Details Required!\n\n" +
                                  "Please save the employee information first before enrolling fingerprint.\n\n" +
                                  "Steps to follow:\n" +
                                  "1. Fill in all employee details\n" +
                                  "2. Click 'Save' button\n" +
                                  "3. Then click 'Enroll' for fingerprint",
                                  "Missing Employee Data", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                int empId = Convert.ToInt32(selectedEmpId);

               
                enroll enrollForm = new enroll(empId);
                enrollForm.OnTemplate += this.OnTemplate;
                enrollForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Enrollment Error!\n\n" +
                              "An unexpected error occurred during fingerprint enrollment.\n\n" +
                              "Error Details: " + ex.Message + "\n\n" +
                              "Please try again or contact system administrator if the problem persists.",
                              "Enrollment Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }
    }
}
