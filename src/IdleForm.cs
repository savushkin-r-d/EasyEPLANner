using System;
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
        /// Инициализация таймера.
        /// </summary>
        public void RunCountdown()
        {
            timerLabel.Text = $"Осталось: {startingCountdown} с.";
            RunTimer();
        }

        /// <summary>
        /// Запуск отсчета времени и запуск формы.
        /// </summary>
        private void RunTimer()
        {
            countdownTimer = new Timer();
            countdownTimer.Interval = countdownInterval;
            countdownTimer.Tick += new EventHandler(TimerWorking);
            countdownTimer.Start();

            Application.Run(this);
        }

        /// <summary>
        /// Остановить таймер.
        /// </summary>
        private void StopCountdown()
        {
            countdownTimer.Stop();
            countdownTimer.Tick -= new EventHandler(TimerWorking);
        }

        /// <summary>
        /// Работа таймера.
        /// </summary>
        /// <param name="sencder"></param>
        /// <param name="e"></param>
        private void TimerWorking(object sencder, EventArgs e)
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
        /// Подтверждение активности.
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
        /// Получить форму.
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
        /// Таймер.
        /// </summary>
        private Timer countdownTimer;

        /// <summary>
        /// Стартовое значение таймера.
        /// </summary>
        private int startingCountdown = 60;

        /// <summary>
        /// Интервал опроса таймера в миллисекундах.
        /// </summary>
        private const int countdownInterval = 1000;

        /// <summary>
        /// Форма с таймером.
        /// </summary>
        private static IdleForm idleForm = null;
    }
}
