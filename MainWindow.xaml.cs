using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CalculatorLogic;

namespace Vorobeva
{
    public partial class MainWindow : Window
    {
        private Calculator _calculator = new Calculator();
        private double _previousValue = 0;
        private string _pendingOperator = "";
        private string _currentInput = "0";
        private bool _newInput = true;
        private string _expressionText = "";

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            ExpressionText.Text = _expressionText;
            ResultText.Text = _currentInput;
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string digit = btn.Tag.ToString();

            if (_newInput || _currentInput == "0")
            {
                _currentInput = digit == "." ? "0." : digit;
                _newInput = false;
            }
            else
            {
                if (digit == "." && _currentInput.Contains(".")) return;
                _currentInput += digit;
            }
            UpdateDisplay();
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string op = btn.Tag.ToString();

            if (_pendingOperator != "" && !_newInput) ExecuteCalculation();
            else _previousValue = double.Parse(_currentInput, CultureInfo.InvariantCulture);

            _pendingOperator = op;
            _newInput = true;
            _expressionText = _previousValue.ToString() + " " + GetSymbol(op);
            UpdateDisplay();
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pendingOperator == "" || _newInput) return;

            string expressionBefore = _previousValue.ToString() + " " + GetSymbol(_pendingOperator) + " " + _currentInput + " =";
            ExecuteCalculation();
            _expressionText = expressionBefore;
            _pendingOperator = "";
            _newInput = true;
            UpdateDisplay();
        }

        private void ExecuteCalculation()
        {
            try
            {
                double current = double.Parse(_currentInput, CultureInfo.InvariantCulture);
                double result = _calculator.Evaluate(_previousValue, current, _pendingOperator);
                _previousValue = result;
                _currentInput = result.ToString("G15", CultureInfo.InvariantCulture);
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Деление на ноль невозможно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                ClearAll();
            }
        }

        private string GetSymbol(string op)
        {
            if (op == "+") return "+";
            if (op == "-") return "−";
            if (op == "*") return "×";
            if (op == "/") return "÷";
            if (op == "^") return "^";
            return op;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) { ClearAll(); }

        private void ClearAll()
        {
            _currentInput = "0";
            _previousValue = 0;
            _pendingOperator = "";
            _expressionText = "";
            _newInput = true;
            UpdateDisplay();
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_newInput) return;
            if (_currentInput.Length <= 1) _currentInput = "0";
            else _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
            UpdateDisplay();
        }
    }
}