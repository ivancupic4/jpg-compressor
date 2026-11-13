using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

string desktop = Environment.GetFolderPath(
    Environment.SpecialFolder.DesktopDirectory);

string inputFolder = Path.Combine(desktop, "Images");
string outputFolder = Path.Combine(inputFolder, "Compressed");

if (!Directory.Exists(inputFolder))
{
    Console.WriteLine($"Folder not found: {inputFolder}");
    Console.WriteLine("Create a folder named Images on your Desktop and put the JPGs in it.");
    return;
}

Directory.CreateDirectory(outputFolder);

string[] files = Directory.GetFiles(inputFolder)
    .Where(file =>
        file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
        file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
    .ToArray();

if (files.Length == 0)
{
    Console.WriteLine($"No JPG images found in: {inputFolder}");
    return;
}

long originalTotal = 0;
long compressedTotal = 0;

foreach (string file in files)
{
    try
    {
        using Image image = Image.Load(file);

        // const double pixelScale = 1.000000000; // sqrt(1.00) — keep 100% of pixels
        // const double pixelScale = 0.948683298; // sqrt(0.90) — keep 90% of pixels
        // const double pixelScale = 0.894427191; // sqrt(0.80) — keep 80% of pixels
        // const double pixelScale = 0.836660027; // sqrt(0.70) — keep 70% of pixels
        // const double pixelScale = 0.774596669; // sqrt(0.60) — keep 60% of pixels
        // const double pixelScale = 0.707106781; // sqrt(0.50) — keep 50% of pixels
        // const double pixelScale = 0.632455532; // sqrt(0.40) — keep 40% of pixels
        // const double pixelScale = 0.547722558; // sqrt(0.30) — keep 30% of pixels
        // const double pixelScale = 0.447213595; // sqrt(0.20) — keep 20% of pixels
        // const double pixelScale = 0.316227766; // sqrt(0.10) — keep 10% of pixels

        // Scale width and height by sqrt(0.10) to keep 10% of the pixels.
        const double pixelScale = 0.316227766; // sqrt(0.10)
        int newWidth = Math.Max(1, (int)Math.Round(image.Width * pixelScale));
        int newHeight = Math.Max(1, (int)Math.Round(image.Height * pixelScale));

        image.Mutate(x => x.Resize(newWidth, newHeight));

        string outputPath = Path.Combine(
            outputFolder,
            Path.GetFileNameWithoutExtension(file) + ".jpg");

        image.Save(outputPath, new JpegEncoder { Quality = 80 });

        long before = new FileInfo(file).Length;
        long after = new FileInfo(outputPath).Length;

        originalTotal += before;
        compressedTotal += after;

        Console.WriteLine(
            $"{Path.GetFileName(file)}: {FormatSize(before)} → {FormatSize(after)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not process {Path.GetFileName(file)}: {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine($"Finished. Compressed copies are in: {outputFolder}");
Console.WriteLine($"Total: {FormatSize(originalTotal)} → {FormatSize(compressedTotal)}");

static string FormatSize(long bytes) =>
    $"{bytes / 1024.0 / 1024.0:F2} MB";