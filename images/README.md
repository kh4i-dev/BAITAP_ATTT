# Ảnh minh chứng — BAITAP_ATTT

## A. Đã có (sơ đồ / biểu đồ tự tạo)
| File | Nội dung |
|---|---|
| `06-so-do-aes.png` | Sơ đồ quy trình mã hóa AES-128 (FIPS-197) |
| `07-so-do-rsa.png` | Sơ đồ sinh cặp khóa RSA (p, q, n, φ, e, d) |
| `08-so-do-hybrid.png` | Sơ đồ mã hóa lai ghép RSA + AES |
| `09-bieu-do-rsa-vs-aes.png` | Biểu đồ so sánh tốc độ RSA-2048 và AES-128 |

## B. Cần chụp thêm — ảnh chụp màn hình terminal (bằng chứng chạy thật)
Chạy trên Ubuntu (VM), chụp màn hình kết quả rồi lưu đúng tên dưới đây:

| File | Lệnh chạy | Cần thấy trong ảnh |
|---|---|---|
| `01-aes-fips197.png` | `python3 bt01_des-aes/aes.py` | dòng `FIPS-197 ... -> PASS` và `round-trip: OK` |
| `02-rsa-keygen.png` | `python3 bt02_rsa-keygen/rsa_keygen.py` | `p, q, n, phi, e, d` và `giai ma m' -> OK` |
| `03-rsa-models.png` | `cd bt03_rsa-models-hybrid && python3 rsa_models.py` | 3 mô hình đều `HỢP LỆ` / `OK` |
| `04-hybrid.png` | `cd bt03_rsa-models-hybrid && python3 hybrid_rsa_aes.py` | khóa AES `khớp`, `Ket qua: OK` |
| `05-benchmark.png` | `cd bt03_rsa-models-hybrid && python3 bench_rsa_vs_aes.py` | bảng MB/s của AES và µs/ms của RSA |

> Mẹo: chạy `bash run-all.sh` rồi chụp 1 ảnh cho mỗi mục, hoặc chụp cả màn hình cuộn.
> Sau khi có ảnh, thêm dòng tương ứng vào `README.md` (mục 6. Kết quả) bằng:
> `<img src="./images/01-aes-fips197.png" alt="AES FIPS-197">`
