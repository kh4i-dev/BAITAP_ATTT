# Đề bài mới — An toàn và Bảo mật Thông tin

> Đây là đề mới (AES + RSA trên .NET). Đề cũ (DES/AES/RSA mô hình + hybrid) xem ở
> [`../docs/de-bai.md`](../docs/de-bai.md).
> Bài làm: [`Bai1_AES/`](Bai1_AES/) và [`Bai2_RSA/`](Bai2_RSA/).

---

## Bài 1: Mã hoá AES — mã hoá đối xứng (mã hoá, giải mã cùng dùng 1 khoá)

Tạo app (không phải web): console hoặc form, gồm các chức năng:

1. Sinh khoá AES
2. Lưu khoá ra file
3. Import khoá từ file
4. Mã hoá 1 đoạn văn bản → kết quả mã hoá ra màn hình
5. Mã hoá 1 file → lưu vào 1 file

**Yêu cầu:**
- Thử nghiệm với file có kích thước lớn (hàng GB)
- Tính thời gian mã hoá, thời gian giải mã
- Compare theo bytes: file nguồn và file giải mã
- ⇒ Kết luận dựa theo số liệu thu được

---

## Bài 2: Mã hoá RSA — mã hoá bất đối xứng (mã hoá 1 chìa, giải mã bằng chìa còn lại)

Tạo app (.NET):

1. Sinh cặp khoá
2. Xuất cặp khoá ra file (PEM)
3. View PEM: cùng ý nghĩa các byte trong format PEM
4. Nhập 1 file PEM: public key → được `n` và `e`
   Nhập 1 file PEM: private key → được `n`, `e`, `d`, ...
5. Mã hoá dữ liệu sử dụng key vừa import:
   - Dữ liệu là 1 mảng byte không quá 190 byte
     (khoá AES 256 bit ⇔ 32 byte (base64))
   - Kết quả mã hoá là mảng byte (lưu ở dạng base64) → lưu file
6. Giải mã: đầu vào là PEM + file chứa data đã mã hoá (base64)
   - So sánh kết quả giải mã với nguồn trước khi mã hoá
7. Viết đoạn code đánh giá tốc độ mã hoá, giải mã
   - Xem dùng khoá nào (public key: `n`, `e` bé) so với private key (`n`, `d` to đùng) thì tốc độ ra sao?
8. Tạo giao diện sinh chuỗi băm SHA-256
   - Khi thay đổi đầu vào → băm mới: đánh giá sự thay đổi của kết quả băm
     khi dữ liệu đầu vào thay đổi (rất ít)
9. CHỮ KÝ SỐ: thực ra là dùng RSA với khoá bí mật cho kết quả băm của dữ liệu
   → được gọi là chữ ký số của dữ liệu đó
   - ⇒ Dữ liệu khác nhau: có chữ ký số khác nhau (của cùng người ký)

**Mong muốn:**
- Cùng 1 dữ liệu
- Cùng 1 người ký (cùng private key)
- ⇒ Thì chữ ký số cũng **khác nhau** với các lần ký khác nhau

**HÃY ĐỀ XUẤT PHƯƠNG PHÁP ĐỂ ĐẠT ĐƯỢC ĐIỀU NÀY.**

→ Gợi ý lời giải: xem mục "Chữ ký số & đề xuất" trong [`Bai2_RSA/README.md`](Bai2_RSA/README.md)
(dùng **RSA-PSS** — có salt ngẫu nhiên mỗi lần ký nên mỗi lần ra một chữ ký khác nhau).
