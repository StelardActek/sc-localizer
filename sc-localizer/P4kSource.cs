using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text;

namespace sc_localizer;

internal class P4kSource
{
    private readonly byte[] KEY = new byte[] { 0x5E, 0x7A, 0x20, 0x02, 0x30, 0x2E, 0xEB, 0x1A, 0x3B, 0xB6, 0x17, 0xC3, 0x0F, 0xDE, 0x1E, 0x47 };

    private FileInfo _sourceInfo;

    public P4kSource(FileInfo sourceInfo)
    {
        _sourceInfo = sourceInfo;
    }

    public Stream? GetDataStream(string filename)
    {
        var p4k = new ZipFile(_sourceInfo.FullName) { Key = KEY };

        foreach (ZipEntry entry in p4k)
        {
            var crypto = entry.IsCrypted ? "Crypt" : "Plain";

            if (filename != null && entry.Name != filename)
                continue;

            //Console.WriteLine($"{entry.CompressionMethod} | {crypto} | {entry.Name}");
            return p4k.GetInputStream(entry);
        }

        return null;
    }
}