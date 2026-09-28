# bt02 — RSA: nguyên lý sinh cặp khóa bí mật / công khai

## 1. Nguyên lý

RSA dựa trên độ khó của bài toán **phân tích một số lớn thành tích hai số nguyên tố**.
- **Khóa công khai:** `(e, n)` — dùng để **mã hóa** / **xác thực chữ ký**.
- **Khóa bí mật:** `(d, n)` — dùng để **giải mã** / **ký**.

## 2. Quy trình sinh cặp khóa

1. Chọn 2 số nguyên tố lớn `p`, `q` (giữ bí mật).
2. Tính `n = p * q` (công khai) và `φ(n) = (p-1)(q-1)` (giữ bí mật).
3. Chọn số mũ công khai `e` sao cho `1 < e < φ(n)` và `gcd(e, φ(n)) = 1` (thường `e = 65537`).
4. Tính số mũ bí mật `d = e⁻¹ mod φ(n)` (nghịch đảo modulo).
5. **Khóa công khai = `(e, n)`**, **khóa bí mật = `(d, n)`**.

- Mã hóa: `c = m^e mod n`
- Giải mã: `m = c^d mod n`

## 3. Cài đặt (`rsa_keygen.py`)

Cài đặt thuần Python: sinh số nguyên tố bằng **Miller–Rabin**, nghịch đảo modulo bằng **thuật toán Euclid mở rộng**.

```bash
python3 rsa_keygen.py
```

## 4. Kết quả chạy (RSA-2048)

```text
=== SINH CAP KHOA RSA-2048 ===
p (bi mat, so nguyen to): 1131584948740925752900152870405280230081 ...
q (bi mat, so nguyen to): 1012199331372755350627722613022255391177 ...
n = p*q (cong khai)      : 1145389528507038683739183344448573185351 ...
phi(n) = (p-1)(q-1)      : ...
e (cong khai)            : 65537
d = e^-1 mod phi         : 5903727401768125903948855360402948288932 ...
-> PUBLIC (e,n) , PRIVATE (d,n)

=== THU MA HOA / GIAI MA ===
ban ro   m : 123456789
ban ma   c : 10326593124364460637469581181289204786953785363276 ...
giai ma  m': 123456789 -> OK
```

> Để tăng độ an toàn: `p`, `q` phải là số nguyên tố ngẫu nhiên đủ lớn (≥ 1024 bit mỗi số) và **khác nhau**; `n` nên ≥ 2048 bit.
