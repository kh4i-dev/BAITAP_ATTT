# Ảnh minh chứng — BAITAP_ATTT

## A. Đã có (sơ đồ / biểu đồ tự tạo)
| File | Nội dung |
|---|---|
| `06-so-do-aes.png` | Sơ đồ quy trình mã hóa AES-128 (FIPS-197) |
| `07-so-do-rsa.png` | Sơ đồ sinh cặp khóa RSA (p, q, n, φ, e, d) |
| `08-so-do-hybrid.png` | Sơ đồ mã hóa lai ghép RSA + AES |
| `09-bieu-do-rsa-vs-aes.png` | Biểu đồ so sánh tốc độ RSA-2048 và AES-128 |

## B. Ảnh kết quả chạy (render từ output thật — đã có)
| File | Nội dung |
|---|---|
| `01-aes-fips197.png` | `python3 bt01_des-aes/aes.py` — FIPS-197 PASS + CBC OK |
| `02-rsa-keygen.png` | `python3 bt02_rsa-keygen/rsa_keygen.py` — sinh khóa RSA-2048 |
| `03-rsa-models.png` | `python3 bt03_rsa-models-hybrid/rsa_models.py` — 3 mô hình |
| `04-hybrid.png` | `python3 bt03_rsa-models-hybrid/hybrid_rsa_aes.py` — hybrid OK |
| `05-benchmark.png` | `python3 bt03_rsa-models-hybrid/bench_rsa_vs_aes.py` — so sánh tốc độ |

> Mẹo: chạy `bash run-all.sh` rồi chụp 1 ảnh cho mỗi mục, hoặc chụp cả màn hình cuộn.
> Sau khi có ảnh, thêm dòng tương ứng vào `README.md` (mục 6. Kết quả) bằng:
> `<img src="./images/01-aes-fips197.png" alt="AES FIPS-197">`
