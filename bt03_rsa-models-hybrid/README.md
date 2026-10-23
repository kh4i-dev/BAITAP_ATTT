# bt03 — Các mô hình áp dụng RSA, so sánh RSA–AES, kết hợp RSA+AES

## 1. Ba mô hình áp dụng RSA

| Mô hình | Cách làm | Đảm bảo |
|---|---|---|
| **1. Xác thực người GỬI** | Người gửi **ký** bằng khóa **bí mật** của mình; người nhận **xác thực** bằng khóa **công khai** của người gửi | Tính xác thực, toàn vẹn |
| **2. Xác thực người NHẬN** | Người gửi **mã hóa** bằng khóa **công khai** của người nhận; chỉ người nhận (có khóa bí mật) mới giải được | Tính bảo mật (bí mật) |
| **3. Cả hai** | **Ký trước** (khóa bí mật người gửi) → **mã hóa** chữ ký (khóa công khai người nhận) | Bảo mật + Xác thực |

Chạy: `python3 rsa_models.py`

![Ba mô hình áp dụng RSA](../images/03-rsa-models.png)

## 2. So sánh thời gian mã hóa / giải mã RSA vs AES

Chạy: `python3 bench_rsa_vs_aes.py` (AES dùng thư viện chuẩn `pycryptodome` — bản C).

![So sánh tốc độ RSA vs AES](../images/05-benchmark.png)

**Nhận xét:**
- **AES** đạt hàng chục–trăm MB/s → phù hợp **dữ liệu lớn**.
- **RSA** rất chậm (giải mã ~**22 ms**/khối) và chỉ mã hóa được khối ≤ kích thước `n` → phù hợp **khóa / chữ ký**.
- `e = 65537` nhỏ nên **mã hóa RSA nhanh**, còn **giải mã chậm** hơn nhiều lần (số mũ `d` lớn).

## 3. Kết hợp RSA + AES (hybrid encryption)

Chạy: `python3 hybrid_rsa_aes.py`

![Mã hóa lai ghép RSA + AES](../images/04-hybrid.png)

**Cách làm:** dữ liệu lớn → mã hóa bằng **AES** (nhanh); khóa phiên AES → mã hóa bằng **RSA** (khóa công khai người nhận). Người nhận giải mã khóa AES bằng khóa bí mật rồi giải mã dữ liệu. Đây chính là cách **TLS/HTTPS, PGP** hoạt động.

## Files
| File | Nội dung |
|---|---|
| `rsa_util.py` | Sinh khóa, ký, xác thực RSA |
| `aes.py` | Bản AES (copy từ bt01) dùng cho hybrid |
| `rsa_models.py` | 3 mô hình áp dụng RSA |
| `hybrid_rsa_aes.py` | Mã hóa lai ghép RSA + AES |
| `bench_rsa_vs_aes.py` | Đo và so sánh thời gian RSA vs AES |
