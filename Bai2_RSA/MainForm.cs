using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

// Bài 2: Mã hóa bất đối xứng RSA
// mã hóa bằng public key, giải mã bằng private key
class MainForm : Form
{
    TextBox txtLog = new TextBox();     // vùng hiện kết quả
    TextBox txtInput = new TextBox();   // ô nhập dữ liệu

    // các file dùng chung
    string filePublic = "public.pem";
    string filePrivate = "private.pem";
    string fileBanMa = "data_mahoa.txt";

    byte[] duLieuGoc = null;   // dữ liệu trước khi mã hóa (để so sánh)
    string hashLanTruoc = "";  // hash lần trước (để xem thay đổi)

    public MainForm()
    {
        Text = "Bài 2 - RSA";
        Size = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9);

        // khung bên trái chứa các nút
        Panel trai = new Panel();
        trai.AutoScroll = true;
        trai.Dock = DockStyle.Fill;
        trai.Padding = new Padding(8);

        Label lbl = new Label();
        lbl.Text = "Dữ liệu nhập (cho mục 5, 8, 9):";
        lbl.Left = 8;
        lbl.Top = 8;
        lbl.Width = 240;
        trai.Controls.Add(lbl);

        txtInput.Left = 8;
        txtInput.Top = 30;
        txtInput.Width = 240;
        trai.Controls.Add(txtInput);

        int y = 66;
        y = ThemNut(trai, "1-2. Sinh cặp khóa + lưu PEM", y, SinhKhoa);
        y = ThemNut(trai, "3-4. Xem PEM + ý nghĩa các byte", y, XemPem);
        y = ThemNut(trai, "5. Mã hóa dữ liệu (public key)", y, MaHoaDuLieu);
        y = ThemNut(trai, "6. Giải mã + so sánh (private key)", y, GiaiMaDuLieu);
        y = ThemNut(trai, "7. Đo tốc độ: mã hóa vs giải mã", y, DoTocDo);
        y = ThemNut(trai, "8. Băm SHA-256", y, BamSha256);
        y = ThemNut(trai, "9a. Ký số PKCS#1 v1.5 (2 lần)", y, KyV15);
        y = ThemNut(trai, "9b. Ký số PSS - mỗi lần khác nhau", y, KyPss);
        y = ThemNut(trai, "Xóa màn hình", y, XoaLog);

        // vùng log bên phải
        txtLog.Multiline = true;
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Both;
        txtLog.WordWrap = false;
        txtLog.Dock = DockStyle.Fill;
        txtLog.Font = new Font("Consolas", 10);

