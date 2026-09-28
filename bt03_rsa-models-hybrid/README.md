# bt03 — Các mô hình áp dụng RSA, so sánh RSA–AES, kết hợp RSA+AES

## 1. Ba mô hình áp dụng RSA

| Mô hình | Cách làm | Đảm bảo |
|---|---|---|
| **1. Xác thực người GỬI** | Người gửi **ký** bằng khóa **bí mật** của mình; người nhận **xác thực** bằng khóa **công khai** của người gửi | Tính xác thực, toàn vẹn |
| **2. Xác thực người NHẬN** | Người gửi **mã hóa** bằng khóa **công khai** của người nhận; chỉ người nhận (có khóa bí mật) mới giải được | Tính bảo mật (bí mật) |
| **3. Cả hai** | **Ký trước** (khóa bí mật người gửi) → **mã hóa** chữ ký (khóa công khai người nhận) | Bảo mật + Xác thực |

Chạy: `python3 rsa_models.py`

```text
================ MO HINH 1: XAC THUC NGUOI GUI ================
  Nguoi gui KY bang khoa BI MAT cua minh   : sig = 30123996918829764227464475678558946381980387 ...
  Nguoi nhan XAC THUC bang khoa CONG KHAI  : HOP LE
  Thu sua noi dung -> xac thuc             : BI TU CHOI (dung)

================ MO HINH 2: XAC THUC NGUOI NHAN ================
  Nguoi gui MA HOA bang khoa CONG KHAI nguoi nhan
  Nguoi nhan GIAI MA bang khoa BI MAT cua minh: Chuyen 1.000.000 VND cho tai khoan 123456
  Ket qua: OK (chi nguoi nhan doc duoc)

================ MO HINH 3: CA HAI (KY + MA HOA) ===============
  Buoc 1: nguoi gui KY                  -> sig
  Buoc 2: nguoi gui MA HOA sig (khoa nhan) -> c
  Nguoi nhan GIAI MA -> KY -> XAC THUC  : HOP LE (bao mat + xac thuc)
```

## 2. So sánh thời gian mã hóa / giải mã RSA vs AES

Chạy: `python3 bench_rsa_vs_aes.py` (AES dùng thư viện chuẩn `pycryptodome` — bản C).

```text
===== AES-128-CBC (pycryptodome) =====
     1024 bytes : enc     1.268 ms (    0.8 MB/s) | dec     0.048 ms (   21.3 MB/s)
    10240 bytes : enc     0.174 ms (   58.9 MB/s) | dec     0.084 ms (  121.8 MB/s)
  1048576 bytes : enc    14.936 ms (   70.2 MB/s) | dec    15.792 ms (   66.4 MB/s)

===== RSA-2048 (1 khoi nho) =====
  ma hoa  :     128.8 us/lan
  giai ma :   22096.6 us/lan
```

**Nhận xét:**
- **AES** đạt hàng chục–trăm MB/s → phù hợp **dữ liệu lớn**.
- **RSA** rất chậm (giải mã ~**22 ms**/khối) và chỉ mã hóa được khối ≤ kích thước `n` → phù hợp **khóa / chữ ký**.
- `e = 65537` nhỏ nên **mã hóa RSA nhanh**, còn **giải mã chậm** hơn nhiều lần (số mũ `d` lớn).

## 3. Kết hợp RSA + AES (hybrid encryption)

Chạy: `python3 hybrid_rsa_aes.py`

```text
Kich thuoc ban ro: 900 bytes
--- GOI TIN GUI DI (hybrid) ---
Khoa phien AES (bi mat)  : 390da283230798a53ddd557b4ad8b251
Khoa AES da ma hoa (RSA) : 29821801876463739614195654235147345656903441 ...
IV                       : 931d91cdb8e2b52c007c2b6feb35fd52
Ban ma AES (rut gon)     : 6ca072f39ad00bd9002b08e0f972e9de16ceb8e06edf36fd262c3f137fdf00bc ...
--- NGUOI NHAN GIAI MA ---
Khoa AES khoi phuc       : 390da283230798a53ddd557b4ad8b251 -> khop
Ket qua                  : OK
```

**Cách làm:** dữ liệu lớn → mã hóa bằng **AES** (nhanh); khóa phiên AES → mã hóa bằng **RSA** (khóa công khai người nhận). Người nhận giải mã khóa AES bằng khóa bí mật rồi giải mã dữ liệu. Đây chính là cách **TLS/HTTPS, PGP** hoạt động.

## Files
| File | Nội dung |
|---|---|
| `rsa_util.py` | Sinh khóa, ký, xác thực RSA |
| `aes.py` | Bản AES (copy từ bt01) dùng cho hybrid |
| `rsa_models.py` | 3 mô hình áp dụng RSA |
| `hybrid_rsa_aes.py` | Mã hóa lai ghép RSA + AES |
| `bench_rsa_vs_aes.py` | Đo và so sánh thời gian RSA vs AES |
