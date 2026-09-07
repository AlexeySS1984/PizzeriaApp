using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PizzeriaApp.Pages
{
    public partial class CombosPage : Page
    {
        public class ComboRow
        {
            public Combos Entity { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Composition { get; set; }
            public decimal Price { get; set; }
        }

        public CombosPage()
        {
            InitializeComponent();

            if (Core.CurrentUser == null)
            {
                NavigationService.Navigate(new LoginPage());
                return;
            }

            HelloTextBlock.Text = "Здравствуйте, " + Core.CurrentUser.FullName;
            SortComboBox.SelectedIndex = 0;
            LoadCombos();
        }

        private void LoadCombos()
        {
            List<Combos> combos = Core.Context.Combos
                .Include("ComboDishes.Dishes")
                .Where(c => c.IsAvailable)
                .ToList();

            if (SortComboBox.SelectedIndex == 1)
                combos = combos.OrderByDescending(c => c.Price).ToList();
            else
                combos = combos.OrderBy(c => c.Price).ToList();

            List<ComboRow> rows = combos.Select(c => new ComboRow
            {
                Entity = c,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                Composition = string.Join(", ",
                    c.ComboDishes
                     .OrderBy(cd => cd.Dishes.DishTypeId)
                     .Select(cd => cd.Dishes.Name +
                                   (cd.Quantity > 1 ? " x" + cd.Quantity : "")))
            }).ToList();

            CombosDataGrid.ItemsSource = rows;
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CombosDataGrid != null)
                LoadCombos();
        }

        private void OrderComboButton_Click(object sender, RoutedEventArgs e)
        {
            ComboRow row = CombosDataGrid.SelectedItem as ComboRow;

            if (row == null)
            {
                MessageBox.Show("Выберите готовое комбо.");
                return;
            }

            NavigationService.Navigate(new CheckoutPage(row.Entity));
        }

        private void BuildOrderButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BuilderPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }
    }
}
