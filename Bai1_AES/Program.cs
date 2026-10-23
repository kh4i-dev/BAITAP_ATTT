using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Bài 1: Mã hóa đối xứng AES
// mã hóa và giải mã dùng cùng một khóa
class Program
{
    // khóa 32 byte => AES-256, IV 16 byte
    static byte[] khoa = new byte[0];
    static byte[] iv = new byte[0];
    static string fileKhoa = "aes_key.txt";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===== BÀI 1: MÃ HÓA AES =====");
            Console.WriteLine("1. Sinh khóa AES");
            Console.WriteLine("2. Lưu khóa ra file");
            Console.WriteLine("3. Import khóa từ file");
            Console.WriteLine("4. Mã hóa / giải mã 1 đoạn văn bản");
            Console.WriteLine("5. Mã hóa 1 file, lưu ra file khác");
            Console.WriteLine("6. Test file lớn (đo thời gian + so sánh từng byte)");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn: ");

            string chon = Console.ReadLine();
            Console.WriteLine();

            if (chon == "1") { SinhKhoa(); }
            else if (chon == "2") { LuuKhoa(); }
            else if (chon == "3") { DocKhoa(); }
            else if (chon == "4") { VanBan(); }
            else if (chon == "5") { MaHoaFileMenu(); }
            else if (chon == "6") { TestFileLon(); }
            else if (chon == "0") { break; }
            else { Console.WriteLine("Chọn sai rồi, chọn lại đi."); }
        }
    }

    // 1. sinh khóa AES
    static void SinhKhoa()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;      // 256 bit
            aes.GenerateKey();      // sinh khóa ngẫu nhiên
            aes.GenerateIV();       // sinh IV ngẫu nhiên
            khoa = aes.Key;
            iv = aes.IV;
        }

        Console.WriteLine("Đã sinh khóa AES-256.");
        Console.WriteLine("Khóa (base64): " + Convert.ToBase64String(khoa));
        Console.WriteLine("IV   (base64): " + Convert.ToBase64String(iv));
    }

    // 2. lưu khóa ra file
    static void LuuKhoa()
    {
        if (khoa.Length == 0)
        {
            Console.WriteLine("Chưa có khóa. Chọn số 1 để sinh khóa trước đã.");
            return;
        }

        // ghi 2 dòng: dòng 1 là khóa, dòng 2 là IV, đều ở dạng base64
        File.WriteAllText(fileKhoa, Convert.ToBase64String(khoa) + "\n" + Convert.ToBase64String(iv));
        Console.WriteLine("Đã lưu khóa vào file: " + fileKhoa);
    }

    // 3. import khóa từ file
    static void DocKhoa()
    {
        if (!File.Exists(fileKhoa))
        {
            Console.WriteLine("Không thấy file " + fileKhoa + ". Chọn số 2 để lưu khóa trước.");
            return;
        }

        string[] dong = File.ReadAllLines(fileKhoa);
        khoa = Convert.FromBase64String(dong[0]);
        iv = Convert.FromBase64String(dong[1]);

        Console.WriteLine("Đã import khóa từ file.");
        Console.WriteLine("Khóa (base64): " + Convert.ToBase64String(khoa));
    }

    // 4. mã hóa / giải mã văn bản
    static void VanBan()
    {
        if (khoa.Length == 0)
        {
            Console.WriteLine("Chưa có khóa. Chọn số 1 trước đã.");
            return;
        }

        Console.Write("Nhập đoạn văn bản: ");
        string s = Console.ReadLine();
        byte[] goc = Encoding.UTF8.GetBytes(s);

        byte[] ma = MaHoa(goc);
        Console.WriteLine("Bản mã (base64): " + Convert.ToBase64String(ma));

        byte[] lai = GiaiMa(ma);
        Console.WriteLine("Giải mã lại     : " + Encoding.UTF8.GetString(lai));
    }

    // 5. mã hóa 1 file -> file khác
    static void MaHoaFileMenu()
    {
        if (khoa.Length == 0)
        {
            Console.WriteLine("Chưa có khóa. Chọn số 1 trước đã.");
            return;
        }

        Console.Write("File nguồn      : ");
        string vao = Console.ReadLine();
        Console.Write("File mã hóa ra  : ");
        string ra = Console.ReadLine();

        if (!File.Exists(vao))
        {
            Console.WriteLine("Không thấy file nguồn.");
            return;
        }

        MaHoaFile(vao, ra);
        Console.WriteLine("Đã mã hóa, lưu vào: " + ra);

        Console.Write("Thử giải mã ra file: ");
        string ra2 = Console.ReadLine();
        GiaiMaFile(ra, ra2);
        Console.WriteLine("Đã giải mã, lưu vào: " + ra2);
    }

    // 6. test file lớn: tạo file -> mã hóa -> giải mã -> so sánh byte
    static void TestFileLon()
    {
        if (khoa.Length == 0)
        {
            Console.WriteLine("Chưa có khóa. Chọn số 1 trước đã.");
            return;
        }

        Console.Write("Kích thước file test (MB): ");
        int mb;
        if (!int.TryParse(Console.ReadLine(), out mb) || mb <= 0)
        {
            Console.WriteLine("Số không hợp lệ.");
            return;
        }

        string nguon = "file_nguon.bin";
        string tepma = "file_mahoa.bin";
        string giaima = "file_giaima.bin";

        // 1) tạo file nguồn
        Console.Write("Đang tạo file " + mb + " MB ... ");
        Stopwatch sw = Stopwatch.StartNew();
        TaoFile(nguon, (long)mb * 1024 * 1024);
        sw.Stop();
        Console.WriteLine("xong (" + sw.ElapsedMilliseconds + " ms)");

        // 2) mã hóa
        sw.Restart();
        MaHoaFile(nguon, tepma);
        sw.Stop();
        long tMaHoa = sw.ElapsedMilliseconds;

        // 3) giải mã
        sw.Restart();
        GiaiMaFile(tepma, giaima);
        sw.Stop();
        long tGiaiMa = sw.ElapsedMilliseconds;

        // 4) so sánh từng byte giữa file gốc và file giải mã
        sw.Restart();
        bool giong = SoSanhByte(nguon, giaima);
        sw.Stop();
        long tSoSanh = sw.ElapsedMilliseconds;

        Console.WriteLine();
        Console.WriteLine("---- KẾT QUẢ ----");
        Console.WriteLine("Mã hóa : " + tMaHoa + " ms  (" + TocDo(mb, tMaHoa) + " MB/s)");
        Console.WriteLine("Giải mã: " + tGiaiMa + " ms  (" + TocDo(mb, tGiaiMa) + " MB/s)");
        Console.WriteLine("So sánh byte: " + tSoSanh + " ms");
        Console.WriteLine("File gốc vs file giải mã: " + (giong ? "GIỐNG HỆT (100%)" : "KHÁC NHAU"));
        Console.WriteLine();
    }

    // ---- các hàm phụ ----

    // mã hóa 1 mảng byte
    static byte[] MaHoa(byte[] duLieu)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = khoa;
            aes.IV = iv;
            using (ICryptoTransform enc = aes.CreateEncryptor())
            {
                return enc.TransformFinalBlock(duLieu, 0, duLieu.Length);
            }
        }
    }

    // giải mã 1 mảng byte
    static byte[] GiaiMa(byte[] duLieu)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = khoa;
            aes.IV = iv;
            using (ICryptoTransform dec = aes.CreateDecryptor())
            {
                return dec.TransformFinalBlock(duLieu, 0, duLieu.Length);
            }
        }
    }

    // mã hóa file bằng CryptoStream (đọc/ghi từng khối nên chịu được file lớn)
    static void MaHoaFile(string vao, string ra)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = khoa;
            aes.IV = iv;
            using (FileStream fsVao = File.OpenRead(vao))
            using (FileStream fsRa = File.Create(ra))
            using (CryptoStream cs = new CryptoStream(fsRa, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                fsVao.CopyTo(cs);
            }
        }
    }

    static void GiaiMaFile(string vao, string ra)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = khoa;
            aes.IV = iv;
            using (FileStream fsVao = File.OpenRead(vao))
            using (CryptoStream cs = new CryptoStream(fsVao, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (FileStream fsRa = File.Create(ra))
            {
                cs.CopyTo(fsRa);
            }
        }
    }

    // tạo file test với dữ liệu giả
    static void TaoFile(string ten, long kichThuoc)
    {
        byte[] dem = new byte[1024 * 1024];
        for (int i = 0; i < dem.Length; i++)
        {
            dem[i] = (byte)(i % 256);
        }

        using (FileStream fs = File.Create(ten))
        {
            long daGhi = 0;
            while (daGhi < kichThuoc)
            {
                int canGhi = (int)Math.Min(dem.Length, kichThuoc - daGhi);
                fs.Write(dem, 0, canGhi);
                daGhi += canGhi;
            }
        }
    }

    // so sánh 2 file theo từng byte, dùng cho cả file lớn
    static bool SoSanhByte(string f1, string f2)
    {
        FileInfo i1 = new FileInfo(f1);
        FileInfo i2 = new FileInfo(f2);
        if (i1.Length != i2.Length)
        {
            return false;
        }

        using (FileStream s1 = File.OpenRead(f1))
        using (FileStream s2 = File.OpenRead(f2))
        {
            byte[] b1 = new byte[1024 * 1024];
            byte[] b2 = new byte[1024 * 1024];

            while (true)
            {
                int n1 = s1.Read(b1, 0, b1.Length);
                int n2 = s2.Read(b2, 0, b2.Length);
                if (n1 != n2) return false;
                if (n1 == 0) break;

                for (int i = 0; i < n1; i++)
                {
                    if (b1[i] != b2[i]) return false;
                }
            }
        }

        return true;
    }

    // tính tốc độ MB/s
    static double TocDo(int mb, long ms)
    {
        if (ms == 0) return 0;
        return Math.Round(mb * 1000.0 / ms, 1);
    }
}
