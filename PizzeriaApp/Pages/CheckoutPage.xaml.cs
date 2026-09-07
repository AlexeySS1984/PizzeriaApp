using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PizzeriaApp.Pages
{
    public partial class CheckoutPage : Page
    {
        private readonly Combos _combo;
        private readonly List<Dishes> _customDishes;
        private readonly decimal _total;

        // Готовое комбо.
        public CheckoutPage(Combos combo)
        {
            InitializeComponent();

            _combo = combo;
            _customDishes = null;
            _total = combo.Price;

            OrderInfoTextBlock.Text = "Готовое комбо: " + combo.Name;
            InitializePayment();
        }

        // Собранный пользователем заказ.
        public CheckoutPage(List<Dishes> dishes)
        {
            InitializeComponent();

            _combo = null;
            _customDishes = dishes;
            _total = dishes.Sum(d => d.Price);

            OrderInfoTextBlock.Text =
                "Свой заказ: " + string.Join(", ", dishes.Select(d => d.Name));

            InitializePayment();
        }

        private void InitializePayment()
        {
            TotalTextBlock.Text = "К оплате: " + _total.ToString("N2") + " ₽";

            PaymentMethodComboBox.ItemsSource = Core.Context.PaymentMethods
                .OrderBy(p => p.PaymentMethodId)
                .ToList();

            if (PaymentMethodComboBox.Items.Count > 0)
                PaymentMethodComboBox.SelectedIndex = 0;
        }

        private bool IsCash()
        {
            PaymentMethods method =
                PaymentMethodComboBox.SelectedItem as PaymentMethods;

            return method != null && method.Name == "Наличные";
        }

        private void PaymentMethodComboBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            CashPanel.Visibility =
                IsCash() ? Visibility.Visible : Visibility.Collapsed;

            UpdateChangePreview();
        }

        private void CashReceivedTextBox_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            UpdateChangePreview();
        }

        private void UpdateChangePreview()
        {
            if (!IsCash())
            {
                ChangeTextBlock.Text = "";
                return;
            }

            decimal received;
            if (decimal.TryParse(CashReceivedTextBox.Text, out received) &&
                received >= _total)
            {
                ChangeTextBlock.Text =
                    "Сдача: " + (received - _total).ToString("N2") + " ₽";
            }
            else
            {
                ChangeTextBlock.Text = "Введите сумму не меньше стоимости заказа.";
            }
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Сначала авторизуйтесь.");
                NavigationService.Navigate(new LoginPage());
                return;
            }

            PaymentMethods payment =
                PaymentMethodComboBox.SelectedItem as PaymentMethods;

            if (payment == null)
            {
                MessageBox.Show("Выберите способ оплаты.");
                return;
            }

            decimal? cashReceived = null;
            decimal? changeAmount = null;

            if (payment.Name == "Наличные")
            {
                decimal received;

                if (!decimal.TryParse(CashReceivedTextBox.Text, out received))
                {
                    MessageBox.Show("Введите внесённую сумму числом.");
                    return;
                }

                if (received < _total)
                {
                    MessageBox.Show("Внесённая сумма меньше стоимости заказа.");
                    return;
                }

                cashReceived = received;
                changeAmount = received - _total;
            }

            if (_combo == null &&
                (_customDishes == null || _customDishes.Count == 0))
            {
                MessageBox.Show("Пустой заказ сохранить нельзя.");
                return;
            }

            Orders order = new Orders
            {
                UserId = Core.CurrentUser.UserId,
                ComboId = _combo == null ? (int?)null : _combo.ComboId,
                OrderDate = DateTime.Now,
                TotalAmount = _total,
                PaymentMethodId = payment.PaymentMethodId,
                CashReceived = cashReceived,
                ChangeAmount = changeAmount
            };

            // Для своего заказа сохраняем каждое выбранное блюдо в БД.
            // Для готового комбо достаточно ComboId: его состав уже хранится в ComboDishes.
            if (_combo == null)
            {
                foreach (Dishes dish in _customDishes)
                {
                    order.OrderDishes.Add(new OrderDishes
                    {
                        DishId = dish.DishId,
                        Quantity = 1,
                        PriceAtOrder = dish.Price
                    });
                }
            }

            Core.Context.Orders.Add(order);
            Core.Context.SaveChanges();

            string result = "Заказ №" + order.OrderId + " сохранён в БД.";

            if (changeAmount.HasValue)
                result += "\nСдача: " + changeAmount.Value.ToString("N2") + " ₽";

            MessageBox.Show(result);
            NavigationService.Navigate(new CombosPage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CombosPage());
        }
    }
}
