using System.Drawing;
using System.Windows.Forms;

namespace ConsoleApp2;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}

internal static class Ui
{
    public static readonly Color Green = Color.FromArgb(43, 107, 78);
    public static readonly Color LightGreen = Color.FromArgb(235, 247, 239);

    public static void Prepare(Form form, string title)
    {
        form.Text = title;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.ClientSize = new Size(720, 480);
        form.BackColor = LightGreen;
        form.Font = new Font("Segoe UI", 10F);
    }

    public static Label Heading(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = new Font("Segoe UI", 24F, FontStyle.Bold),
        ForeColor = Green,
        Margin = new Padding(0, 0, 0, 20)
    };

    public static Button Button(string text) => new()
    {
        Text = text,
        AutoSize = true,
        MinimumSize = new Size(150, 44),
        BackColor = Green,
        ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat,
        Cursor = Cursors.Hand,
        Margin = new Padding(8)
    };

    public static Label FieldLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        ForeColor = Color.FromArgb(45, 45, 45),
        Margin = new Padding(3, 10, 3, 3)
    };
}

internal sealed class LoginForm : Form
{
    private readonly TextBox _username = new() { Width = 260 };
    private readonly TextBox _password = new() { Width = 260, UseSystemPasswordChar = true };
    private readonly Label _message = new() { AutoSize = true, ForeColor = Color.Firebrick };

    public LoginForm()
    {
        Ui.Prepare(this, "Rental Service - Login");

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 8,
            Padding = new Padding(180, 55, 180, 35)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(Ui.Heading("Mushroom & Bicycle Rental"), 0, 0);
        layout.Controls.Add(new Label { Text = "Please log in to continue", AutoSize = true }, 0, 1);
        layout.Controls.Add(Ui.FieldLabel("Username"), 0, 2);
        layout.Controls.Add(_username, 0, 3);
        layout.Controls.Add(Ui.FieldLabel("Password"), 0, 4);
        layout.Controls.Add(_password, 0, 5);

        var login = Ui.Button("Login");
        login.Click += LoginClicked;
        var demo = new Button
        {
            Text = "Use demo login",
            AutoSize = true,
            BackColor = Color.White,
            ForeColor = Ui.Green,
            FlatStyle = FlatStyle.Flat,
            Margin = new Padding(8)
        };
        demo.Click += (_, _) =>
        {
            _username.Text = "student";
            _password.Text = "rent123";
            _message.Text = "Demo login selected. Click Login.";
        };

        var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill };
        buttons.Controls.Add(login);
        buttons.Controls.Add(demo);
        layout.Controls.Add(buttons, 0, 6);
        layout.Controls.Add(_message, 0, 7);
        Controls.Add(layout);

        AcceptButton = login;
    }

    private void LoginClicked(object? sender, EventArgs e)
    {
        if (_username.Text.Trim() == "student" && _password.Text == "rent123")
        {
            Hide();
            using var choice = new ChoiceForm(this);
            choice.ShowDialog(this);
            Show();
        }
        else
        {
            _message.Text = "Incorrect username or password.";
        }
    }
}

internal sealed class ChoiceForm : Form
{
    private readonly Form _login;

    public ChoiceForm(Form login)
    {
        _login = login;
        Ui.Prepare(this, "Rental Service - Choose an item");

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(100, 70, 100, 70)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(Ui.Heading("Choose a rental"), 0, 0);
        layout.Controls.Add(new Label
        {
            Text = "Click one of the buttons below to continue:",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 20)
        }, 0, 1);

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            Anchor = AnchorStyles.None,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(10)
        };
        var bicycle = Ui.Button("Bicycle");
        bicycle.Click += (_, _) => OpenRental("Bicycle", 8.00m);
        var mushroom = Ui.Button("Mushroom");
        mushroom.Click += (_, _) => OpenRental("Mushroom", 5.00m);
        buttons.Controls.Add(bicycle);
        buttons.Controls.Add(mushroom);
        layout.Controls.Add(buttons, 0, 2);

        var logout = new Button { Text = "Logout", AutoSize = true, Anchor = AnchorStyles.None };
        logout.Click += (_, _) => Close();
        layout.Controls.Add(logout, 0, 3);
        Controls.Add(layout);
    }

    private void OpenRental(string itemName, decimal dailyPrice)
    {
        using var rental = new RentalForm(itemName, dailyPrice);
        rental.ShowDialog(this);
    }
}

internal sealed class RentalForm : Form
{
    private readonly string _itemName;
    private readonly decimal _dailyPrice;
    private readonly TextBox _customerName = new() { Width = 260 };
    private readonly NumericUpDown _quantity = new() { Minimum = 1, Maximum = 20, Value = 1, Width = 100 };
    private readonly NumericUpDown _days = new() { Minimum = 1, Maximum = 30, Value = 1, Width = 100 };
    private readonly Label _total = new() { AutoSize = true, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Ui.Green };

    public RentalForm(string itemName, decimal dailyPrice)
    {
        _itemName = itemName;
        _dailyPrice = dailyPrice;
        Ui.Prepare(this, $"Rent a {itemName}");

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 7,
            Padding = new Padding(130, 45, 130, 45)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        layout.Controls.Add(Ui.Heading($"{itemName} rental"), 0, 0);
        layout.SetColumnSpan(layout.GetControlFromPosition(0, 0), 2);
        layout.Controls.Add(Ui.FieldLabel("Customer name"), 0, 1);
        layout.Controls.Add(_customerName, 1, 1);
        layout.Controls.Add(Ui.FieldLabel("Quantity"), 0, 2);
        layout.Controls.Add(_quantity, 1, 2);
        layout.Controls.Add(Ui.FieldLabel("Number of days"), 0, 3);
        layout.Controls.Add(_days, 1, 3);
        layout.Controls.Add(new Label { Text = $"Price: {_dailyPrice:C} per item/day", AutoSize = true }, 0, 4);
        layout.SetColumnSpan(layout.GetControlFromPosition(0, 4), 2);
        layout.Controls.Add(_total, 0, 5);
        layout.SetColumnSpan(_total, 2);

        var rent = Ui.Button("Confirm rental");
        rent.Click += ConfirmRental;
        var back = new Button { Text = "Back", AutoSize = true };
        back.Click += (_, _) => Close();
        var buttons = new FlowLayoutPanel { AutoSize = true };
        buttons.Controls.Add(rent);
        buttons.Controls.Add(back);
        layout.Controls.Add(buttons, 0, 6);
        layout.SetColumnSpan(buttons, 2);
        Controls.Add(layout);
        UpdateTotal();
    }

    private void ConfirmRental(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_customerName.Text))
        {
            MessageBox.Show("Please enter the customer name.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _customerName.Focus();
            return;
        }

        MessageBox.Show($"Rental confirmed for {_customerName.Text.Trim()}!\n\nItem: {_itemName}\nQuantity: {_quantity.Value}\nDays: {_days.Value}\nTotal: {_total.Text.Replace("Total: ", "")}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateTotal()
    {
        _total.Text = $"Total: {_dailyPrice * _quantity.Value * _days.Value:C}";
    }
}