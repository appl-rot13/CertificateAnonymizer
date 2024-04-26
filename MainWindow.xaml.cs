
namespace CertificateAnonymizer
{
    using System.IO;
    using System.Windows;

    using SkiaSharp;

    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);

            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            base.OnPreviewDrop(e);

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filePaths == null)
            {
                return;
            }

            foreach (var filePath in filePaths)
            {
                try
                {
                    Anonymize(filePath);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{exception}");
                }
            }
        }

        private static void Anonymize(string readFilePath)
        {
            if (string.IsNullOrWhiteSpace(readFilePath) || Path.GetExtension(readFilePath) != ".jpg")
            {
                return;
            }

            var writeFilePath = Path.Combine(
                Path.GetDirectoryName(readFilePath) ?? string.Empty,
                string.Format(
                    "{0}_Anonymized{1}",
                    Path.GetFileNameWithoutExtension(readFilePath),
                    Path.GetExtension(readFilePath)));

            using (var bitmap = SKBitmap.Decode(readFilePath))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.DrawRect(100, 720, 700, 80, new SKPaint { Color = SKColors.White });
                canvas.DrawBitmap(bitmap, 0, 0);

                using (var stream = new SKFileWStream(writeFilePath))
                {
                    bitmap.Encode(stream, SKEncodedImageFormat.Jpeg, 100);
                }
            }
        }
    }
}
