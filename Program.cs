using System.Diagnostics;
using System.IO.Compression;

CyberTrack[] tracks =
{
    new("bgm_cyber.awb", 0, "1-1", "Database"),
    new("bgm_cyber.awb", 14, "1-2", "Flowing"),
    new("bgm_cyber.awb", 12, "1-3", "Digital Cave"),
    new("bgm_cyber.awb", 38, "1-4", "Genshi"),
    new("bgm_cyber.awb", 56, "1-5", "Dropaholic"),
    new("bgm_cyber.awb", 48, "1-6", "Go Back 2 Your Roots"),
    new("bgm_cyber.awb", 18, "1-7", "Time Flyer"),
    new("bgm_cyber.awb", 24, "2-1", "Slice & Sway"),
    new("bgm_cyber.awb", 10, "2-2", "Heavenly Sky"),
    new("bgm_cyber.awb", 8, "2-3", "Nostalgic Sweep"),
    new("bgm_cyber.awb", 16, "2-4", "Hype Street"),
    new("bgm_cyber.awb", 32, "2-5", "Déjà vu"),
    new("bgm_cyber.awb", 58, "2-6", "Transparent Highway"),
    new("bgm_cyber.awb", 36, "2-7", "Floating in the Blue"),
    new("bgm_cyber.awb", 20, "3-1", "Escape the Loop"),
    new("bgm_cyber.awb", 2, "3-2", "Go Slap"),
    new("bgm_cyber.awb", 6, "3-3", "Memory will tell"),
    new("bgm_cyber.awb", 34, "3-4", "Constructure"),
    new("bgm_cyber.awb", 40, "3-5", "BMB"),
    new("bgm_cyber.awb", 52, "3-6", "Enjoy this World"),
    new("bgm_cyber.awb", 22, "3-7", "All Reality"),
    new("bgm_cyber.awb", 54, "4-1", "Exceed Mach"),
    new("bgm_cyber.awb", 26, "4-2", "Ephemeral"),
    new("bgm_cyber.awb", 44, "4-3", "Rumble Rave"),
    new("bgm_cyber.awb", 42, "4-4", "Wishes in the Wind"),
    new("bgm_cyber.awb", 28, "4-5", "Arrow of Time"),
    new("bgm_cyber.awb", 46, "4-6", "Fog Funk"),
    new("bgm_cyber.awb", 30, "4-7", "Rewing to go ahead"),
    new("bgm_cyber.awb", 50, "4-8", "No Pain, No Gain"),
    new("bgm_cyber.awb", 4, "4-9", "Signs"),
    new("bgm_cyber_w1r06.awb", 0, "4-A", "Genshi Remix"),
    new("bgm_cyber_w1r06.awb", 2, "4-B", "Escape the Loop Remix"),
    new("bgm_cyber_w1r06.awb", 4, "4-C", "Arrow of Time Remix"),
    new("bgm_cyber_w1r06.awb", 6, "4-D", "Rumble Rave Remix"),
    new("bgm_cyber_w1r06.awb", 8, "4-E", "Dropaholic Remix"),
    new("bgm_cyber_w1r06.awb", 10, "4-F", "Hype Street Remix"),
    new("bgm_cyber_w1r06.awb", 12, "4-G", "Ephemeral Remix"),
    new("bgm_cyber_w1r06.awb", 14, "4-H", "Wishes in the Wind Remix"),
    new("bgm_cyber_w1r06.awb", 16, "4-I", "Time Flyer Remix"),
};

var tempDirectory = Path.Combine(Path.GetTempPath(), "vgmstream_" + DateTime.Now.ToBinary());
Directory.CreateDirectory(tempDirectory);

try
{
    using (var client = new HttpClient())
    {
        Console.Write("Downloading & extracting vgmstream... ");
        var bytes = await client.GetByteArrayAsync("https://github.com/vgmstream/vgmstream-releases/releases/download/nightly/vgmstream-win64.zip");
        var zipFile = Path.Combine(tempDirectory, "vgmstream.zip");
        await File.WriteAllBytesAsync(zipFile, bytes);
        ZipFile.ExtractToDirectory(zipFile, tempDirectory);
        Console.WriteLine("Done!");
    }

    var vgmstream = Path.Combine(tempDirectory, "vgmstream-cli.exe");

    var cyber_sound = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
        "Steam\\steamapps\\common\\SonicFrontiers\\image\\x64\\raw\\sound\\cyber_sound"
    );

    if (!Directory.Exists(cyber_sound))
    {
        Console.WriteLine("Could not detect a valid Sonic Frontiers Steam installation.");
        throw new Exception();
    }

    var outputDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "Cyberspace OST"
    );

    Directory.CreateDirectory(outputDirectory);

    Console.WriteLine("You'll find your tracks in: " + outputDirectory);

    foreach (var x in tracks)
    {
        var cmdArgs = $"-o \"{outputDirectory}\\{x.Stage} - {x.Name}.wav\" -s {x.SongIndex + 1}  \"{cyber_sound}\\{x.FileName}\"";
        Console.Write($"{x.Stage} - {x.Name}... ");
        var info = new ProcessStartInfo
        {
            FileName = vgmstream,
            Arguments = cmdArgs,
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };
        var process = Process.Start(info)!;
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            throw new Exception();
        }
        Console.WriteLine("Done!");
    }
} catch
{
    Console.WriteLine("Error!");
}
finally
{
    Directory.Delete(tempDirectory, true);
    Console.Write("Press any key to exit.");
    Console.ReadKey();
}

public record CyberTrack(string FileName, int SongIndex, string Stage, string Name);