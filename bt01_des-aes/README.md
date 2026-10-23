# bt01 — DES & AES: thuật toán, quy trình mã hóa/giải mã, cài đặt AES

## 1. DES (Data Encryption Standard)

- **Loại:** mã hóa khối (block cipher), khối **64 bit (8 byte)**, khóa **56 bit** (+ 8 bit kiểm tra chẵn lẻ = 64 bit).
- **Cấu trúc:** mạng **Feistel**, **16 vòng**.
- **Quy trình mã hóa:**
  1. Hoán vị đầu (Initial Permutation – IP) trên khối 64 bit.
  2. Tách thành 2 nửa 32 bit: `L0`, `R0`.
  3. Lặp 16 vòng: `Li = R(i-1)`, `Ri = L(i-1) XOR F(R(i-1), Ki)`
     - Hàm `F`: mở rộng `R` 32→48 bit (Expansion), XOR với khóa vòng `Ki`, thay thế qua **8 hộp S-box** (48→32 bit), rồi hoán vị `P`.
  4. Đổi 2 nửa và hoán vị cuối (Final Permutation – IP⁻¹) → bản mã.
- **Giải mã:** giống hệt mã hóa nhưng dùng khóa vòng theo thứ tự **ngược lại** (`K16..K1`).
- **Nhận xét:** khóa chỉ 56 bit → ngày nay bị phá bằng brute-force. Có biến thể **3DES**.

## 2. AES (Advanced Encryption Standard)

- **Loại:** mã hóa khối, khối **128 bit (16 byte)**, khóa **128 / 192 / 256 bit** → tương ứng **10 / 12 / 14 vòng** (đây làm AES-128).
- **Cấu trúc:** mạng thay thế–hoán vị (SPN), **không** phải Feistel. Chuẩn **FIPS-197**.
- **Trạng thái (State):** ma trận 4×4 byte (xếp theo cột).
- **Quy trình mã hóa AES-128:**
  1. **Key Expansion:** từ khóa 128 bit sinh ra **11 khóa vòng** (mỗi khóa 128 bit).
  2. **AddRoundKey** với khóa vòng 0.
  3. Lặp **9 vòng**, mỗi vòng gồm 4 bước:
     - **SubBytes** — thay thế từng byte qua **S-box** (nghịch đảo trên GF(2⁸)).
     - **ShiftRows** — dịch vòng từng hàng (0,1,2,3 byte).
     - **MixColumns** — nhân mỗi cột với một đa thức trên GF(2⁸).
     - **AddRoundKey** — XOR với khóa vòng.
  4. **Vòng cuối (không có MixColumns):** SubBytes → ShiftRows → AddRoundKey.
- **Giải mã:** áp dụng các phép **nghịch đảo** theo thứ tự ngược (InvShiftRows, InvSubBytes, InvMixColumns, AddRoundKey).
- **Ưu điểm:** khóa dài (128–256 bit), an toàn, nhanh cả trên phần cứng lẫn phần mềm.

## 3. Cài đặt AES (`aes.py`)

Cài đặt **thuần Python**, không dùng thư viện mã hóa — minh họa đầy đủ
`SubBytes / ShiftRows / MixColumns / AddRoundKey`, `KeyExpansion`, hỗ trợ **ECB (1 khối)** và **CBC**.

```bash
python3 aes.py
```

### Kết quả chạy (kiểm thử bằng vector chuẩn FIPS-197)

![Chạy aes.py — kiểm thử FIPS-197 và CBC](../images/01-aes-fips197.png)

> Bản `aes.py` này viết bằng Python thuần để **học thuật toán** nên chậm; khi cần tốc độ thực tế dùng thư viện chuẩn (xem benchmark ở `bt03_rsa-models-hybrid`).
