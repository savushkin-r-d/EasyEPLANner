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
        private IdleForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Инициализация формы
        /// </summary>
        public void RunCountdown()
        {
            Application.Run(this);
            timerLabel.Text = $"Осталось: {startingCountdown} с.";
            RunTimer();
        }

        /// <summary>
        /// Запуск отсчета времени
        /// </summary>
        private void RunTimer()
        {
            countdownTimer = new Timer();
            countdownTimer.Interval = countdownInterval;
            countdownTimer.Tick += new EventHandler(ChangeTimeInLabel);
            countdownTimer.Start();
        }

        /// <summary>
        /// Остановить таймер.
        /// </summary>
        private void StopCountdown()
        {
            countdownTimer.Stop();
            countdownTimer.Tick -= new EventHandler(ChangeTimeInLabel);
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
                StopCountdown();
                IdleModule.Stop();
                IdleModule.CloseApplication();
            }
        }

        /// <summary>
        /// Подтверждение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acceptButton_Click(object sender, EventArgs e)
        {
            StopCountdown();
            this.Hide();
            this.Close();
        }

        /// <summary>
        /// Возвращает форму.
        /// </summary>
        public static IdleForm Form
        {
            get
            {
                if (idleForm == null)
                {
                    idleForm = new IdleForm();
                }
                if (idleForm.IsDisposed == true)
                {
                    idleForm = new IdleForm();
                }

                return idleForm;
            }
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

        /// <summary>
        /// Форма с отсчетом
        /// </summary>
        private static IdleForm idleForm = null;
    }
}
