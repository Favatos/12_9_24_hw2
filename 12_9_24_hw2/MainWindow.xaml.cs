using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _12_9_24_hw2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static byte[] buffer = new byte[4096];
        private const int Threads = 4;

        private static readonly CountdownEvent remaining = new(Threads);
        public MainWindow()
        {
            InitializeComponent();

            ProgressBar[] progresses = { p1, p2, p3, p4 };
            string[] filesCopy =
            {
                @"folder1\1.txt",
                @"folder1\2.txt",
                @"folder1\3.txt",
                @"folder1\4.txt"
            };
            string[] filesWrite =
            {
                @"folder2\1.txt",
                @"folder2\2.txt",
                @"folder2\3.txt",
                @"folder2\4.txt"
            };

            for (int i = 0; i < Threads; i++)
            {
                ThreadPool.QueueUserWorkItem(param => Copy(filesCopy[i], filesWrite[i], progresses[i]));
            }
            remaining.Wait();
        }

        private static void Copy(string path1, string path2, ProgressBar p)
        {
            long copied = 0, length = new FileInfo(path1).Length;
            using (Stream writer = File.Create(path2)) {
                using (Stream reader = File.OpenRead(path1))
                {
                    while (copied < length)
                    {
                        int read = reader.Read(buffer, 0, buffer.Length);
			            writer.Write(buffer, 0, read);
                        copied += read;
                        p.Value = 100 * copied / length;
                    }
                }
            }
            remaining.Signal();
        }
    }
}