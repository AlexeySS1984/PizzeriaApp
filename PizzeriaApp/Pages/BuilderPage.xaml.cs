using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PizzeriaApp.Pages
{
    public partial class BuilderPage : Page
    {
        // Ключ = DishTypeId. Поэтому для каждого типа физически может храниться только одно блюдо.
        private readonly Dictionary<int, Dishes> _selectedDishes =
            new Dictionary<int, Dishes>();

        public BuilderPage()
        {
            InitializeComponent();

            DishTypeComboBox.ItemsSource = Core.Context.DishTypes
                .OrderBy(t => t.DishTypeId)
                .ToList();

            SortComboBox.SelectedIndex = 0;

            if (DishTypeComboBox.Items.Count > 0)
                DishTypeComboBox.SelectedIndex = 0;

            RefreshSelected();
        }

        private void ReloadDishes()
        {
            DishTypes type = DishTypeComboBox.SelectedItem as DishTypes;
            if (type == null)
                return;

            string search = SearchTextBox.Text.Trim();

            IQueryable<Dishes> query = Core.Context.Dishes
                .Where(d => d.IsAvailable && d.DishTypeId == type.DishTypeId);

            // Одно поле ищет сразу и по названию, и по составу.
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    d.Composition.Contains(search));
            }

            if (SortComboBox.SelectedIndex == 1)
                query = query.OrderByDescending(d => d.Price);
            else
                query = query.OrderBy(d => d.Price);

            DishesDataGrid.ItemsSource = query.ToList();
        }

        private void DishTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DishesDataGrid != null)
                ReloadDishes();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DishesDataGrid != null)
                ReloadDishes();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DishesDataGrid != null)
                ReloadDishes();
        }

        private void AddDishButton_Click(object sender, RoutedEventArgs e)
        {
            Dishes dish = DishesDataGrid.SelectedItem as Dishes;

            if (dish == null)
            {
                MessageBox.Show("Выберите блюдо.");
                return;
            }

            // Если блюдо такого типа уже было, оно заменяется новым.
            _selectedDishes[dish.DishTypeId] = dish;
            RefreshSelected();
        }

        private void RemoveDishButton_Click(object sender, RoutedEventArgs e)
        {
            Dishes dish = SelectedDishesDataGrid.SelectedItem as Dishes;

            if (dish == null)
            {
                MessageBox.Show("Выберите блюдо в нижнем списке.");
                return;
            }

            _selectedDishes.Remove(dish.DishTypeId);
            RefreshSelected();
        }

        private void RefreshSelected()
        {
            List<Dishes> selected = _selectedDishes.Values
                .OrderBy(d => d.DishTypeId)
                .ToList();

            SelectedDishesDataGrid.ItemsSource = null;
            SelectedDishesDataGrid.ItemsSource = selected;

            decimal total = selected.Sum(d => d.Price);
            TotalTextBlock.Text = "Итого: " + total.ToString("N2") + " ₽";
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDishes.Count == 0)
            {
                MessageBox.Show("Нельзя создать пустой заказ. Выберите хотя бы одно блюдо.");
                return;
            }

            NavigationService.Navigate(
                new CheckoutPage(_selectedDishes.Values.ToList()));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CombosPage());
        }
    }
}
