using System.Collections.Generic;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace Test_Checker_PC
{
    class ImageProcessing
    {
        public static Bitmap Format(Bitmap img)
        {
            Bitmap _img = img;
            //Применение чёрно-белого фильтра
            Grayscale grayfilter = new Grayscale(0.2125, 0.7154, 0.0721);
            _img = grayfilter.Apply(_img);

            //Удаление лишних пикселов            
            BradleyLocalThresholding filter = new BradleyLocalThresholding();
            filter.PixelBrightnessDifferenceLimit = 0.1F;
            filter.ApplyInPlace(_img);

            //Рассчёт угла наклона 
            DocumentSkewChecker skewChecker = new DocumentSkewChecker();
            skewChecker.MaxSkewToDetect = 90;
            double angle = skewChecker.GetSkewAngle(_img);

            //Выравнивание изображения 
            RotateBilinear rotationFilter = new RotateBilinear(-angle);
            rotationFilter.FillColor = Color.White;
            _img = rotationFilter.Apply(_img);

            //Удаление дефектов после выравнивания
            filter.PixelBrightnessDifferenceLimit = 0.05F;
            filter.ApplyInPlace(_img);
            return _img;
        }

        public static List<Blob> FindObjects(Bitmap img, double fullness, int MinHeight, int MinWidth)
        {
            Bitmap _img = img;
            double _fullness = fullness;
            // Нахождение маркеров на изображении.
            // Инвертирование цветов для корректной работы blobcounter.
            Invert invert = new Invert();
            invert.ApplyInPlace(_img);
            // Инициализация фильтра.
            BlobCounter bc = new BlobCounter();
            // Настройка фильтра.
            bc.FilterBlobs = true;
            bc.MinHeight = MinHeight;
            bc.MaxHeight = _img.Height / 4;
            bc.MinWidth = MinWidth;
            bc.MaxWidth = _img.Width / 4;
            bc.ObjectsOrder = ObjectsOrder.Size;
            bc.ProcessImage(_img);
            // Поиск объектов.
            Blob[] blobs = bc.GetObjectsInformation();
            var markers = new List<Blob>();
            // Отбор подходящих объектов.
            foreach (Blob blob in blobs)
            {
                if (blob.Fullness >= _fullness)
                {
                    markers.Add(blob);
                }
            }
            invert.ApplyInPlace(_img);
            return markers;
        }

        public static List<Blob> FindBlobs(Bitmap img, double fullness, int MinHeight, int MinWidth, int MaxHeight, int MaxWidth)
        {
            Bitmap _img = img;
            // Инициализация фильтра.
            BlobCounterBase bc = new BlobCounter();
            // Игнорировать объекты размером меньше, чем заданные.
            bc.FilterBlobs = true;
            bc.MinHeight = MinHeight;
            bc.MaxHeight = MaxHeight;
            bc.MinWidth = MinWidth;
            bc.MaxWidth = MaxWidth;

            bc.ObjectsOrder = ObjectsOrder.Area;
            bc.ProcessImage(_img);

            Blob[] blobsArr = bc.GetObjectsInformation();
            var blobs = new List<Blob>();
            // Поиск маркеров
            foreach (Blob blob in blobsArr)
            {
                if (blob.Fullness >= fullness)
                {
                    blobs.Add(blob);
                }
            }
            return blobs;
        }

        public static List<Blob> SortByFullness(List<Blob> markers)
        {
            List<Blob> tmplist = markers;
            int j;
            int step = tmplist.Count / 2;
            while (step > 0)
            {
                for (int i = 0; i < (tmplist.Count - step); i++)
                {
                    j = i;
                    while ((j >= 0) && (tmplist[j].Fullness < tmplist[j + step].Fullness))
                    {
                        Blob tmp = tmplist[j];
                        tmplist[j] = tmplist[j + step];
                        tmplist[j + step] = tmp;
                        j--;
                    }
                }
                step = step / 2;
            }
            return tmplist;
        }

        public static List<Blob> SortMarkers(List<Blob> markers)
        {
            List<Blob> tmplist = markers;
            int j;
            int step = tmplist.Count / 2;
            while (step > 0)
            {
                for (int i = 0; i < (tmplist.Count - step); i++)
                {
                    j = i;
                    while ((j >= 0) && (tmplist[j].Rectangle.Y > tmplist[j + step].Rectangle.Y))
                    {
                        Blob tmp = tmplist[j];
                        tmplist[j] = tmplist[j + step];
                        tmplist[j + step] = tmp;
                        j--;
                    }
                }
                step = step / 2;
            }
            return tmplist;
        }

        public static List<Blob> SortBlobsX(List<Blob> blobs, int Q, int N)
        {
            List<Blob> tmplist = blobs;            
            int j;
            int step = Q / 2;
            while (step > 0)
            {
                for (int i = 0; i < (N * Q - step); i++)
                {
                    j = i;
                    while ((j >= 0) && (tmplist[j].Rectangle.X > tmplist[j + step].Rectangle.X))
                    {
                        Blob tmp = tmplist[j];
                        tmplist[j] = tmplist[j + step];
                        tmplist[j + step] = tmp;
                        j--;
                    }
                }
                step = step / 2;
            }
            return tmplist;
        }

        public static List<Blob> SortBlobsY(List<Blob> blobs, int Q, int N)
        {
            int forstep = Q * (int)System.Math.Ceiling((double)(N / 20));
            List<Blob> tmplist = blobs;
            for (int n = 0; n < N * Q; n += forstep)
            {
                int step = forstep / 2;
                while (step > 0)
                {
                    for (int i = n; i < (n + forstep - step); i++)
                    {
                        int j = i;
                        while ((j >= n) && (tmplist[j].Rectangle.Y > tmplist[j + step].Rectangle.Y))
                        {
                            Blob tmp = tmplist[j];
                            tmplist[j] = tmplist[j + step];
                            tmplist[j + step] = tmp;
                            j--;
                        }
                    }
                    step = step / 2;
                }
            }
            return tmplist;
        }

        public static Bitmap Rotate180(Bitmap img)
        {
            Bitmap _newimg = new Bitmap(img.Width, img.Height);
            Bitmap _img = img;
            int angle = 180;            
            RotateBilinear rotate = new RotateBilinear(angle, true);
            _newimg = rotate.Apply(_img);
            return _newimg;
        }

        public static Bitmap ExtractTable(Bitmap img, List<Blob> markers)
        {
            // Сортировка полученных маркеров по Y
            markers = ImageProcessing.SortMarkers(markers);
            // Рассчёт размеров области ответов
            int Xmin = markers[0].Rectangle.Right;
            int Xmax = 0;
            int Ymin = markers[0].Rectangle.Bottom;
            int Ymax = 0;

            for (int i = 1; i < 5; i++)
            {
                if (markers[i].Rectangle.Right < Xmin)
                {
                    Xmin = markers[i].Rectangle.Right;
                }
                if (markers[i].Rectangle.Left > Xmax)
                {
                    Xmax = markers[i].Rectangle.Left;
                }
                if (markers[i].Rectangle.Bottom < Ymin)
                {
                    Ymin = markers[i].Rectangle.Bottom;
                }
                if (markers[i].Rectangle.Top > Ymax)
                {
                    Ymax = markers[i].Rectangle.Top;
                }
            }
            int tablewidth = System.Math.Abs(Xmax - Xmin);
            int tableheight = System.Math.Abs(Ymax - Ymin);
            // Извлечение области ответов из базового изображения
            Rectangle section = new Rectangle(0, 0, tablewidth, tableheight);
            Bitmap table = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(table))
            {
                g.DrawImage(img, section, Xmin, Ymin, tablewidth, tableheight, GraphicsUnit.Pixel);
            }
            table = ImageProcessing.Format(table);
            return table;
        }

        public static List<Blob> OrderCells(List<Blob> cells, int N, int Q)
        {
            // Ячейками являются только первые N * Q элементов, остальные - дефекты.
            var sortedCells = new List<Blob>();            
            for (int i =0; i< N * Q;i++)
            {
                sortedCells.Add(cells[i]);
            }

            // Сортировка ячеек по Х
            sortedCells = ImageProcessing.SortBlobsX(sortedCells, Q, N);

            // Сортировка ячеек каждого вопроса по У 
            sortedCells = ImageProcessing.SortBlobsY(sortedCells, Q, N);

            // Разделение вопросов по таблицам.
            int T = (int)System.Math.Ceiling((double)(N / 20));
            if (T > 1)
            {
                var Lists = new List<List<Blob>>();
                for (int i = 0; i < T; i++)
                {
                    Lists.Add(new List<Blob>());
                }
                for (int i = 0; i < N * Q ; i+= T * Q)
                {
                    for (int g = 0; g < T; g++)
                    {
                        for (int q = 0; q < Q; q++)
                        {
                            Lists[g].Add(sortedCells[i + g*Q +q]);
                        }
                    }
                }
                sortedCells.Clear();
                foreach (List<Blob> list in Lists)
                {
                    sortedCells.AddRange(list);
                }
            }
            return sortedCells;
        }

        public static int[] GetAnswers(Bitmap table, List<Blob> cells, int N, int Q, double minFullness, double maxFullness)
        {
            int[] answers = new int[N * Q];
            int index = 0;

            for (int i = 0; i < N * Q; i++)
            {
                Bitmap tmp = new Bitmap(cells[i].Rectangle.Width, cells[i].Rectangle.Height);
                Rectangle rect = new Rectangle(0, 0, cells[i].Rectangle.Width, cells[i].Rectangle.Height);

                using (Graphics g = Graphics.FromImage(tmp))
                {
                    g.DrawImage(table, rect, cells[i].Rectangle.X, cells[i].Rectangle.Y, cells[i].Rectangle.Width, cells[i].Rectangle.Height, GraphicsUnit.Pixel);
                }
                // Высока вероятность того, что распознаётся не только ячейка, но и дефекты (либо ячейка разбита меткой на несколько частей). 
                // Для этого берётся наибольший найденый объект. Если найденные объекты слишком малы, то ячейка - закрашена, а объекты являются дефектами.
                var cellblob = ImageProcessing.FindBlobs(tmp, 0, 5, 5, tmp.Height, tmp.Width);
                if (cellblob.Count == 0)
                {
                    answers[index] = 0;
                }else 
                    if ((int)(cellblob[0].Fullness * 100) <= maxFullness && (int)(cellblob[0].Fullness * 100) > minFullness)
                {
                    answers[index] = 1;
                }
                else
                {
                    answers[index] = 0;
                }
                index++;
            }
            return answers;
        }
       

    }
}
