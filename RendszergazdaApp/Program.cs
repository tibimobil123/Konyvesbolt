using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LibraryAdmin
{
    public class MainForm : Form
    {
        private DataGridView grid;
        private Panel topPanel;
        private Panel sideMenu;
        private Button menuButton;
        private Label titleLabel;
        private bool menuOpen = false;

        string connStr = "server=localhost;user=root;password=;database=library;";

        public MainForm()
        {
            this.Text = "RendszergazdaAPP";
            this.Width = 1000;
            this.Height = 600;

            InitializeTopPanel();
            InitializeSideMenu();
            InitializeGrid();

            this.Controls.Add(grid);
            this.Controls.Add(sideMenu);
            this.Controls.Add(topPanel);
        }

        private void InitializeTopPanel()
        {
            topPanel = new Panel();
            topPanel.Height = 60;
            topPanel.Dock = DockStyle.Top;
            topPanel.BackColor = Color.Teal;

            menuButton = new Button();
            menuButton.Text = "☰";
            menuButton.Width = 50;
            menuButton.Height = 40;
            menuButton.Left = 10;
            menuButton.Top = 10;
            menuButton.FlatStyle = FlatStyle.Flat;
            menuButton.FlatAppearance.BorderSize = 0;
            menuButton.ForeColor = Color.White;
            menuButton.Click += ToggleMenu;

            titleLabel = new Label();
            titleLabel.Text = "RendszergazdaAPP";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Left = (topPanel.Width - titleLabel.Width) / 2;
            titleLabel.Top = (topPanel.Height - titleLabel.Height) / 2;

            topPanel.Resize += (s, e) => { titleLabel.Left = (topPanel.Width - titleLabel.Width) / 2; };

            topPanel.Controls.Add(menuButton);
            topPanel.Controls.Add(titleLabel);
        }

        private void InitializeSideMenu()
        {
            sideMenu = new Panel();
            sideMenu.Width = 0;
            sideMenu.Dock = DockStyle.Left;
            sideMenu.BackColor = Color.Gray;

            Button usersBtn = new Button { Text = "Felhasználók", Dock = DockStyle.Top, Height = 50 };
            usersBtn.Click += LoadUsers;

            Button booksBtn = new Button { Text = "Könyvek", Dock = DockStyle.Top, Height = 50 };
            booksBtn.Click += LoadBooks;

            Button purchasesBtn = new Button { Text = "Vásárlások", Dock = DockStyle.Top, Height = 50 };
            purchasesBtn.Click += LoadPurchases;

            Button statsBtn = new Button { Text = "Statisztika", Dock = DockStyle.Top, Height = 50 };
            statsBtn.Click += LoadStats;

            sideMenu.Controls.Add(statsBtn);
            sideMenu.Controls.Add(purchasesBtn);
            sideMenu.Controls.Add(booksBtn);
            sideMenu.Controls.Add(usersBtn);
        }

        private void InitializeGrid()
        {
            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.CellClick += Grid_CellClick;
        }

        private void ToggleMenu(object sender, EventArgs e)
        {
            sideMenu.Width = menuOpen ? 0 : 200;
            menuOpen = !menuOpen;
        }

        private void AddDeleteButton()
        {
            // Ha már van, töröljük, hogy ne duplikálódjon
            foreach (DataGridViewColumn col in grid.Columns)
                if (col is DataGridViewButtonColumn) { grid.Columns.Remove(col); break; }

            DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn
            {
                HeaderText = "Törlés",
                Text = "Törlés",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(deleteButton);
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!(grid.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            var idCell = grid.Columns.Contains("id") ? grid.Rows[e.RowIndex].Cells["id"] : null;
            if (idCell == null || idCell.Value == null) return;

            var id = idCell.Value;
            var result = MessageBox.Show("Biztosan törölni szeretnéd?", "Törlés megerősítés", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            string tableName = "";
            if (grid.Columns.Contains("email")) tableName = "users";
            else if (grid.Columns.Contains("title") && grid.Columns.Contains("author")) tableName = "books";
            else if (grid.Columns.Contains("Könyvek") || grid.Columns.Contains("Összes Ár")) tableName = "purchases";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = $"DELETE FROM {tableName} WHERE id=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            if (tableName == "users") LoadUsers(null, null);
            else if (tableName == "books") LoadBooks(null, null);
            else if (tableName == "purchases") LoadPurchases(null, null);
        }

        private void LoadUsers(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT id, email, password FROM users";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                grid.DataSource = table;
                AddDeleteButton();
            }
        }

        private void LoadBooks(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT id, title, author, price FROM books";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                grid.DataSource = table;
                AddDeleteButton();
            }
        }

        private void LoadPurchases(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        user_name AS 'Felhasználó',
                        GROUP_CONCAT(books.title SEPARATOR ', ') AS 'Könyvek',
                        SUM(books.price) AS 'Összes Ár'
                    FROM purchases
                    JOIN books ON purchases.book_id = books.id
                    GROUP BY user_name";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                grid.DataSource = table;
                AddDeleteButton();
            }
        }

        private void LoadStats(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        (SELECT title FROM books 
                         JOIN purchases ON books.id=purchases.book_id 
                         GROUP BY books.id ORDER BY COUNT(*) DESC LIMIT 1) AS 'Legtöbbet eladott könyv',
                        (SELECT SUM(books.price) FROM books JOIN purchases ON books.id=purchases.book_id) AS 'Összes bevétel',
                        (SELECT COUNT(*) FROM users) AS 'Felhasználók száma',
                        (SELECT COUNT(*) FROM books) AS 'Könyvek száma'
                ";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                grid.DataSource = table;
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }
    }
}