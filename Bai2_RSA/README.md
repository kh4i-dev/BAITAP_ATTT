# Bài 2 — Mã hóa RSA (bất đối xứng)

Ứng dụng Windows Forms (.NET 10): mã hóa bằng **public key**, giải mã bằng **private key**.
Giao diện gồm các nút bấm, kết quả hiện ở khung bên phải.

## Chức năng

| STT | Chức năng | Nút |
|---|---|---|
| 1, 2 | Sinh cặp khóa RSA-2048, xuất ra file PEM | "1-2. Sinh cặp khóa + lưu PEM" |
| 3, 4 | Xem nội dung PEM + giải thích ý nghĩa các trường (n, e, d, p, q…) | "3-4. Xem PEM…" |
| 5 | Mã hóa dữ liệu ≤ 190 byte (ví dụ khóa AES 32 byte) bằng public key, lưu base64 | "5. Mã hóa…" |
| 6 | Giải mã bằng private key, so sánh với dữ liệu gốc | "6. Giải mã…" |
| 7 | Đo tốc độ: public key (e nhỏ) vs private key (d lớn) | "7. Đo tốc độ…" |
| 8 | Băm SHA-256, xem đầu vào đổi chút thì hash đổi bao nhiêu bit | "8. Băm SHA-256" |
| 9 | Chữ ký số: so sánh PKCS#1 v1.5 và PSS | "9a / 9b" |

## File sinh ra

- `public.pem` — `-----BEGIN PUBLIC KEY-----`
- `private.pem` — `-----BEGIN RSA PRIVATE KEY-----` (PKCS#1)
- `data_mahoa.txt` — bản mã (base64)

## Cách chạy

```bash
cd Bai2_RSA
dotnet run
```

## Giải thích định dạng PEM

- Dòng `-----BEGIN/END-----` chỉ là mốc báo, không phải dữ liệu.
- Phần ở giữa là **dữ liệu nhị phân (ASN.1 DER) đã mã hóa Base64**.
- Giải Base64 ra, dữ liệu chia theo kiểu **Tag – Length – Value**.

Các trường trong khóa (RSA-2048):

| Trường | Ý nghĩa | Kích thước |
|---|---|---|
| n | Modulus = p × q | 256 byte |
| e | Public exponent (65537) | 3 byte |
| d | Private exponent | 256 byte |
| p, q | Hai số nguyên tố bí mật | 128 byte mỗi số |
| dp, dq, qInv | Tham số hỗ trợ giải mã bằng CRT | 128 byte mỗi số |

## Kết quả thực nghiệm

**Mã hóa / giải mã (dữ liệu 32 byte):** bản mã 256 byte → giải mã khớp lại 100%.

**Tốc độ (RSA-2048, đo 100 lần):**

| Phép toán | Khóa | Đặc điểm | Thời gian/lần |
|---|---|---|---|
| Mã hóa | public key (n, e) | e = 65537 (17 bit) | ~0.0169 ms |
| Giải mã | private key (n, d) | d ~ 2048 bit | ~0.5090 ms (chậm hơn ~30 lần) |

→ Public key nhanh vì `e` nhỏ (chỉ ~17 bit), private key chậm vì `d` rất lớn (~2048 bit), phải làm rất nhiều phép lũy thừa modulo.

**SHA-256 (hiệu ứng tuyết lở):**

```
"An toan thong tin"  → E2082ED8...D514A8CD
"An toan thong tin." → 50481223...F5B481E2
Số bit khác nhau: 130/256 (50.78%)
```

→ Chỉ thêm 1 dấu chấm mà hash đổi gần một nửa số bit.

## Chữ ký số & đề xuất

- **PKCS#1 v1.5:** cùng dữ liệu + cùng private key → ký nhiều lần ra **chữ ký giống nhau** (tất định).
- **RSA-PSS:** cùng dữ liệu + cùng private key → mỗi lần ký ra **chữ ký khác nhau**, nhưng vẫn xác minh đúng bằng public key.

**Đề xuất:** dùng **RSA-PSS** thay cho v1.5. PSS chèn thêm một chuỗi **salt ngẫu nhiên** mỗi lần ký nên chữ ký luôn khác nhau — đúng yêu cầu "cùng dữ liệu, cùng người ký nhưng chữ ký khác nhau", đồng thời chống được tấn công phát lại (replay).

## Ảnh minh chứng

![Sinh cặp khóa và lưu PEM](images/01-sinh-khoa-pem.png)

![Xem PEM và ý nghĩa các byte](images/02-xem-pem.png)

![Mã hóa dữ liệu bằng public key](images/03-ma-hoa-du-lieu.png)

![Giải mã bằng private key và so sánh](images/04-giai-ma-so-sanh.png)

![Đo tốc độ mã hóa vs giải mã](images/05-do-toc-do.png)

![Băm SHA-256 và hiệu ứng tuyết lở](images/06-bam-sha256.png)

![Chữ ký số PKCS#1 v1.5 (2 lần giống nhau)](images/07-chu-ky-pkcs1v15.png)

![Chữ ký số RSA-PSS (2 lần khác nhau)](images/08-chu-ky-pss.png)
