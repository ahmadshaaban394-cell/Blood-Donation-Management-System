using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BloodDonationUI
{
    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient = new HttpClient();
        private readonly string apiBaseUrl = "http://localhost:5248/api";

        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public Form1()
        {
            InitializeComponent();

            Text = "Blood Donation Management System";
            Width = 1150;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            BuildUI();
        }

        private void BuildUI()
        {
            Label title = new Label
            {
                Text = "Blood Donation Management System",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Height = 70,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };

            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            tabs.TabPages.Add(CreateDonorsTab());
            tabs.TabPages.Add(CreateBloodStocksTab());
            tabs.TabPages.Add(CreateBloodRequestsTab());
            tabs.TabPages.Add(CreateUsersTab());

            Controls.Add(tabs);
            Controls.Add(title);
        }

        private TabPage CreateDonorsTab()
        {
            TabPage tab = new TabPage("Donors");

            TextBox txtId = CreateTextBox(150, 30);
            TextBox txtFullName = CreateTextBox(150, 70);
            TextBox txtBloodType = CreateTextBox(150, 110);
            TextBox txtPhone = CreateTextBox(150, 150);
            TextBox txtLocation = CreateTextBox(150, 190);

            txtId.ReadOnly = true;

            AddLabel(tab, "ID", 30, 30);
            AddLabel(tab, "Full Name", 30, 70);
            AddLabel(tab, "Blood Type", 30, 110);
            AddLabel(tab, "Phone", 30, 150);
            AddLabel(tab, "Location", 30, 190);

            tab.Controls.Add(txtId);
            tab.Controls.Add(txtFullName);
            tab.Controls.Add(txtBloodType);
            tab.Controls.Add(txtPhone);
            tab.Controls.Add(txtLocation);

            DataGridView grid = CreateGrid();
            tab.Controls.Add(grid);

            Button btnView = CreateButton("View", 30, 250);
            Button btnAdd = CreateButton("Add", 170, 250);
            Button btnUpdate = CreateButton("Update", 310, 250);
            Button btnDelete = CreateButton("Delete", 450, 250);
            Button btnClear = CreateButton("Clear", 590, 250);

            tab.Controls.Add(btnView);
            tab.Controls.Add(btnAdd);
            tab.Controls.Add(btnUpdate);
            tab.Controls.Add(btnDelete);
            tab.Controls.Add(btnClear);

            btnView.Click += async (s, e) =>
            {
                await LoadData<Donor>(grid, "Donors");
            };

            btnAdd.Click += async (s, e) =>
            {
                Donor donor = new Donor
                {
                    FullName = txtFullName.Text,
                    BloodType = txtBloodType.Text,
                    Phone = txtPhone.Text,
                    Location = txtLocation.Text
                };

                await AddData("Donors", donor);
                await LoadData<Donor>(grid, "Donors");
                ClearTextBoxes(txtId, txtFullName, txtBloodType, txtPhone, txtLocation);
            };

            btnUpdate.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a donor first.");
                    return;
                }

                Donor donor = new Donor
                {
                    Id = int.Parse(txtId.Text),
                    FullName = txtFullName.Text,
                    BloodType = txtBloodType.Text,
                    Phone = txtPhone.Text,
                    Location = txtLocation.Text
                };

                await UpdateData("Donors", donor.Id, donor);
                await LoadData<Donor>(grid, "Donors");
            };

            btnDelete.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a donor first.");
                    return;
                }

                await DeleteData("Donors", int.Parse(txtId.Text));
                await LoadData<Donor>(grid, "Donors");
                ClearTextBoxes(txtId, txtFullName, txtBloodType, txtPhone, txtLocation);
            };

            btnClear.Click += (s, e) =>
            {
                ClearTextBoxes(txtId, txtFullName, txtBloodType, txtPhone, txtLocation);
            };

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = grid.Rows[e.RowIndex];

                    txtId.Text = row.Cells["Id"].Value?.ToString();
                    txtFullName.Text = row.Cells["FullName"].Value?.ToString();
                    txtBloodType.Text = row.Cells["BloodType"].Value?.ToString();
                    txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                    txtLocation.Text = row.Cells["Location"].Value?.ToString();
                }
            };

            return tab;
        }

        private TabPage CreateBloodStocksTab()
        {
            TabPage tab = new TabPage("Blood Stocks");

            TextBox txtId = CreateTextBox(150, 30);
            TextBox txtBloodType = CreateTextBox(150, 70);
            TextBox txtQuantity = CreateTextBox(150, 110);

            txtId.ReadOnly = true;

            AddLabel(tab, "ID", 30, 30);
            AddLabel(tab, "Blood Type", 30, 70);
            AddLabel(tab, "Quantity", 30, 110);

            tab.Controls.Add(txtId);
            tab.Controls.Add(txtBloodType);
            tab.Controls.Add(txtQuantity);

            DataGridView grid = CreateGrid();
            tab.Controls.Add(grid);

            Button btnView = CreateButton("View", 30, 180);
            Button btnAdd = CreateButton("Add", 170, 180);
            Button btnUpdate = CreateButton("Update", 310, 180);
            Button btnDelete = CreateButton("Delete", 450, 180);
            Button btnClear = CreateButton("Clear", 590, 180);

            tab.Controls.Add(btnView);
            tab.Controls.Add(btnAdd);
            tab.Controls.Add(btnUpdate);
            tab.Controls.Add(btnDelete);
            tab.Controls.Add(btnClear);

            btnView.Click += async (s, e) =>
            {
                await LoadData<BloodStock>(grid, "BloodStocks");
            };

            btnAdd.Click += async (s, e) =>
            {
                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Quantity must be a number.");
                    return;
                }

                BloodStock stock = new BloodStock
                {
                    BloodType = txtBloodType.Text,
                    Quantity = quantity
                };

                await AddData("BloodStocks", stock);
                await LoadData<BloodStock>(grid, "BloodStocks");
                ClearTextBoxes(txtId, txtBloodType, txtQuantity);
            };

            btnUpdate.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a stock first.");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Quantity must be a number.");
                    return;
                }

                BloodStock stock = new BloodStock
                {
                    Id = int.Parse(txtId.Text),
                    BloodType = txtBloodType.Text,
                    Quantity = quantity
                };

                await UpdateData("BloodStocks", stock.Id, stock);
                await LoadData<BloodStock>(grid, "BloodStocks");
            };

            btnDelete.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a stock first.");
                    return;
                }

                await DeleteData("BloodStocks", int.Parse(txtId.Text));
                await LoadData<BloodStock>(grid, "BloodStocks");
                ClearTextBoxes(txtId, txtBloodType, txtQuantity);
            };

            btnClear.Click += (s, e) =>
            {
                ClearTextBoxes(txtId, txtBloodType, txtQuantity);
            };

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = grid.Rows[e.RowIndex];

                    txtId.Text = row.Cells["Id"].Value?.ToString();
                    txtBloodType.Text = row.Cells["BloodType"].Value?.ToString();
                    txtQuantity.Text = row.Cells["Quantity"].Value?.ToString();
                }
            };

            return tab;
        }

        private TabPage CreateBloodRequestsTab()
        {
            TabPage tab = new TabPage("Blood Requests");

            TextBox txtId = CreateTextBox(150, 30);
            TextBox txtPatientName = CreateTextBox(150, 70);
            TextBox txtBloodType = CreateTextBox(150, 110);
            TextBox txtQuantity = CreateTextBox(150, 150);
            TextBox txtHospitalName = CreateTextBox(150, 190);
            TextBox txtStatus = CreateTextBox(150, 230);

            txtId.ReadOnly = true;

            AddLabel(tab, "ID", 30, 30);
            AddLabel(tab, "Patient Name", 30, 70);
            AddLabel(tab, "Blood Type", 30, 110);
            AddLabel(tab, "Quantity", 30, 150);
            AddLabel(tab, "Hospital", 30, 190);
            AddLabel(tab, "Status", 30, 230);

            tab.Controls.Add(txtId);
            tab.Controls.Add(txtPatientName);
            tab.Controls.Add(txtBloodType);
            tab.Controls.Add(txtQuantity);
            tab.Controls.Add(txtHospitalName);
            tab.Controls.Add(txtStatus);

            DataGridView grid = CreateGrid();
            tab.Controls.Add(grid);

            Button btnView = CreateButton("View", 30, 300);
            Button btnAdd = CreateButton("Add", 170, 300);
            Button btnUpdate = CreateButton("Update", 310, 300);
            Button btnDelete = CreateButton("Delete", 450, 300);
            Button btnClear = CreateButton("Clear", 590, 300);

            tab.Controls.Add(btnView);
            tab.Controls.Add(btnAdd);
            tab.Controls.Add(btnUpdate);
            tab.Controls.Add(btnDelete);
            tab.Controls.Add(btnClear);

            btnView.Click += async (s, e) =>
            {
                await LoadData<BloodRequest>(grid, "BloodRequests");
            };

            btnAdd.Click += async (s, e) =>
            {
                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Quantity must be a number.");
                    return;
                }

                BloodRequest request = new BloodRequest
                {
                    PatientName = txtPatientName.Text,
                    BloodType = txtBloodType.Text,
                    Quantity = quantity,
                    HospitalName = txtHospitalName.Text,
                    Status = txtStatus.Text
                };

                await AddData("BloodRequests", request);
                await LoadData<BloodRequest>(grid, "BloodRequests");
                ClearTextBoxes(txtId, txtPatientName, txtBloodType, txtQuantity, txtHospitalName, txtStatus);
            };

            btnUpdate.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a request first.");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    MessageBox.Show("Quantity must be a number.");
                    return;
                }

                BloodRequest request = new BloodRequest
                {
                    Id = int.Parse(txtId.Text),
                    PatientName = txtPatientName.Text,
                    BloodType = txtBloodType.Text,
                    Quantity = quantity,
                    HospitalName = txtHospitalName.Text,
                    Status = txtStatus.Text
                };

                await UpdateData("BloodRequests", request.Id, request);
                await LoadData<BloodRequest>(grid, "BloodRequests");
            };

            btnDelete.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a request first.");
                    return;
                }

                await DeleteData("BloodRequests", int.Parse(txtId.Text));
                await LoadData<BloodRequest>(grid, "BloodRequests");
                ClearTextBoxes(txtId, txtPatientName, txtBloodType, txtQuantity, txtHospitalName, txtStatus);
            };

            btnClear.Click += (s, e) =>
            {
                ClearTextBoxes(txtId, txtPatientName, txtBloodType, txtQuantity, txtHospitalName, txtStatus);
            };

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = grid.Rows[e.RowIndex];

                    txtId.Text = row.Cells["Id"].Value?.ToString();
                    txtPatientName.Text = row.Cells["PatientName"].Value?.ToString();
                    txtBloodType.Text = row.Cells["BloodType"].Value?.ToString();
                    txtQuantity.Text = row.Cells["Quantity"].Value?.ToString();
                    txtHospitalName.Text = row.Cells["HospitalName"].Value?.ToString();
                    txtStatus.Text = row.Cells["Status"].Value?.ToString();
                }
            };

            return tab;
        }

        private TabPage CreateUsersTab()
        {
            TabPage tab = new TabPage("Users");

            TextBox txtId = CreateTextBox(150, 30);
            TextBox txtFullName = CreateTextBox(150, 70);
            TextBox txtEmail = CreateTextBox(150, 110);
            TextBox txtPassword = CreateTextBox(150, 150);
            TextBox txtRole = CreateTextBox(150, 190);

            txtId.ReadOnly = true;

            AddLabel(tab, "ID", 30, 30);
            AddLabel(tab, "Full Name", 30, 70);
            AddLabel(tab, "Email", 30, 110);
            AddLabel(tab, "Password", 30, 150);
            AddLabel(tab, "Role", 30, 190);

            tab.Controls.Add(txtId);
            tab.Controls.Add(txtFullName);
            tab.Controls.Add(txtEmail);
            tab.Controls.Add(txtPassword);
            tab.Controls.Add(txtRole);

            DataGridView grid = CreateGrid();
            tab.Controls.Add(grid);

            Button btnView = CreateButton("View", 30, 250);
            Button btnAdd = CreateButton("Add", 170, 250);
            Button btnUpdate = CreateButton("Update", 310, 250);
            Button btnDelete = CreateButton("Delete", 450, 250);
            Button btnClear = CreateButton("Clear", 590, 250);

            tab.Controls.Add(btnView);
            tab.Controls.Add(btnAdd);
            tab.Controls.Add(btnUpdate);
            tab.Controls.Add(btnDelete);
            tab.Controls.Add(btnClear);

            btnView.Click += async (s, e) =>
            {
                await LoadData<AppUser>(grid, "AppUsers");
            };

            btnAdd.Click += async (s, e) =>
            {
                AppUser user = new AppUser
                {
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text,
                    Role = txtRole.Text
                };

                await AddData("AppUsers", user);
                await LoadData<AppUser>(grid, "AppUsers");
                ClearTextBoxes(txtId, txtFullName, txtEmail, txtPassword, txtRole);
            };

            btnUpdate.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a user first.");
                    return;
                }

                AppUser user = new AppUser
                {
                    Id = int.Parse(txtId.Text),
                    FullName = txtFullName.Text,
                    Email = txtEmail.Text,
                    Password = txtPassword.Text,
                    Role = txtRole.Text
                };

                await UpdateData("AppUsers", user.Id, user);
                await LoadData<AppUser>(grid, "AppUsers");
            };

            btnDelete.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Select a user first.");
                    return;
                }

                await DeleteData("AppUsers", int.Parse(txtId.Text));
                await LoadData<AppUser>(grid, "AppUsers");
                ClearTextBoxes(txtId, txtFullName, txtEmail, txtPassword, txtRole);
            };

            btnClear.Click += (s, e) =>
            {
                ClearTextBoxes(txtId, txtFullName, txtEmail, txtPassword, txtRole);
            };

            grid.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = grid.Rows[e.RowIndex];

                    txtId.Text = row.Cells["Id"].Value?.ToString();
                    txtFullName.Text = row.Cells["FullName"].Value?.ToString();
                    txtEmail.Text = row.Cells["Email"].Value?.ToString();
                    txtPassword.Text = row.Cells["Password"].Value?.ToString();
                    txtRole.Text = row.Cells["Role"].Value?.ToString();
                }
            };

            return tab;
        }

        private async Task LoadData<T>(DataGridView grid, string endpoint)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}/{endpoint}");

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Error loading data: " + response.StatusCode);
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                List<T>? data = JsonSerializer.Deserialize<List<T>>(json, jsonOptions);

                grid.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async Task AddData<T>(string endpoint, T item)
        {
            try
            {
                string json = JsonSerializer.Serialize(item, jsonOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync($"{apiBaseUrl}/{endpoint}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Added successfully.");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Add failed: " + response.StatusCode + "\n" + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async Task UpdateData<T>(string endpoint, int id, T item)
        {
            try
            {
                string json = JsonSerializer.Serialize(item, jsonOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync($"{apiBaseUrl}/{endpoint}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Updated successfully.");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Update failed: " + response.StatusCode + "\n" + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async Task DeleteData(string endpoint, int id)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{apiBaseUrl}/{endpoint}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Deleted successfully.");
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Delete failed: " + response.StatusCode + "\n" + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void AddLabel(TabPage tab, string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Left = x,
                Top = y + 5,
                Width = 110,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            tab.Controls.Add(label);
        }

        private TextBox CreateTextBox(int x, int y)
        {
            return new TextBox
            {
                Left = x,
                Top = y,
                Width = 220,
                Height = 30,
                Font = new Font("Segoe UI", 10)
            };
        }

        private Button CreateButton(string text, int x, int y)
        {
            Button button = new Button
            {
                Text = text,
                Left = x,
                Top = y,
                Width = 120,
                Height = 40,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Left = 400,
                Top = 30,
                Width = 700,
                Height = 540,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };
        }

        private void ClearTextBoxes(params TextBox[] textBoxes)
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Clear();
            }
        }
    }

    public class Donor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string BloodType { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Location { get; set; } = "";
    }

    public class BloodStock
    {
        public int Id { get; set; }
        public string BloodType { get; set; } = "";
        public int Quantity { get; set; }
    }

    public class BloodRequest
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = "";
        public string BloodType { get; set; } = "";
        public int Quantity { get; set; }
        public string HospitalName { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public class AppUser
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }
}