        // dùng TableLayoutPanel cho chắc (1 cột trái cố định, 1 cột log co giãn)
        TableLayoutPanel bang = new TableLayoutPanel();
        bang.Dock = DockStyle.Fill;
        bang.ColumnCount = 2;
        bang.RowCount = 1;
        bang.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270));
        bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        bang.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bang.Controls.Add(trai, 0, 0);
        bang.Controls.Add(txtLog, 1, 0);
        Controls.Add(bang);

        Log("Chào bạn. Bấm nút 1 để sinh cặp khóa trước tiên.");
    }

    // tạo 1 nút và trả về tọa độ y cho nút kế tiếp
    static int ThemNut(Control cha, string ten, int y, EventHandler viec)
    {
        Button b = new Button();
        b.Text = ten;
        b.Left = 8;
        b.Top = y;
        b.Width = 240;
        b.Height = 32;
        b.Click += viec;
        cha.Controls.Add(b);
        return y + 38;
    }

    void Log(string s)
    {
        txtLog.AppendText(s + "\r\n");
    }

    void XoaLog(object sender, EventArgs e)
    {
        txtLog.Clear();
    }

    // 1 + 2. sinh cặp khóa rồi xuất ra file PEM
    void SinhKhoa(object sender, EventArgs e)
    {
        using (RSA rsa = RSA.Create(2048))
        {
            File.WriteAllText(filePublic, rsa.ExportSubjectPublicKeyInfoPem());
            File.WriteAllText(filePrivate, rsa.ExportRSAPrivateKeyPem());
        }

        Log("Đã sinh cặp khóa RSA-2048.");
        Log("Public key  -> " + filePublic + "  (-----BEGIN PUBLIC KEY-----)");
        Log("Private key -> " + filePrivate + " (-----BEGIN RSA PRIVATE KEY-----)");
    }

    // 3 + 4. xem nội dung PEM và giải thích ý nghĩa các trường
    void XemPem(object sender, EventArgs e)
    {
        if (!File.Exists(filePublic))
        {
            Log("Chưa có file PEM. Bấm nút 1 trước đã.");
            return;
        }

        Log("--- NỘI DUNG " + filePublic + " ---");
        Log(File.ReadAllText(filePublic).Trim());
        Log("");

        Log("Giải thích định dạng PEM:");
        Log("- Dòng -----BEGIN/END----- chỉ là mốc báo, không phải dữ liệu.");
        Log("- Phần ở giữa là dữ liệu nhị phân (ASN.1 DER) đã mã hóa Base64.");
        Log("- Giải Base64 ra, dữ liệu chia theo kiểu Tag - Length - Value.");
        Log("");

        // import lại public key để lấy n và e
        using (RSA k = RSA.Create())
        {
            k.ImportFromPem(File.ReadAllText(filePublic));
            RSAParameters p = k.ExportParameters(false);

            Log("Các tham số trong PUBLIC KEY:");
            Log("  n (modulus) = " + p.Modulus.Length + " byte (" + k.KeySize + " bit)");
            Log("  e (exponent)= " + p.Exponent.Length + " byte = " + BitConverter.ToString(p.Exponent));
            Log("  n (base64)  = " + Convert.ToBase64String(p.Modulus));
        }

        Log("");

        // import private key để lấy n, e, d, p, q...
        if (File.Exists(filePrivate))
        {
            using (RSA k = RSA.Create())
            {
                k.ImportFromPem(File.ReadAllText(filePrivate));
                RSAParameters p = k.ExportParameters(true);

                Log("Các tham số trong PRIVATE KEY:");
                Log("  n  (modulus)    = " + p.Modulus.Length + " byte");
                Log("  e  (public)     = " + p.Exponent.Length + " byte");
                Log("  d  (private)    = " + p.D.Length + " byte");
                Log("  p  (nguyên tố 1)= " + p.P.Length + " byte");
                Log("  q  (nguyên tố 2)= " + p.Q.Length + " byte");
                Log("  dp, dq, qInv    = " + p.DP.Length + ", " + p.DQ.Length + ", " + p.InverseQ.Length + " byte");
            }
        }
    }

    // 5. mã hóa dữ liệu (không quá 190 byte) bằng public key
    void MaHoaDuLieu(object sender, EventArgs e)
    {
        if (!File.Exists(filePublic))
        {
            Log("Chưa có public key. Bấm nút 1 trước đã.");
            return;
        }

        byte[] duLieu;
        if (txtInput.Text.Length > 0)
        {
            duLieu = Encoding.UTF8.GetBytes(txtInput.Text);
        }
        else
        {
            // không nhập gì thì lấy 32 byte ngẫu nhiên (giả lập khóa AES)
            duLieu = new byte[32];
            RandomNumberGenerator.Fill(duLieu);
        }

        if (duLieu.Length > 190)
        {
            Log("Dữ liệu dài quá (" + duLieu.Length + " byte). RSA-2048 chỉ mã hóa tối đa 190 byte.");
            return;
        }

        duLieuGoc = duLieu;

        using (RSA k = RSA.Create())
        {
            k.ImportFromPem(File.ReadAllText(filePublic));
            byte[] banMa = k.Encrypt(duLieu, RSAEncryptionPadding.OaepSHA256);

            File.WriteAllText(fileBanMa, Convert.ToBase64String(banMa));

            Log("Dữ liệu gốc (" + duLieu.Length + " byte): " + Convert.ToBase64String(duLieu));
            Log("Bản mã      (" + banMa.Length + " byte): " + Convert.ToBase64String(banMa));
            Log("Đã lưu bản mã (base64) vào " + fileBanMa);
        }
    }

    // 6. giải mã bằng private key rồi so sánh với dữ liệu gốc
    void GiaiMaDuLieu(object sender, EventArgs e)
    {
        if (!File.Exists(filePrivate))
        {
            Log("Chưa có private key. Bấm nút 1 trước đã.");
            return;
        }
        if (!File.Exists(fileBanMa))
        {
            Log("Chưa có file bản mã. Bấm nút 5 trước đã.");
            return;
        }

        byte[] banMa = Convert.FromBase64String(File.ReadAllText(fileBanMa));

        using (RSA k = RSA.Create())
        {
            k.ImportFromPem(File.ReadAllText(filePrivate));
            byte[] lai = k.Decrypt(banMa, RSAEncryptionPadding.OaepSHA256);

            Log("Dữ liệu giải mã (" + lai.Length + " byte): " + Convert.ToBase64String(lai));

            if (duLieuGoc != null)
            {
                bool giong = GiongNhau(lai, duLieuGoc);
                Log("So với dữ liệu gốc: " + (giong ? "GIỐNG HỆT (100%)" : "KHÁC NHAU"));
            }
            else
            {
                Log("(Chưa có dữ liệu gốc trong bộ nhớ để so sánh.)");
            }
        }
    }

    // 7. đo tốc độ mã hóa (public key, e nhỏ) vs giải mã (private key, d lớn)
    void DoTocDo(object sender, EventArgs e)
    {
        if (!File.Exists(filePublic) || !File.Exists(filePrivate))
        {
            Log("Chưa có khóa. Bấm nút 1 trước đã.");
            return;
        }

        using (RSA pub = RSA.Create())
        using (RSA pri = RSA.Create())
        {
            pub.ImportFromPem(File.ReadAllText(filePublic));
            pri.ImportFromPem(File.ReadAllText(filePrivate));

            byte[] duLieu = new byte[190];
            RandomNumberGenerator.Fill(duLieu);

            int soLan = 100;

            // mã hóa bằng public key (e = 65537, chỉ 17 bit)
            Stopwatch sw = Stopwatch.StartNew();
            byte[] banMa = null;
            for (int i = 0; i < soLan; i++)
            {
                banMa = pub.Encrypt(duLieu, RSAEncryptionPadding.OaepSHA256);
            }
            sw.Stop();
            double tMa = sw.Elapsed.TotalMilliseconds / soLan;

            // giải mã bằng private key (d ~ 2048 bit)
            sw.Restart();
            for (int i = 0; i < soLan; i++)
            {
                pri.Decrypt(banMa, RSAEncryptionPadding.OaepSHA256);
            }
            sw.Stop();
            double tGiai = sw.Elapsed.TotalMilliseconds / soLan;

            Log("Đo tốc độ (" + soLan + " lần, RSA-2048):");
            Log("  Mã hóa  (public key, e=65537 nhỏ) : " + tMa.ToString("0.0000") + " ms/lần");
            Log("  Giải mã (private key, d rất lớn)  : " + tGiai.ToString("0.0000") + " ms/lần");
            Log("  => Giải mã chậm hơn khoảng " + (tGiai / tMa).ToString("0.0") + " lần");
            Log("  e nhỏ (17 bit) nên tính lũy thừa nhanh; d lớn (~2048 bit) nên chậm hơn nhiều.");
        }
    }

    // 8. băm SHA-256, xem đổi 1 chút dữ liệu thì hash đổi bao nhiêu
    void BamSha256(object sender, EventArgs e)
    {
        string s = txtInput.Text;
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(s));
        string hex = Convert.ToHexString(hash);

        Log("Dữ liệu : \"" + s + "\"");
        Log("SHA-256 : " + hex);

        if (hashLanTruoc != "")
        {
            int khac = DemBitKhac(hashLanTruoc, hex);
            Log("Số bit khác với lần băm trước: " + khac + "/256 (" + (khac * 100 / 256) + "%)");
            Log("=> Đổi dữ liệu chỉ 1 chút mà hash đổi gần một nửa số bit (hiệu ứng tuyết lở).");
        }

        hashLanTruoc = hex;
    }

    // 9a. ký số bằng PKCS#1 v1.5 -> ký nhiều lần cũng ra 1 kết quả
    void KyV15(object sender, EventArgs e)
    {
        if (!File.Exists(filePrivate))
        {
            Log("Chưa có private key. Bấm nút 1 trước đã.");
            return;
        }

        string s = txtInput.Text.Length > 0 ? txtInput.Text : "Xin chào";
        byte[] duLieu = Encoding.UTF8.GetBytes(s);

        using (RSA k = RSA.Create())
        {
            k.ImportFromPem(File.ReadAllText(filePrivate));

            byte[] c1 = k.SignData(duLieu, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            byte[] c2 = k.SignData(duLieu, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            Log("Ký 2 lần bằng PKCS#1 v1.5 (cùng dữ liệu, cùng private key):");
            Log("  Chữ ký lần 1: " + Convert.ToBase64String(c1));
            Log("  Chữ ký lần 2: " + Convert.ToBase64String(c2));
            Log("  Giống nhau?  " + GiongNhau(c1, c2));
            Log("  => v1.5 là kiểu ký tất định, ký mấy lần cũng ra 1 kết quả.");
        }
    }

    // 9b. ký số bằng PSS -> mỗi lần ra 1 chữ ký khác nhau (có salt ngẫu nhiên)
    void KyPss(object sender, EventArgs e)
    {
        if (!File.Exists(filePrivate) || !File.Exists(filePublic))
        {
            Log("Chưa có khóa. Bấm nút 1 trước đã.");
            return;
        }

        string s = txtInput.Text.Length > 0 ? txtInput.Text : "Xin chào";
        byte[] duLieu = Encoding.UTF8.GetBytes(s);

        using (RSA k = RSA.Create())
        {
            k.ImportFromPem(File.ReadAllText(filePrivate));

            byte[] c1 = k.SignData(duLieu, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            byte[] c2 = k.SignData(duLieu, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

            Log("Ký 2 lần bằng RSA-PSS (cùng dữ liệu, cùng private key):");
            Log("  Chữ ký lần 1: " + Convert.ToBase64String(c1));
            Log("  Chữ ký lần 2: " + Convert.ToBase64String(c2));
            Log("  Giống nhau?  " + GiongNhau(c1, c2));
            Log("");

            // vẫn xác minh đúng bằng public key
            using (RSA pub = RSA.Create())
            {
                pub.ImportFromPem(File.ReadAllText(filePublic));
                Log("  Xác minh lại bằng public key:");
                Log("    Lần 1 đúng? " + pub.VerifyData(duLieu, c1, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
                Log("    Lần 2 đúng? " + pub.VerifyData(duLieu, c2, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
            }

            Log("");
            Log("=> ĐỀ XUẤT: dùng PSS thay cho v1.5. PSS nhét thêm 1 chuỗi salt ngẫu");
            Log("   nhiên mỗi lần ký nên cùng dữ liệu + cùng người ký sẽ ra chữ ký KHÁC");
            Log("   nhau, nhưng vẫn xác minh đúng bằng cùng public key.");
        }
    }

    // ---- hàm phụ ----

    static bool GiongNhau(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    // đếm số bit khác nhau giữa 2 chuỗi hash hex
    static int DemBitKhac(string h1, string h2)
    {
        int dem = 0;
        for (int i = 0; i < h1.Length; i++)
        {
            int a = Convert.ToInt32(h1.Substring(i, 1), 16);
            int b = Convert.ToInt32(h2.Substring(i, 1), 16);
            int x = a ^ b;
            while (x > 0)
            {
                dem += x & 1;
                x >>= 1;
            }
        }
        return dem;
    }
}
