using System;
using System.Drawing;
using ZXing;

namespace Test_Checker_PC
{
    class QRProcessing
    {
        public static int[] Decoder(Bitmap img)
        {
            // create a barcode reader instance
            IBarcodeReader reader = new BarcodeReader();
            // load a bitmap
            var barcodeBitmap = img; ;
            // detect and decode the barcode inside the bitmap
            var result = reader.Decode(barcodeBitmap);
            // do something with the result
            string Text = result.Text;

            string[] strings = Text.Split('/');
            int N = Int32.Parse(strings[0]);
            int[] answers = new int[N * 4];
            int index = 0;
            for (int i = 1; i <= N; i++)
            {
                string tmp = strings[i];
                for (int g = 0; g < 4; g++)
                {
                    answers[index] = Int32.Parse(Convert.ToString(tmp[g]));
                    index++;
                }
            }

            for (int i = 0; i < N; i++)
            {
                int tmp = 0;
                for (int g = 0; g < 4; g++)
                {
                    tmp += answers[i * 4 + g];
                }
                if (tmp > 10)
                {
                    answers[i * 4] = Math.Abs(answers[i * 4] - 3);
                    answers[i * 4 + 1] = Math.Abs(answers[i * 4 + 1] - 4);
                    answers[i * 4 + 2] = Math.Abs(answers[i * 4 + 2] - 3);
                    answers[i * 4 + 3] = Math.Abs(answers[i * 4 + 3] - 5);
                }
                else
                {
                    answers[i * 4] = Math.Abs(answers[i * 4] - 4);
                    answers[i * 4 + 1] = Math.Abs(answers[i * 4 + 1] - 1);
                    answers[i * 4 + 2] = Math.Abs(answers[i * 4 + 2] - 2);
                    answers[i * 4 + 3] = Math.Abs(answers[i * 4 + 3] - 3);
                }
            }

            return answers;
        }
    }
}
