using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyEPlanner
{
    public partial class IdleForm : Form
    {
        public IdleForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Инициализация формы
        /// </summary>
        public async void Init()
        {
            timerLabel.Text = $"Осталось: {startingCountdown} с.";
            await Task.Run(RunCountdown);
        }

        /// <summary>
        /// Запуск отсчета времени
        /// </summary>
        private void RunCountdown()
        {
            countdownTimer = new Timer();
            countdownTimer.Interval = countdownInterval;
            countdownTimer.Tick += new EventHandler(ChangeTimeInLabel);
            countdownTimer.Start();
        }

        /// <summary>
        /// Изменение состояния таймера
        /// </summary>
        /// <param name="sencder"></param>
        /// <param name="e"></param>
        private void ChangeTimeInLabel(object sencder, EventArgs e)
        {
            startingCountdown--;
            if (startingCountdown > 0)
            {
                timerLabel.Text = $"Осталось: {startingCountdown} с.";
            }
            else
            {
                //TODO: Инициирование закрытия Eplan.
            }
        }

        /// <summary>
        /// Подтверждение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acceptButton_Click(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            countdownTimer.Tick -= new EventHandler(ChangeTimeInLabel);
            countdownTimer.Interval = int.MaxValue;
            this.Hide();
        }

        /// <summary>
        /// Таймер
        /// </summary>
        private Timer countdownTimer;

        /// <summary>
        /// Стартовое значение таймера
        /// </summary>
        private int startingCountdown = 60;

        /// <summary>
        /// Интервал опроса таймера
        /// </summary>
        private const int countdownInterval = 1000;
    }
}
