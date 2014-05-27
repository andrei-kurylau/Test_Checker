using System;
using System.Drawing;
using System.Windows.Forms;
using WIA;
using System.IO;

namespace Test_Checker_PC
{
    public partial class MainForm : Form
    {
        const int Q = 4;
        public string dir = Path.GetDirectoryName(Application.ExecutablePath);
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonAddDevice_Click(object sender, EventArgs e)
        {
            // Очистка ListBox'a
            Devices.Items.Clear();

            // Создаётся DeviceManager (необходимо для работы с устройствами)
            var deviceManager = new DeviceManager();

            // Перебор всех найденых устройств
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Отбор сканеров, добавление их в список
                
                if (deviceManager.DeviceInfos[i].Type == WiaDeviceType.ScannerDeviceType)
                {
                    Devices.Items.Add(new Scanner(deviceManager.DeviceInfos[i]));
                }
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            // Проверка "Выбран ли сканер из списка?"
            var device = Devices.SelectedItem as Scanner;
            if (device == null)
            {
                MessageBox.Show("Please select a device.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Сканирование
            var image = device.Scan();

            // Сохранение изображения (Необходимо для конвертации в Bitmap)
            var path = dir + "\\scan.jpeg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }                
            try
            {
                image.SaveFile(path);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Not enough memory to continue.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Check();
        }

        private void Check()
        {

            // Считывается последнее отсканированое изображение .
            if (!File.Exists(dir + "\\scan.jpeg"))
            {
                MessageBox.Show("You need to scan something before checking it!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Bitmap img = AForge.Imaging.Image.FromFile(dir + "\\scan.jpeg");


            // Далее вырезается часть изображения, где должен быть QR код. 
            // Если QR код не найден, изображение поворачивается на 180 градусов и QR код ищется снова.
            // Если после поворота код не найден, изображение считается не валидным.
            
            Rectangle section = new Rectangle(0, 0, img.Width / 4, img.Height / 4);
            Bitmap QRImage = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(QRImage))
            {
                g.DrawImage(img, section, 0, 3 * img.Height / 4, img.Width / 4, img.Height / 4, GraphicsUnit.Pixel);
            }
            int[] trueanswers = new int[1];

            try
            {
               trueanswers = QRProcessing.Decoder(QRImage);               
            }
            catch (NullReferenceException)
            {
                img = ImageProcessing.Rotate180(img);
                try
                {
                    using (Graphics g = Graphics.FromImage(QRImage))
                    {
                        g.DrawImage(img, section, 0, 3 * img.Height / 4, img.Width / 4, img.Height / 4, GraphicsUnit.Pixel);
                    }
                    trueanswers = QRProcessing.Decoder(QRImage);                    
                }
                catch (NullReferenceException)
                {                    
                    MessageBox.Show("Can't recognize QR code on this image.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            int N = trueanswers.Length / Q;
            // Запись в файл ответов на тест.
            //StreamWriter sw = File.CreateText("answers.txt");
            //for (int i = 0; i < 4; i++)
            //{
            //    for (int g = 0; g < N * 4; g = g + 4)
            //    {
            //        sw.Write(" {0} ", trueanswers[i + g]);
            //    }
            //    sw.WriteLine();
            //}
            //sw.Close();

            // Далее вырезается часть с таблицей.
            Rectangle section2 = new Rectangle(0, 0,  img.Width, 3 * img.Height / 4);
            Bitmap TableImage = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(TableImage))
            {
                g.DrawImage(img, section, 0, 0, img.Width , 3*img.Height / 4, GraphicsUnit.Pixel);
            }
            
            // Форматирование входного изображения
            TableImage = ImageProcessing.Format(TableImage);

            // Поиск маркеров
            var markers = ImageProcessing.FindObjects(TableImage, 0.6, 5, 5);

            // Сортировка по параметру Fullness
            markers = ImageProcessing.SortByFullness(markers);


            // Этот кусок кода избавляется от маленьких дефектов, которые были ошибочно приняты за маркеры.
            int listsize = markers.Count;
            if (listsize > 5)
            {
                for (int i = 5; i < listsize; i++)
                {
                    markers.RemoveAt(5);
                }
            } 

            if (listsize < 5)
            {
                MessageBox.Show("Couldn't recognize markers on this image.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }                                             

            // Извлечение таблицы ответов из исходного изображения
            Bitmap table = ImageProcessing.ExtractTable(TableImage, markers);
            // Поиск ячеек в таблице
            var tempCells = ImageProcessing.FindObjects(table, 0, 0, 0);
                        
            if (tempCells.Count < N  * Q)
            {
                MessageBox.Show("Couldn't recognize all cells.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);  
                return; ;
            }

            // Упорядочивание ячеек 
            var cells = ImageProcessing.OrderCells(tempCells, N, Q);

            // Проверка полученных ячеек
            int[] answers = ImageProcessing.GetAnswers(table, cells, N, Q, 30, 85);
                       
            string str;
            int mistakes = 0;
            for (int i = 0; i < N; i++)
            {
                for (int g = 0; g < 4; g++)
                {
                    if (answers[i * Q + g] != trueanswers[i * Q + g])
                    {                        
                        mistakes++;   
                        break;
                    }                    
                }
            }
            double result = 100 * (N - mistakes) / N;
            str = "Total correct: " + result +"%";            
            TotalLabel.Text = str;

            // Далее реализован графический интерфейс. Пользователь видит таблицу с ответами, где правильные ответы 
            // подсвечены зелёным цветом, а неправильные - красным.
            Bitmap MarkedTable = new Bitmap(table.Width, table.Height);
            using (Graphics g = Graphics.FromImage(MarkedTable))
            {
                g.DrawImage(table, 0, 0);
                Pen GreenPen = new Pen( Color.FromArgb(0,204,0), 2 );
                Pen RedPen = new Pen( Color.Red, 2 );
                Pen BluePen = new Pen(Color.Blue, 2);
                
                for (int i = 0; i < N*Q; i++)
                {
                    System.Drawing.Point[] points = {
                                                        new System.Drawing.Point(cells[i].Rectangle.Left,  cells[i].Rectangle.Top),
                                                        new System.Drawing.Point(cells[i].Rectangle.Right,  cells[i].Rectangle.Top),
                                                        new System.Drawing.Point(cells[i].Rectangle.Right,  cells[i].Rectangle.Bottom),
                                                        new System.Drawing.Point(cells[i].Rectangle.Left,  cells[i].Rectangle.Bottom)

                                                    };
                    if (answers[i] == 1 && trueanswers[i] == 1 )
                    {
                        g.DrawPolygon(GreenPen, points);
                        continue;
                    }
                    if (answers[i] == 1 && trueanswers[i] == 0)
                    {
                        g.DrawPolygon(RedPen, points);
                    }
                    if (answers[i] == 0 && trueanswers[i] == 1)
                    {
                        g.DrawLine(BluePen, new System.Drawing.Point(cells[i].Rectangle.Left + 1, cells[i].Rectangle.Bottom - 1),
                                                      new System.Drawing.Point(cells[i].Rectangle.Right - 1, cells[i].Rectangle.Top + 1));
                        g.DrawLine(BluePen, new System.Drawing.Point(cells[i].Rectangle.Left + 1, cells[i].Rectangle.Top + 1),
                                                      new System.Drawing.Point(cells[i].Rectangle.Right - 1, cells[i].Rectangle.Bottom - 1));
                    }                 
                }                
                
            }
            pictureBoxScannedImg.Image = MarkedTable;
          
        }
       

        private void buttonFromFile_Click(object sender, EventArgs e)
        {
            // Пользователь выбирает путь к файлу
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.bmp;*.jpg;*.jpeg;*.png";
            openFileDialog.Multiselect = false;
            openFileDialog.FilterIndex = 1;
            // Если нажата кнопка Open (Открыть) , то считывается файл.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                try
                {
                    try
                    {
                        File.Copy(path, dir + "\\scan.jpeg", true);
                    }
                    catch (OutOfMemoryException)
                    {
                        MessageBox.Show("Not enough memory to continue.", "Error",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    Check();
                }
                catch (IOException)
                {
                    Check();
                }
                
            }
        } 

                                       
    }
}
